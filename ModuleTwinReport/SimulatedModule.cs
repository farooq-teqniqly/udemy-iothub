using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace simulatedDevice
{
    class SimulatedDevice
    {
        static string ModuleConnectionString =
            "HostName=udemy-iot-course-hub.azure-devices.net;DeviceId=iot-dev-dps-01;ModuleId=device-module-1;SharedAccessKey=xxxx";

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Connecting to hub");
                ModuleClient Client = ModuleClient.CreateFromConnectionString(
                    ModuleConnectionString,
                    TransportType.Mqtt
                );
                Console.WriteLine("Retrieving twin");
                var twin = await Client.GetTwinAsync();

                var reportedProperties = new TwinCollection();
                var tempControl = new TwinCollection();
                tempControl["minTemp"] = -10;
                tempControl["maxTemp"] = 70;
                reportedProperties["tempControl"] = tempControl;

                await Client.UpdateReportedPropertiesAsync(reportedProperties);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
