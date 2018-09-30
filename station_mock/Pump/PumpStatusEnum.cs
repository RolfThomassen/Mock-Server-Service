using System;
using System.Collections.Generic;

namespace station_mock.Pump
{
    public enum PumpStatusEnum
    {
        Unknown,
        Idle,
        Reserved,
        Authorized,
        NozzlePickUp,
        StartedDispend,
        Dispensing,
        DispenseCompleted,
        Finalized,
        NotAuthorized, NoAuthorise = NotAuthorized,
        Blocked, PumpBlocked = Blocked,
        NoResponse, NoRespond = NoResponse,

        Disabled = 255
    }

    public static class PumpStatusData
    {
        public static PumpStatusEnum GetPumpStatus(string pumpStatus)
        {
            switch (pumpStatus)
            {
                case "02":
                    return PumpStatusEnum.Idle;
                case "0#":
                    return PumpStatusEnum.Reserved;
                case "0:":
                case "1:":
                    return PumpStatusEnum.Authorized;
                case "0;":
                    return PumpStatusEnum.StartedDispend;
                case "0?":
                case "1?":
                case "2?":
                    return PumpStatusEnum.Dispensing;
                case "92":
                case "12":
                    return PumpStatusEnum.DispenseCompleted;
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

        public static Dictionary<PumpStatusEnum, string> dicPumpStatus = new Dictionary<PumpStatusEnum, string>()
        {
            {PumpStatusEnum.Idle, "02"},
            {PumpStatusEnum.Reserved, "0#"},
            {PumpStatusEnum.Authorized, "0:"},
            {PumpStatusEnum.NozzlePickUp, "0;"},
            {PumpStatusEnum.Dispensing, "0?"},
            {PumpStatusEnum.DispenseCompleted, "12"},
            {PumpStatusEnum.Finalized, "42"},
            {PumpStatusEnum.PumpBlocked, "82"},
            {PumpStatusEnum.NoAuthorise, "23"},
            {PumpStatusEnum.PumpBlocked, "82"},
            {PumpStatusEnum.Disabled, "7F"},
            {PumpStatusEnum.Unknown , "FF"}
        };
        public static string GetPumpStatusEnum(PumpStatusEnum pumpStatusEnum)
        {
            switch (pumpStatusEnum)
            {
                case PumpStatusEnum.Idle: return "02";
                case PumpStatusEnum.Reserved: return "0#";
                case PumpStatusEnum.Authorized: return "0:";
                case PumpStatusEnum.NozzlePickUp: return "0;";
                case PumpStatusEnum.StartedDispend: return "0;";
                case PumpStatusEnum.Dispensing: return "0?";
                case PumpStatusEnum.DispenseCompleted: return "12";
                case PumpStatusEnum.Finalized: return "42";
                case PumpStatusEnum.NoRespond: return "00";
                case PumpStatusEnum.NoAuthorise: return "03";
                case PumpStatusEnum.PumpBlocked: return "82";
                case PumpStatusEnum.Disabled: return "7F";
                default: return $"[{pumpStatusEnum.ToString()}|{(int)pumpStatusEnum}]";
            }
        }

        public static string GetPumpStatusEnumText(PumpStatusEnum pumpStatusEnum)
        {
            try
            {
                return Enum.GetName(typeof(PumpStatusEnum), (int)pumpStatusEnum);
                //switch (pumpStatusEnum)
                //{
                //    case PumpStatusEnum.Idle: return "Idle";
                //    case PumpStatusEnum.Reserved: return "Reserved";
                //    case PumpStatusEnum.Authorized: return "Authorized";
                //    case PumpStatusEnum.NozzlePickUp: return "NozzlePickUp";
                //    case PumpStatusEnum.StartedDispend: return "StartedDispend";
                //    case PumpStatusEnum.Dispensing: return "Dispensing";
                //    case PumpStatusEnum.DispenseCompleted: return "DispenseCompleted";
                //    case PumpStatusEnum.Finalized: return "Finalized";
                //    case PumpStatusEnum.NoRespond: return "NoRespond";
                //    case PumpStatusEnum.NoAuthorise: return "NoAuthorise";
                //    case PumpStatusEnum.PumpBlocked: return "PumpBlocked";
                //    case PumpStatusEnum.Disabled: return "Disabled";
                //    default: return $"[{pumpStatusEnum.ToString()}|{(int)pumpStatusEnum}]";
                //}
            }
            catch {
                //    Console.WriteLine("The 4th value of the Styles Enum is {0}", Enum.GetName(typeof(Styles), 3));
                //string res = Enum.GetName(typeof(PumpStatusEnum), (int)pumpStatusEnum);
                return $"[{(int)pumpStatusEnum}]";
                //return $"[{pumpStatusEnum.ToString()}|{(int)pumpStatusEnum}]";
            }

        }
    }
}
