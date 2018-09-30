using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using station_mock.Pump;
using System.Reflection;
using log4net;
using station_mock;

namespace station_mock.Pump
{

    public class Pump
    {
        private static readonly ILog log = LogManager.GetLogger(LogHelp.AppName);

        //public static string LogText(string logtext)
        //{
        //    return $"[{Environment.MachineName}]{logtext}";
        //}
        public string GetPumpStatusEnum(PumpStatusEnum pumpStatusEnum)
        {
            //switch (pumpStatusEnum)
            //{
            //    case PumpStatusEnum.Idle: return "02";
            //    case PumpStatusEnum.Reserved: return "0#";
            //    case PumpStatusEnum.StartedDispend: return "0;";
            //    case PumpStatusEnum.Dispensing: return "0?";
            //    case PumpStatusEnum.DispenseCompleted: return "12";
            //    case PumpStatusEnum.Authorized: return "0:";
            //    case PumpStatusEnum.NoRespond: return "00";
            //    case PumpStatusEnum.NoAuthorise: return "03";
            //    case PumpStatusEnum.PumpBlocked: return "82";
            //    case PumpStatusEnum.Disabled: return "7F";
            //    default: return "FF";// PumpStatusEnum.Unknown;
            //}
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
        //public static string GetPumpStatusEnum(PumpStatusEnum pumpStatusEnum)
        //{
        //    switch (pumpStatusEnum)
        //    {
        //        case PumpStatusEnum.Idle: return "02";
        //        case PumpStatusEnum.Reserved: return "0#";
        //        case PumpStatusEnum.Authorized: return "0:";
        //        case PumpStatusEnum.NozzlePickUp: return "0;";
        //        case PumpStatusEnum.StartedDispend: return "0;";
        //        case PumpStatusEnum.Dispensing: return "0?";
        //        case PumpStatusEnum.DispenseCompleted: return "12";
        //        case PumpStatusEnum.Finalized: return "42";
        //        case PumpStatusEnum.NoRespond: return "00";
        //        case PumpStatusEnum.NoAuthorise: return "03";
        //        case PumpStatusEnum.PumpBlocked: return "82";
        //        case PumpStatusEnum.Disabled: return "7F";
        //        default: return $"[{pumpStatusEnum.ToString()}|{(int)pumpStatusEnum}]";
        //    }
        //}


        private PumpStatusEnum pumpStatus;
        public PumpStatusEnum PumpStatus
        {
            get { return pumpStatus; }
            set
            {
                this.pumpStatus = value;
                this.pumpTime = DateTime.Now;
            }
        }
        public string Status
        {
            get => GetPumpStatusEnum(pumpStatus);
            set
            {
                this.pumpStatus = PumpStatusData.GetPumpStatus(value);
                this.pumpTime = DateTime.Now;
            }
        }

        public int PumpNo { get => pumpNo; set => pumpNo = value; }
        public decimal Volume { get => volume; set => volume = value; }
        public decimal Amount { get => amount; set => amount = value; }
        public decimal Limit { get => limit; set => limit = value; }
        public decimal Ppu { get => ppu; set => ppu = value; }
        public int Hose { get => hose; set => hose = value; }
        public string Flag { get => flag; set => flag = value; }
        public DateTime PumpTime { get => pumpTime; set => pumpTime = value; }

        private DateTime pumpTime = DateTime.MinValue;
        private int pumpNo = 0;
        private decimal amount = 0;
        private decimal volume = 0;
        private decimal limit = 0; // Maximus amout a pump must deliver
        private decimal ppu = 0; // Price per unit ( volume / amount!=0?amount:1 )
        private int hose = 0;
        private string flag = "";

        public Pump(int pumpNo) { this.PumpNo = pumpNo; this.PumpStatus = PumpStatusEnum.Idle; }
        public Pump(int pumpNo, PumpStatusEnum pumpStatus) { this.PumpNo = pumpNo; this.PumpStatus = pumpStatus; }
        public Pump(int pumpNo, PumpStatusEnum pumpStatus, decimal amount)
        {
            this.PumpNo = pumpNo;
            this.PumpStatus = pumpStatus;
            this.Limit = amount;
        }
        public Pump(int pumpNo, PumpStatusEnum pumpStatus, decimal volume, decimal amount)
        {
            this.PumpNo = pumpNo;
            this.PumpStatus = pumpStatus;
            this.Volume = volume;
            this.Limit = amount;
        }


        public void SetLimit(decimal Limit)
        {
            limit = Limit;
        }

        public void SetAmount(decimal Amount)
        {
            amount = Amount;
            ppu = amount / (volume.Equals(0) ? 1 : volume);
        }

        public void ResetPump()
        {
            pumpStatus = PumpStatusEnum.Idle;
            volume = 0;
            limit = 0;
            pumpTime = DateTime.MinValue;
            pumpNo = 0;
            amount = 0;
            ppu = 0;
            hose = 0;
            flag = "";
        }

    }

}
