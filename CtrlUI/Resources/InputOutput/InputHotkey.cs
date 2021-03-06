﻿using System.Diagnostics;
using System.Windows.Interop;
using static ArnoldVinkCode.AVInputOutputClass;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle hotkey
        async void HandleHotkey(MSG windowMessage)
        {
            try
            {
                Debug.WriteLine("Hotkey pressed.");

                //Check the pressed keys
                KeysVirtual usedVirtualKey = (KeysVirtual)(((int)windowMessage.lParam >> 16) & 0xFFFF);
                if (usedVirtualKey == KeysVirtual.CapsLock)
                {
                    await AppWindow_HideShow();
                }
            }
            catch { }
        }
    }
}