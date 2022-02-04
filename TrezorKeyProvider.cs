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
using TrezorKeyProviderPlugin.Forms;
using TrezorKeyProviderPlugin.Device;

namespace TrezorKeyProviderPlugin
{
    public sealed class TrezorKeyProvider : KeyProvider
    {
        private readonly Func<Form, DialogResult> _showDialogAndDestroyHandler;
        private Form _dlg = null;

        private DialogResult ShowTrezorInfoDialog(string title, string description, string message)
        {
            using (var dlg = new TrezorConnectForm(title, description, message))
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

        private void CloseCurrentDialog()
        {
            if (_dlg != null)
                _dlg.Invoke((MethodInvoker)delegate
                {
                    // close the form on the forms thread
                    _dlg.DialogResult = DialogResult.OK;
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
                return "Trezor Key Provider";
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
            var db = new PwDatabase()
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
                // KeePass API have no proper method to read database headers
                // without attempt to decrypt it so all we have to do is try to
                // decrypt database with dummy master key and ignore thrown exception.
            }

            // TODO: error handling

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
                MessageService.ShowWarning(ex.Message);
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
                // generate random key ID unique for each database encrypted with Trezor.
                keyId = KeePassLib.Cryptography.CryptoRandom.Instance.GetRandomBytes(7);
                // the first byte of the key ID is zero and is reserved for future use.
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
                    throw new Exception("Invalid Trezor master key version. You may need a newer version of the Trezor Key Provider Plugin.");
                }
            }

            Task<byte[]> task;
            try
            {
                using (IKeyProviderDevice device = new TrezorDevice())
                {
                    device.OnChangeState += (Object sender, KeyDeviceStateEvent stateEvent) =>
                    {
                        var state = stateEvent.State;
                        //if (state == TrezorDevice.TrezorState.Connected)
                        {
                            CloseCurrentDialog();
                        }
                    };

                    byte[] secret;
                    var startTime = DateTime.Now;
                    var request = string.Format("Unlock encrypted KeePass storage{0}?", keyId != null ? " " + KeyToString(keyId) : "");
                    using (task = device.GetKeyByRequest(request))
                    {
                        while (!task.IsCompleted)
                        {
                            if (device.State == KeyDeviceState.Disconnected)
                            //&& DateTime.Now.Subtract(startTime).Seconds > 0)
                            {
                                if (DialogResult.OK != ShowTrezorInfoDialog(
                                    "Connect Trezor",
                                    "Connect your Trezor device",
                                    "Connect your Trezor device"))
                                {
                                    device.Close();

                                    return null;
                                }
                            }
                            else if (device.State == KeyDeviceState.Connected)
                            //&& DateTime.Now.Subtract(startTime).Seconds > 0)
                            {
                                if (DialogResult.OK != ShowTrezorInfoDialog(
                                    "Trezor Connected",
                                    "Trezor device connected",
                                    device.StateMessage))
                                {
                                    device.Close();
                                    return null;
                                }
                            }
                            else if (device.State == KeyDeviceState.WaitConfirmation)
                            {
                                if (DialogResult.OK != ShowTrezorInfoDialog(
                                    "Confirm Trezor",
                                    "Confirm on your Trezor device",
                                    "Confirm unlocking the KeePass encrypted storage on your Trezor device." + (keyId != null ? "\r\n\r\nKey ID: " + KeyToString(keyId) + "" : "")))
                                {
                                    return null;
                                }
                            }
                            else if (device.State == KeyDeviceState.Processing)
                            {
                                if (DialogResult.OK != ShowTrezorInfoDialog(
                                    "Processing Trezor",
                                    "Trezor is working now",
                                    "Please wait while Trezor working"))
                                    return null;
                            }
                            else if (device.State == KeyDeviceState.Error)
                            {
                                //if (DialogResult.OK != ShowTrezorInfoDialog(
                                //    "Trezor error",
                                //    "Trezor throws error",
                                //    device.StateMessage))
                                throw new Exception(device.StateMessage);
                                //return null;
                            }
                            else if (device.State == KeyDeviceState.WaitPIN || device.State == KeyDeviceState.WaitPassphrase)
                            {
                                using (var dlg = new TrezorPinPromptForm())
                                {
                                    if (_showDialogAndDestroyHandler(dlg) == DialogResult.OK)
                                    {
                                        device.SetPin(dlg.Pin);
                                    }
                                    else
                                    {
                                        return null;
                                    }
                                }
                            }
                            Application.DoEvents();
                        }
                        CloseCurrentDialog();
                        if (task.Status == TaskStatus.Faulted)
                        {
                            throw new Exception(device.StateMessage);
                            //ShowTrezorInfoDialog(
                            //        "Trezor error",
                            //        "Trezor throws error",
                            //        device.StateMessage);
                            //return null;
                        }
                        secret = task.Result;
                    }
                    return secret;
                }
            }
            catch (Exception)
            {
                throw;
                //ShowTrezorInfoDialog("System error", "Error", ex.Message);
            }
        }
    }
}
