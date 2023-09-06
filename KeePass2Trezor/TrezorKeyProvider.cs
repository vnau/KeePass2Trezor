using KeePass.UI;
using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using KeePassLib.Utility;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePass2Trezor.Forms;
using KeePass2Trezor.Device;
using KeePass2Trezor.Properties;
using KeePass.Resources;
using Trezor.Net;

namespace KeePass2Trezor
{
    public sealed class TrezorKeyProvider : KeyProvider
    {
        private readonly Func<Form, DialogResult> _showDialogAndDestroyHandler;
        private Form _dlg = null;

        private DialogResult ShowTrezorInfoDialog(string title, string caption, string description, string message)
        {
            using (var dlg = new TrezorConnectForm(title, caption, description, message))
            {
                return _showDialogAndDestroyHandler(dlg);
            }
        }
        private DialogResult ShowDialogAndDestroy(Form form)
        {
            _dlg = form;
            var result = UIUtil.ShowDialogAndDestroy(form);
            _dlg = null;
            return result;
        }

        private void CloseCurrentDialog(DialogResult result = DialogResult.Retry)
        {
            if (_dlg != null)
                _dlg.Invoke((MethodInvoker)delegate
                {
                    // close the form on the forms thread
                    _dlg.DialogResult = result;
                    _dlg.Close();
                });
        }

        public TrezorKeyProvider()
        {
            _showDialogAndDestroyHandler = ShowDialogAndDestroy;
        }

        public TrezorKeyProvider(Func<Form, DialogResult> ShowDialogAndDestroy)
        {
            _showDialogAndDestroyHandler = ShowDialogAndDestroy;
        }

        public static string ProviderName
        {
            get
            {
                return Resources.TrezorKeyProvider;
            }
        }

        public override string Name
        {
            get
            {
                return ProviderName;
            }
        }

        public override bool SecureDesktopCompatible
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Read Trezor key Id from source.
        /// </summary>
        /// <param name="ioSource">Source connection.</param>
        /// <returns>Trezor key Id</returns>
        private byte[] ReadTrezorKeyId(IOConnectionInfo ioSource)
        {
            PwDatabase db;
            System.Reflection.MethodInfo loadHeaderMethod = typeof(PwDatabase).GetMethod("LoadHeader");
            if (loadHeaderMethod != null)
            {
                // Use the static method PwDatabase.LoadHeader(ioSource) to load header through reflection for KeePass v2.52+
                db = loadHeaderMethod.Invoke(null, new[] { ioSource }) as PwDatabase;
            }
            else
            {
                // Use the tricky way for older versions of the KeePass
                db = new PwDatabase()
                {
                    MasterKey = new CompositeKey(),
                    RootGroup = new PwGroup()
                };
                KdbxFile kdbx = new KdbxFile(db);
                try
                {
                    using (Stream s = IOConnection.OpenRead(ioSource))
                    {
                        kdbx.Load(s, KdbxFormat.Default, new NullStatusLogger());
                    }
                }
                catch (InvalidCompositeKeyException)
                {
                    // HACK:
                    // The KeePass API prior to version 2.52 did not have a proper method for reading
                    // database headers without attempt to decrypt it so all we have to do is try to
                    // decrypt the database with a dummy master key and ignore the thrown exception.
                }
            }

            return db.PublicCustomData.GetByteArray(TrezorKeysCache.TrezorPropertyKey);
        }

        public override byte[] GetKey(KeyProviderQueryContext ctx)
        {
            try
            {
                byte[] pb = Create(ctx, ctx.CreatingNewKey);
                if (pb == null)
                    return null;

                // KeePass clears the returned byte array, thus make a copy
                byte[] pbRet = new byte[pb.Length];
                Array.Copy(pb, pbRet, pb.Length);
                return pbRet;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex is AggregateException)
                {
                    ex = ex.InnerException;
                    message = ex.Message;
                }

                var fex = ex as FailureException<Trezor.Net.Contracts.Common.Failure>;
                if (fex != null)
                {
                    message = fex.Failure.Message;
                }

                MessageService.ShowWarning(message);
            }

            return null;
        }

        /// <summary>
        /// Convert a key to the string representation.
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        private static string KeyToString(byte[] keyId)
        {
            return Convert.ToBase64String(keyId.Skip(1).ToArray());
        }

