using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace DeviceTwinBackend
{
    class Program
    {
        static RegistryManager registryManager;

        // CHANGE THE CONNECTION STRING TO THE ACTUAL CONNETION STRING OF THE IOT HUB (SERVICE POLICY)
        static string connectionString =
            "HostName=udemy-iot-course-hub.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=xxxx";

        public static async Task SetDeviceTags()
        {
            var twin = await registryManager.GetTwinAsync("iot-dev-dps-01", "device-module-1");
            var patch =
                @"{
                        properties: {
                            desired:  {
                                speedAlertFrom: 120
                            }
                        }
                    }";
            await registryManager.UpdateTwinAsync(twin.DeviceId, twin.ModuleId, patch, twin.ETag);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Device Twin backend...");
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            SetDeviceTags().Wait();
            Console.WriteLine("Hit Enter to exit...");
            Console.ReadLine();
        }
    }
}
