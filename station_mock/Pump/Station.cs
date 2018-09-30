using System;
using System.Collections.Generic;
using System.Reflection;
using station_mock;
using station_mock.storyman;
using log4net;

namespace station_mock.Pump
{

    public class Station
    {
        private static readonly ILog log = LogManager.GetLogger(LogHelp.AppName);

        public StoryManager Story = new StoryManager();
        public Dictionary<int, Pump> Pumps = new Dictionary<int, Pump>();
        protected Dictionary<int, PumpStory> pumpStories = new Dictionary<int, PumpStory>();

        public void SetStatus(int pumpNo, PumpStatusEnum Status)
        {
            if (Pumps.ContainsKey(pumpNo))
            {
                Pump pump = Pumps[pumpNo];
                PumpStatusEnum OrgStatus = pump.PumpStatus;
                string old = Enum.GetName(typeof(PumpStatusEnum), (int)OrgStatus); 
                string ny =  Enum.GetName(typeof(PumpStatusEnum), (int)Status);

                // Pump #1: Idle => Reserved"
                if (Story.stationStories.ContainsKey(pumpNo))
                {
                    pumpStories = Story.stationStories[pumpNo];
                    PumpStory pumpStory = pumpStories[Story.stationStoriesLine[pumpNo]];

                    if (Status.Equals(PumpStatusEnum.Idle) && pumpStory.CanResetPump)
                    {
                        pump.ResetPump();
                        Story.stationStoriesLine[pumpNo] = 1;
                    }

                    if (Status.Equals(pumpStory.NextStatus))
                    {
                        pump.PumpStatus = Status;
                        Story.stationStoriesLine[pumpNo] = pumpStory.NextStoryLine;
                    }
                    if (Pumps.ContainsKey(pumpNo)) { Pumps.Remove(pumpNo); }
                    Pumps.Add(pumpNo, pump);

                    string storyText = "";
                    switch (pump.PumpStatus)
                    {
                        case PumpStatusEnum.Authorized:
                            if (OrgStatus.Equals(PumpStatusEnum.Reserved))
                            {
                                storyText = $"| ({pump.Volume:N3}L or {pump.Limit:N2}RM)";
                            }
                            break;

                        case PumpStatusEnum.Idle:
                            if (OrgStatus.Equals(PumpStatusEnum.DispenseCompleted))
                            {
                                storyText = $"| ({pump.Volume:N3}L or {pump.Amount:N2}RM)";
                            }
                            break;
                        default:
                            break;
                    }

                    old = Enum.GetName(typeof(PumpStatusEnum), (int)OrgStatus);
                    ny = Enum.GetName(typeof(PumpStatusEnum), (int)Status);
                    log.Info(LogHelp.LogText($"Pump #{pumpNo:00}: {old} =) {ny} {storyText}"));

                }
                else
                {
                    pump.PumpStatus = Status;
                    if (Pumps.ContainsKey(pumpNo)) { Pumps.Remove(pumpNo); }
                    Pumps.Add(pumpNo, pump);

                    old = Enum.GetName(typeof(PumpStatusEnum), (int)OrgStatus);
                    ny = Enum.GetName(typeof(PumpStatusEnum), (int)Status);
                    log.Warn(LogHelp.LogText($"Pump #{pumpNo:00}: No Pump Storie: {old} =) {ny}"));
                }
            }
            else
            {
                log.Error(LogHelp.LogText($"Pump #{pumpNo:00}: Does not exist"));
            }
        }
        /// <summary>
        /// Making story lines move to next step in scripted story
        /// </summary>
        /// <param name="pumpNo">Pump Number / Pump Story</param>
        public void NextStepHistory(int pumpNo)
        {
            if (Pumps.ContainsKey(pumpNo))
            {
                Pump pump = Pumps[pumpNo];
                PumpStatusEnum OrgStatus = pump.PumpStatus;
                if (Story.stationStories.ContainsKey(pumpNo))
                {
                    pumpStories = Story.stationStories[pumpNo];
                    PumpStory pumpStory = pumpStories[Story.stationStoriesLine[pumpNo]];

                    DateTime tick = pump.PumpTime;
                    int nextTick = pumpStory.NextTick;
                    long pumpStatusTimeOut = pumpStory.PumpStatusTimeOut;

                    // If NextTick is gt 0 then the story line will automatic do next step in the story linie
                    // And move on the story for next event to happen.
                    if (nextTick > 0)
                    {
                        if (DateTime.Compare(DateTime.Now, tick.AddSeconds(nextTick)) > 0)
                        {
                            Story.stationStoriesLine[pumpNo] = pumpStory.NextStoryLine;
                            string storytext = pumpStory.Name;
                            if (pumpStory.NextStatus.Equals(PumpStatusEnum.NozzlePickUp) || pumpStory.NextStatus.Equals(PumpStatusEnum.DispenseCompleted))
                            {
                                if (!pumpStory.Amount.Equals(0)) { pump.Amount = pumpStory.Amount; storytext += $"|Amount:{pump.Amount}"; }
                                if (!pumpStory.Volume.Equals(0)) { pump.Volume = pumpStory.Volume; storytext += $"|Volume:{pump.Volume}"; }

                                if (!pumpStory.PctOfLimit.Equals(0)) { pump.Amount = pump.Limit * ((decimal)pumpStory.PctOfLimit / 100M); storytext += $"|Amount:{pump.Amount}"; }
                                if (!pumpStory.Ppu.Equals(0)) { pump.Volume = pump.Amount / (decimal)pumpStory.Ppu; storytext += $"|Volume:{pump.Volume}"; }

                                decimal vol = pump.Volume; vol = vol.Equals(0) ? 1 : vol;
                                log.Debug(LogHelp.LogText(
                                            $"Pump #{pumpNo:00}: " +
                                            $"Status #{Enum.GetName(typeof(PumpStatusEnum), (int)pump.PumpStatus)}: " +
                                            $"Hose: {pump.Hose:0} " +
                                            $"Flag: {pump.Flag:0} " +
                                            $"Amount: {pump.Amount:N2}RM " +
                                            $"Volume: {pump.Volume:N3} " +
                                            $"Price pr unit: {(pump.Amount / (vol)):N3}RM " +
                                            $"Limit: {pump.Limit:N3}L"
                                        )
                                    );
                            }

                            pump.PumpStatus = pumpStory.NextStatus;
                            if (Pumps.ContainsKey(pumpNo)) { Pumps.Remove(pumpNo); }
                            Pumps.Add(pumpNo, pump);
                            log.Info(LogHelp.LogText($"Pump #{pumpNo:00}: " +
                                                      $"{Enum.GetName(typeof(PumpStatusEnum), (int)OrgStatus)} " +
                                                      $" =) "+
                                                      $"{Enum.GetName(typeof(PumpStatusEnum), (int)pumpStory.NextStatus)} " /*+*/
                                                      //$"\"{storytext}\""
                                                     )
                                     );
                        }
                    }

                    // If pumpStatusTimeOut is gt 0 then the Storyline will TimeOut, when the time become more 
                    // than the timout value and reset the pump / story line to Zero / eg first step in story
                    if (nextTick.Equals(0) && !pumpStatusTimeOut.Equals(0))
                    {
                        if (DateTime.Compare(DateTime.Now, tick.AddSeconds(pumpStatusTimeOut)) > 0)
                        {
                            Story.stationStoriesLine[pumpNo] = 1;
                            pump.ResetPump();
                            pump.PumpStatus = PumpStatusEnum.Idle;

                            if (Pumps.ContainsKey(pumpNo)) { Pumps.Remove(pumpNo); }
                            Pumps.Add(pumpNo, pump);
                            log.Info(LogHelp.LogText($"Pump #{pumpNo:00}: " +
                                                      $"{Enum.GetName(typeof(PumpStatusEnum), (int)OrgStatus)} " +
                                                      $" =) " +
                                                      $"{Enum.GetName(typeof(PumpStatusEnum), (int)pumpStory.NextStatus)} " /*+*/
                                                      //$"\"{pumpStory.Name}\""
                                                     )
                                     );
                        }
                    }

                }
            }
        }




