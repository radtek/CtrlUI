﻿using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVInteropDll;
using static ArnoldVinkCode.ProcessWin32Functions;
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.Keyboard
{
    partial class WindowKeyboard
    {
        //Handle key press
        void ButtonKey_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    //Send the clicked button
                    KeyButtonClick(sender);
                }
            }
            catch { }
        }
        void ButtonKey_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Send the clicked button
                KeyButtonClick(sender);
            }
            catch { }
        }

        //Keyboard type string
        async Task KeyboardTypeString(string typeString)
        {
            try
            {
                foreach (char charString in typeString)
                {
                    KeysVirtual virtualKeyScan = VkKeyScanEx(charString, IntPtr.Zero);
                    bool shiftPressed = ((short)virtualKeyScan & 0x100) == 0x100;
                    if (shiftPressed)
                    {
                        await KeyPressComboAuto(KeysVirtual.Shift, virtualKeyScan);
                    }
                    else
                    {
                        await KeyPressSingleAuto(virtualKeyScan);
                    }
                }
            }
            catch { }
        }

        //Send the clicked button
        async void KeyButtonClick(object sender)
        {
            try
            {
                Button sendButton = sender as Button;
                string sendKeyName = sendButton.Tag.ToString();
                KeysVirtual sendKeyVirtual = KeysVirtual.Null;
                try
                {
                    sendKeyVirtual = (KeysVirtual)Enum.Parse(typeof(KeysVirtual), sendKeyName, true);
                }
                catch { }
                Debug.WriteLine("Sending key: " + sendKeyName + "/" + sendKeyVirtual);
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false);

                //Check for string text input
                if (sendKeyName == "DotCom")
                {
                    string extensionString = string.Empty;
                    if (vCapsEnabled)
                    {
                        extensionString = ConfigurationManager.AppSettings["KeyboardDomainExtension"].ToString();
                    }
                    else
                    {
                        extensionString = ".com";
                    }
                    await KeyboardTypeString(extensionString);
                    return;
                }

                //Check if the caps lock is enabled
                if (vCapsEnabled)
                {
                    if (sendKeyVirtual == KeysVirtual.Shift)
                    {
                        await KeyPressComboAuto(KeysVirtual.Control, KeysVirtual.X);
                    }
                    else if (sendKeyVirtual == KeysVirtual.Control)
                    {
                        await KeyPressComboAuto(KeysVirtual.Control, KeysVirtual.C);
                    }
                    else if (sendKeyVirtual == KeysVirtual.Alt)
                    {
                        await KeyPressComboAuto(KeysVirtual.Control, KeysVirtual.V);
                    }
                    else if (sendKeyVirtual == KeysVirtual.Space)
                    {
                        await ProcessLauncherWin32Async(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\Taskmgr.exe", "", "", false, false);
                    }
                    else if (sendKeyVirtual == KeysVirtual.Enter)
                    {
                        await KeyPressComboAuto(KeysVirtual.Control, KeysVirtual.Z);
                    }
                    else if (sendKeyVirtual == KeysVirtual.LeftWindows)
                    {
                        await KeyPressComboAuto(KeysVirtual.Control, KeysVirtual.A);
                    }
                    else if (sendKeyVirtual == KeysVirtual.Home)
                    {
                        await KeyPressSingleAuto(KeysVirtual.Home);
                    }
                    else if (sendKeyVirtual == KeysVirtual.End)
                    {
                        await KeyPressSingleAuto(KeysVirtual.End);
                    }
                    else
                    {
                        await KeyPressComboAuto(KeysVirtual.Shift, sendKeyVirtual);
                    }
                }
                else
                {
                    await KeyPressSingleAuto(sendKeyVirtual);
                }
            }
            catch { }
        }

        //Handle capslock
        async void ButtonCaps_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await SwitchCapsLock();
                }
            }
            catch { }
        }
        async void ButtonCaps_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await SwitchCapsLock();
            }
            catch { }
        }

        //Handle ScrollMove
        void ButtonScrollMove_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    SwitchKeyboardMode();
                }
            }
            catch { }
        }
        void ButtonScrollMove_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SwitchKeyboardMode();
            }
            catch { }
        }

        //Update the keyboard mode
        void UpdateKeyboardMode()
        {
            try
            {
                if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardMode"]) == 0)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_ThumbRightOff.Text = "Move";
                        image_ScrollMove.Source = FileToBitmapImage(new string[] { "Assets/Icons/KeyboardScroll.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        //ToolTip newTooltip = new ToolTip() { Content = "Switch to mouse wheel mode" };
                        //key_ScrollMove.ToolTip = newTooltip;
                    });
                }
                else
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_ThumbRightOff.Text = "Scroll";
                        image_ScrollMove.Source = FileToBitmapImage(new string[] { "Assets/Icons/KeyboardMove.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        //ToolTip newTooltip = new ToolTip() { Content = "Switch to window move mode" };
                        //key_ScrollMove.ToolTip = newTooltip;
                    });
                }
            }
            catch { }
        }

        //Switch between keyboard modes
        void SwitchKeyboardMode()
        {
            try
            {
                if (Convert.ToInt32(ConfigurationManager.AppSettings["KeyboardMode"]) == 0)
                {
                    SettingSave(vConfigurationApplication, "KeyboardMode", "1");
                    UpdateKeyboardMode();
                }
                else
                {
                    SettingSave(vConfigurationApplication, "KeyboardMode", "0");
                    UpdateKeyboardMode();
                }
            }
            catch { }
        }

        //Handle close
        void ButtonClose_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    this.Hide();
                }
            }
            catch { }
        }
        void ButtonClose_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.Hide();
            }
            catch { }
        }
    }
}