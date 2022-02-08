# 🔐 KeePass2Trezor
[![Build status](https://ci.appveyor.com/api/projects/status/tddh86twfhcpo5kt?svg=true)](https://ci.appveyor.com/project/vnau/keepass2trezor)


This plugin for [KeePass 2.x](https://keepass.info/) uses [Trezor's](https://trezor.io/) security design to encrypt the password database.
The decryption key can only be read from the Trezor by physically pressing the confirmation button on the Trezor device. 

> ⚠ **THE PLUGIN IS IN BETA** 
> 
> ⚠ Please do not use with sensitive data. Make regular backups, this plugin is still in beta and needs further testing on the _Trezor One_ and especially _Trezor model T_ wallets!
> 
> ⚠ Exporting the database in any format except _kdbx_ will cause loss of the Key Id and therefore decryption of these containers will not be possible. This is because these containers do not support public custom data (unencrypted) where the Key ID is stored.
> 
> 🌱 Any feedback and contribution is much appreciated!

## Review

* KeePass database securely encrypted with your personal TREZOR device.
* A simple click on your Trezor button to unlock your password manager.
* Use a 24 words recovery seed to regain access to your passwords.
* Can be used with or without master password.

## How to use

* Copy __KeePass2Trezor.dll__ from the [latest release](https://github.com/vnau/keepass2trezor/releases) to the **Plugin** folder of the [KeePass 2.x](https://keepass.info/).
* Create a new database selecting __Trezor Key Provider__ in the __Key file/provider__ field.
* Follow instructions, unlock Trezor if necessary and confirm decryption of the key by clicking button on the device.

## Dependencies

* _Keepass 2.35_ or newer
* _.NET Framework 4.6.1_ or higher

## Technical details

__Keepass2Trezor__ is a _key provider plugin_ for the KeePass 2.x password manager. It uses much the same approach to derive master key as [Trezor Password Manager](https://trezor.io/passwords/) described in the [SLIP-0016](https://github.com/satoshilabs/slips/blob/master/slip-0016.md) document.
