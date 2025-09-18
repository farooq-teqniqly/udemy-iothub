// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace simulatedDevice
{
    class SimulatedDevice
    {
        private static DeviceClient s_deviceClient;
        private static DeviceClient s_serviceClient;

        // The device connection string to authenticate the device with your IoT hub.
        private const string s_connectionString = "HostName=udemy-iot-course-hub.azure-devices.net;DeviceId=iot-dev-01;SharedAccessKey=xxxxx";
        private const string s_serviceConnectionString = "HostName=udemy-iot-course-hub.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=xxxx";

        // Async method to send simulated telemetry
        private static async void SendDeviceToCloudMessagesAsync()
        {
            Random rand = new();

            while (true)
            {

                var isError = rand.Next(0, 10) < 2;

                var telemetryDataPoint = new
                {
                    status = isError ? "error" : "success",
                    message = isError ? "device is not working" : "device is working fine"
                };

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);

                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString))
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8"
                };

                // Send the tlemetry message
                await s_deviceClient.SendEventAsync(message).ConfigureAwait(false);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        private static async Task ReceiveC2dAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");

            while (true)
            {
                Microsoft.Azure.Devices.Client.Message receivedMessage = await s_deviceClient.ReceiveAsync();

                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await s_deviceClient.CompleteAsync(receivedMessage);
            }
        }

        private static async Task SendCloudToDeviceMessageAsync(string targetDevice)
        {
            var serviceClient = ServiceClient.CreateFromConnectionString(s_serviceConnectionString);

            var commandMessage = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes("Cloud to device message YAY!."));
            await serviceClient.SendAsync(targetDevice, commandMessage);
        }

        static async void HandleDesiredPropertiesChange()
        {
	        await s_deviceClient.SetDesiredPropertyUpdateCallbackAsync(async (desired, ctx) =>
            {
                Newtonsoft.Json.Linq.JValue fpsJson = desired["FPS"];
                var fps = fpsJson.Value;

                Console.WriteLine("Received desired FPS: {0}", fps);

            }, null);
}

        private static async Task Main()
        {
            Console.WriteLine("IoT Hub Quickstarts - Simulated device. Ctrl-C to exit.\n");

            // Connect to the IoT hub using the MQTT protocol
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
            // SendDeviceToCloudMessagesAsync();
            // await ReceiveC2dAsync();
            // await SendCloudToDeviceMessageAsync("iot-dev-01");
            HandleDesiredPropertiesChange();

            Console.ReadLine();
        }
    }
}
