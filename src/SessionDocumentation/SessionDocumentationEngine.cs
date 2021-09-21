using System;
using System.Collections.Generic;
using ApexVisual.SessionManagement;
using ApexVisual.SessionDocumentation;
using ApexVisual.LiveCoaching;

namespace ApexVisual.SessionDocumentation
{
    public class SessionDocumentationEngine
    {
        //private Variables
        private OriginalCapture _OriginalCapture;
        private Session _Session;
        private List<Lap> _Laps;
        private List<TelemetrySnapshot> _TelemetrySnapshots;
        private List<WheelDataArray> _WheelDataArrays;

        //Publicly accessing variables
        public OriginalCapture OriginalCapture
        {
            get
            {
                return _OriginalCapture;
            }
        }
        public Session Session
        {
            get
            {
                return _Session;
            }
        }
        public Lap[] Laps
        {
            get
            {
                return _Laps.ToArray();
            }
        }
        public TelemetrySnapshot[] TelemetrySnapshots
        {
            get
            {
                return _TelemetrySnapshots.ToArray();
            }
        }
        public WheelDataArray[] WheelDataArrays
        {
            get
            {
                return _WheelDataArrays.ToArray();
            }
        }
        
        //Private variables for tracking status
        private ApexVisualSessionManager sm;
        private byte driver_index;
        private CommonSessionData LastSeen;
        private LiveCoach lc;

        //Private variables for tracking mid-lap statistics
        private uint OnThrottle;
        private uint OnBrake;
        private uint OnCoasting;
        private uint OnMaxThrottle;
        private uint OnMaxBrake;
        private uint GearChanges;
        private sbyte LastGearSeen;

        //In construction (working on)
        private Lap ConstructingLap;
        private List<TelemetrySnapshot> HoldingTelemetrySnapshotsForThisLap;
        private List<WheelDataArray> HoldingWheelDataArraysForThisLap;

        public SessionDocumentationEngine()
        {
            _Session = null;
            _Laps = new List<Lap>();
            _TelemetrySnapshots = new List<TelemetrySnapshot>();
            _WheelDataArrays = new List<WheelDataArray>();
            
            //Set up the session manager
            sm = new ApexVisualSessionManager();
            sm.DataUpdateAvailable += Update;

            LastSeen = null;
            ConstructingLap = null;
            HoldingTelemetrySnapshotsForThisLap = new List<TelemetrySnapshot>();
            HoldingWheelDataArraysForThisLap = new List<WheelDataArray>();
        }

        public void Update(byte[] bytes)
        {
            sm.IngestBytes(bytes);
        }

