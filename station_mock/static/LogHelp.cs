using System;
using log4net;
using System.Net;
using System.Configuration;

namespace station_mock
{

    public static class StaticCode
    {

    }

    public static class LogHelp
    {
        public static readonly string AppName = "StationMockService";
        public static readonly string AppMode = "LOCAL";

        public static EndPoint localEndpoint;
        public static EndPoint remoteEndPoint;

        public static readonly ILog log = LogManager.GetLogger(AppName);

        private enum DirectionEnum
        {
            read,
            write
        }
        public enum LogLevel
        {
            OFF, //- nothing gets logged (cannot be called)
            fatal,
            error,
            warn,
            info,
            debug,
            ALL //- everything gets logged(cannot be called)
        }

        private static void logline(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.fatal: log.Fatal(message); break;
                case LogLevel.error: log.Error(message); break;
                case LogLevel.warn:  log.Warn(message); break;
                case LogLevel.info:  log.Info(message); break;
                case LogLevel.debug: log.Debug(message); break;
                default:             log.Debug(message); break;
            }
        }

        private static void LogReadwrite(Guid clientID, DirectionEnum direction, string logtext, LogLevel logLevel = LogLevel.info)
        {
            // [StationMockService][LOCAL] 198.54.233.154:50581 <= 172.24.6.17:3000 
            logline(logLevel, $"[{AppName}][{AppMode}] " +
                   $"{remoteEndPoint} " +
                   $"{(direction.Equals(DirectionEnum.read) ? "(=" : "=)")} " +
                   $"{localEndpoint} " +
                   $"\"{logtext}\" ");
        }

        public static string LogText(string logtext)
        {
            return $"[{Environment.MachineName}] {logtext}";
        }
        public static void LogLine(string logtext, LogLevel logLevel = LogLevel.info)
        {
            logline(logLevel, LogText(logtext));
        }

        public static void LogRead(Guid clientID, string logtext, LogLevel logLevel = LogLevel.info)
        {
             LogReadwrite(clientID, DirectionEnum.read, logtext, logLevel);
        }
        public static void LogWrite(Guid clientID, string logtext, LogLevel logLevel = LogLevel.info)
        {
             LogReadwrite(clientID, DirectionEnum.write, logtext, logLevel);
        }
    }
    public static class ConfigStatic
    {
        #region ConfigSettings
        public static ConfigSetting GetConfigSetting()
        {
            var useConfigSetting = UseConfigSetting.ToLower().Equals("true");
            var useAutoIPconfig = UseConfigSetting.ToLower().Equals("auto");
            var hostIPAddress = ConfigurationManager.AppSettings["HostIPAddress"];
            var port = ConfigurationManager.AppSettings["Port"];

            try
            {
                hostIPAddress = Environment.GetEnvironmentVariable("SOCKET_ADDRESS");
                port = Environment.GetEnvironmentVariable("SOCKET_PORT");
                LogHelp.log.Info(LogHelp.LogText($"Connection to Host {Environment.GetEnvironmentVariable("SOCKET_ADDRESS")} @ Port {Environment.GetEnvironmentVariable("SOCKET_PORT")}"));
            }
            catch
            {
            }

            return new ConfigSetting
            {
                UseConfigSetting = Convert.ToBoolean(useConfigSetting),
                UseAutoIPconfig = Convert.ToBoolean(useAutoIPconfig),
                HostIPAddress = hostIPAddress,
                Port = Convert.ToInt32(port)
            };
        }
        public static string HostIpAddress
        {
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
        #endregion
    }

    public class ConfigSetting
    {
        public bool UseAutoIPconfig { get; set; }
        public bool UseConfigSetting { get; set; }
        public string HostIPAddress { get; set; }
        public int Port { get; set; }
    }

}
