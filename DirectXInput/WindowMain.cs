﻿using ArnoldVinkCode;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.ProcessFunctions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Startup
        public async Task Startup()
        {
            try
            {
                //Initialize Settings
                Settings_Check();
                await Settings_Load();
                Settings_Save();

                //Create the tray menu
                Application_CreateTrayMenu();

                //Check settings if window needs to be shown
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["AppFirstLaunch"]))
                {
                    Debug.WriteLine("First launch showing the window.");
                    Application_ShowHideWindow();
                }

                //Check if application has launched as admin
                if (vAdministratorPermission)
                {
                    this.Title += " (Admin)";
                }

                //Load the help text
                LoadHelp();

                //Register Interface Handlers
                RegisterInterfaceHandlers();

                //Load other controller tools
                JsonLoadAppsOtherTools();

                //Close running controller tools
                CloseControllerTools();

                //Load controllers profile
                JsonLoadControllerProfile();

                //Load controllers supported
                JsonLoadList_ControllerSupported();

                //Check xbox bus driver status
                await CheckX360Bus();

                //Reset HidGuardian to defaults
                HidGuardianResetDefaults();

                //Allow DirectXInput process in HidGuardian
                HidGuardianAllowProcess();

                //Start application tasks
                TasksBackgroundStart();

                //Set application first launch to false
                SettingSave("AppFirstLaunch", "False");

                //Enable the socket server
                EnableSocketServer();

                Debug.WriteLine("Application has launched.");
            }
            catch { }
        }

        //Enable the socket server
        private void EnableSocketServer()
        {
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = "CtrlUI.exe.Config";
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

                int SocketServerPort = Convert.ToInt32(config.AppSettings.Settings["ServerPort"].Value) + 1;

                vArnoldVinkSockets = new ArnoldVinkSockets("127.0.0.1", SocketServerPort);
                vArnoldVinkSockets.EventBytesReceived += ReceivedSocketHandler;
            }
            catch { }
        }

        //Map button to a certain control
        async void Btn_MapController_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus ManageController = GetManageController();
                if (ManageController != null)
                {
                    Button SendButton = sender as Button;

                    //Set button to map
                    ManageController.Mapping[0] = "Map";
                    ManageController.Mapping[1] = SendButton.Tag.ToString();

                    //Disable interface
                    txt_Application_Status.Text = "Waiting for " + SendButton.Tag + " press on the controller... ";
                    pb_UpdateProgress.IsIndeterminate = true;
                    grid_ControllerPreview.IsEnabled = false;
                    grid_ControllerPreview.Opacity = 0.50;

                    //Create timeout timer
                    int CountdownTimeout = 0;
                    vDispatcherTimer.Stop();
                    vDispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                    vDispatcherTimer.Tick += delegate
                    {
                        if (CountdownTimeout++ >= 10)
                        {
                            //Reset controller button mapping
                            ManageController.Mapping[0] = "Cancel";
                            ManageController.Mapping[1] = "None";
                        }
                        else
                        {
                            txt_Application_Status.Text = "Waiting for " + SendButton.Tag + " press on the controller... " + (11 - CountdownTimeout).ToString() + "sec.";
                        }
                    };
                    vDispatcherTimer.Start();

                    //Check if button is mapped
                    while (ManageController.Mapping[0] == "Map") { await Task.Delay(500); }
                    vDispatcherTimer.Stop();

                    if (ManageController.Mapping[0] == "Done") { txt_Application_Status.Text = "Changed " + SendButton.Tag + " to the pressed controller button."; }
                    else { txt_Application_Status.Text = "Cancelled button mapping, please select a button to change:"; }

                    //Enable interface
                    pb_UpdateProgress.IsIndeterminate = false;
                    grid_ControllerPreview.IsEnabled = true;
                    grid_ControllerPreview.Opacity = 1.00;
                }
            }
            catch { }
        }

        //Test the rumble button
        async void Btn_TestRumble_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus ManageController = GetManageController();
                if (ManageController != null && !vControllerRumbleTest)
                {
                    vControllerRumbleTest = true;
                    Button SendButton = sender as Button;

                    if (SendButton.Name == "btn_RumbleTestLight") { SendXRumbleData(ManageController, true, true, false); } else { SendXRumbleData(ManageController, true, false, true); }
                    await Task.Delay(1000);
                    SendXRumbleData(ManageController, true, false, false);

                    vControllerRumbleTest = false;
                }
            }
            catch { }
        }

        //Disconnect and stop the controller
        async void Btn_DisconnectController_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus ManageController = GetManageController();
                if (ManageController != null)
                {
                    await StopController(ManageController, false);
                }
            }
            catch { }
        }

        //Remove the controller from the list
        async void Btn_RemoveController_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus ManageController = GetManageController();
                if (ManageController != null)
                {
                    int Result = await MessageBoxPopup("Do you really want to remove this controller?", "This will reset the manage controller to it's defaults and disconnect it.", "Remove controller", "Cancel", "", "");
                    if (Result == 1)
                    {
                        Debug.WriteLine("Removed the controller: " + ManageController.Details.DisplayName);
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            txt_Controller_Information.Text = "Removed the controller: " + ManageController.Details.DisplayName;
                        });

                        List_ControllerProfile.Remove(ManageController.Details.Profile);
                        await StopController(ManageController, false);

                        //Save changes to Json file
                        JsonSaveControllerProfile();
                    }
                }
            }
            catch { }
        }

        //Monitor window state changes
        void CheckWindowStateAndSize(object sender, EventArgs e)
        {
            try
            {
                if (WindowState == WindowState.Minimized) { Application_ShowHideWindow(); }
            }
            catch { }
        }

        //Close other running controller tools
        void CloseControllerTools()
        {
            try
            {
                Debug.WriteLine("Closing other running controller tools.");
                foreach (string CloseTool in vAppsOtherTools)
                {
                    try
                    {
                        CloseProcessesByName(CloseTool, false);
                    }
                    catch { }
                }
            }
            catch { }
        }

        //Application Close Handler
        protected async override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                await Application_Exit(false);
            }
            catch { }
        }

        //Close the application
        async Task<bool> Application_Exit(bool SilentClose)
        {
            try
            {
                int Result = 2; //Cancel
                if (!SilentClose) { Result = await MessageBoxPopup("Do you really want to close DirectXInput?", "This will disconnect all your currently connected controllers.", "Close", "Cancel", "", ""); }
                else { Result = 1; }

                if (Result == 1)
                {
                    Debug.WriteLine("Exiting DirectXInput.");

                    CloseProcessesByName("KeyboardController", false);
                    TasksBackgroundStop();
                    await StopAllControllers();

                    //Disable the socket server
                    await vArnoldVinkSockets.SocketServerDisable();

                    TrayNotifyIcon.Visible = false;
                    Environment.Exit(0);
                    return true;
                }
                else { return false; }
            }
            catch { return true; }
        }
    }
}