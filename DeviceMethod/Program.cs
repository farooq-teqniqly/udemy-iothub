using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace AzCourse.IoT.Method
{
    public class Program
    {
        const string DEVICE_CONNECTION_STRING =
            "HostName=udemy-iot-course-hub.azure-devices.net;DeviceId=iot-dev-dps-01;SharedAccessKey=xxxx";

        public static async Task<int> Main(string[] args)
        {
            using var deviceClient = DeviceClient.CreateFromConnectionString(
                DEVICE_CONNECTION_STRING,
                TransportType.Amqp
            );
            var sample = new MethodSample(deviceClient);
            await sample.RunMethod();
            await deviceClient.CloseAsync();

            Console.WriteLine("Done.");
            return 0;
        }
    }
}
