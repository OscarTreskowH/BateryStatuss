
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Timers;
using System.Windows.Forms;
using Serilog;

namespace BateryTest
{
    public partial class BatteryService : ServiceBase
    {
        public BatteryService()
        {
            InitializeComponent();
            string logDirectory = ConfigurationManager.AppSettings["LogDirectory"];
            string logFile = ConfigurationManager.AppSettings["LogFile"];
            string logPath = System.IO.Path.Combine(logDirectory, logFile);

            if (!System.IO.Directory.Exists(logDirectory))
            {
                System.IO.Directory.CreateDirectory(logDirectory);
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Level:u3} | {Message:lj} | Battery %: {batteryPorcentaje:0.0} | Status: {BatteryStatus} | {NewLine}{Exception}",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
                )
                .CreateLogger();
        }
        
        protected override void OnStart(string[] args)
        {
            Log.Information("Servicio Iniciado...");
            timer2.Interval = 10000; 
            timer2.Enabled = true;
            timer2.Start();
        }

        protected override void OnStop()
        {
            Log.Information("Servicio Detenido...");
            timer2.Stop();
        }

        private double GetCpuLoad()
        {
            var cpuCounter = new System.Diagnostics.PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            return cpuCounter.NextValue();
        }

        private double GetAvailableMemory()
        {
            var memoryCounter = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
            return memoryCounter.NextValue(); 
        }


        private void timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            var batteryStatus = SystemInformation.PowerStatus;
            var batteryPorcentaje = batteryStatus.BatteryLifePercent * 100;
            var batteryStatusMessage = batteryStatus.BatteryChargeStatus.ToString();

            Log.Information("Porcentaje de batería: {batteryPorcentaje:0.0}% | Estado: {batteryStatusMessage} | Hora: {Timestamp} | Carga: {CPU_Load}% | Memoria: {Memory_Free}MB",
                batteryPorcentaje, 
                batteryStatusMessage, 
                DateTime.Now.ToString("HH:mm:ss"), 
                GetCpuLoad(), 
                GetAvailableMemory());
        }
    }


}
