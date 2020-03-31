﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check process window
        async Task<IntPtr> CheckProcessWindowsAuto(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                if (processMulti.Type == ProcessType.UWP)
                {
                    return processMulti.WindowHandle;
                }
                else if (processMulti.Type == ProcessType.Win32 || processMulti.Type == ProcessType.Win32Store)
                {
                    return await CheckProcessWindowsWin32AndWin32Store(dataBindApp, processMulti);
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        //Launch new process
        async Task<bool> LaunchProcessDatabindAuto(DataBindApp dataBindApp)
        {
            bool appLaunched = false;
            try
            {
                //Launch new process
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    appLaunched = await LaunchProcessDatabindUwpAndWin32Store(dataBindApp);
                }
                else if (dataBindApp.LaunchFilePicker)
                {
                    appLaunched = await LaunchProcessDatabindWin32FilePicker(dataBindApp);
                }
                else if (dataBindApp.Category == AppCategory.Emulator)
                {
                    appLaunched = await LaunchProcessDatabindWin32Emulator(dataBindApp);
                }
                else
                {
                    appLaunched = await LaunchProcessDatabindWin32(dataBindApp);
                }

                //Check keyboard controller launch
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                bool keyboardOpenProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower());
                if ((keyboardOpenProcess || dataBindApp.LaunchKeyboard) && appLaunched && vControllerAnyConnected())
                {
                    LaunchKeyboardController(true);
                }
            }
            catch { }
            return appLaunched;
        }

        //Restart the process
        async Task<bool> RestartPrepareAuto(ProcessMulti processMulti, DataBindApp dataBindApp)
        {
            bool appLaunched = false;
            try
            {
                //Restart the process
                if (processMulti.Type == ProcessType.UWP)
                {
                    appLaunched = await RestartPrepareUwp(dataBindApp, processMulti);
                }
                else if (processMulti.Type == ProcessType.Win32Store)
                {
                    appLaunched = await RestartPrepareWin32Store(dataBindApp, processMulti);
                }
                else
                {
                    appLaunched = await RestartPrepareWin32(dataBindApp, processMulti);
                }

                //Check keyboard controller launch
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                bool keyboardOpenProcess = vCtrlKeyboardProcessName.Any(x => x.String1.ToLower() == fileNameNoExtension.ToLower());
                if ((keyboardOpenProcess || dataBindApp.LaunchKeyboard) && appLaunched && vControllerAnyConnected())
                {
                    LaunchKeyboardController(true);
                }
            }
            catch { }
            return appLaunched;
        }

        //Close single process
        async Task<bool> CloseSingleProcessAuto(ProcessMulti processMulti, DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                if (processMulti.Type == ProcessType.UWP)
                {
                    return await CloseSingleProcessUwp(dataBindApp, processMulti, resetProcess, removeProcess);
                }
                else if (processMulti.Type == ProcessType.Win32 || processMulti.Type == ProcessType.Win32Store)
                {
                    return await CloseSingleProcessWin32AndWin32Store(dataBindApp, processMulti, resetProcess, removeProcess);
                }
            }
            catch { }
            return false;
        }

        //Close all processes
        async Task<bool> CloseAllProcessesAuto(ProcessMulti processMulti, DataBindApp dataBindApp, bool resetProcess, bool removeProcess)
        {
            try
            {
                if (processMulti.Type == ProcessType.UWP)
                {
                    return await CloseAllProcessesUwp(dataBindApp, resetProcess, removeProcess);
                }
                else
                {
                    return await CloseAllProcessesWin32AndWin32Store(dataBindApp, resetProcess, removeProcess);
                }
            }
            catch { }
            return false;
        }
    }
}