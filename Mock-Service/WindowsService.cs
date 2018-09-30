using System.ServiceProcess;
using System.Timers;
using log4net;
using station_mock;
using station_mock.Net;
using station_mock.Net.SocketV3;
using Mock_Service;
using System.Reflection;
using System.Runtime.InteropServices;
using System;

namespace WindowsService
{
    partial class WindowsService : ServiceBase
    {
        public static readonly ILog log = log4net.LogManager.GetLogger("Mock_Server_Service");

        private System.ComponentModel.IContainer components = null;
        protected static ServerSocket socket = null;
        static Timer aTimer = new Timer();

        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public WindowsService()
        {
            var allOk = false;

            this.ServiceName = "StationMockService";
            this.EventLog.Log = "Application";
            
            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;

            var setting = ConfigStatic.GetConfigSetting();
            if (setting.UseConfigSetting)
            {
                socket = new ServerSocket(setting.HostIPAddress, setting.Port);
                log.Debug(LogHelp.LogText($"Socket Loaded {setting.HostIPAddress}:{setting.Port}"));
                allOk = true;
            }
            else if (setting.UseAutoIPconfig)
            {
                var ipAddresses = IPAddressResolver.GetList();
                foreach (var item in ipAddresses) { ConfigStatic.HostIpAddress = item.Address; }
                ConfigStatic.HostPort = "3000";
                ConfigStatic.UseConfigSetting = "true";
                socket = new ServerSocket(setting.HostIPAddress, setting.Port);
                log.Info(LogHelp.LogText($"Socket Loaded {setting.HostIPAddress}:{setting.Port}"));
                allOk = true;
            }

            if (allOk)
            {
                log.Debug(LogHelp.LogText($"Start... Initialize"));
                socket.Initialize();

                log.Debug(LogHelp.LogText($"Start... Stories"));

                log.Debug(LogHelp.LogText($"Start... Timer"));
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.Interval = 5000;
                aTimer.Enabled = true;

                log.Debug(LogHelp.LogText($"Start... StartListening"));
                socket.StartListening();
                log.Debug(LogHelp.LogText($"Start... OK!"));
            }

        }

        /// <summary>
        /// Dispose of objects that need it here.
        /// </summary>
        /// <param name="disposing">Whether or not disposing is going on.</param>
        protected override void Dispose(bool disposing)
        {
            log.Debug(LogHelp.LogText($"Dispose..."));
            base.Dispose(disposing);
        }

        /// <summary>
        /// OnStart(): Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            //log.Debug(LogHelp.LogText($"OnStart..."));
            //base.OnStart(args);

            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            return;
        }

        private static readonly object padlock = new object();
        // Specify what you want to happen when the Elapsed event is raised.
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                aTimer.Stop();
                lock (padlock)
                {
                    socket.TimerTickTask();
                }
            }
            finally
            {
                aTimer.Start();
            }

        }

        /// <summary>
        /// OnStop(): Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            log.Debug(LogHelp.LogText($"OnStop..."));
            base.OnStop();
        }

        /// <summary>
        /// OnPause: Put your pause code here
        /// - Pause working threads, etc.
        /// </summary>
        protected override void OnPause()
        {
            log.Debug(LogHelp.LogText($"OnPause..."));
            base.OnPause();
        }

        /// <summary>
        /// OnContinue(): Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
            log.Debug(LogHelp.LogText($"OnContinue..."));
            base.OnContinue();
        }

        /// <summary>
        /// OnShutdown(): Called when the System is shutting down
        /// - Put code here when you need special handling
        ///   of code that deals with a system shutdown, such
        ///   as saving special data before shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            log.Debug(LogHelp.LogText($"OnShutdown..."));
            base.OnShutdown();
        }

        /// <summary>
        /// OnCustomCommand(): If you need to send a command to your
        ///   service without the need for Remoting or Sockets, use
        ///   this method to do custom methods.
        /// </summary>
        /// <param name="command">Arbitrary Integer between 128 & 256</param>
        protected override void OnCustomCommand(int command)
        {
            //  A custom command can be sent to a service by using this method:
            //#  int command = 128; //Some Arbitrary number between 128 & 256
            //#  ServiceController sc = new ServiceController("NameOfService");
            //#  sc.ExecuteCommand(command);

            log.Debug(LogHelp.LogText($"OnCustomCommand...({command})"));
            base.OnCustomCommand(command);
        }

        /// <summary>
        /// OnPowerEvent(): Useful for detecting power status changes,
        ///   such as going into Suspend mode or Low Battery for laptops.
        /// </summary>
        /// <param name="powerStatus">The Power Broadcast Status
        /// (BatteryLow, Suspend, etc.)</param>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            log.Debug(LogHelp.LogText($"OnPowerEvent...({powerStatus.ToString()})"));
            return base.OnPowerEvent(powerStatus);
        }



        /// <summary>
        /// OnSessionChange(): To handle a change event
        ///   from a Terminal Server session.
        ///   Useful if you need to determine
        ///   when a user logs in remotely or logs off,
        ///   or when someone logs into the console.
        /// </summary>
        /// <param name="changeDescription">The Session Change
        /// Event that occured.</param>
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            log.Debug(LogHelp.LogText($"OnSessionChange...({changeDescription.ToString()})"));
            base.OnSessionChange(changeDescription);
        }

        private void InitializeComponent()
        {
            // 
            // WindowsService
            // 
            components = new System.ComponentModel.Container();
            this.ServiceName = LogHelp.AppName; // "MockStationService";


        }
        
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}