﻿using ArnoldVinkCode;
using System;
using System.Threading.Tasks;
using System.Windows;
using static CtrlUI.AppVariables;
using static LibraryShared.FocusFunctions;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the popup
        void Popup_Show_Element(FrameworkElement elementTarget)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Show the popup
                    elementTarget.Visibility = Visibility.Visible;
                    elementTarget.IsEnabled = true;

                    //Hide the background
                    grid_Video_Background.Opacity = 0.08;
                    grid_Main.Opacity = 0.08;
                    grid_Main.IsEnabled = false;

                    //Hide other popups
                    if (elementTarget != grid_Popup_TextInput && vTextInputOpen)
                    {
                        grid_Popup_TextInput.Opacity = 0.02;
                        grid_Popup_TextInput.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_MessageBox && vMessageBoxOpen)
                    {
                        grid_Popup_MessageBox.Opacity = 0.02;
                        grid_Popup_MessageBox.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_FilePicker && vFilePickerOpen)
                    {
                        grid_Popup_FilePicker.Opacity = 0.02;
                        grid_Popup_FilePicker.IsEnabled = false;
                    }
                    if (elementTarget != vPopupElementTarget && vPopupOpen)
                    {
                        vPopupElementTarget.Opacity = 0.02;
                        vPopupElementTarget.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_ColorPicker && vColorPickerOpen)
                    {
                        grid_Popup_ColorPicker.Opacity = 0.02;
                        grid_Popup_ColorPicker.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_Search && vSearchOpen)
                    {
                        grid_Popup_Search.Opacity = 0.02;
                        grid_Popup_Search.IsEnabled = false;
                    }
                    if (elementTarget != grid_Popup_MainMenu && vMainMenuOpen)
                    {
                        grid_Popup_MainMenu.Opacity = 0.02;
                        grid_Popup_MainMenu.IsEnabled = false;
                    }
                });
            }
            catch { }
        }

        //Hide the popup
        void Popup_Hide_Element(FrameworkElement elementTarget)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Hide the popup
                    elementTarget.Visibility = Visibility.Collapsed;
                    elementTarget.IsEnabled = false;

                    //Show the background
                    if (!Popup_Any_Open())
                    {
                        double backgroundBrightness = (double)Convert.ToInt32(Setting_Load(vConfigurationCtrlUI, "BackgroundBrightness")) / 100;
                        grid_Video_Background.Opacity = backgroundBrightness;
                        grid_Main.Opacity = 1.00;
                        grid_Main.IsEnabled = true;
                        return;
                    }

                    //Show other popups
                    if (vTextInputOpen)
                    {
                        grid_Popup_TextInput.Opacity = 1.00;
                        grid_Popup_TextInput.IsEnabled = true;
                    }
                    else if (vMessageBoxOpen)
                    {
                        grid_Popup_MessageBox.Opacity = 1.00;
                        grid_Popup_MessageBox.IsEnabled = true;
                    }
                    else if (vFilePickerOpen)
                    {
                        grid_Popup_FilePicker.Opacity = 1.00;
                        grid_Popup_FilePicker.IsEnabled = true;
                    }
                    else if (vPopupOpen)
                    {
                        vPopupElementTarget.Opacity = 1.00;
                        vPopupElementTarget.IsEnabled = true;
                    }
                    else if (vColorPickerOpen)
                    {
                        grid_Popup_ColorPicker.Opacity = 1.00;
                        grid_Popup_ColorPicker.IsEnabled = true;
                    }
                    else if (vSearchOpen)
                    {
                        grid_Popup_Search.Opacity = 1.00;
                        grid_Popup_Search.IsEnabled = true;
                    }
                    else if (vMainMenuOpen)
                    {
                        grid_Popup_MainMenu.Opacity = 1.00;
                        grid_Popup_MainMenu.IsEnabled = true;
                    }
                });
            }
            catch { }
        }

        //Hide or show the main menu
        async Task Popup_ShowHide_MainMenu(bool ForceShow)
        {
            try
            {
                if (vMainMenuOpen)
                {
                    await Popup_Close_Top();
                    return;
                }

                if (ForceShow)
                {
                    await Popup_Close_All();
                }

                if (Popup_Any_Open())
                {
                    return;
                }

                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                //Save the previous focus element
                FrameworkElementFocusSave(vMainMenuElementFocus, null);

                //Show the popup
                Popup_Show_Element(grid_Popup_MainMenu);

                vMainMenuOpen = true;

                //Focus on the menu listbox
                await ListboxFocusIndex(listbox_MainMenu, false, false, -1, vProcessCurrent.MainWindowHandle);

                //Update the clock with date
                UpdateClockTime();

                //Update the controller help
                UpdateControllerHelp();
            }
            catch { }
        }

        //Close all the open popups
        async Task Popup_Close_MainMenu()
        {
            try
            {
                if (vMainMenuOpen)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                    //Reset popup variables
                    vMainMenuOpen = false;

                    //Hide the popup
                    Popup_Hide_Element(grid_Popup_MainMenu);

                    //Update the clock without date
                    UpdateClockTime();

                    //Focus on the previous focus element
                    await FrameworkElementFocusFocus(vMainMenuElementFocus, vProcessCurrent.MainWindowHandle);
                }
            }
            catch { }
        }

        //Show specific popup
        async Task Popup_Show(FrameworkElement ShowPopup, FrameworkElement FocusElement)
        {
            try
            {
                if (!vPopupOpen)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                    //Update popup variables
                    vPopupElementTarget = ShowPopup;

                    //Save the previous focus element
                    FrameworkElementFocusSave(vPopupElementFocus, null);

                    //Show the popup
                    Popup_Show_Element(ShowPopup);

                    vPopupOpen = true;

                    //Force focus on element
                    if (FocusElement != null)
                    {
                        await FrameworkElementFocus(FocusElement, false, vProcessCurrent.MainWindowHandle);
                    }
                }
            }
            catch { }
        }

        //Close all the open popups
        async Task Popup_Close()
        {
            try
            {
                if (vPopupOpen)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                    //Reset popup variables
                    vPopupOpen = false;

                    //Hide the popup
                    Popup_Hide_Element(vPopupElementTarget);

                    //Focus on the previous focus element
                    await FrameworkElementFocusFocus(vPopupElementFocus, vProcessCurrent.MainWindowHandle);
                }
            }
            catch { }
        }

        //Close open top popup (xaml order)
        async Task Popup_Close_Top()
        {
            try
            {
                if (vTextInputOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_TextInput(); }); }
                else if (vMessageBoxOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MessageBox(); }); }
                else if (vFilePickerOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_FilePicker(false, false); }); }
                else if (vColorPickerOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_ColorPicker(); }); }
                else if (vPopupOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close(); }); }
                else if (vSearchOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_Search(); }); }
                else if (vMainMenuOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MainMenu(); }); }
            }
            catch { }
        }

        //Close all open popups (xaml order)
        async Task Popup_Close_All()
        {
            try
            {
                if (vTextInputOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_TextInput(); }); }
                if (vMessageBoxOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MessageBox(); }); }
                if (vFilePickerOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_FilePicker(false, false); }); }
                if (vColorPickerOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_ColorPicker(); }); }
                if (vPopupOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close(); }); }
                if (vSearchOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_Search(); }); }
                if (vMainMenuOpen) { await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Close_MainMenu(); }); }
            }
            catch { }
        }

        //Check if there is any popup open
        bool Popup_Any_Open()
        {
            try
            {
                if (vPopupOpen || vColorPickerOpen || vSearchOpen || vMainMenuOpen || vFilePickerOpen || vMessageBoxOpen || vTextInputOpen)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}