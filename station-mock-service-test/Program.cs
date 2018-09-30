using System;
using System.Text;
using System.Configuration;
using station_mock.Net;
using station_mock.Net.SocketV3;
using station_mock.storyman;
using log4net;
using log4net.Config;
using System.Timers;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;


namespace station_mock_service
{
    class Program
    {
        public static readonly ILog log = log4net.LogManager.GetLogger("Program"); //(MethodBase.GetCurrentMethod().DeclaringType);
        public static ServerSocket socket = null;

        static void Main(string[] args)
        {
            var ipAddresses = IPAddressResolver.GetList();
            var setting = GetConfigSetting();
            var allOk = false;

            #region IP
            #endregion

            if (setting.UseConfigSetting)
            {
                socket = new ServerSocket(setting.HostIPAddress, setting.Port);
                log.Debug($"Socket Loaded {setting.HostIPAddress}:{setting.Port}");
                allOk = true;
            }
            else if (setting.UseAutoIPconfig)
            {
                //var ipAddresses = IPAddressResolver.GetList();
                foreach (var item in ipAddresses)
                {
                    HostIpAddress = item.Address;
                }
                HostPort = "3000";
                UseConfigSetting = "true";
            }
            else
            {
                var counter = 0;

                var sb = new StringBuilder();
                sb.AppendFormat("Choose your IP Address from list below:\n");

                foreach (var item in ipAddresses)
                {
                    counter += 1;
                    sb.AppendFormat($"{counter}. {item.Address}\n");
                }

                Console.Write(sb.ToString());
                Console.Write(">>");
                var input = Console.ReadLine();

                int result = -1;
                try
                {
                    if (int.TryParse(input, out result))
                    {
                        var indexer = result - 1;
                        var indexLocator = ipAddresses[indexer];

                        Console.WriteLine("Enter Port Number");
                        Console.Write(">>");

                        var portInput = Console.ReadLine();
                        if (int.TryParse(portInput, out result))
                        {
                            socket = new ServerSocket(indexLocator.Address, result);
                            log.Debug($"Socket Loaded {setting.HostIPAddress}:{setting.Port}");
                            allOk = true;
                        }
                        else
                        {
                            log.Error("Invalid input...");
                            allOk = false;
                        }
                    }
                    else
                    {
                        log.Error("Invalid input...");
                        allOk = false;
                    }
                }
                catch (ArgumentOutOfRangeException AEX)
                {
                    log.Error($"Invalid input...{AEX.Message}");
                    allOk = false;
                }
                catch (Exception ex)
                {
                    log.Error($"Invalid input...{ex.Message}");
                    allOk = false;
                }

                //var ipAddress = IPAddressResolver.GetList().FirstOrDefault(x => x.IsIPV4);
            }

            if (allOk)
            {
                log.Debug($"Start...");
                socket.Initialize();

                log.Debug($"Start... Stories");
                TimerMain();
                //storyman.StoryHandler;


                log.Debug($"Initialize...");
                socket.StartListening();
                log.Debug($"Start Listening...");
            }
        }

        private static ConfigSetting GetConfigSetting()
        {
            var useConfigSetting = UseConfigSetting.ToLower().Equals("true");  //ConfigurationManager.AppSettings["UseConfigSetting"].ToLower().Equals("true");
            var useAutoIPconfig = UseConfigSetting.ToLower().Equals("auto");  //ConfigurationManager.AppSettings["UseConfigSetting"].ToLower().Equals("auto") ;
            var hostIPAddress = ConfigurationManager.AppSettings["HostIPAddress"];
            var port = ConfigurationManager.AppSettings["Port"];

            return new ConfigSetting
            {
                UseConfigSetting = Convert.ToBoolean(useConfigSetting),
                UseAutoIPconfig = Convert.ToBoolean(useAutoIPconfig),
                HostIPAddress = hostIPAddress,
                Port = Convert.ToInt32(port)
            };
        }
        public static string HostIpAddress {
            get => GetSetting("HostIPAddress");
            set => SetSetting("HostIPAddress", value);
        }
        public static string HostPort
        {
            get => GetSetting("Port");
            set => SetSetting("Port", value);
        }
        public static string UseConfigSetting
        {
            get => GetSetting("UseConfigSetting");
            set => SetSetting("UseConfigSetting", value);
        }

        private static string GetSetting(string key)
        { return (" " + ConfigurationManager.AppSettings[key]).Trim(); }

        private static void SetSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings.Remove(key);
            configuration.AppSettings.Settings.Add(key, value);
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }
       
        public static void TimerMain()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 5000;
            aTimer.Enabled = true;
            log.Debug($"Start... Timer");
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            socket.TimerTickTask();
        }

    }

    public class ConfigSetting
    {
        public bool UseAutoIPconfig { get; set; }
        public bool UseConfigSetting { get; set; }
        public string HostIPAddress { get; set; }
        public int Port { get; set; }
    }


}