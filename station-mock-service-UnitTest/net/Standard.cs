using Socket.Shared.Net;
using System;
using System.Configuration;

namespace station_mock_service_UnitTest.net
{
    public static class Standard
    {
        public const byte STX = 0x02;
        public const byte ETX = 0x03;
        public const byte ACT = 0x06;


        //private static readonly string hostName = "192.168.43.25";
        //private static readonly int portNumber = 3000;


        public static ConfigSetting GetConfigSetting()
        {
            var useConfigSetting = ConfigurationManager.AppSettings["UseConfigSetting"];
            var hostIPAddress = ConfigurationManager.AppSettings["HostIPAddress"];
            var port = ConfigurationManager.AppSettings["Port"];

            return new ConfigSetting
            {
                UseConfigSetting = Convert.ToBoolean(useConfigSetting),
                HostIPAddress = hostIPAddress,
                Port = Convert.ToInt32(port)
            };
        }

    }
}
