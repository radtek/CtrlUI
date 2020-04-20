﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.ProcessClasses;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.AppStartupCheck;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        async void Button_AddAppLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check if there is an application name set
                if (string.IsNullOrWhiteSpace(tb_AddAppName.Text) || tb_AddAppName.Text == "Select application executable file first")
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please enter an application name first", "", "", Answers);
                    return;
                }

                vFilePickerFilterIn = new List<string> { "jpg", "png" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Application Image";
                vFilePickerDescription = "Please select a new application image:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Load the new application image
                BitmapImage applicationImage = FileToBitmapImage(new string[] { vFilePickerResult.PathFile }, IntPtr.Zero, 120, 0);

                //Update the new application image
                img_AddAppLogo.Source = applicationImage;
                if (vEditAppDataBind != null)
                {
                    vEditAppDataBind.ImageBitmap = applicationImage;
                }

                //Copy the new application image
                File_Copy(vFilePickerResult.PathFile, "Assets\\Apps\\" + tb_AddAppName.Text + ".png", true);
            }
            catch { }
        }

        async void Button_AddAppExePath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vFilePickerFilterIn = new List<string> { "exe" };
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Application Executable";
                vFilePickerDescription = "Please select an application executable:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = true;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                //Set fullpath to exe path textbox
                tb_AddAppExePath.Text = vFilePickerResult.PathFile;
                tb_AddAppExePath.IsEnabled = true;

                //Set application launch path to textbox
                tb_AddAppPathLaunch.Text = Path.GetDirectoryName(vFilePickerResult.PathFile);
                tb_AddAppPathLaunch.IsEnabled = true;
                btn_AddAppPathLaunch.IsEnabled = true;

                //Set application name to textbox
                tb_AddAppName.Text = vFilePickerResult.Name.Replace(".exe", "");
                tb_AddAppName.IsEnabled = true;

                //Set application image to image preview
                img_AddAppLogo.Source = FileToBitmapImage(new string[] { tb_AddAppName.Text, vFilePickerResult.PathFile }, IntPtr.Zero, 120, 0);
                btn_AddAppLogo.IsEnabled = true;
                btn_Manage_ResetAppLogo.IsEnabled = true;
            }
            catch { }
        }

        async void Button_AddAppPathLaunch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vFilePickerFilterIn = new List<string>();
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Launch Folder";
                vFilePickerDescription = "Please select the launch folder:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = false;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                tb_AddAppPathLaunch.Text = vFilePickerResult.PathFile;
                tb_AddAppPathLaunch.IsEnabled = true;
                btn_AddAppPathLaunch.IsEnabled = true;
            }
            catch { }
        }

        async void Button_AddAppPathRoms_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vFilePickerFilterIn = new List<string>();
                vFilePickerFilterOut = new List<string>();
                vFilePickerTitle = "Rom Folder";
                vFilePickerDescription = "Please select the rom folder:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = false;
                vFilePickerShowDirectories = true;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("PC", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }

                tb_AddAppPathRoms.Text = vFilePickerResult.PathFile;
                tb_AddAppPathRoms.IsEnabled = true;
            }
            catch { }
        }

        //Update manage interface according to category
        void Lb_Manage_AddAppCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Get the currently selected category
                AppCategory selectedAddCategory = (AppCategory)lb_Manage_AddAppCategory.SelectedIndex;

                //Check if the application is UWP
                bool UwpApplication = sp_AddAppExePath.Visibility == Visibility.Collapsed;

                if (UwpApplication || selectedAddCategory != AppCategory.Emulator)
                {
                    sp_AddAppPathRoms.Visibility = Visibility.Collapsed;
                }
                else
                {
                    sp_AddAppPathRoms.Visibility = Visibility.Visible;
                }

                if (UwpApplication || selectedAddCategory != AppCategory.App)
                {
                    checkbox_AddLaunchFilePicker.Visibility = Visibility.Collapsed;
                }
                else
                {
                    checkbox_AddLaunchFilePicker.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        //Reset the application image
        void Button_Manage_ResetAppLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (vEditAppDataBind != null)
                {
                    string imageFileTitle = "Assets\\Apps\\" + vEditAppDataBind.Name + ".png";
                    string imageFileExe = "Assets\\Apps\\" + Path.GetFileNameWithoutExtension(vEditAppDataBind.PathExe) + ".png";

                    //Check the application image
                    bool defaultImage = AssetsAppsFiles.Contains(imageFileTitle) || AssetsAppsFiles.Contains(imageFileExe);
                    if (defaultImage)
                    {
                        Popup_Show_Status("Close", "Cannot reset image");
                        Debug.WriteLine("Default application images cannot be reset.");
                        return;
                    }

                    //Remove application image files
                    File_Delete(imageFileTitle);
                    File_Delete(imageFileExe);

                    //Reload the application image
                    BitmapImage applicationImage = FileToBitmapImage(new string[] { vEditAppDataBind.Name, vEditAppDataBind.PathExe, vEditAppDataBind.PathImage }, IntPtr.Zero, 120, 0);
                    img_AddAppLogo.Source = applicationImage;
                    vEditAppDataBind.ImageBitmap = applicationImage;

                    Popup_Show_Status("Restart", "App image reset");
                    Debug.WriteLine("App image reset: " + vEditAppDataBind.Name);
                }
                else
                {
                    string imageFileTitle = "Assets\\Apps\\" + tb_AddAppName.Text + ".png";
                    string imageFileExe = "Assets\\Apps\\" + Path.GetFileNameWithoutExtension(tb_AddAppExePath.Text) + ".png";

                    //Check the application image
                    bool defaultImage = AssetsAppsFiles.Contains(imageFileTitle) || AssetsAppsFiles.Contains(imageFileExe);
                    if (defaultImage)
                    {
                        Popup_Show_Status("Close", "Cannot reset image");
                        Debug.WriteLine("Default application images cannot be reset.");
                        return;
                    }

                    //Remove application image files
                    File_Delete(imageFileTitle);
                    File_Delete(imageFileExe);

                    //Reload the application image
                    BitmapImage applicationImage = FileToBitmapImage(new string[] { tb_AddAppName.Text, tb_AddAppExePath.Text }, IntPtr.Zero, 120, 0);
                    img_AddAppLogo.Source = applicationImage;
                }
            }
            catch { }
        }

        //Add or edit application to list apps and Json file
        async void Button_Manage_SaveEditApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check the selected application category
                AppCategory selectedAddCategory = (AppCategory)lb_Manage_AddAppCategory.SelectedIndex;

                //Check if there is an application name set
                if (string.IsNullOrWhiteSpace(tb_AddAppName.Text) || tb_AddAppName.Text == "Select application executable file first")
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please enter an application name first", "", "", Answers);
                    return;
                }

                //Check if there is an application exe set
                if (string.IsNullOrWhiteSpace(tb_AddAppExePath.Text))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Please select an application executable file first", "", "", Answers);
                    return;
                }

                //Prevent CtrlUI from been added to the list
                if (tb_AddAppExePath.Text.Contains("CtrlUI.exe") || tb_AddAppExePath.Text.Contains("CtrlUI-Admin.exe"))
                {
                    List<DataBindString> Answers = new List<DataBindString>();
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                    Answer1.Name = "Alright";
                    Answers.Add(Answer1);

                    await Popup_Show_MessageBox("Sorry, you can't add CtrlUI as an application", "", "", Answers);
                    return;
                }

                //Check if the application paths exist for Win32 apps
                if (vEditAppDataBind != null && vEditAppDataBind.Type == ProcessType.Win32)
                {
                    //Validate the launch target
                    if (!File.Exists(tb_AddAppExePath.Text))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Application exe not found, please select another one", "", "", Answers);
                        return;
                    }

                    //Validate the launch path
                    if (!string.IsNullOrWhiteSpace(tb_AddAppPathLaunch.Text) && !Directory.Exists(tb_AddAppPathLaunch.Text))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("Launch folder not found, please select another one", "", "", Answers);
                        return;
                    }

                    //Check if application is emulator and validate the rom path
                    if (selectedAddCategory == AppCategory.Emulator)
                    {
                        if (string.IsNullOrWhiteSpace(tb_AddAppPathRoms.Text))
                        {
                            List<DataBindString> Answers = new List<DataBindString>();
                            DataBindString Answer1 = new DataBindString();
                            Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                            Answer1.Name = "Alright";
                            Answers.Add(Answer1);

                            await Popup_Show_MessageBox("Please select an emulator rom folder first", "", "", Answers);
                            return;
                        }
                        if (!Directory.Exists(tb_AddAppPathRoms.Text))
                        {
                            List<DataBindString> Answers = new List<DataBindString>();
                            DataBindString Answer1 = new DataBindString();
                            Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                            Answer1.Name = "Alright";
                            Answers.Add(Answer1);

                            await Popup_Show_MessageBox("Rom folder not found, please select another one", "", "", Answers);
                            return;
                        }
                    }
                }

                //Check if application needs to be edited or added
                string BtnTextContent = (sender as Button).Content.ToString();
                if (BtnTextContent == "Add the application as filled in above")
                {
                    //Check if new application already exists
                    if (CombineAppLists(false, false).Any(x => x.Name.ToLower() == tb_AddAppName.Text.ToLower() || x.PathExe.ToLower() == tb_AddAppExePath.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("This application already exists", "", "", Answers);
                        return;
                    }

                    PlayInterfaceSound("Confirm", false);

                    Popup_Show_Status("Plus", "Added " + tb_AddAppName.Text);
                    Debug.WriteLine("Adding Win32 app: " + tb_AddAppName.Text + " to the list.");
                    DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Win32, Category = selectedAddCategory, Name = tb_AddAppName.Text, PathExe = tb_AddAppExePath.Text, PathLaunch = tb_AddAppPathLaunch.Text, PathRoms = tb_AddAppPathRoms.Text, Argument = tb_AddAppArgument.Text, LaunchFilePicker = (bool)checkbox_AddLaunchFilePicker.IsChecked, LaunchKeyboard = (bool)checkbox_AddLaunchKeyboard.IsChecked };
                    await AddAppToList(dataBindApp, true, true);

                    //Close the open popup
                    await Popup_Close_Top();

                    //Focus on the application list
                    if (selectedAddCategory == AppCategory.Game)
                    {
                        await ListboxFocusIndex(lb_Games, false, true, -1);
                    }
                    else if (selectedAddCategory == AppCategory.App)
                    {
                        await ListboxFocusIndex(lb_Apps, false, true, -1);
                    }
                    else if (selectedAddCategory == AppCategory.Emulator)
                    {
                        await ListboxFocusIndex(lb_Emulators, false, true, -1);
                    }
                }
                else
                {
                    //Check if application name already exists
                    if (vEditAppDataBind.Name.ToLower() == tb_AddAppName.Text.ToLower())
                    {
                        Debug.WriteLine("Application name has not changed or just caps.");
                    }
                    else if (CombineAppLists(false, false).Any(x => x.Name.ToLower() == tb_AddAppName.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("This application name already exists, please use another one", "", "", Answers);
                        return;
                    }

                    //Check if application executable already exists
                    if (vEditAppDataBind.PathExe == tb_AddAppExePath.Text)
                    {
                        Debug.WriteLine("Application executable has not changed.");
                    }
                    else if (CombineAppLists(false, false).Any(x => x.PathExe.ToLower() == tb_AddAppExePath.Text.ToLower()))
                    {
                        List<DataBindString> Answers = new List<DataBindString>();
                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Check.png" }, IntPtr.Zero, -1, 0);
                        Answer1.Name = "Alright";
                        Answers.Add(Answer1);

                        await Popup_Show_MessageBox("This application executable already exists, please select another one", "", "", Answers);
                        return;
                    }

                    //Rename application logo based on name and reload it
                    string imageFileNameOld = "Assets\\Apps\\" + vEditAppDataBind.Name + ".png";
                    if (vEditAppDataBind.Name != tb_AddAppName.Text && File.Exists(imageFileNameOld))
                    {
                        Debug.WriteLine("App name changed and application logo file exists.");
                        string imageFileNameNew = "Assets\\Apps\\" + tb_AddAppName.Text + ".png";

                        //Check the application image
                        bool defaultImage = AssetsAppsFiles.Contains(imageFileNameOld);
                        if (defaultImage)
                        {
                            Debug.WriteLine("Default application images cannot be renamed.");
                            File_Copy(imageFileNameOld, imageFileNameNew, true);
                        }
                        else
                        {
                            File_Move(imageFileNameOld, imageFileNameNew, true);
                        }
                    }

                    //Edit application in the list
                    vEditAppDataBind.Category = selectedAddCategory;
                    vEditAppDataBind.Name = tb_AddAppName.Text;
                    vEditAppDataBind.PathExe = tb_AddAppExePath.Text;
                    vEditAppDataBind.PathLaunch = tb_AddAppPathLaunch.Text;
                    vEditAppDataBind.PathRoms = tb_AddAppPathRoms.Text;
                    vEditAppDataBind.Argument = tb_AddAppArgument.Text;
                    vEditAppDataBind.LaunchFilePicker = (bool)checkbox_AddLaunchFilePicker.IsChecked;
                    vEditAppDataBind.LaunchKeyboard = (bool)checkbox_AddLaunchKeyboard.IsChecked;
                    vEditAppDataBind.StatusAvailable = Visibility.Collapsed;
                    vEditAppDataBind.ImageBitmap = FileToBitmapImage(new string[] { vEditAppDataBind.Name, vEditAppDataBind.PathExe, vEditAppDataBind.PathImage }, IntPtr.Zero, 90, 0);

                    Popup_Show_Status("Edit", "Edited " + vEditAppDataBind.Name);
                    Debug.WriteLine("Editing application: " + vEditAppDataBind.Name + " in the list.");

                    //Save changes to Json file
                    JsonSaveApplications();

                    //Close the open popup
                    await Popup_Close_Top();
                    await Task.Delay(500);

                    //Focus on the application list
                    if (vEditAppCategoryPrevious != vEditAppDataBind.Category)
                    {
                        Debug.WriteLine("App category changed to: " + vEditAppDataBind.Category);

                        //Remove app from previous category
                        if (vEditAppCategoryPrevious == AppCategory.Game)
                        {
                            await ListBoxRemoveItem(lb_Games, List_Games, vEditAppDataBind, false);
                        }
                        else if (vEditAppCategoryPrevious == AppCategory.App)
                        {
                            await ListBoxRemoveItem(lb_Apps, List_Apps, vEditAppDataBind, false);
                        }
                        else if (vEditAppCategoryPrevious == AppCategory.Emulator)
                        {
                            await ListBoxRemoveItem(lb_Emulators, List_Emulators, vEditAppDataBind, false);
                        }

                        //Add application to new category
                        if (vEditAppDataBind.Category == AppCategory.Game)
                        {
                            await ListBoxAddItem(lb_Games, List_Games, vEditAppDataBind, false, false);
                        }
                        else if (vEditAppDataBind.Category == AppCategory.App)
                        {
                            await ListBoxAddItem(lb_Apps, List_Apps, vEditAppDataBind, false, false);
                        }
                        else if (vEditAppDataBind.Category == AppCategory.Emulator)
                        {
                            await ListBoxAddItem(lb_Emulators, List_Emulators, vEditAppDataBind, false, false);
                        }

                        //Focus on the edited item listbox
                        if (vSearchOpen)
                        {
                            await ListboxFocusIndex(lb_Search, false, false, -1);
                        }
                        else
                        {
                            if (vEditAppDataBind.Category == AppCategory.Game) { await ListboxFocusIndex(lb_Games, false, true, -1); }
                            else if (vEditAppDataBind.Category == AppCategory.App) { await ListboxFocusIndex(lb_Apps, false, true, -1); }
                            else if (vEditAppDataBind.Category == AppCategory.Emulator) { await ListboxFocusIndex(lb_Emulators, false, true, -1); }
                        }
                    }
                    else
                    {
                        //Focus on the item listbox
                        if (vSearchOpen)
                        {
                            await ListboxFocusIndex(lb_Search, false, false, -1);
                        }
                        else
                        {
                            if (vEditAppDataBind.Category == AppCategory.Game) { await ListboxFocusIndex(lb_Games, false, false, -1); }
                            else if (vEditAppDataBind.Category == AppCategory.App) { await ListboxFocusIndex(lb_Apps, false, false, -1); }
                            else if (vEditAppDataBind.Category == AppCategory.Emulator) { await ListboxFocusIndex(lb_Emulators, false, false, -1); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("App edit failed: " + ex.Message);
            }
        }

        void Btn_Manage_MoveAppRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MoveApplicationList_Right();
            }
            catch { }
        }

        void Btn_Manage_MoveAppLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MoveApplicationList_Left();
            }
            catch { }
        }
    }
}