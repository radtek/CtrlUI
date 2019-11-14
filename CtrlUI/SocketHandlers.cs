﻿using ArnoldVinkCode;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static ArnoldVinkCode.AVClassConverters;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle received socket data
        public async Task ReceivedSocketHandler(TcpClient tcpClient, byte[] receivedBytes)
        {
            try
            {
                async void TaskAction()
                {
                    try
                    {
                        await ReceivedSocketHandlerThread(tcpClient, receivedBytes);
                    }
                    catch { }
                }
                await AVActions.TaskStart(TaskAction, null);
            }
            catch { }
        }

        async Task ReceivedSocketHandlerThread(TcpClient tcpClient, byte[] receivedBytes)
        {
            try
            {
                //Deserialize the received bytes
                SocketSendContainer DeserializedBytes = DeserializeBytesToClass<SocketSendContainer>(receivedBytes);

                //Get the source server ip and port
                //Debug.WriteLine("Received socket from (C): " + DeserializedBytes.SourceIp + ":" + DeserializedBytes.SourcePort + "/" + DeserializedBytes.Object);

                //Check what kind of object was received
                if (DeserializedBytes.Object is ControllerInput)
                {
                    if (!vControllerBusy)
                    {
                        vControllerBusy = true;

                        ControllerInput receivedControllerInput = (ControllerInput)DeserializedBytes.Object;
                        await ControllerInteraction(receivedControllerInput);

                        vControllerBusy = false;
                    }
                }
                else if (DeserializedBytes.Object is List<ControllerStatusSummary>)
                {
                    List<ControllerStatusSummary> controllerStatusSummaryList = (List<ControllerStatusSummary>)DeserializedBytes.Object;
                    UpdateControllerStatus(controllerStatusSummaryList);
                }
            }
            catch { }
        }
    }
}