        /// <summary>
        /// Set StatusText on Pump with out changing storyling, eg overrideing storyline status
        /// </summary>
        /// <param name="pumpNo">Pump Number / Pump Story</param>
        /// <param name="StatusText">New Status tekst on pump</param>
        public void SetStatusText(int pumpNo, string StatusText)
        {
            SetStatus(pumpNo, PumpStatusData.GetPumpStatus(StatusText));
        }

        /// <summary>
        /// Set Volume on Pump, to change Volume of fuel the pump have used
        /// A way to make test data on the pump for the test case to see data change
        /// </summary>
        /// <param name="pumpNo">Pump Number / Pump Story</param>
        /// <param name="Volume">How many liter use by pump, with 3 decimals</param>
        public void SetVolume(int pumpNo, decimal Volume)
        {
            if (Pumps.ContainsKey(pumpNo))
            {
                Pump pump = Pumps[pumpNo];
                pump.Volume = Volume;

                if (Pumps.ContainsKey(pumpNo)) { Pumps.Remove(pumpNo); }
                Pumps.Add(pumpNo, pump);
                log.Info(LogHelp.LogText($"Pump #{pumpNo:00}: Volume:{Pumps[pumpNo].Volume:N3}L"));
            }
        }

        /// <summary>
        /// Test data to pump.
        /// Set how much the pump maximum can dispence in liters, with 3 decimals
        /// </summary>
        /// <param name="pumpNo">Pump Number / Pump Story</param>
        /// <param name="Limit">Max Liters, with 3 decimals</param>
        public void SetLimit(int pumpNo, decimal Limit)
        {
            if (Pumps.ContainsKey(pumpNo))
            {
                Pump pump = Pumps[pumpNo];
                pump.SetLimit(Limit);

                if (Pumps.ContainsKey(pumpNo)) { Pumps.Remove(pumpNo); }
                Pumps.Add(pumpNo, pump);
                log.Info(LogHelp.LogText($"Pump #{pumpNo:00}: Limit:{Pumps[pumpNo].Limit:N2}RM"));
            }
        }

