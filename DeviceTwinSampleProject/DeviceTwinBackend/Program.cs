using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace DeviceTwinBackend
{
    class Program
    {
        static RegistryManager registryManager;

        // CHANGE THE CONNECTION STRING TO THE ACTUAL CONNETION STRING OF THE IOT HUB (SERVICE POLICY)
        static string connectionString = "";

        public static async Task SetDeviceTags(string deviceId)
        {
            var twin = await registryManager.GetTwinAsync(deviceId);

            var patch =
                @"{
                        tags: {
                            location:  {
                                country: 'USA',
                                city: 'Seattle'
                            }
                        },
                        properties: {
                            desired:  {
                                FPS: 30
                            }
                        }
                    }";

            await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Device Twin backend...");
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            SetDeviceTags("iot-dev-01").Wait();
            Console.WriteLine("Hit Enter to exit...");
            Console.ReadLine();
        }
    }
}
