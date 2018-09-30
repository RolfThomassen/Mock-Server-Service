using System.Collections.Generic;

namespace station_mock_service.Pump
{
    public enum PumpStatusEnum
    {
        Idle,
        Reserved,
        Authorized,
        NozzlePickUp,
        StartedDispend,
        Dispensing,
        DispenseCompleted,
        NotAuthorized,
        Blocked,
        NoResponse,
        Unknown,
        Disabled
    }

    public static class PumpStatus
    {
        public static PumpStatusEnum GetPumpStatus(string pumpStatus)
        {
            switch (pumpStatus)
            {
                case "02":
                    return PumpStatusEnum.Idle;
                case "0#":
                    return PumpStatusEnum.Reserved;
                case "0;":
                    return PumpStatusEnum.StartedDispend;
                case "0?":
                case "1?":
                case "2?":
                    return PumpStatusEnum.Dispensing;
                case "92":
                case "12":
                    return PumpStatusEnum.DispenseCompleted;
                case "0:":
                case "1:":
                    return PumpStatusEnum.Authorized;
                case "00":
                    return PumpStatusEnum.NoRespond;
                case "03":
                case "13":
                case "23":
                    return PumpStatusEnum.NoAuthorise;
                case "82":
                    return PumpStatusEnum.PumpBlocked;
                case "7F":
                    return PumpStatusEnum.Disabled;
                default:
                    return PumpStatusEnum.Unknown;
            }
        }

        public static Dictionary<int, string> pumpStatus = new Dictionary<int, string>()
        {
            {(int)PumpStatusEnum.Idle, "02"},
            {(int)PumpStatusEnum.Reserved, "0#"},
            {(int)PumpStatusEnum.Authorized, "0:"},
            {(int)PumpStatusEnum.NozzlePickUp, "0;"},
            {(int)PumpStatusEnum.Dispensing, "0?"},
            {(int)PumpStatusEnum.DispenseCompleted, "12"},
            {(int)PumpStatusEnum.Disabled, "7F"},
            {(int)PumpStatusEnum.Unknown , "FF"}
        };
    }

}