        public void Update(CommonSessionData csd)
        {

            #region "set up"

            //Set the driver index. This is a priority!
            driver_index = csd.PlayerCarIndex;

            if (lc == null)
            {
                if (csd.SessionTrack.HasValue)
                {
                    lc = new LiveCoach(csd.SessionTrack.Value);
                    lc.CornerStageChanged += CornerStagedChanged;
                }
            }

            if (lc != null)
            {
                lc.Update(csd);
            }

            #endregion

            #region "Session"

            //Add the session if we need to
            if (csd.SessionId != 0) //Wait until it is not 0. Because if it is not zero, it must have a lot of data related to session!
            {

                //Add the session if we don't have the session yet.
                bool NeedToAdd = false;
                if (_Session == null)
                {
                    NeedToAdd = true;
                }

                //Add it if we need to add it
                if (NeedToAdd)
                {
                    Session ToAdd = new Session();
                    ToAdd.SessionId = csd.SessionId;
                    ToAdd.CreatedAtUtc = DateTime.UtcNow;
                    _Session = ToAdd;

                    //Add the original capture too
                    OriginalCapture oc = new OriginalCapture();
                    oc.SessionId = csd.SessionId;
                    oc.CapturedAtUtc = DateTime.UtcNow;
                    _OriginalCapture = oc;
                } 
            }

            //Get the session to update
            Session SessionToEdit = _Session;

            //Throw an error if the ID of the session being provided does NOT match what we have. In other words - it is a different session.
            if (csd.SessionId != SessionToEdit.SessionId)
            {
                throw new Exception("New Session ID was provided to SessionDocumentationEngine. Expecting session: " + SessionToEdit.SessionId.ToString() + ". Provided: " + csd.SessionId.ToString());
            }
            
            //Update details if we have a session to edit
            if (SessionToEdit != null)
            {
                
                //Update track
                if (csd.SessionTrack.HasValue)
                {
                    SessionToEdit.Track = csd.SessionTrack.Value;
                }

                //Mode
                if (csd.ThisSessionType.HasValue)
                {
                    SessionToEdit.Mode = csd.ThisSessionType.Value;
                }

                //Team
                if (csd.FieldData != null)
                {
                    if (csd.FieldData.Length > 0)
                    {
                        SessionToEdit.Team = csd.FieldData[driver_index].Constructor;
                        SessionToEdit.Driver = csd.FieldData[driver_index].Pilot;
                    }
                }

                //Format/Game
                switch (csd.Format)
                {
                    case 2017:
                        SessionToEdit.Game = CodemastersF1Game.F1_2017;
                        break;
                    case 2018:
                        SessionToEdit.Game = CodemastersF1Game.F1_2018;
                        break;
                    case 2019:
                        SessionToEdit.Game = CodemastersF1Game.F1_2019;
                        break;
                    case 2020:
                        SessionToEdit.Game = CodemastersF1Game.F1_2020;
                        break;
                    case 2021:
                        SessionToEdit.Game = CodemastersF1Game.F1_2021;
                        break;
                }

            }


            #endregion
            
            #region "Lap"

            if (LastSeen != null)
            {
                if (csd.FieldData != null && LastSeen.FieldData != null)
                {
                    //Are we on a new lap now? (just crossed the line)
                    if (csd.FieldData[driver_index].CurrentLapNumber > LastSeen.FieldData[driver_index].CurrentLapNumber)
                    {
                        //Close the lap   
                        CloseLap(csd.FieldData[driver_index].LastLapTimeSeconds);

                        //Set up for the new lap
                        ConstructingLap.FromSession = csd.SessionId;
                        ConstructingLap.LapNumber = csd.FieldData[driver_index].CurrentLapNumber;
                        ConstructingLap.EquippedTyreCompound = csd.FieldData[driver_index].EquippedTyreCompound;
                    }
                    else //Document the mid-lap statistics. For example, percent on throttle.
                    {
                        //On Throttle?
                        if (csd.FieldData[driver_index].Throttle > 0)
                        {
                            OnThrottle = OnThrottle + 1;
                        }

                        //On Brake?
                        if (csd.FieldData[driver_index].Brake > 0)
                        {
                            OnBrake = OnBrake + 1;
                        }

                        //Coasting?
                        if (csd.FieldData[driver_index].Throttle == 0 && csd.FieldData[driver_index].Brake == 0)
                        {
                            OnCoasting = OnCoasting + 1;
                        }

                        //On Max Throttle?
                        if (csd.FieldData[driver_index].Throttle == 1)
                        {
                            OnMaxThrottle = OnMaxThrottle + 1;
                        }

                        //On Max Brake?
                        if (csd.FieldData[driver_index].Brake == 1)
                        {
                            OnMaxBrake = OnMaxBrake + 1;
                        }

                        //Gear changes?
                        if (LastGearSeen != csd.FieldData[driver_index].Gear)
                        {
                            GearChanges = GearChanges + 1;
                            LastGearSeen = csd.FieldData[driver_index].Gear;
                        }
                    }
                }
            }

            #endregion

            //Mark the last seen!
            LastSeen = csd;
        }

        private void CornerStagedChanged(CornerStage stage)
        {
            if (stage == CornerStage.Apex)
            {
                if (LastSeen != null)
                {
                    if (LastSeen.FieldData != null)
                    {
                        //Construct the corner
                        TelemetrySnapshot ts = new TelemetrySnapshot();
                        ts.Id = Guid.NewGuid();
                        ts.FromLap = ConstructingLap.Id;
                        ts.LocationType = TrackLocationType.Corner;
                        ts.LocationNumber = lc.AtCorner;
                        ts.PositionX = LastSeen.FieldData[driver_index].PositionX;
                        ts.PositionY = LastSeen.FieldData[driver_index].PositionY;
                        ts.PositionZ = LastSeen.FieldData[driver_index].PositionZ;
                        ts.CurrentLapTime = LastSeen.FieldData[driver_index].CurrentLapTimeSeconds;
                        ts.CarPosition = LastSeen.FieldData[driver_index].CarPosition;
                        ts.LapInvalid = LastSeen.FieldData[driver_index].CurrentLapInvalid;
                        ts.SpeedKph = Convert.ToInt16(LastSeen.FieldData[driver_index].SpeedKph);
                        ts.Throttle = Convert.ToByte(Math.Round(LastSeen.FieldData[driver_index].Throttle * 100f, 0));
                        ts.Steer = Convert.ToInt16(Math.Round(LastSeen.FieldData[driver_index].Steer, 0));
                        ts.Brake = Convert.ToByte(Math.Round(LastSeen.FieldData[driver_index].Brake * 100f, 0));
                        
                        //Gear
                        switch (LastSeen.FieldData[driver_index].Gear)
                        {
                            case -1:
                                ts.Gear = Gear.Reverse;
                                break;
                            case 0:
                                ts.Gear = Gear.Neutral;
                                break;
                            case 1:
                                ts.Gear = Gear.Gear1;
                                break;
                            case 2:
                                ts.Gear = Gear.Gear2;
                                break;
                            case 3:
                                ts.Gear = Gear.Gear3;
                                break;
                            case 4:
                                ts.Gear = Gear.Gear4;
                                break;
                            case 5:
                                ts.Gear = Gear.Gear5;
                                break;
                            case 6:
                                ts.Gear = Gear.Gear6;
                                break;
                            case 7:
                                ts.Gear = Gear.Gear7;
                                break;
                            case 8:
                                ts.Gear = Gear.Gear8;
                                break;
                        }

                        ts.DrsActive = LastSeen.FieldData[driver_index].DrsActive;
                        
                        //Tyre wear percent
                        WheelDataArray wda_TyreWearPercent = new WheelDataArray();
                        wda_TyreWearPercent.Id = Guid.NewGuid();
                        wda_TyreWearPercent.RearLeft = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreWear.RearLeft);
                        wda_TyreWearPercent.RearRight = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreWear.RearRight);
                        wda_TyreWearPercent.FrontRight = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreWear.FrontRight);
                        wda_TyreWearPercent.FrontLeft = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreWear.FrontLeft);
                        ts.TyreWearPercent = wda_TyreWearPercent.Id;