        /// <summary>
        /// Test data to pump
        /// Set how much the pump have dispenced in Currenct, with 2 decimals
        /// </summary>
        /// <param name="pumpNo">Pump number / pump Story</param>
        /// <param name="Amount">Amount of Currenct, with 2 decimals</param>
        public void SetAmount(int pumpNo, decimal Amount)
        {
            if (Pumps.ContainsKey(pumpNo))
            {
                Pump pump = Pumps[pumpNo];
                pump.SetAmount(Amount);

                if (Pumps.ContainsKey(pumpNo)) { Pumps.Remove(pumpNo); }
                Pumps.Add(pumpNo, pump);
                log.Info(LogHelp.LogText($"Pump #{pumpNo:00}: Amount:{Pumps[pumpNo].Amount:N2}RM"));
            }
        }



        /// <summary>
        /// Initialize Station with pump
        /// </summary>
        /// <param name="maxPumps">Number of pumps on this station</param>
        public Station(int maxPumps)
        {
            if (maxPumps > 32)
            {
                log.Warn($"To many pumps on the stattion, trying to initialize {maxPumps}, maximum pumps allowed is 32");
                maxPumps = 32;
            }
            for (int i = 1; i < maxPumps; i++)
            {
                Pumps.Add(i, new Pump(i, PumpStatusEnum.Idle, 0, 0));
            }
        }

    }
}
