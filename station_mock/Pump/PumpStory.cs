using System.Collections;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace station_mock.Pump
{
    public class PumpStory  : IEnumerable
    {

        public PumpStory()
        {
        }

        /// <summary>
        /// Set story line parameter
        /// </summary>
        /// <param name="name">Description of story line, what happens here</param>
        /// <param name="pumpstatus">Curent Pump Status</param>
        /// <param name="nexttick">How long until next tick, (0=no step)</param>
        /// <param name="nextstatus">This will be next step Status of pump</param>
        /// <param name="nextStoryLine">This is number of next story line</param>
        /// <param name="statusTimeOut">Automatic Timeout of status (0=no auto)</param>
        /// <param name="canChangeStatus">Story line allows pump to change status</param>
        /// <param name="canResetPump">Story line allows Pump to Reset</param>
        public PumpStory(
                            string name,
                            PumpStatusEnum pumpstatus,
                            int nexttick,
                            PumpStatusEnum nextstatus,
                            int nextStoryLine,
                            long statusTimeOut,
                            bool canChangeStatus,
                            bool canResetPump
                        )
        {
            pumpStoryName = name; Name = name;
            Status = pumpstatus;
            nextTick = nexttick;
            NextStatus = nextstatus;
            PumpStatusTimeOut = statusTimeOut;
            CanChangeStatus = canChangeStatus;
            CanResetPump = canResetPump;
            NextStoryLine = nextStoryLine;
        }

        /// <summary>
        /// Set story line parameter
        /// </summary>
        /// <param name="name">Description of story line, what happens here</param>
        /// <param name="pumpstatus">Curent Pump Status</param>
        /// <param name="nexttick">How long until next tick, (0=no step)</param>
        /// <param name="nextstatus">This will be next step Status of pump</param>
        /// <param name="nextStoryLine">This is number of next story line</param>
        /// <param name="statusTimeOut">Automatic Timeout of status (0=no auto)</param>
        /// <param name="canChangeStatus">Story line allows pump to change status</param>
        /// <param name="canResetPump">Story line allows Pump to Reset</param>
        /// <param name="volume">Story line Set Volume</param>
        /// <param name="amount">Story line Set Amount</param>
        public PumpStory(
                            string name,
                            PumpStatusEnum pumpstatus,
                            int nexttick,
                            PumpStatusEnum nextstatus,
                            int nextStoryLine,
                            long statusTimeOut,
                            bool canChangeStatus,
                            bool canResetPump,
                            decimal volume,
                            decimal amount
                        )
        {
            pumpStoryName = name; Name = name;
            Status = pumpstatus;
            nextTick = nexttick;
            NextStatus = nextstatus;
            PumpStatusTimeOut = statusTimeOut;
            CanChangeStatus = canChangeStatus;
            CanResetPump = canResetPump;
            NextStoryLine = nextStoryLine;
            Volume = volume;
            Amount = amount;
        }


        /// <summary>
        /// Set story line parameter
        /// </summary>
        /// <param name="name">Description of story line, what happens here</param>
        /// <param name="pumpstatus">Curent Pump Status</param>
        /// <param name="nexttick">How long until next tick, (0=no step)</param>
        /// <param name="nextstatus">This will be next step Status of pump</param>
        /// <param name="nextStoryLine">This is number of next story line</param>
        /// <param name="statusTimeOut">Automatic Timeout of status (0=no auto)</param>
        /// <param name="canChangeStatus">Story line allows pump to change status</param>
        /// <param name="canResetPump">Story line allows Pump to Reset</param>
        /// <param name="pctOfLimit">Story line Set Pct Of Limit to return as Amount</param>
        /// <param name="ppu">Story line Set Price Per Unit</param>
        public PumpStory(
                            string name,
                            PumpStatusEnum pumpstatus,
                            int nexttick,
                            PumpStatusEnum nextstatus,
                            int nextStoryLine,
                            long statusTimeOut,
                            bool canChangeStatus,
                            bool canResetPump,
                            double pctOfLimit,
                            double ppu
                        )
        {
            pumpStoryName = name; Name = name;
            Status = pumpstatus;
            nextTick = nexttick;
            NextStatus = nextstatus;
            PumpStatusTimeOut = statusTimeOut;
            CanChangeStatus = canChangeStatus;
            CanResetPump = canResetPump;
            NextStoryLine = nextStoryLine;
            PctOfLimit =  pctOfLimit;
            Ppu = ppu;
        }

        /// <summary>
        /// Description of story line, what happens here
        /// </summary>
        public string PumpStoryName { get => pumpStoryName; set => pumpStoryName = value; }
        /// <summary>
        /// Not used at the moment
        /// </summary>
        public int PumpStoryID { get => pumpStoryID; set => pumpStoryID = value; }
        /// <summary>
        /// Pump Number
        /// </summary>
        public int PumpNo { get => pumpNo; set => pumpNo = value; }

        /// <summary>
        /// Description of story line, what happens here
        /// </summary>
        public string Name { get => name; set => name = value; }
        /// <summary>
        /// Storyline PumpStatus
        /// </summary>
        public PumpStatusEnum Status { get => status; set => status = value; }
        /// <summary>
        /// How long until automatic excute next tick, or not if value = 0
        /// </summary>
        public int NextTick { get => nextTick; set => nextTick = value; }
        /// <summary>
        /// This will be pump next value after nextTick
        /// </summary>
        public PumpStatusEnum NextStatus { get => nextStatus; set => nextStatus = value; }
        /// <summary>
        /// How long time until pumpStory line times out, or no time out if value = 0
        /// </summary>
        public long PumpStatusTimeOut { get => pumpStatusTimeOut; set => pumpStatusTimeOut = value; }
        /// <summary>
        /// Can NextTick change pump status
        /// </summary>
        public bool CanChangeNextStatus { get => canChangeNextStatus; set => canChangeNextStatus = value; }
        /// <summary>
        /// Can Pump Change Status
        /// </summary>
        public bool CanChangeStatus { get => canChangeStatus; set => canChangeStatus = value; }
        /// <summary>
        /// Can pump be reset or is it lock in current status
        /// </summary>
        public bool CanResetPump { get => canResetPump; set => canResetPump = value; }
        /// <summary>
        /// Next Storyline after this one
        /// </summary>
        public int NextStoryLine { get => nextStoryLine; set => nextStoryLine = value; }

        public decimal Volume { get => volume; set => volume = value; }
        public decimal Amount { get => amount; set => amount = value; }
        public double Ppu { get => ppu; set => ppu = value; }
        public double PctOfLimit { get => pctOfLimit; set => pctOfLimit = value; }

        private double pctOfLimit;
        private double ppu;
        private decimal amount;
        private decimal volume;

        private int nextStoryLine;
        private bool canResetPump;
        private bool canChangeStatus;
        private bool canChangeNextStatus;
        private long pumpStatusTimeOut;
        private int pumpStoryID;
        private int pumpNo;
        private string pumpStoryName;
        private string name;
        private PumpStatusEnum status;
        private int nextTick;
        private PumpStatusEnum nextStatus;


        #region Override
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }

}
