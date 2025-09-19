using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace InvokeDeviceMethod
{
    internal class Program
    {
        private static JobClient jobClient;
        private static ServiceClient serviceClient;

        private static string connectionString =
            "HostName=udemy-iot-course-hub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xxxxx";
        private static string deviceId = "iot-dev-dps-01";

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Invoking direct method...");

            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

            //await InvokeDirectMethodAsync();
            await ScheduleMethodJob();

            serviceClient.Dispose();

            Console.WriteLine("\nPress Enter to exit.");
            Console.ReadLine();
        }

        // Invoke the direct method on the device, passing the payload
        private static async Task InvokeDirectMethodAsync()
        {
            var methodName = "SetFlashlightState";

            var methodInvocation = new CloudToDeviceMethod(methodName)
            {
                ResponseTimeout = TimeSpan.FromSeconds(30),
            };
            methodInvocation.SetPayloadJson("\"Off\"");

            Console.WriteLine("Invoking direct method '{0}'", methodName);

            var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);

            Console.WriteLine(
                $"\nResponse status: {response.Status}, payload:\n\t{response.GetPayloadAsJson()}"
            );
        }

        private static async Task ScheduleMethodJob()
        {
            jobClient = JobClient.CreateFromConnectionString(connectionString);

            string methodJobId = Guid.NewGuid().ToString();

            var methodName = "SetFlashlightState";

            var methodInvocation = new CloudToDeviceMethod(methodName)
            {
                ResponseTimeout = TimeSpan.FromSeconds(30),
            };
            methodInvocation.SetPayloadJson("\"On\"");

            JobResponse result = await jobClient.ScheduleDeviceMethodAsync(
                methodJobId,
                $"DeviceId IN ['{deviceId}']",
                methodInvocation,
                DateTime.UtcNow.AddSeconds(20),
                (long)TimeSpan.FromMinutes(2).TotalSeconds
            );

            Console.WriteLine("Started Method Job");

            MonitorJob(methodJobId).Wait();
            Console.WriteLine("Press ENTER to run the next job.");
            Console.ReadLine();
        }

        public static async Task MonitorJob(string jobId)
        {
            JobResponse result;
            do
            {
                result = await jobClient.GetJobAsync(jobId);
                Console.WriteLine("Job Status : " + result.Status.ToString());
                Thread.Sleep(2000);
            } while ((result.Status != JobStatus.Completed) && (result.Status != JobStatus.Failed));
        }
    }
}
