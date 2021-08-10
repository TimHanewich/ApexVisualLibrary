using System;
using ApexVisual;
using Codemasters.F1_2021;

namespace ApexVisual.SessionManagement
{
    public partial class ApexVisualSessionManager
    {

        public void Load2021Bytes(byte[] bytes)
        {
            PacketType pt = CodemastersToolkit.GetPacketType(bytes);

            if (pt == PacketType.Motion)
            {
                MotionPacket mp = new MotionPacket();
                mp.LoadBytes(bytes);

                //Add the correct number of common car data
                if (OngoingCanvas.FieldData == null || OngoingCanvas.FieldData.Length < mp.FieldMotionData.Length)
                {
                    OngoingCanvas.FieldData = new CommonCarData[mp.FieldMotionData.Length];
                }

                //Update the basics
                OngoingCanvas.SessionId = mp.UniqueSessionId;
                OngoingCanvas.SessionTime = mp.SessionTime;
                OngoingCanvas.FrameIdentifier = mp.FrameIdentifier;
                OngoingCanvas.PlayerCarIndex = mp.PlayerCarIndex;

                //Update the car data
                for (int i = 0; i < mp.FieldMotionData.Length; i++)
                {
                    OngoingCanvas.FieldData[i].PositionX = mp.FieldMotionData[i].PositionX;
                    OngoingCanvas.FieldData[i].PositionY = mp.FieldMotionData[i].PositionY;
                    OngoingCanvas.FieldData[i].PositionZ = mp.FieldMotionData[i].PositionZ;
                    OngoingCanvas.FieldData[i].gForceLateral = mp.FieldMotionData[i].gForceLateral;
                    OngoingCanvas.FieldData[i].gForceLongitudinal = mp.FieldMotionData[i].gForceLongitudinal;
                    OngoingCanvas.FieldData[i].gForceVertical = mp.FieldMotionData[i].gForceVertical;

                    //Wheel speed
                    OngoingCanvas.FieldData[i].WheelSpeed = new WheelDataArray();
                    OngoingCanvas.FieldData[i].WheelSpeed.FrontLeft = mp.WheelSpeed.FrontLeft;
                    OngoingCanvas.FieldData[i].WheelSpeed.FrontRight = mp.WheelSpeed.FrontRight;
                    OngoingCanvas.FieldData[i].WheelSpeed.RearLeft = mp.WheelSpeed.RearLeft;
                    OngoingCanvas.FieldData[i].WheelSpeed.RearRight = mp.WheelSpeed.RearRight;
                    
                    //Front wheel angle
                    OngoingCanvas.FieldData[i].FrontWheelAngle = mp.FrontWheelAngle;
                }
            }
            else if (pt == PacketType.Session)
            {
                SessionPacket sp = new SessionPacket();
                sp.LoadBytes(bytes);

                //Weather
                if (sp.CurrentWeatherCondition == SessionPacket.WeatherCondition.Clear)
                {
                    OngoingCanvas.CurrentWeather = WeatherCondition.Clear;
                }
                else if (sp.CurrentWeatherCondition == SessionPacket.WeatherCondition.LightClouds)
                {
                    OngoingCanvas.CurrentWeather = WeatherCondition.LightCloud;
                }
                else if (sp.CurrentWeatherCondition == SessionPacket.WeatherCondition.Overcast)
                {
                    OngoingCanvas.CurrentWeather = WeatherCondition.Overcast;
                }
                else if (sp.CurrentWeatherCondition == SessionPacket.WeatherCondition.LightRain)
                {
                    OngoingCanvas.CurrentWeather = WeatherCondition.LightRain;
                }
                else if (sp.CurrentWeatherCondition == SessionPacket.WeatherCondition.HeavyRain)
                {
                    OngoingCanvas.CurrentWeather = WeatherCondition.HeavyRain;
                }
                else if (sp.CurrentWeatherCondition == SessionPacket.WeatherCondition.Storm)
                {
                    OngoingCanvas.CurrentWeather = WeatherCondition.Storm;
                }

                OngoingCanvas.TrackTemperatureCelsius = sp.TrackTemperatureCelsius;
                OngoingCanvas.AirTemperatureCelsius = sp.AirTemperatureCelsius;
                OngoingCanvas.LapsInRace = sp.TotalLapsInRace;

                //Session type
                switch (sp.SessionTypeMode)
                {
                    case SessionPacket.SessionType.Unknown:
                        OngoingCanvas.ThisSessionType = SessionType.Unknown;
                        break;
                    case SessionPacket.SessionType.P1:
                        OngoingCanvas.ThisSessionType = SessionType.Practice1;
                        break;
                    case SessionPacket.SessionType.P2:
                        OngoingCanvas.ThisSessionType = SessionType.Practice2;
                        break;
                    case SessionPacket.SessionType.P3:
                        OngoingCanvas.ThisSessionType = SessionType.Practice3;
                        break;
                    case SessionPacket.SessionType.ShortPractice:
                        OngoingCanvas.ThisSessionType = SessionType.ShortPractice;
                        break;
                    case SessionPacket.SessionType.Q1:
                        OngoingCanvas.ThisSessionType = SessionType.Qualifying1;
                        break;
                    case SessionPacket.SessionType.Q2:
                        OngoingCanvas.ThisSessionType = SessionType.Qualifying2;
                        break;
                    case SessionPacket.SessionType.Q3:
                        OngoingCanvas.ThisSessionType = SessionType.Qualifying3;
                        break;
                    case SessionPacket.SessionType.ShortQualifying:
                        OngoingCanvas.ThisSessionType = SessionType.ShortQualifying;
                        break;
                    case SessionPacket.SessionType.OneShotQualifying:
                        OngoingCanvas.ThisSessionType = SessionType.OneShotQualifying;
                        break;
                    case SessionPacket.SessionType.Race:
                        OngoingCanvas.ThisSessionType = SessionType.Race;
                        break;
                    case SessionPacket.SessionType.Race2:
                        OngoingCanvas.ThisSessionType = SessionType.Race2;
                        break;
                    case SessionPacket.SessionType.TimeTrial:
                        OngoingCanvas.ThisSessionType = SessionType.TimeTrial;
                        break;
                    default:
                        OngoingCanvas.ThisSessionType = SessionType.Unknown;
                        break;   
                }

                //Track
                switch (sp.SessionTrack)
                {
                    case Codemasters.F1_2021.Track.Melbourne:
                        OngoingCanvas.SessionTrack = Track.Melbourne;
                        break;
                    case Codemasters.F1_2021.Track.PaulRicard:
                        OngoingCanvas.SessionTrack = Track.PaulRicard;
                        break;
                    case Codemasters.F1_2021.Track.Shanghai:
                        OngoingCanvas.SessionTrack = Track.Shanghai;
                        break;
                    case Codemasters.F1_2021.Track.Sakhir:
                        OngoingCanvas.SessionTrack = Track.Sakhir;
                        break;
                    case Codemasters.F1_2021.Track.Catalunya:
                        OngoingCanvas.SessionTrack = Track.Catalunya;
                        break;
                    case Codemasters.F1_2021.Track.Monaco:
                        OngoingCanvas.SessionTrack = Track.Monaco;
                        break;
                    case Codemasters.F1_2021.Track.Montreal:
                        OngoingCanvas.SessionTrack = Track.Montreal;
                        break;
                    case Codemasters.F1_2021.Track.Silverstone:
                        OngoingCanvas.SessionTrack = Track.Silverstone;
                        break;
                    case Codemasters.F1_2021.Track.Hockenheim:
                        OngoingCanvas.SessionTrack = Track.Hockenheim;
                        break;
                    case Codemasters.F1_2021.Track.Hungaroring:
                        OngoingCanvas.SessionTrack = Track.Hungaroring;
                        break;
                    case Codemasters.F1_2021.Track.Spa:
                        OngoingCanvas.SessionTrack = Track.Spa;
                        break;
                    case Codemasters.F1_2021.Track.Monza:
                        OngoingCanvas.SessionTrack = Track.Monza;
                        break;
                    case Codemasters.F1_2021.Track.Singapore:
                        OngoingCanvas.SessionTrack = Track.Singapore;
                        break;
                    case Codemasters.F1_2021.Track.Suzuka:
                        OngoingCanvas.SessionTrack = Track.Suzuka;
                        break;
                    case Codemasters.F1_2021.Track.AbuDhabi:
                        OngoingCanvas.SessionTrack = Track.AbuDhabi;
                        break;
                    case Codemasters.F1_2021.Track.Texas:
                        OngoingCanvas.SessionTrack = Track.Texas;
                        break;
                    case Codemasters.F1_2021.Track.Brazil:
                        OngoingCanvas.SessionTrack = Track.Brazil;
                        break;
                    case Codemasters.F1_2021.Track.Austria:
                        OngoingCanvas.SessionTrack = Track.Austria;
                        break;
                    case Codemasters.F1_2021.Track.Sochi:
                        OngoingCanvas.SessionTrack = Track.Mexico;
                        break;
                    case Codemasters.F1_2021.Track.Mexico:
                        OngoingCanvas.SessionTrack = Track.Mexico;
                        break;
                    case Codemasters.F1_2021.Track.Baku:
                        OngoingCanvas.SessionTrack = Track.Baku;
                        break;
                    case Codemasters.F1_2021.Track.SakhirShort:
                        OngoingCanvas.SessionTrack = Track.SakhirShort;
                        break;
                    case Codemasters.F1_2021.Track.SilverstoneShort:
                        OngoingCanvas.SessionTrack = Track.SilverstoneShort;
                        break;
                    case Codemasters.F1_2021.Track.TexasShort:
                        OngoingCanvas.SessionTrack = Track.TexasShort;
                        break;
                    case Codemasters.F1_2021.Track.SuzukaShort:
                        OngoingCanvas.SessionTrack = Track.SuzukaShort;
                        break;
                    case Codemasters.F1_2021.Track.Hanoi:
                        OngoingCanvas.SessionTrack = Track.Hanoi;
                        break;
                    case Codemasters.F1_2021.Track.Zandvoort:
                        OngoingCanvas.SessionTrack = Track.Zandvoort;
                        break;
                    case Codemasters.F1_2021.Track.Imola:
                        OngoingCanvas.SessionTrack = Track.Imola;
                        break;
                    case Codemasters.F1_2021.Track.Portimao:
                        OngoingCanvas.SessionTrack = Track.Portimao;
                        break;
                    case Codemasters.F1_2021.Track.Jeddah:
                        OngoingCanvas.SessionTrack = Track.Jeddah;
                        break;
                }

                OngoingCanvas.SessionTimeLeftSeconds = sp.SessionTimeLeft;
                OngoingCanvas.SessionDurationSeconds = sp.SessionDuration;

                //Safety car status
                switch (sp.CurrentSafetyCarStatus)
                {
                    case SessionPacket.SafetyCarStatus.Full:
                        OngoingCanvas.CurrentSafetyCarStatus = SafetyCarStatus.Full;
                        break;
                    case SessionPacket.SafetyCarStatus.None:
                        OngoingCanvas.CurrentSafetyCarStatus = SafetyCarStatus.None;
                        break;
                    case SessionPacket.SafetyCarStatus.Virtual:
                        OngoingCanvas.CurrentSafetyCarStatus = SafetyCarStatus.Virtual;
                        break;
                }       
            }
            else if (pt == PacketType.Lap)
            {
                LapPacket lp = new LapPacket();
                lp.LoadBytes(bytes);

                if (OngoingCanvas.FieldData == null || OngoingCanvas.FieldData.Length != lp.FieldLapData.Length)
                {
                    OngoingCanvas.FieldData = new CommonCarData[lp.FieldLapData.Length];
                }

                //Update all
                for (int i = 0; i < lp.FieldLapData.Length; i++)
                {
                    OngoingCanvas.FieldData[i].LastLapTimeSeconds = lp.FieldLapData[i].LastLapTime;
                    OngoingCanvas.FieldData[i].CurrentLapTimeSeconds = lp.FieldLapData[i].CurrentLapTime;
                    OngoingCanvas.FieldData[i].Sector1TimeSeconds = Convert.ToSingle(lp.FieldLapData[i].Sector1TimeMilliseconds) / 1000f;
                    OngoingCanvas.FieldData[i].Sector2TimeSeconds = Convert.ToSingle(lp.FieldLapData[i].Sector2TimeMilliseconds) / 1000f;
                    OngoingCanvas.FieldData[i].LapDistanceMeters = lp.FieldLapData[i].LapDistance;
                    OngoingCanvas.FieldData[i].TotalDistanceMeters = lp.FieldLapData[i].TotalDistance;
                    OngoingCanvas.FieldData[i].CarPosition = lp.FieldLapData[i].CarPosition;
                    OngoingCanvas.FieldData[i].CurrentLapNumber = lp.FieldLapData[i].CurrentLapNumber;

                    //Pit status
                    switch (lp.FieldLapData[i].CurrentPitStatus)
                    {
                        case Codemasters.F1_2021.PitStatus.OnTrack:
                            OngoingCanvas.FieldData[i].CurrentPitStatus = PitStatus.None;
                            break;
                        case Codemasters.F1_2021.PitStatus.PitLane:
                            OngoingCanvas.FieldData[i].CurrentPitStatus = PitStatus.Pitting;
                            break;
                        case Codemasters.F1_2021.PitStatus.PitArea:
                            OngoingCanvas.FieldData[i].CurrentPitStatus = PitStatus.InPitArea;
                            break;
                    }
                    
                    //Sector
                    switch (lp.FieldLapData[i].Sector)
                    {
                        case 0:
                            OngoingCanvas.FieldData[i].CurrentSector = Sector.Sector1;
                            break;
                        case 1:
                            OngoingCanvas.FieldData[i].CurrentSector = Sector.Sector2;
                            break;
                        case 2:
                            OngoingCanvas.FieldData[i].CurrentSector = Sector.Sector3;
                            break;
                    }
                    
                    OngoingCanvas.FieldData[i].CurrentLapInvalid = lp.FieldLapData[i].CurrentLapInvalid;
                    OngoingCanvas.FieldData[i].Penalties = lp.FieldLapData[i].Penalties;
                    OngoingCanvas.FieldData[i].StartingGridPosition = lp.FieldLapData[i].StartingGridPosition;
                    
                    //Driver status
                    switch (lp.FieldLapData[i].CurrentDriverStatus)
                    {
                        case LapPacket.DriverStatus.FlyingLap:
                            OngoingCanvas.FieldData[i].CurrentDriverStatus = DriverStatus.FlyingLap;
                            break;
                        case LapPacket.DriverStatus.InGarage:
                            OngoingCanvas.FieldData[i].CurrentDriverStatus = DriverStatus.InGarage;
                            break;
                        case LapPacket.DriverStatus.InLap:
                            OngoingCanvas.FieldData[i].CurrentDriverStatus = DriverStatus.InLap;
                            break;
                        case LapPacket.DriverStatus.OnTrack:
                            OngoingCanvas.FieldData[i].CurrentDriverStatus = DriverStatus.OnTrack;
                            break;
                        case LapPacket.DriverStatus.OutLap:
                            OngoingCanvas.FieldData[i].CurrentDriverStatus = DriverStatus.OutLap;
                            break;
                    }
                    
                }

            }

        }

    }
}