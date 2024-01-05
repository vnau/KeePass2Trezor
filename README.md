<p align="center"><img src="images/kp2t_logo.svg" alt="Keepass2Trezor" height="60"/></p>

<h1 align="center">KeePass2Trezor</h1>
<p align="center">Less clicks, more security.</p>

<p align="center">
  <a href="https://github.com/vnau/keepass2trezor/actions/workflows/build.yml"><img src="https://github.com/vnau/keepass2trezor/actions/workflows/build.yml/badge.svg?color=gray"/></a>
  <a href="https://github.com/vnau/keepass2trezor/releases/latest"><img src="https://img.shields.io/github/release/vnau/keepass2trezor"/></a>
  <a href="https://github.com/vnau/keepass2trezor/releases/latest/download/keepass2trezor.zip"><img src="https://img.shields.io/github/downloads/vnau/keepass2trezor/total.svg"/></a>  
</p>
<br/>

The plugin for [KeePass 2.x](https://keepass.info/) uses [Trezor's](https://trezor.io/) security design to encrypt the password database.
The decryption key can only be read from the Trezor by physically pressing the confirmation button on the Trezor device.

It supports Trezor One, Model M and the new Safe 3 on Windows and Linux.
<img align="right" width="300" height="300" alt="Using Trezor Hardware Wallet as key provider for KeePass 2.x" src="images/kp2t_animation.gif">

## Review

- KeePass database securely encrypted with your personal TREZOR device.
- A simple click on your Trezor button to unlock your password manager.
- Use a 24 words recovery seed to regain access to your passwords.
- Can be used with or without master password.

## How to use

- Copy **KeePass2Trezor.dll** from the [latest release](https://github.com/vnau/keepass2trezor/releases) to the **Plugin** folder of the [KeePass 2.x](https://keepass.info/).
- Create a new database, selecting **Trezor Key Provider** in the **Key file/provider** field.
- Follow instructions, unlock Trezor if necessary and confirm decryption of the key by clicking button on the device.

### Linux users

Although the plugin works on Linux, it requires several steps:

1. **Configure udev rules:**

   - Follow the [udev rules configuration guide](https://trezor.io/learn/a/udev-rules) to establish communication with Trezor devices.

2. **Install `mono-develop` package:**

   - Ensure that the `mono-develop` package is installed, as the plugin relies on netstandard2.0, which is included with it.

3. **Check `libusb-1.0` installation:**

   - Verify the installation of `libusb-1.0-0`. If **KeePass2Trezor** still hangs with the message _"Connect your Trezor device"_ even with libusb installed, consider either creating a symlink [according to this instruction](https://github.com/LibUsbDotNet/LibUsbDotNet?tab=readme-ov-file#linux-users) or install `libusb-1.0-dev` package to address the issue.

4. **Disconnect and reconnect the device:**
   - After completing the configuration steps, disconnect and then reconnect your Trezor device to ensure the changes take effect.

## Requirements

- _KeePass 2.35_ or newer
- _.NET Framework 4.6.2_ or higher
- _libusb-1.0_ for Linux

## Security considerations

⚠ If your device is lost or broken, you will need to purchase a new [Trezor](https://trezor.io/) or build a [PiTrezor](https://www.pitrezor.com) and initialize it using the saved seed phrase to regain access to the KeePass database.

⚠ Exporting the database in any format except _kdbx_ will cause loss of the Key Id and therefore decryption of these containers will not be possible. This is because these containers do not support public custom data (unencrypted) where the Key ID is stored.

## Technical details

**KeePass2Trezor** is a _key provider plugin_ for the KeePass 2.x password manager. It uses much the same approach to derive master key as [Trezor Password Manager](https://trezor.io/passwords/) described in the [SLIP-0016](https://github.com/satoshilabs/slips/blob/master/slip-0016.md) document.

## Contribution

🌱 Any feedback and contribution is much appreciated!
