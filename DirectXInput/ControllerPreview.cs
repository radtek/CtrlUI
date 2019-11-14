﻿using ArnoldVinkCode;
using System;
using System.Windows;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Update interface controller preview
        void ControllerPreview(ControllerStatus Controller)
        {
            try
            {
                //Update the interface when window is active
                if (vAppActivated && !vAppMinimized && Controller.Manage)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        try
                        {
                            //Update latency
                            int Latency = Environment.TickCount - Controller.LastActive;
                            if (Latency > 0)
                            {
                                double LatencyMs = new TimeSpan(Latency).TotalMilliseconds;
                                txt_ManageControllerLatency.Text = LatencyMs + " ms";
                            }

                            //Update battery
                            if (Controller.BatteryPercentageCurrent == -2)
                            {
                                txt_ManageControllerBattery.Text = "Battery charging";
                            }
                            else if (Controller.BatteryPercentageCurrent >= 0)
                            {
                                txt_ManageControllerBattery.Text = "Battery is at " + Controller.BatteryPercentageCurrent + "%";
                            }
                            else
                            {
                                txt_ManageControllerBattery.Text = "Battery unknown";
                            }

                            //D-Pad
                            if (Controller.InputCurrent.DPadLeft) { img_ControllerPreview_DPadLeft.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadLeft.Visibility = Visibility.Collapsed; }
                            if (Controller.InputCurrent.DPadUp) { img_ControllerPreview_DPadUp.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadUp.Visibility = Visibility.Collapsed; }
                            if (Controller.InputCurrent.DPadRight) { img_ControllerPreview_DPadRight.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadRight.Visibility = Visibility.Collapsed; }
                            if (Controller.InputCurrent.DPadDown) { img_ControllerPreview_DPadDown.Visibility = Visibility.Visible; } else { img_ControllerPreview_DPadDown.Visibility = Visibility.Collapsed; }

                            //Buttons
                            if (Controller.InputCurrent.ButtonA) { img_ControllerPreview_ButtonA.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonA.Visibility = Visibility.Collapsed; }
                            if (Controller.InputCurrent.ButtonB) { img_ControllerPreview_ButtonB.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonB.Visibility = Visibility.Collapsed; }
                            if (Controller.InputCurrent.ButtonX) { img_ControllerPreview_ButtonX.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonX.Visibility = Visibility.Collapsed; }
                            if (Controller.InputCurrent.ButtonY) { img_ControllerPreview_ButtonY.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonY.Visibility = Visibility.Collapsed; }

                            if (Controller.InputCurrent.ButtonBack) { img_ControllerPreview_ButtonBack.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonBack.Visibility = Visibility.Collapsed; }
                            if (Controller.InputCurrent.ButtonStart) { img_ControllerPreview_ButtonStart.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonStart.Visibility = Visibility.Collapsed; }
                            if (Controller.InputCurrent.ButtonGuide) { img_ControllerPreview_ButtonGuide.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonGuide.Visibility = Visibility.Collapsed; }

                            if (Controller.InputCurrent.ButtonShoulderLeft) { img_ControllerPreview_ButtonShoulderLeft.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonShoulderLeft.Visibility = Visibility.Collapsed; }
                            if (Controller.InputCurrent.ButtonShoulderRight) { img_ControllerPreview_ButtonShoulderRight.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonShoulderRight.Visibility = Visibility.Collapsed; }

                            if (Controller.InputCurrent.ButtonThumbLeft) { img_ControllerPreview_ButtonThumbLeft.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonThumbLeft.Visibility = Visibility.Collapsed; }
                            if (Controller.InputCurrent.ButtonThumbRight) { img_ControllerPreview_ButtonThumbRight.Visibility = Visibility.Visible; } else { img_ControllerPreview_ButtonThumbRight.Visibility = Visibility.Collapsed; }

                            //Triggers
                            if (!Controller.Details.Profile.UseButtonTriggers)
                            {
                                img_ControllerPreview_TriggerLeft.Opacity = (double)(Controller.InputCurrent.TriggerLeft * 257) / 65535;
                                img_ControllerPreview_TriggerRight.Opacity = (double)(Controller.InputCurrent.TriggerRight * 257) / 65535;
                            }
                            else
                            {
                                if (Controller.InputCurrent.ButtonTriggerLeft) { img_ControllerPreview_TriggerLeft.Opacity = 1.00; } else { img_ControllerPreview_TriggerLeft.Opacity = 0.00; }
                                if (Controller.InputCurrent.ButtonTriggerRight) { img_ControllerPreview_TriggerRight.Opacity = 1.00; } else { img_ControllerPreview_TriggerRight.Opacity = 0.00; }
                            }

                            //Thumb Left and Right Fix
                            int LeftX = Controller.InputCurrent.ThumbLeftX / 1000;
                            int LeftY = -Controller.InputCurrent.ThumbLeftY / 1000;
                            int RightX = Controller.InputCurrent.ThumbRightX / 1000;
                            int RightY = -Controller.InputCurrent.ThumbRightY / 1000;
                            img_ControllerPreview_LeftAxe.Margin = new Thickness(LeftX, LeftY, 0, 0);
                            img_ControllerPreview_RightAxe.Margin = new Thickness(RightX, RightY, 0, 0);
                        }
                        catch { }
                    });
                }
            }
            catch { }
        }
    }
}