        private byte[] Create(KeyProviderQueryContext ctx, bool creatingNewKey)
        {
            byte[] keyId = null;
            if (creatingNewKey)
            {
                // Generate a random key ID unique for each database encrypted with Trezor.
                keyId = KeePassLib.Cryptography.CryptoRandom.Instance.GetRandomBytes(7);
                // The first byte of the key ID is zero and is reserved for future use.
                keyId[0] = 0;
                TrezorKeysCache.Instance.Add(ctx.DatabaseIOInfo, keyId);
            }
            else
            {
                keyId = ReadTrezorKeyId(ctx.DatabaseIOInfo);
                if (keyId == null)
                {
                    keyId = TrezorKeysCache.Instance.Get(ctx.DatabaseIOInfo);
                }
                else if (keyId.Length == 0 || keyId.First() > 0)
                {
                    throw new Exception(Resources.ExceptionInvalidTrezorMasterKeyVersion);
                }
            }

            IKeyProviderDevice device = null;
            Task<byte[]> task = null;

            try
            {
                device = new TrezorDevice();
                device.OnChangeState += (Object sender, KeyDeviceStateEvent stateEvent) =>
                {
                    var state = stateEvent.State;
                    //if (state == TrezorDevice.TrezorState.Connected)
                    {
                        CloseCurrentDialog();
                    }
                };

                string databasePath = ctx.DatabasePath;
                string databaseName = UrlUtil.GetFileName(databasePath);
                string windowTitle = KPRes.OpenDatabase + ((!string.IsNullOrEmpty(databaseName)) ? " - " + databaseName : "");

                byte[] secret;
                var startTime = DateTime.Now;
                var request = string.Format("Unlock encrypted KeePass storage{0}?", keyId != null ? " " + KeyToString(keyId) : "");
                task = device.GetKeyByRequest(request);
                {
                    while (!task.IsCompleted)
                    {
                        if (device.State == KeyDeviceState.Disconnected)
                        {
                            if (DialogResult.Cancel == ShowTrezorInfoDialog(
                                windowTitle,
                                Resources.ConnectTrezorCaption,
                                databasePath,
                                Resources.ConnectTrezorMessage))
                            {
                                return null;
                            }
                        }
                        else if (device.State == KeyDeviceState.Connected)
                        {
                            if (DialogResult.Cancel == ShowTrezorInfoDialog(
                                windowTitle,
                                Resources.ConnectedTrezorCaption,
                                databasePath,
                                device.StateMessage))
                            {
                                return null;
                            }
                        }
                        else if (device.State == KeyDeviceState.WaitConfirmation)
                        {
                            if (DialogResult.Cancel == ShowTrezorInfoDialog(
                                windowTitle,
                                Resources.ConfirmTrezorCaption,
                                databasePath,
                                Resources.ConfirmTrezorMessage + (keyId != null ? string.Format(Resources.ConfirmTrezorKeyID, KeyToString(keyId)) : "")))
                            {
                                return null;
                            }
                        }
                        else if (device.State == KeyDeviceState.Processing)
                        {
                            if (DialogResult.Cancel == ShowTrezorInfoDialog(
                                windowTitle,
                                Resources.TrezorWorkingCaption,
                                databasePath,
                                Resources.TrezorWorkingMessage))
                                return null;
                        }
                        else if (device.State == KeyDeviceState.WaitPIN || device.State == KeyDeviceState.WaitPassphrase)
                        {
                            using (var dlg = new TrezorPinPromptForm(windowTitle))
                            {
                                switch (_showDialogAndDestroyHandler(dlg))
                                {
                                    case DialogResult.OK:
                                        device.SetPin(dlg.Pin);
                                        break;
                                    case DialogResult.Cancel:
                                        return null;
                                }
                            }
                        }
                        else if (device.State == KeyDeviceState.Error)
                        {
                            throw new Exception(device.StateMessage);
                        }

                        Application.DoEvents();
                    }
                    CloseCurrentDialog();
                    if (task.Status == TaskStatus.Faulted)
                    {
                        throw new Exception(device.StateMessage);
                    }
                    secret = task.Result;
                }
                return secret;
            }
            finally
            {
                if (device != null)
                {
                    device.Close();
                }

                // Wait for the task complete
                if (task != null)
                {
                    task.Wait();
                    task.Dispose();
                }

                // Dispose resources
                if (device != null)
                {
                    device.Dispose();
                }
            }
        }
    }
}
