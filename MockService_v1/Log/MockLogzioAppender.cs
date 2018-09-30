using log4net.Core;
using Logzio.DotNet.Log4net;
using System.Collections.Generic;
using MockService_v1.Code;

namespace MockService_v1.Net
{
    public class MockLogzioAppender : LogzioAppender
    {
        protected override void ExtendValues(LoggingEvent loggingEvent, Dictionary<string, object> values)
        {
            values["service"] = "StationMockService";
            values["host"] = $"{StaticCode.HostIpAddress}:{StaticCode.HostPort}";
            values["type"] = ".NET Framework";
        }
    }

}