using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TimHanewich.Toolkit;
using ApexVisual;
using ApexVisual.SessionManagement;

namespace ApexVisual.Analysis
{
    public class Session
    {
        //Session data
        public ulong SessionId {get; set;}
        public Track Circuit {get; set;}
        public SessionType SessionMode {get; set;}
        public Team SelectedTeam {get; set;}
        public Driver SelectedDriver {get; set;}
        public DateTimeOffset SessionCreatedAt {get; set;}

        //Analysis objects
        public Lap[] Laps {get; set;}
        public LocationPerformanceAnalysis[] Corners {get; set;}

        //For reporting purposes
        public float PercentLoadComplete;
        public bool LoadComplete;

        public void LoadSummary(CommonSessionData[] session_data, byte driver_index)
        {
            if (session_data.Length == 0)
            {
                throw new Exception("The length of the supplied session array was 0!");
            }

            SessionId = session_data[0].SessionId;
            
            //Get circuit
            foreach (CommonSessionData csd in session_data)
            {
                if (csd.SessionTrack.HasValue)
                {
                    Circuit = csd.SessionTrack.Value;
                }
                if (csd.ThisSessionType.HasValue)
                {
                    SessionMode = csd.ThisSessionType.Value;
                }

                //Selected Team & Driver
                if (csd.FieldData != null)
                {
                    SelectedTeam = csd.FieldData[driver_index].Constructor;
                    SelectedDriver = csd.FieldData[driver_index].Pilot;

                    //Name
                    if (csd.FieldData[driver_index].Name != null)
                    {
                        string driver_name = csd.FieldData[driver_index].Name.TrimEnd('\0');
                        string driver_name_clean = "";
                        foreach (char c in driver_name)
                        {
                            int as_int = Convert.ToInt32(c);
                            if ((as_int < 127 && as_int != 92) == true || as_int == 160) //It is in the normal character range and not a backward slash OR it is a blank space.
                            {
                                driver_name_clean = driver_name_clean + c.ToString();
                            }
                        }
                    }
                }
            }
            SessionCreatedAt = DateTimeOffset.Now;
        }

