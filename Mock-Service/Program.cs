using System.ServiceProcess;
using station_mock.Net.SocketV3;

namespace Mock_Service
{
    static class Program
    {
        public static ServerSocket socket = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            {
                new WindowsService.WindowsService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
