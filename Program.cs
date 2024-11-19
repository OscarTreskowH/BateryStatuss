
using System.ServiceProcess;

namespace BateryTest
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BatteryService()
            };
            ServiceBase.Run(ServicesToRun);
        }

    }
}
