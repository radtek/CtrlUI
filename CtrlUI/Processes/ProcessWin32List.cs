﻿using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get all the active Win32 processes and update the lists
        async Task ListLoadCheckProcessesWin32(IEnumerable<Process> processesList, List<IntPtr> activeProcessesWindow, IEnumerable<DataBindApp> currentListApps, bool showStatus)
        {
            try
            {
                if (ConfigurationManager.AppSettings["ShowOtherProcesses"] == "False")
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        sp_Processes.Visibility = Visibility.Collapsed;
                    });
                    List_Processes.Clear();
                    GC.Collect();
                    return;
                }

                //Check if processes list is provided
                if (processesList == null)
                {
                    processesList = Process.GetProcesses();
                }

                //Check if there are active processes
                if (processesList.Any())
                {
                    //Show refresh status message
                    if (showStatus) { Popup_Show_Status("Refresh", "Refreshing desktop apps"); }
                    //Debug.WriteLine("Checking desktop processes.");

                    //Add new running process if needed
                    foreach (Process processApp in processesList)
                    {
                        try
                        {
                            //Process window handle
                            IntPtr processWindowHandle = processApp.MainWindowHandle;

                            //Process application type
                            ProcessType processType = ProcessType.Win32;

                            //Process Windows Store Status
                            Visibility processStatusStore = Visibility.Collapsed;

                            //Get the process title
                            string processName = GetWindowTitleFromProcess(processApp);

                            //Check if application title is blacklisted
                            if (vAppsBlacklistProcess.Any(x => x.ToLower() == processName.ToLower()))
                            {
                                continue;
                            }

                            //Validate the process state
                            if (!ValidateProcessState(processApp, true, true))
                            {
                                continue;
                            }

                            //Validate the window handle
                            if (!ValidateWindowHandle(processWindowHandle))
                            {
                                continue;
                            }

                            //Get the executable path
                            string processPathExe = GetExecutablePathFromProcess(processApp);
                            string processPathExeLower = processPathExe.ToLower();
                            string processPathImage = processPathExe;
                            string processNameExe = Path.GetFileName(processPathExe);
                            string processNameExeLower = processNameExe.ToLower();
                            string processNameExeNoExt = Path.GetFileNameWithoutExtension(processPathExe);
                            string processNameExeNoExtLower = processNameExeNoExt.ToLower();

                            //Check if application process is blacklisted
                            if (vAppsBlacklistProcess.Any(x => x.ToLower() == processNameExeNoExtLower))
                            {
                                continue;
                            }

                            //Add active process
                            activeProcessesWindow.Add(processWindowHandle);

                            //Check the process running time
                            int processRunningTime = ProcessRuntimeMinutes(processApp);

                            //Check if the process is suspended
                            Visibility processStatusRunning = Visibility.Visible;
                            Visibility processStatusSuspended = Visibility.Collapsed;
                            if (CheckProcessSuspended(processApp.Threads))
                            {
                                processStatusRunning = Visibility.Collapsed;
                                processStatusSuspended = Visibility.Visible;
                            }

                            //Check the process launch arguments
                            string processArgument = GetLaunchArgumentsFromProcess(processApp, processPathExe);

                            //Set the combined application filter
                            Func<DataBindApp, bool> filterCombinedApp = x => Path.GetFileNameWithoutExtension(x.PathExe).ToLower() == processNameExeNoExtLower;

                            //Check if process is a Win32Store app
                            string processAppUserModelId = GetAppUserModelIdFromProcess(processApp);
                            if (!string.IsNullOrWhiteSpace(processAppUserModelId))
                            {
                                processType = ProcessType.Win32Store;
                                processPathExe = processAppUserModelId;
                                processPathExeLower = processAppUserModelId.ToLower();
                                processStatusStore = Visibility.Visible;
                                filterCombinedApp = x => Path.GetFileNameWithoutExtension(x.NameExe).ToLower() == processNameExeNoExtLower;
                            }

                            //Convert Process To ProcessMulti
                            ProcessMulti processMultiNew = ConvertProcessToProcessMulti(processType, processApp);

                            //Check all the lists for the application
                            DataBindApp existingCombinedApp = currentListApps.Where(filterCombinedApp).FirstOrDefault();
                            DataBindApp existingProcessApp = List_Processes.Where(x => x.ProcessMulti.Any(z => z.WindowHandle == processWindowHandle)).FirstOrDefault();

                            //Check if process is in combined list and update it
                            if (existingCombinedApp != null)
                            {
                                //Update the process running time
                                existingCombinedApp.RunningTime = processRunningTime;

                                //Update the process running status
                                existingCombinedApp.StatusRunning = processStatusRunning;

                                //Update the process suspended status
                                existingCombinedApp.StatusSuspended = processStatusSuspended;

                                //Update the application last runtime
                                existingCombinedApp.RunningTimeLastUpdate = Environment.TickCount;

                                //Add the new process multi
                                if (!existingCombinedApp.ProcessMulti.Any(x => x.WindowHandle == processWindowHandle))
                                {
                                    existingCombinedApp.ProcessMulti.Add(processMultiNew);
                                }

                                //Remove app from processes list
                                if (ConfigurationManager.AppSettings["HideAppProcesses"] == "True")
                                {
                                    await AVActions.ActionDispatcherInvokeAsync(async delegate
                                    {
                                        await ListBoxRemoveAll(lb_Processes, List_Processes, filterCombinedApp);
                                    });
                                    continue;
                                }
                            }

                            //Check if process is in process list and update it
                            if (existingProcessApp != null)
                            {
                                //Update the process title
                                if (existingProcessApp.Name != processName) { existingProcessApp.Name = processName; }

                                //Update the process running time
                                existingProcessApp.RunningTime = processRunningTime;

                                //Update the process suspended status
                                existingProcessApp.StatusSuspended = processStatusSuspended;

                                continue;
                            }

                            //Load the application image
                            BitmapImage processImageBitmap = FileToBitmapImage(new string[] { processName, processNameExeNoExt, processPathImage, processPathExe }, processWindowHandle, 90);

                            //Create new ProcessMulti list
                            List<ProcessMulti> listProcessMulti = new List<ProcessMulti>();
                            listProcessMulti.Add(processMultiNew);

                            //Add the process to the list
                            AVActions.ActionDispatcherInvoke(delegate
                            {
                                List_Processes.Add(new DataBindApp() { Type = processType, Category = AppCategory.Process, ProcessMulti = listProcessMulti, ImageBitmap = processImageBitmap, Name = processName, NameExe = processNameExe, PathExe = processPathExe, Argument = processArgument, StatusStore = processStatusStore, StatusSuspended = processStatusSuspended, RunningTime = processRunningTime });
                            });
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Failed adding Win32 application: " + ex.Message);
                        }
                    }
                }
            }
            catch { }
        }
    }
}