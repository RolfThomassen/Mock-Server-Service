namespace station_mock.Pump
{
    public class PumpStoryStatus
    {
        public PumpStoryStatus()
        {
        }
        public PumpStoryStatus(PumpStatusEnum StoryStatus)
        {
        }

        public PumpStatusEnum StoryStatus { get => pumpStoryStatus; set => pumpStoryStatus = value; }
        public long TimeToNextStep { get => timeToNextStep; set => timeToNextStep = value; }
        public PumpStatusEnum NextStepStatus { get => nextStepStatus; set => nextStepStatus = value; }
        public bool CanChangeStatus { get => canChangeStatus; set => canChangeStatus = value; }
        public bool CanCancelPump { get => canCancelPump; set => canCancelPump = value; }
        public bool Disabled { get => disabled; set => disabled = value; }


        private bool disabled;
        private bool canCancelPump;
        private bool canChangeStatus = true;
        private PumpStatusEnum nextStepStatus;
        private PumpStatusEnum pumpStoryStatus;
        private long  timeToNextStep = 10; //seconds


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

        #endregion
    }

}
