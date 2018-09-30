using System.Collections.Generic;
using station_mock.Pump;

namespace station_mock.storyman
{ 
    public class StoryManager 
    {
        public Dictionary<int, Dictionary<int, PumpStory>> stationStories = new Dictionary<int, Dictionary<int, PumpStory>>();
        public Dictionary<int, int> stationStoriesLine = new Dictionary<int, int>();
        public List<PumpStory> pumpStories = new List<PumpStory>();

        public Dictionary<int, PumpStory> CreateStoryPump01()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Idel to Reserved", PumpStatusEnum.Idle, 0, PumpStatusEnum.Reserved, 2, 0, true, false) },
                { 2, new PumpStory("Pump1", PumpStatusEnum.Reserved, 0, PumpStatusEnum.Authorized, 3, 60, true, true) },
                { 3, new PumpStory("Pump1", PumpStatusEnum.Authorized, 10, PumpStatusEnum.NozzlePickUp, 4, 180, true, true , 100D, 2.2D ) },
                { 4, new PumpStory("Pump1", PumpStatusEnum.NozzlePickUp, 10, PumpStatusEnum.Dispensing, 5, 0, true, false) },
                { 5, new PumpStory("Pump1", PumpStatusEnum.Dispensing, 20, PumpStatusEnum.DispenseCompleted, 6, 0, true, false) },
                { 6, new PumpStory("Pump1", PumpStatusEnum.DispenseCompleted, 0, PumpStatusEnum.Idle, 1, 0, true, false) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump02()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Pump2", PumpStatusEnum.Idle, 0, PumpStatusEnum.Reserved, 2, 0, false, false) },
                { 2, new PumpStory("Pump2", PumpStatusEnum.Reserved, 0, PumpStatusEnum.Authorized, 3, 60, true, false) },
                { 3, new PumpStory("Pump2", PumpStatusEnum.Authorized, 10, PumpStatusEnum.NozzlePickUp, 4, 0, true, false, 60D, 2.6D) },
                { 4, new PumpStory("Pump2", PumpStatusEnum.NozzlePickUp, 5, PumpStatusEnum.Authorized, 5, 300, true, true) },
                { 5, new PumpStory("Pump2", PumpStatusEnum.Authorized, 10, PumpStatusEnum.NozzlePickUp, 6, 0, true, false) },
                { 6, new PumpStory("Pump2", PumpStatusEnum.NozzlePickUp, 10, PumpStatusEnum.Dispensing, 7, 300, true, true) },
                { 7, new PumpStory("Pump2", PumpStatusEnum.Dispensing, 7, PumpStatusEnum.DispenseCompleted, 8, 0, true, true) },
                { 8, new PumpStory("Pump2", PumpStatusEnum.DispenseCompleted, 0, PumpStatusEnum.Idle, 1, 0, true, false) },
            };        
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump03()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Pump3", PumpStatusEnum.Idle, 0, PumpStatusEnum.Reserved, 2, 0, true, false) },
                { 2, new PumpStory("Pump3", PumpStatusEnum.Reserved, 0, PumpStatusEnum.Authorized, 3, 60, true, false) },
                { 3, new PumpStory("Pump3", PumpStatusEnum.Authorized, 0, PumpStatusEnum.NozzlePickUp, 4, 180, true, false, 80D, 2.22D) },
                { 4, new PumpStory("Pump3", PumpStatusEnum.NozzlePickUp, 10, PumpStatusEnum.Dispensing, 5, 0, true, false) },
                { 5, new PumpStory("Pump3", PumpStatusEnum.Dispensing, 20, PumpStatusEnum.DispenseCompleted, 6, 0, true, false) },
                { 6, new PumpStory("Pump3", PumpStatusEnum.DispenseCompleted, 0, PumpStatusEnum.Idle, 1, 0, true, false) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump04()
        {

            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Pump4", PumpStatusEnum.Idle, 0, PumpStatusEnum.Reserved, 1, 0, false, false) }
            };            //List<PumpStory> Pump02 = new List<PumpStory>
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump05()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Pump5", PumpStatusEnum.Idle, 0, PumpStatusEnum.Reserved, 2, 0, true, false) },
                { 2, new PumpStory("Pump5", PumpStatusEnum.Reserved, 0, PumpStatusEnum.Authorized , 1, 60, false, true) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump06()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Pump6", PumpStatusEnum.Idle, 0, PumpStatusEnum.Reserved, 2, 0, true, false) },
                { 2, new PumpStory("Pump6", PumpStatusEnum.Reserved, 0, PumpStatusEnum.Authorized, 3, 60, true, true) },
                { 3, new PumpStory("Pump6", PumpStatusEnum.Authorized, 0, PumpStatusEnum.NozzlePickUp, 1, 180, false, false, 60D, 2.6D) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump07()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Pump7", PumpStatusEnum.Idle, 0, PumpStatusEnum.Reserved, 2, 0, true, false) },
                { 2, new PumpStory("Pump7", PumpStatusEnum.Reserved, 0, PumpStatusEnum.Authorized, 3, 60, false, false) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump08()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Pump8", PumpStatusEnum.Idle, 0, PumpStatusEnum.Reserved, 2, 0, true, false) },
                { 2, new PumpStory("Pump8", PumpStatusEnum.Reserved, 0, PumpStatusEnum.Authorized, 3, 60, true, true) },
                { 3, new PumpStory("Pump8", PumpStatusEnum.Authorized, 10, PumpStatusEnum.NozzlePickUp, 4, 180, true, true, 60D, 2.6D) },
                { 4, new PumpStory("Pump8", PumpStatusEnum.NozzlePickUp, 10, PumpStatusEnum.Authorized, 5, 0, true, false) },
                { 5, new PumpStory("Pump8", PumpStatusEnum.Authorized, 0, PumpStatusEnum.Idle, 1, 240, true, true) },
            };
            return PumpStory;
        }


        public Dictionary<int, PumpStory> CreateStoryPump09()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Goto NozzlePickUp", PumpStatusEnum.Idle, 1, PumpStatusEnum.NozzlePickUp, 2, 0, true, false, 60D, 2.6D) },
                { 2, new PumpStory("NozzlePickUp", PumpStatusEnum.NozzlePickUp, 0, PumpStatusEnum.Idle, 1, 0, false, false) }
            };
            return PumpStory;
        }

