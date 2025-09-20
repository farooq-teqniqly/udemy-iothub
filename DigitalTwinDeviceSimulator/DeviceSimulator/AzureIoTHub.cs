using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace DeviceSimulator
{
    public static class AzureIoTHub
    {
        /// <summary>
        /// Please replace with correct device connection string
        /// The device connect string could be got from Azure IoT Hub -> Devices -> {your device name } -> Connection string
        /// </summary>
        private const string deviceConnectionString =
            "HostName=udemy-iot-course-hub.azure-devices.net;DeviceId=iot-dev-dps-01;SharedAccessKey=xxxx";

        public static async Task SendDeviceToCloudMessageAsync(CancellationToken cancelToken)
        {
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);

            double avgTemperature = 70.0D;
            var rand = new Random();

            while (!cancelToken.IsCancellationRequested)
            {
                double currentTemperature = avgTemperature + rand.NextDouble() * 4 - 3;

                var telemetryDataPoint = new { Temperature = currentTemperature };
                var messageString = JsonSerializer.Serialize(telemetryDataPoint);
                var message = new Microsoft.Azure.Devices.Client.Message(
                    Encoding.UTF8.GetBytes(messageString)
                )
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8",
                };
                await deviceClient.SendEventAsync(message);
                Console.WriteLine($"{DateTime.Now} > Sending message: {messageString}");

                //Keep this value above 1000 to keep a safe buffer above the ADT service limits
                //See https://aka.ms/adt-limits for more info
                await Task.Delay(1000);
            }
        }
    }
}