                        //Tyre damage percent
                        WheelDataArray wda_TyreDamagePercent = new WheelDataArray();
                        wda_TyreDamagePercent.Id = Guid.NewGuid();
                        wda_TyreDamagePercent.RearLeft = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreDamage.RearLeft);
                        wda_TyreDamagePercent.RearRight = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreDamage.RearRight);
                        wda_TyreDamagePercent.FrontLeft = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreDamage.FrontLeft);
                        wda_TyreDamagePercent.FrontRight = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreDamage.FrontRight);
                        ts.TyreDamagePercent = wda_TyreDamagePercent.Id;

                        ts.StoredErs = LastSeen.FieldData[driver_index].StoredErsEnergy;

                        //Add it to the holding pen. It will be added to the results later after the lap is completed.
                        HoldingTelemetrySnapshotsForThisLap.Add(ts);
                        HoldingWheelDataArraysForThisLap.Add(wda_TyreWearPercent);
                        HoldingWheelDataArraysForThisLap.Add(wda_TyreDamagePercent);
                    }
                } 
            }
        }
    
        private void CloseLap(float last_lap_time)
        {
            //Save the last lap
            if (ConstructingLap != null)
            {
                ConstructingLap.Sector3Time = last_lap_time - LastSeen.FieldData[driver_index].Sector1TimeSeconds - LastSeen.FieldData[driver_index].Sector2TimeSeconds;
                ConstructingLap.EndingFuel = LastSeen.FieldData[driver_index].FuelInTank;
                ConstructingLap.Sector1Time = LastSeen.FieldData[driver_index].Sector1TimeSeconds;
                ConstructingLap.Sector2Time = LastSeen.FieldData[driver_index].Sector2TimeSeconds;
                ConstructingLap.EndingErs = LastSeen.FieldData[driver_index].StoredErsEnergy;
                
                //Ending tyre wear
                WheelDataArray EndingTyreWear = new WheelDataArray();
                EndingTyreWear.Id = Guid.NewGuid();
                EndingTyreWear.FrontLeft = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreWear.FrontLeft);
                EndingTyreWear.FrontRight = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreWear.FrontRight);
                EndingTyreWear.RearLeft = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreWear.RearLeft);
                EndingTyreWear.RearRight = ApexVisualToolkit.FloatPercentToByte(LastSeen.FieldData[driver_index].TyreWear.RearRight);
                ConstructingLap.EndingTyreWear = EndingTyreWear.Id;

                #region "mid-lap statistis"

                uint TotalCount = OnThrottle + OnBrake + OnCoasting; //Represents the # of packets in this lap.

                ConstructingLap.PercentOnThrottle = ApexVisualToolkit.FloatPercentToByte(Convert.ToSingle(OnThrottle) / Convert.ToSingle(TotalCount));
                ConstructingLap.PercentOnBrake = ApexVisualToolkit.FloatPercentToByte(Convert.ToSingle(OnBrake) / Convert.ToSingle(TotalCount));
                ConstructingLap.PercentCoasting = ApexVisualToolkit.FloatPercentToByte(Convert.ToSingle(OnCoasting) / Convert.ToSingle(TotalCount));
                ConstructingLap.PercentOnMaxThrottle = ApexVisualToolkit.FloatPercentToByte(Convert.ToSingle(OnMaxThrottle) / Convert.ToSingle(TotalCount));
                ConstructingLap.PercentOnMaxBrake = ApexVisualToolkit.FloatPercentToByte(Convert.ToSingle(OnMaxBrake) / Convert.ToSingle(TotalCount));
                ConstructingLap.GearChanges = Convert.ToInt16(GearChanges);

                #endregion
                
                //Add all of the data
                _Laps.Add(ConstructingLap); //Add the lap
                _WheelDataArrays.Add(EndingTyreWear); //Add the wheel data arrays that tie directly to the lap
                _TelemetrySnapshots.AddRange(HoldingTelemetrySnapshotsForThisLap); //Add the telemetry snapshots for this lap
                _WheelDataArrays.AddRange(HoldingWheelDataArraysForThisLap); //Add the wheel data arrays for the lap
            }

            //Clear it for next time
            ConstructingLap = new Lap();
            ConstructingLap.Id = Guid.NewGuid();
            HoldingTelemetrySnapshotsForThisLap.Clear();
            HoldingWheelDataArraysForThisLap.Clear();
            OnThrottle = 0;
            OnBrake = 0;
            OnCoasting = 0;
            OnMaxThrottle = 0;
            OnMaxBrake = 0;
            GearChanges = 0;
        }
    }
}