        //28020202020202020202FF0?1203820000000000000000000000000000000000004
        //   1 2 3 4 5 6 7 8 9
        public Dictionary<int, PumpStory> CreateStoryPump10()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Goto Dispensing", PumpStatusEnum.Idle, 1, PumpStatusEnum.Dispensing, 2, 0, true, false) },
                { 2, new PumpStory("Dispensing", PumpStatusEnum.Dispensing, 0, PumpStatusEnum.Idle, 1, 0, false, false) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump11()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Goto DispenseCompleted", PumpStatusEnum.Idle, 1, PumpStatusEnum.DispenseCompleted, 2, 0, true, false) },
                { 2, new PumpStory("DispenseCompleted", PumpStatusEnum.DispenseCompleted, 0, PumpStatusEnum.Idle, 1, 0, false, false) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump12()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Goto NotAuthorized", PumpStatusEnum.Idle, 1, PumpStatusEnum.NotAuthorized, 2, 0, true, false, 60D, 2.6D) },
                { 2, new PumpStory("NotAuthorized", PumpStatusEnum.NotAuthorized, 0, PumpStatusEnum.Idle, 1, 0, false, false) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump13()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Goto Blocked", PumpStatusEnum.Idle, 1, PumpStatusEnum.Blocked, 2, 0, true, false) },
                { 2, new PumpStory("Blocked", PumpStatusEnum.Blocked, 0, PumpStatusEnum.Idle, 1, 0, false, false) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump14()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Goto NoResponse", PumpStatusEnum.Idle, 1, PumpStatusEnum.NoResponse, 2, 0, true, false) },
                { 2, new PumpStory("NoResponse", PumpStatusEnum.NoResponse, 0, PumpStatusEnum.Idle, 1, 0, false, false) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump15()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Goto Disabled", PumpStatusEnum.Idle, 1, PumpStatusEnum.Disabled , 2, 0, true, false) },
                { 2, new PumpStory("Disabled", PumpStatusEnum.Disabled, 0, PumpStatusEnum.Idle, 1, 0, false, false) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump16()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                /*
                    State: Idle -> Reserved on Reserve Request (Validate w/ Read Request)
                    State: Reserved -> Authorized on Authorize Request (Validate w/ Read Request) (No Time Out).
                    State: Authorized -> Dispensing Complete (Automated) (Validate w/ Read Request)
                    State: Dispensing Complete -> Idle on Get Finalization Request (Send Finalization Info Response) (Validate w/ Read Request)
                */
                { 1, new PumpStory("Idel to Reserved", PumpStatusEnum.Idle, 0, PumpStatusEnum.Reserved, 2, 0, true, false) },
                { 2, new PumpStory("Reserved to Authorized", PumpStatusEnum.Reserved, 0, PumpStatusEnum.Authorized, 3, 60, true, true) },
                { 3, new PumpStory("Authorized to DispenseCompleted", PumpStatusEnum.Authorized, 10, PumpStatusEnum.DispenseCompleted, 4, 0, true, true , 100D, 2.2D ) },
                { 4, new PumpStory("DispenseCompleted to Idel", PumpStatusEnum.DispenseCompleted, 0, PumpStatusEnum.Idle, 1, 0, true, false) }
            };
            return PumpStory;
        }

        public Dictionary<int, PumpStory> CreateStoryPump17()
        {
            Dictionary<int, PumpStory> PumpStory = new Dictionary<int, PumpStory>
            {
                { 1, new PumpStory("Idel to Reserved", PumpStatusEnum.Idle, 0, PumpStatusEnum.Reserved, 2, 0, true, false) },
                { 2, new PumpStory("Pump1", PumpStatusEnum.Reserved, 0, PumpStatusEnum.Authorized, 3, 60, true, true) },
                { 3, new PumpStory("Pump1", PumpStatusEnum.Authorized, 10, PumpStatusEnum.NozzlePickUp, 4, 180, true, true , 100D, 2.2D ) },
                { 4, new PumpStory("Pump1", PumpStatusEnum.NozzlePickUp, 10, PumpStatusEnum.Dispensing, 5, 0, true, false) },
                { 5, new PumpStory("Pump1", PumpStatusEnum.Dispensing, 20, PumpStatusEnum.Authorized, 6, 0, true, false) },
                { 6, new PumpStory("Pump1", PumpStatusEnum.Authorized, 4, PumpStatusEnum.DispenseCompleted, 7, 0, true, false) },
                { 7, new PumpStory("Pump1", PumpStatusEnum.DispenseCompleted, 0, PumpStatusEnum.Idle, 1, 0, true, false) }
            };
            return PumpStory;
        }

        /// <summary>
        /// Add a story to a pump for the Station
        /// </summary>
        /// <param name="PumpNo"></param>
        /// <param name="PumpStory"></param>
        private void AddStory(int PumpNo, Dictionary<int, PumpStory> PumpStory)
        {
            stationStories.Add(PumpNo, PumpStory);
            stationStoriesLine.Add(PumpNo, 1);
        }
        /// <summary>
        /// Load all Stories into the Station's Story lib
        /// </summary>
        public StoryManager()
        {
            AddStory(1, CreateStoryPump01());
            AddStory(2, CreateStoryPump02());
            AddStory(3, CreateStoryPump03());
            AddStory(4, CreateStoryPump04());
            AddStory(5, CreateStoryPump05());
            AddStory(6, CreateStoryPump06());
            AddStory(7, CreateStoryPump07());
            AddStory(8, CreateStoryPump08());
            //AddStory(9, CreateStoryPump09());
            //AddStory(10, CreateStoryPump10());
            //AddStory(11, CreateStoryPump11());
            //AddStory(12, CreateStoryPump12());
            //AddStory(13, CreateStoryPump13());
            //AddStory(14, CreateStoryPump14());
            //AddStory(15, CreateStoryPump15());
            AddStory(16, CreateStoryPump16());
            AddStory(17, CreateStoryPump17());
        }
    }
}

