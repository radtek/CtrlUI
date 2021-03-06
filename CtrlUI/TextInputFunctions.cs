﻿using ArnoldVinkCode.Styles;
using System.Threading.Tasks;
using System.Windows;
using static CtrlUI.AppVariables;
using static LibraryShared.FocusFunctions;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Hide or show the TextInput
        async Task<string> Popup_ShowHide_TextInput(string textTitle, string textDefault, string buttonTitle, bool focusButton)
        {
            try
            {
                //Check if the popup is open
                if (vTextInputOpen)
                {
                    await Popup_Close_Top();
                    return string.Empty;
                }

                //Reset text input variables
                vTextInputCancelled = false;
                vTextInputResult = string.Empty;
                vTextInputOpen = true;

                //Update the text input title
                if (!string.IsNullOrWhiteSpace(textTitle))
                {
                    grid_Popup_TextInput_textblock_Title.Text = textTitle;
                }
                else
                {
                    grid_Popup_TextInput_textblock_Title.Text = "Text Input";
                }

                //Update the button title
                if (!string.IsNullOrWhiteSpace(buttonTitle))
                {
                    grid_Popup_TextInput_button_ConfirmText.Content = buttonTitle;
                }
                else
                {
                    grid_Popup_TextInput_button_ConfirmText.Content = "Return and use the entered text";
                }

                //Reset the popup to defaults
                await Popup_Reset_TextInput(false, textDefault);

                //Play the opening sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                //Save the previous focus element
                FrameworkElementFocusSave(vTextInputElementFocus, null);

                //Show the popup
                Popup_Show_Element(grid_Popup_TextInput);

                //Force focus on element
                if (focusButton)
                {
                    //Focus on the confirm button
                    await FrameworkElementFocus(grid_Popup_TextInput_button_ConfirmText, false, vProcessCurrent.MainWindowHandle);
                }
                else
                {
                    //Focus on the text input box
                    await FrameworkElementFocus(grid_Popup_TextInput_textbox, false, vProcessCurrent.MainWindowHandle);

                    //Launch the keyboard controller
                    if (vAppActivated && vControllerAnyConnected())
                    {
                        await KeyboardControllerHideShow(true);
                    }
                }

                //Wait for user input
                while (vTextInputResult == string.Empty && !vTextInputCancelled) { await Task.Delay(500); }
                if (vTextInputCancelled) { return string.Empty; }

                //Close and reset the popup
                await Popup_Close_TextInput();
            }
            catch { }
            return vTextInputResult;
        }

        //Reset the popup to defaults
        async Task Popup_Reset_TextInput(bool focusTextbox, string textDefault)
        {
            try
            {
                //Reset the text input
                if (!string.IsNullOrWhiteSpace(textDefault))
                {
                    //Enter text and mouse selection
                    grid_Popup_TextInput_textbox.Text = textDefault;
                    grid_Popup_TextInput_textbox.SelectionStart = grid_Popup_TextInput_textbox.Text.Length;

                    //Force focus on element
                    await FrameworkElementFocus(grid_Popup_TextInput_textbox, false, vProcessCurrent.MainWindowHandle);
                }
                else if (focusTextbox)
                {
                    //Empty the textbox text
                    grid_Popup_TextInput_textbox.Text = string.Empty;

                    //Force focus on element
                    await FrameworkElementFocus(grid_Popup_TextInput_textbox, false, vProcessCurrent.MainWindowHandle);
                }
                else
                {
                    string placeholderString = (string)grid_Popup_TextInput_textbox.GetValue(TextboxPlaceholder.PlaceholderProperty);
                    grid_Popup_TextInput_textbox.Text = placeholderString;
                }

                //Show or hide the keyboard icon
                if (CheckKeyboardEnabled())
                {
                    grid_Popup_TextInput_button_KeyboardControllerIcon.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_Popup_TextInput_button_KeyboardControllerIcon.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
        }

        async Task Popup_Close_TextInput()
        {
            try
            {
                if (vTextInputOpen)
                {
                    //Play the closing sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                    //Reset the popup variables
                    vTextInputCancelled = true;
                    //vTextInputResult = string.Empty;
                    vTextInputOpen = false;

                    //Hide the popup
                    Popup_Hide_Element(grid_Popup_TextInput);

                    //Focus on the previous focus element
                    await FrameworkElementFocusFocus(vTextInputElementFocus, vProcessCurrent.MainWindowHandle);
                }
            }
            catch { }
        }

        //Validate and set the text input result
        async Task ValidateSetTextInput()
        {
            try
            {
                string textboxString = grid_Popup_TextInput_textbox.Text;
                string placeholderString = (string)grid_Popup_TextInput_textbox.GetValue(TextboxPlaceholder.PlaceholderProperty);
                if (textboxString == placeholderString)
                {
                    await Notification_Send_Status("Rename", "Invalid text");
                    vTextInputResult = string.Empty;
                }
                else
                {
                    vTextInputResult = textboxString;
                }
            }
            catch { }
        }
    }
}