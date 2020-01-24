﻿using System.Diagnostics;
using System.Windows;
using static LibraryShared.AppLaunchCheck;

namespace DriverInstaller
{
    public partial class App : Application
    {
        //Application Windows
        public static WindowMain vWindowMain = new WindowMain();

        //Application Startup
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Check the application update
                Application_UpdateCheck();

                //Check the application status
                Application_LaunchCheck("Driver Installer", "DriverInstaller", ProcessPriorityClass.Normal, false);

                //Open the window main from application
                vWindowMain.Show();
            }
            catch { }
        }
    }
}