        public void Load(CommonSessionData[] session_data, byte driver_index)
        {
            PercentLoadComplete = 0;
            LoadComplete = false;

            //Load the session summary data first
            LoadSummary(session_data, driver_index);
            
            //Summon this track
            if (session_data[session_data.Length-1].SessionTrack.HasValue == false)
            {
                throw new Exception("Final data packet does not include session track.");
            }
            Track ToLoad = session_data[session_data.Length-1].SessionTrack.Value;
            TrackDataContainer tdc = TrackDataContainer.LoadTrack(ToLoad);

            //Get list of all laps
            List<byte> AllLaps = new List<byte>();
            foreach (CommonSessionData csd in session_data)
            {
                if (csd.FieldData != null)
                {
                    foreach (CommonCarData ccd in csd.FieldData)
                    {
                        if (AllLaps.Contains(ccd.CurrentLapNumber) == false && ccd.CurrentLapNumber > 0)
                        {
                            AllLaps.Add(ccd.CurrentLapNumber);
                        }
                    }
                }
            }
            
            //Set the total number of corners that need to be analyzed (this is used only for progress reporting purposes)
            int number_of_corners = tdc.Corners.Length * AllLaps.Count;

            //Set the % complete to 5% at this point
            PercentLoadComplete = 0.05f;


            //Generate the frames
            PercentLoadComplete = 0.15f; //Set it to 15% complete at this point

            //Create the lap analysis objects and fill it with corner analysis data.
            //This process also fills in the LapNumber property
            //This process below should take up the % complete from 15% to 90%
            List<Lap> _Lap = new List<Lap>();
            foreach (byte lap_num in AllLaps)
            {

                Lap this_lap_analysis = new Lap();

                //Fill in the lap number
                this_lap_analysis.LapNumber = lap_num;

                //Get all of the frames for this lap
                List<CommonCarData> ThisLapFrames = new List<CommonCarData>();
                foreach (CommonSessionData csd in session_data)
                {
                    if (csd.FieldData != null)
                    {
                        if (csd.FieldData[driver_index].CurrentLapNumber == lap_num)
                        {
                            ThisLapFrames.Add(csd.FieldData[driver_index]);
                        }
                    }
                }

                //Only try to analyze this lap # if there is data for the lap!
                if (ThisLapFrames.Count > 0)
                {
                    //Find the best packetframe for each corner
                    List<TelemetrySnapshot> _TelemetrySnapshot = new List<TelemetrySnapshot>();
                    int c = 1;
                    for (c=1;c<=tdc.Corners.Length;c++)
                    {

                        //Find the best packetframe for this corner
                        TrackLocation this_corner = tdc.Corners[c-1];
                        CommonCarData winner = ThisLapFrames[0];
                        float min_distance_found = float.MaxValue;
                        foreach (CommonCarData ccd in ThisLapFrames)
                        {
                            TrackLocation this_location = new TrackLocation();
                            this_location.PositionX = ccd.PositionX;
                            this_location.PositionY = ccd.PositionY;
                            this_location.PositionZ = ccd.PositionZ;

                            //Sector
                            switch (ccd.CurrentSector)
                            {
                                case Sector.Sector1:
                                    this_location.Sector = 1;
                                    break;
                                case Sector.Sector2:
                                    this_location.Sector = 2;
                                    break;
                                case Sector.Sector3:
                                    this_location.Sector = 3;
                                    break;
                            }

                            if (this_location.Sector == this_corner.Sector) //Only consider packets that are in the same sector
                            {
                                float this_distance = ApexVisualToolkit.DistanceBetweenTwoPoints(this_corner, this_location);
                                if (this_distance < min_distance_found && this_distance < 40) //It has to be be within 40. If it isn't, it is not considered a corner hit!
                                {
                                    winner = ccd;
                                    min_distance_found = this_distance;
                                }
                            }
                        }

                        //Add the corner analysis
                        TelemetrySnapshot ca = new TelemetrySnapshot();
                        ca.LocationNumber = (byte)c;
                        if (min_distance_found < float.MaxValue) //we found a suitable packet, so therefore the min distance shold be less than max. Fill in the details
                        {
                            //Position
                            ca.PositionX = winner.PositionX;
                            ca.PositionY = winner.PositionY;
                            ca.PositionZ = winner.PositionZ;

                            //gForce
                            ca.gForceLateral = winner.gForceLateral;
                            ca.gForceLongitudinal = winner.gForceLongitudinal;
                            ca.gForceVertical = winner.gForceVertical;

                            //Lap data
                            ca.CurrentLapTime = winner.CurrentLapTimeSeconds;
                            ca.CarPosition = winner.CarPosition;
                            ca.LapInvalid = winner.CurrentLapInvalid;
                            ca.Penalties = winner.Penalties;
                            
                            //Telemetry data
                            ca.SpeedKph = winner.SpeedKph;
                            ca.Throttle = winner.Throttle;
                            ca.Steer = winner.Steer;
                            ca.Brake = winner.Brake;
                            ca.Clutch = winner.Clutch;
                            ca.Gear = winner.Gear;
                            ca.EngineRpm = winner.EngineRpm;
                            ca.DrsActive = winner.DrsActive;
                            
                            //Wheel data arrays
                            ca.BrakeTemperature = winner.BrakeTemperature;
                            ca.TyreSurfaceTemperature = winner.TyreSurfaceTemperature;
                            ca.TyreInnerTemperature = winner.TyreInnerTemperature;

                            //Other data
                            ca.EngineTemperature = winner.EngineTemperature;
                            
                            //Car status
                            ca.SelectedFuelMix = winner.ActiveFuelMix;
                            ca.FuelLevel = winner.FuelInTank;

                            //Other wheel data arrays
                            ca.TyreWearPercent = winner.TyreWear;
                            ca.TyreDamagePercent = winner.TyreDamage;

                            //Other data
                            ca.FrontLeftWingDamage = winner.FrontLeftWingDamage;
                            ca.FrontRightWingDamage = winner.FrontRightWingDamage;
                            ca.RearWingDamage = winner.RearWingDamage;
                            ca.ErsStored = winner.StoredErsEnergy;

                            //Add it to the list. (This is in this block because nothing should be added to the list if the corner was not found. So NO EMPTY TelemetrySnapshots in this array to represent a corner that was not found)
                            _TelemetrySnapshot.Add(ca);
                        }
                        else //if we were not able to find a suitable packet for that corner, populate it with just a blank PacketFrame as a place holder.
                        {
                            //Do nothing (that Lap just won't have data for that corner. It can scan through all of the track locations )
                        }
                    }
                    this_lap_analysis.Corners = _TelemetrySnapshot.ToArray();
                    

                    //Get the tyre compound that is being used for this lap
                    List<TyreCompound> CompoundsUsedThisLap = new List<TyreCompound>();
                    foreach (CommonCarData ccd in ThisLapFrames)
                    {
                        //Add the compound to the list
                        TyreCompound this_comp = ccd.EquippedTyreCompound;
                        if (CompoundsUsedThisLap.Contains(this_comp) == false)
                        {
                            CompoundsUsedThisLap.Add(this_comp);
                        }
                    }
                    if (CompoundsUsedThisLap.Count == 1) //If there is only one tyre compound that was used this lap, plug that one in
                    {
                        this_lap_analysis.EquippedTyreCompound = CompoundsUsedThisLap[0];
                    }
                    else //If there were multiple compounds that were used, check which one was used more
                    {

                        //Find the one that is used most
                        int HighestSeen = 0;
                        TyreCompound winner = CompoundsUsedThisLap[0];
                        foreach (TyreCompound tc in CompoundsUsedThisLap)
                        {
                            //Count it
                            int this_times = 0;
                            foreach (CommonCarData ccd in ThisLapFrames)
                            {
                                TyreCompound thistc = ccd.EquippedTyreCompound;
                                if (thistc == tc)
                                {
                                    this_times = this_times + 1;
                                }
                            }

                            //Is it greater? if so, kick out the winner
                            if (this_times >= HighestSeen)
                            {
                                HighestSeen = this_times;
                                winner = tc;
                            }
                        }

                        //Plug it in
                        this_lap_analysis.EquippedTyreCompound = winner;
                    }



                    //Add this to the list of lap analyses
                    _Lap.Add(this_lap_analysis);

                    //Update the percent complete
                    float AdditionalPercentCompletePerLap = (0.90f - 0.15f) / (float)AllLaps.Count;
                    PercentLoadComplete = PercentLoadComplete + AdditionalPercentCompletePerLap;
                }


                

            }
            

            //Sort the packets by time - this is not required for the above process, so I am doing it here for the timing stuff
            List<CommonSessionData> frames_aslist = session_data.ToList();
            List<CommonSessionData> frames_sorted = new List<CommonSessionData>();
            while (frames_aslist.Count > 0)
            {
                CommonSessionData winner = frames_aslist[0];
                foreach (CommonSessionData csd in frames_aslist)
                {
                    if (csd.SessionTime < winner.SessionTime)
                    {
                        winner = csd;
                    }
                }
                frames_sorted.Add(winner);
                frames_aslist.Remove(winner);
            }


            //Plug in the sector times and lap times
            CommonSessionData last_frame = null;
            foreach (CommonSessionData this_frame in frames_sorted)
            {
                if (last_frame != null)
                {
                    if (this_frame.FieldData != null)
                    {
                        if (last_frame.FieldData != null)
                        {
                            float S1_Time_S = 0;
                            float S2_Time_S = 0;
                            float S3_Time_S = 0;
                            float LapTime_S = 0;
                            bool Lap_Invalid_In_Last_Frame = false;

                            if (last_frame.FieldData[driver_index].CurrentSector == Sector.Sector1 && this_frame.FieldData[driver_index].CurrentSector == Sector.Sector2) //We went from sector 1 to sector 2
                            {
                                S1_Time_S = (float)this_frame.FieldData[driver_index].Sector1TimeSeconds;
                            }
                            else if (last_frame.FieldData[driver_index].CurrentSector == Sector.Sector2 && this_frame.FieldData[driver_index].CurrentSector == Sector.Sector3) //We went from sector 2 to sector 3
                            {
                                S2_Time_S = (float)this_frame.FieldData[driver_index].Sector2TimeSeconds;
                            }
                            else if (last_frame.FieldData[driver_index].CurrentLapNumber < this_frame.FieldData[driver_index].CurrentLapNumber) //We just finished the lap, and thus sector 3
                            {
                                float last_s1_seconds = (float)last_frame.FieldData[driver_index].Sector1TimeSeconds;
                                float last_s2_seconds = (float)last_frame.FieldData[driver_index].Sector1TimeSeconds;
                                LapTime_S = this_frame.FieldData[driver_index].LastLapTimeSeconds;
                                S3_Time_S = LapTime_S - last_s1_seconds - last_s2_seconds;
                            }

                            if (last_frame.FieldData[driver_index].CurrentLapInvalid)
                            {
                                Lap_Invalid_In_Last_Frame = true;
                            }


                            //If any of the numbers up there changed (are not 0), it means that we either changed sector or changed lap. If we did, we need to plug that data into the Lap
                            if (S1_Time_S > 0 || S2_Time_S > 0 || S3_Time_S > 0 || Lap_Invalid_In_Last_Frame)
                            {
                                //Find the lap analysis
                                foreach (Lap la in _Lap)
                                {
                                    if (la.LapNumber == last_frame.FieldData[driver_index].CurrentLapNumber)
                                    {
                                        
                                        if (S1_Time_S > 0)
                                        {
                                            la.Sector1Time = S1_Time_S;
                                        }

                                        if (S2_Time_S > 0)
                                        {
                                            la.Sector2Time = S2_Time_S;
                                        }

                                        if (S3_Time_S > 0)
                                        {
                                            la.Sector3Time = S3_Time_S;
                                        }

                                        if (Lap_Invalid_In_Last_Frame)
                                        {
                                            la.LapInvalid = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                last_frame = this_frame;
            }
            PercentLoadComplete = 0.95f; //Mark the percent complete as 95%

            #region "Get fuel consumption for each lap"

            foreach (byte lapnum in AllLaps)
            {
                //Get all packets for this lap
                List<CommonSessionData> lap_frames = new List<CommonSessionData>();
                foreach (CommonSessionData csd in frames_sorted)
                {
                    if (csd.FieldData != null)
                    {
                        if (csd.FieldData[driver_index].CurrentLapNumber == lapnum)
                        {
                            lap_frames.Add(csd);
                        }
                    }
                }

                //Get the min and max and then plug it in
                if (lap_frames.Count > 0)
                {
                    float fuel_start = lap_frames[0].FieldData[driver_index].FuelInTank;
                    float fuel_end = lap_frames[lap_frames.Count - 1].FieldData[driver_index].FuelInTank;
                    float fuel_used = fuel_end - fuel_start;
                    fuel_used = fuel_used * -1;

                    foreach (Lap la in _Lap)
                    {
                        if (la.LapNumber == lapnum)
                        {
                            la.FuelConsumed = fuel_used;
                        }
                    }
                }
            }

            #endregion

            #region "Get percent on throttle/brake/coasting/max throttle/max brake"

            foreach (byte lapnum in AllLaps)
            {
                //Get all packets for this lap
                List<CommonSessionData> lap_frames = new List<CommonSessionData>();
                foreach (CommonSessionData csd in frames_sorted)
                {
                    if (csd.FieldData != null)
                    {
                        if (csd.FieldData[driver_index].CurrentLapNumber == lapnum)
                        {
                            lap_frames.Add(csd);
                        }
                    }
                }

                //Set up counter variables
                int OnThrottle = 0;
                int OnBrake = 0;
                int Coasting = 0;
                int Overlap = 0;
                int FullThrottle = 0;
                int FullBrake = 0;

                //Do the calculations
                foreach (CommonSessionData csd in lap_frames)
                {
                    CommonCarData ccd = csd.FieldData[driver_index];

                    //Basics
                    if (ccd.Throttle > 0 && ccd.Brake == 0)
                    {
                        OnThrottle = OnThrottle + 1;
                    }
                    else if (ccd.Brake > 0 && ccd.Throttle == 0)
                    {
                        OnBrake = OnBrake + 1;
                    }
                    else if (ccd.Throttle == 0 && ccd.Brake == 0)
                    {
                        Coasting = Coasting + 1;
                    }
                    else if (ccd.Throttle > 0 && ccd.Brake > 0)
                    {
                        Overlap = Overlap + 1;
                    }

                    //Full pressures
                    if (ccd.Throttle == 1)
                    {
                        FullThrottle = FullThrottle + 1;
                    }
                    if (ccd.Brake == 1)
                    {
                        FullBrake = FullBrake + 1;
                    }


                }
                
                //Do the calculations
                float percent_on_throttle = (float)OnThrottle / (float)lap_frames.Count;
                float percent_on_brake = (float)OnBrake / (float)lap_frames.Count;
                float percent_coasting = (float)Coasting / (float)lap_frames.Count;
                float percent_overlap = (float)Overlap / (float)lap_frames.Count;
                float full_throttle = (float)FullThrottle / (float)lap_frames.Count;
                float full_brake = (float)FullBrake / (float)lap_frames.Count;

                //plug them in
                foreach (Lap la in _Lap)
                {
                    if (la.LapNumber == lapnum)
                    {
                        la.PercentOnThrottle = percent_on_throttle;
                        la.PercentOnBrake = percent_on_brake;
                        la.PercentCoasting = percent_coasting;
                        la.PercentThrottleBrakeOverlap = percent_overlap;
                        la.PercentOnMaxThrottle = full_throttle;
                        la.PercentOnMaxBrake = full_brake;
                    }
                }
            }

            #endregion

            #region "Get gear changes"

            foreach (byte lapnum in AllLaps)
            {
                //Get all packets for this lap
                List<CommonSessionData> lap_frames = new List<CommonSessionData>();
                foreach (CommonSessionData frame in frames_sorted)
                {
                    if (frame.FieldData != null)
                    {
                        if (frame.FieldData[driver_index].CurrentLapNumber == lapnum)
                        {
                            lap_frames.Add(frame);
                        }
                    } 
                }

                //Count the number of gear changes
                int gear_changes = 0;
                CommonSessionData last_frame_ = null;
                foreach (CommonSessionData csd in lap_frames)
                {
                    if (last_frame_ != null)
                    {
                        sbyte last_gear = last_frame_.FieldData[driver_index].Gear;
                        sbyte this_gear = csd.FieldData[driver_index].Gear;
                        if (this_gear != last_gear)
                        {
                            gear_changes = gear_changes + 1;
                        }
                    }
                    last_frame_ = csd;
                }

                //Plug it in
                foreach (Lap la in _Lap)
                {
                    if (la.LapNumber == lapnum)
                    {
                        la.GearChanges = gear_changes;
                    }
                }

            }

            #endregion

            #region  "Get Top Speed"

            foreach (byte lapnum in AllLaps)
            {
                //Get all packets for this lap
                List<CommonSessionData> lap_frames = new List<CommonSessionData>();
                foreach (CommonSessionData frame in frames_sorted)
                {
                    if (frame.FieldData != null)
                    {
                        if (frame.FieldData[driver_index].CurrentLapNumber == lapnum)
                        {
                            lap_frames.Add(frame);
                        }
                    }
                }

                //Get values
                ushort max_kph = 0;
                foreach (CommonSessionData csd in lap_frames)
                {
                    CommonCarData ccd = csd.FieldData[driver_index];

                    if (ccd.SpeedKph > max_kph)
                    {
                        max_kph = ccd.SpeedKph;
                    }
                }

                //Plug it in
                foreach (Lap la in _Lap)
                {
                    if (la.LapNumber == lapnum)
                    {
                        la.TopSpeedKph = max_kph;
                    }
                }


            }

            #endregion

            #region "Get average incremental tyre wear"

            foreach (byte lapnum in AllLaps)
            {
                //Get all packets for this lap
                List<CommonSessionData> lap_frames = new List<CommonSessionData>();
                foreach (CommonSessionData csd in frames_sorted)
                {
                    if (csd.FieldData != null)
                    {
                        if (csd.FieldData[driver_index].CurrentLapNumber == lapnum)
                        {
                            lap_frames.Add(csd);
                        }
                    }
                }

                if (lap_frames.Count > 0)
                {
                    WheelDataArray tyrewear_start = lap_frames[0].FieldData[driver_index].TyreWear;
                    WheelDataArray tyrewear_end = lap_frames[lap_frames.Count-1].FieldData[driver_index].TyreWear;
                    float AvgTyreWear_Start = (tyrewear_start.RearLeft + tyrewear_start.RearRight + tyrewear_start.FrontLeft + tyrewear_start.FrontRight) / 4f;
                    float AvgTyreWear_End = (tyrewear_end.RearLeft + tyrewear_end.RearRight + tyrewear_end.FrontLeft + tyrewear_end.FrontRight) / 4f;
                    float avginctyrewear = AvgTyreWear_End - AvgTyreWear_Start;

                    //Plug it in
                    foreach (Lap la in _Lap)
                    {
                        if (la.LapNumber == lapnum)
                        {
                            //Incremental tyre wear
                            WheelDataArray wda_IncTyreWear = new WheelDataArray();
                            wda_IncTyreWear.RearLeft = tyrewear_end.RearLeft - tyrewear_start.RearLeft;
                            wda_IncTyreWear.RearRight = tyrewear_end.RearRight - tyrewear_start.RearRight;
                            wda_IncTyreWear.FrontLeft = tyrewear_end.FrontLeft - tyrewear_start.FrontLeft;
                            wda_IncTyreWear.FrontRight = tyrewear_end.FrontRight - tyrewear_start.FrontRight;
                            la.IncrementalTyreWear = wda_IncTyreWear;
                            
                            //Beginning (snapshot) tyre wear
                            WheelDataArray wda_BegTyreWear = new WheelDataArray();
                            wda_BegTyreWear.RearLeft = tyrewear_start.RearLeft;
                            wda_BegTyreWear.RearRight = tyrewear_start.RearRight;
                            wda_BegTyreWear.FrontLeft = tyrewear_start.FrontLeft;
                            wda_BegTyreWear.FrontRight = tyrewear_start.FrontRight;
                            la.BeginningTyreWear = wda_BegTyreWear;
                        }
                    }
                }
            }

            #endregion

            //Close off the laps
            Laps = _Lap.ToArray();

            #region "Now that we have the lap analyses, generate the corner performance analyses (Lap Analyses MUST BE DONE before this)"

            //Generate all of the corner performances
            List<LocationPerformanceAnalysis> corner_performances = new List<LocationPerformanceAnalysis>();
            for (int c = 0; c < tdc.Corners.Length; c++)
            {
                LocationPerformanceAnalysis cpa = new LocationPerformanceAnalysis();

                //Copy over the data from the TrackLocationOptima (by doing a quick Json serialization/deserialization)
                //cpa = JsonConvert.DeserializeObject<LocationPerformanceAnalysis>(JsonConvert.SerializeObject(tdc.Corners[c]));

                //Plug in the corner #
                cpa.LocationNumber = (byte)(c + 1);

                List<ushort> Speeds = new List<ushort>(); //A list of speeds that were carried through this corner
                List<sbyte> Gears = new List<sbyte>(); //A list of gears that the driver used through this corner
                List<float> Distances = new List<float>();

                //Collect the data for each lap
                foreach (Lap la in Laps)
                {
                    //Go through all of the corners for this lap. If you find a corner that matches the corner number that we are analyzing, add the details to the list.
                    foreach (TelemetrySnapshot ts in la.Corners)
                    {
                        if (ts.LocationNumber == (c + 1))
                        {
                            Speeds.Add(ts.SpeedKph);
                            Gears.Add(ts.Gear);
                        }
                    }
                }

                //Get the average speed
                float speed_avg = 0;
                if (Speeds.Count > 0)
                {
                    foreach (ushort us in Speeds)
                    {
                        speed_avg = speed_avg + (float)us;
                    }
                    speed_avg = speed_avg / (float)Speeds.Count;
                    cpa.AverageSpeedKph = speed_avg;
                }
                else
                {
                    cpa.AverageSpeedKph = float.NaN;
                }

                //Get the average gear
                float gear_avg = 0;
                if (Gears.Count > 0)
                {
                    foreach (sbyte sb in Gears)
                    {
                        gear_avg = gear_avg + (float)sb;
                    }
                    gear_avg = gear_avg / (float)Gears.Count;
                    cpa.AverageGear = gear_avg;
                }
                else
                {
                    cpa.AverageGear = float.NaN;
                }


                corner_performances.Add(cpa);

            }

            //Plug in all of the corner performances
            Corners = corner_performances.ToArray();

            #endregion



            //Shut down
            PercentLoadComplete = 1; //Mark the percent complete as 100%
            LoadComplete = true;



        }

        //Get times between corners
        //This will return an array of float values. These float values indicate the gaps between the corners.
        //First value will be gap between the start of lap (start line) and the first corner
        //Lat value will be gap between the last corner and end of lap (start line)
        //In the senario that a corner is missed (i.e. they did not hit corner 2), the values that would have been 1-2 and 2-3 will be NaN
        public float[] CornerGapTimes(byte lap_num)
        {
            #region "Error Checking"

            if (lap_num < 1)
            {
                throw new Exception("Lap #" + lap_num.ToString() + " is invalid.");
            }

            //make sure we have that lap
            bool HasLap = false;
            foreach (Lap l in Laps)
            {
                if (l.LapNumber == lap_num)
                {
                    HasLap = true;
                }
            }
            if (HasLap == false)
            {
                throw new Exception("This session does not have lap #" + lap_num.ToString() + " so it is impossible to calculate the corner gap times.");
            }

            #endregion

            //Find the lap in question
            Lap SubjectLap = null;
            foreach (Lap l in Laps)
            {
                if (l.LapNumber == lap_num)
                {
                    SubjectLap = l;
                }
            }

            //Get the track so we know how many corners should exist here
            TrackDataContainer tdc = TrackDataContainer.LoadTrack(Circuit);
            
            List<float> ToReturn = new List<float>();
            for (int c = 0; c < (tdc.Corners.Length+1); c++)
            {
                //In this C loop:
                // 0 = Start - 1
                // 1 = 1 - 2
                // 2 = 2 - 3
                // 3 = 3 - 4
                // etc...
                //Last corner # = Last corner - Lap end

                //Find this corner
                TelemetrySnapshot Corner1 = null;
                TelemetrySnapshot Corner2 = null;
                foreach (TelemetrySnapshot ts in SubjectLap.Corners)
                {
                    if (ts.LocationNumber == c) //Corner 1 (corrner at start of gap)
                    {
                        Corner1 = ts;
                    }
                    else if (ts.LocationNumber == (c+1)) //Corner 2 (corner at end of gap)
                    {
                        Corner2 = ts;
                    }
                }
                
                //If it is the first corner, get the current lap time (time it has been since it crossed the start/finish line)
                if (c == 0)
                {
                    //if we have the corner (they hit the apex), write the time
                    if (Corner2 != null)
                    {
                        ToReturn.Add(Corner2.CurrentLapTime);
                    }
                    else //If we weren't able to find one (one doesn't exist)
                    {
                        ToReturn.Add(float.NaN);
                    }
                }
                else if (c > 0 && c < tdc.Corners.Length) // This is for any other middle corner (not the first corner, not the last corner)
                {
                    //Even though we try and go find it, it is possible that the next corner will not be there
                    //This would be caused by the driver not getting close enough to the apex to trigger it to register as a corner.

                    //If either the current corner or next corner are null, just plug in a NaN value
                    if (Corner1 == null || Corner2 == null)
                    {
                        ToReturn.Add(float.NaN);
                    }
                    else
                    {
                        ToReturn.Add(Corner2.CurrentLapTime - Corner1.CurrentLapTime);
                    }
                }
                else if (c == tdc.Corners.Length) // If this is the last corner in the lap (we need to measure the time until it took to get to the start line from the last corner)
                {
                    //If we have the last corner (they hit the apex)
                    if (Corner1 != null)
                    {
                        //The way we can measure the amount of time from the apex of the last corner to the finish line for this lap: This lap time - the current lap time at the time they were at the apex of the last corner
                        
                        //If all 3 sectors have time (they FINISHED the lap!)
                        if (SubjectLap.Sector1Time > 0 && SubjectLap.Sector2Time > 0 && SubjectLap.Sector3Time > 0)
                        {
                            ToReturn.Add(SubjectLap.LapTime() - Corner1.CurrentLapTime);
                        }
                        else //If the lap was never comleted, return NaN
                        {
                            ToReturn.Add(float.NaN);
                        }
                    }
                    else //If the last corner was not found
                    {
                        ToReturn.Add(float.NaN);
                    }
                }
                
                

            }

            return ToReturn.ToArray();
        }
    }
}