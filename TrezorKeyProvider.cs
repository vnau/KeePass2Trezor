using KeePass.UI;
using KeePassLib.Keys;
using KeePassLib.Utility;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrezorKeyProviderPlugin.Forms;

namespace TrezorKeyProviderPlugin
{
    public sealed class TrezorKeyProvider : KeyProvider
    {
        private Func<Form, DialogResult> ShowDialogAndDestroyHandler;

        private DialogResult ShowTrezorInfoDialog(string title, string description, string message)
        {
            using (var dlg = new TrezorConnectForm(title, description, message))
            {
                return ShowDialogAndDestroyHandler(dlg);
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
                _dlg?.Invoke((MethodInvoker)delegate
                {
                    // close the form on the forms thread
                    _dlg.DialogResult = DialogResult.OK;
                    _dlg?.Close();
                });
        }

        public TrezorKeyProvider()
        {
            ShowDialogAndDestroyHandler = ShowDialogAndDestroy;
        }

        public TrezorKeyProvider(Func<Form, DialogResult> ShowDialogAndDestroy)
        {
            ShowDialogAndDestroyHandler = ShowDialogAndDestroy;
        }

        public override string Name
        {
            get { return "Trezor Key Provider"; }
        }

        public override bool SecureDesktopCompatible
        {
            get { return true; }
        }

        public override byte[] GetKey(KeyProviderQueryContext ctx)
        {
            try
            {
                byte[] pb = ctx.CreatingNewKey
                    ? Create(ctx)
                    : Create(ctx);
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

        private Form _dlg = null;

        private byte[] Create(KeyProviderQueryContext ctx)
        {
            Task<byte[]> task;
            try
            {
                using (var device = new TrezorDevice())
                {
                    device.OnChangeState += (Object sender, TrezorStateEvent stateEvent) =>
                    {
                        var state = stateEvent.State;
                        //if (state == TrezorDevice.TrezorState.Connected)
                        {
                            CloseCurrentDialog();
                        }
                    };

                    //dlg.InitEx(trezorInfo, ctx);
                    byte[] secret;
                    var startTime = DateTime.Now;

                    using (task = device.Encrypt())
                    {
                        while (!task.IsCompleted)
                        {
                            if (device.State == TrezorDevice.TrezorState.Disconnected)
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
                            else if (device.State == TrezorDevice.TrezorState.Connected)
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
                            else if (device.State == TrezorDevice.TrezorState.ButtonRequest)
                            {
                                if (DialogResult.OK != ShowTrezorInfoDialog(
                                    "Confirm Trezor",
                                    "Confirm on your Trezor device",
                                    "Confirm operation on your Trezor device"))
                                {
                                    return null;
                                }
                            }
                            else if (device.State == TrezorDevice.TrezorState.Processing)
                            {
                                if (DialogResult.OK != ShowTrezorInfoDialog(
                                    "Processing Trezor",
                                    "Trezor is working now",
                                    "Please wait while Trezor working"))
                                    return null;
                            }
                            else if (device.State == TrezorDevice.TrezorState.Error)
                            {
                                if (DialogResult.OK != ShowTrezorInfoDialog(
                                    "Trezor error",
                                    "Trezor throws error",
                                    device.StateMessage))
                                    return null;
                            }
                            else if (device.State == TrezorDevice.TrezorState.WaitPin || device.State == TrezorDevice.TrezorState.WaitPassfrase)
                            {
                                using (var dlg = new TrezorPinPromptForm())
                                {
                                    if (ShowDialogAndDestroyHandler(dlg) == DialogResult.OK)
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
                            ShowTrezorInfoDialog(
                                    "Trezor error",
                                    "Trezor throws error",
                                    device.StateMessage);
                            return null;
                        }
                        secret = task.Result;
                    }
                    return secret;
                }
            }
            catch (Exception ex)
            {
                ShowTrezorInfoDialog("System error", "Error", ex.Message);
                return null;
            }
        }
    }
}
