using System;
using ApexVisual;
using Codemasters.F1_2021;
using System.Collections.Generic;

namespace ApexVisual.SessionManagement
{
    public partial class ApexVisualSessionManager
    {

        private void Load2021Bytes(byte[] bytes)
        {
            PacketType pt = CodemastersToolkit.GetPacketType(bytes);

            //Update the basics (any packet should have this in the header because every packet has a header)
            Packet p = new Packet();
            p.LoadBytes(bytes);

            //Update the basics
            OngoingCanvas.SessionId = p.UniqueSessionId;
            OngoingCanvas.SessionTime = p.SessionTime;
            OngoingCanvas.FrameIdentifier = p.FrameIdentifier;
            OngoingCanvas.PlayerCarIndex = p.PlayerCarIndex;
            OngoingCanvas.Format = p.PacketFormat;

            if (pt == PacketType.Motion)
            {
                MotionPacket mp = new MotionPacket();
                mp.LoadBytes(bytes);

                //Add the correct number of common car data
                OngoingCanvas.InitializeFieldDataIfNeeded(mp.FieldMotionData.Length);
                
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
                    WheelDataArray wda_WheelSpeed = new WheelDataArray();
                    wda_WheelSpeed.FrontLeft = mp.WheelSlip.FrontLeft;
                    wda_WheelSpeed.FrontRight = mp.WheelSpeed.FrontRight;
                    wda_WheelSpeed.RearLeft = mp.WheelSpeed.RearLeft;
                    wda_WheelSpeed.RearRight = mp.WheelSpeed.RearRight;
                    OngoingCanvas.FieldData[i].WheelSpeed = wda_WheelSpeed;                    
                    
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
                        OngoingCanvas.SessionTrack = Track.Sochi;
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

                //Initialize
                OngoingCanvas.InitializeFieldDataIfNeeded(lp.FieldLapData.Length);

                //Update all
                for (int i = 0; i < lp.FieldLapData.Length; i++)
                {
                    OngoingCanvas.FieldData[i].LastLapTimeSeconds = Convert.ToSingle(lp.FieldLapData[i].LastLapTimeMilliseconds) / 1000f;
                    OngoingCanvas.FieldData[i].CurrentLapTimeSeconds = Convert.ToSingle(lp.FieldLapData[i].CurrentLapTimeMilliseconds) / 1000f;
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
                    switch (lp.FieldLapData[i].InSector)
                    {
                        case LapPacket.Sector.Sector1:
                            OngoingCanvas.FieldData[i].CurrentSector = Sector.Sector1;
                            break;
                        case LapPacket.Sector.Sector2:
                            OngoingCanvas.FieldData[i].CurrentSector = Sector.Sector2;
                            break;
                        case LapPacket.Sector.Sector3:
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
            else if (pt == PacketType.Participants)
            {
                ParticipantPacket pp = new ParticipantPacket();
                pp.LoadBytes(bytes);

                OngoingCanvas.NumberOfActiveCars = pp.NumberOfActiveCars;

                //Create them if they don't exist
                OngoingCanvas.InitializeFieldDataIfNeeded(pp.FieldParticipantData.Length);

                for (int i = 0; i < pp.FieldParticipantData.Length; i++)
                {
                    OngoingCanvas.FieldData[i].IsAiControlled = pp.FieldParticipantData[i].IsAiControlled;
                    
                    //Driver
                    List<KeyValuePair<Codemasters.F1_2021.Driver, Driver>> DriverDict = new List<KeyValuePair<Codemasters.F1_2021.Driver, Driver>>();
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.LewisHamilton, Driver.LewisHamilton));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.CarlosSainz, Driver.CarlosSainz));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.DaniilKvyat, Driver.DaniilKvyat));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.DanielRicciardo, Driver.DanielRicciardo));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.FernandoAlonso, Driver.FernandoAlonso));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.FelipeMassa, Driver.FelipeMassa));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.KimiRaikkonen, Driver.KimiRaikkonen));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.MaxVerstappen, Driver.MaxVerstappen));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.NicoHulkenburg, Driver.NicoHulkenburg));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.KevinMagnussen, Driver.KevinMagnussen));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.RomainGrosjean, Driver.RomainGrosjean));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.SebastianVettel, Driver.SebastianVettel));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.SergioPerez, Driver.SergioPerez));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.ValtteriBottas, Driver.ValtteriBottas));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.EstebanOcon, Driver.EstebanOcon));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.LanceStroll, Driver.LanceStroll));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.ArronBarnes, Driver.ArronBarnes));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.MartinGiles, Driver.MartinGiles));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.AlexMurray, Driver.AlexMurray));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.LucasRoth, Driver.LucasRoth));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.IgorCorreia, Driver.IgorCorreia));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.SophieLevasseur, Driver.SophieLevasseur));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.JonasSchiffer, Driver.JonasSchiffer));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.AlainForest, Driver.AlainForest));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.JayLetourneau, Driver.JayLetourneau));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.EstoSaari, Driver.EstoSaari));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.YasarAtiyeh, Driver.YasarAtiyeh));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.CallistoCalabresi, Driver.CallistoCalabresi));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.NaotaIzum, Driver.NaotaIzum));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.HowardClarke, Driver.HowardClarke));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.WilheimKaufmann, Driver.WilheimKaufmann));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.MarieLaursen, Driver.MarieLaursen));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.FlavioNieves, Driver.FlavioNieves));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.PeterBelousov, Driver.PeterBelousov));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.KlimekMichalski, Driver.KlimekMichalski));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.SantiagoMoreno, Driver.SantiagoMoreno));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.BenjaminCoppens, Driver.BenjaminCoppens));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.NoahVisser, Driver.NoahVisser));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.GertWaldmuller, Driver.GertWaldmuller));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.JulianQuesada, Driver.JulianQuesada));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.DanielJones, Driver.DanielJones));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.ArtemMarkelov, Driver.ArtemMarkelov));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.TadasukeMakino, Driver.TadasukeMakino));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.SeanGelael, Driver.SeanGelael));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.NyckDeVries, Driver.NyckDeVries));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.JackAitken, Driver.JackAitken));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.GeorgeRussell, Driver.GeorgeRussell));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.MaximilianGünther, Driver.MaximilianGünther));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.NireiFukuzumi, Driver.NireiFukuzumi));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.LucaGhiotto, Driver.LucaGhiotto));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.LandoNorris, Driver.LandoNorris));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.SergioSetteCamara, Driver.SergioSetteCamara));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.LouisDeletraz, Driver.LouisDeletraz));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.AntonioFuoco, Driver.AntonioFuoco));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.CharlesLeclerc, Driver.CharlesLeclerc));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.PierreGasly, Driver.PierreGasly));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.AlexanderAlbon, Driver.AlexanderAlbon));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.NicholasLatifi, Driver.NicholasLatifi));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.DorianBoccolacci, Driver.DorianBoccolacci));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.NikoKari, Driver.NikoKari));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.RobertKubica, Driver.RobertKubica));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.ArjunMaini, Driver.ArjunMaini));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.AlessioLorandi, Driver.AlessioLorandi));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.RubenMeijer, Driver.RubenMeijer));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.RashidNair, Driver.RashidNair));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.JackTremblay, Driver.JackTremblay));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.DevinButler, Driver.DevinButler));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.LukasWeber, Driver.LukasWeber));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.AntonioGiovinazzi, Driver.AntonioGiovinazzi));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.RobertKubica, Driver.RobertKubica));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.AlainProst, Driver.AlainProst));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.ArytonSenna, Driver.ArytonSenna));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.NobuharuMatsushita, Driver.NobuharuMatsushita));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.NikitaMazepin, Driver.NikitaMazepin));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.GuanyaZhou, Driver.GuanyaZhou));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.MickSchumacher, Driver.MickSchumacher));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.CallumIlott, Driver.CallumIlott));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.JuanManuelCorrea, Driver.JuanManuelCorrea));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.JordanKing, Driver.JordanKing));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.MahaveerRaghunathan, Driver.MahaveerRaghunathan));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.TatianaCalderon, Driver.TatianaCalderon));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.AnthoineHubert, Driver.AnthoineHubert));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.GuilianoAlesi, Driver.GuilianoAlesi));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.RalphBoschung, Driver.RalphBoschung));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.MichaelSchumacher, Driver.MichaelSchumacher));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.DanTicktum, Driver.DanTicktum));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.MarcusArmstrong, Driver.MarcusArmstrong));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.ChristianLundgaard, Driver.ChristianLundgaard));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.YukiTsunoda, Driver.YukiTsunoda));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.JehanDaruvala, Driver.JehanDaruvala));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.GulhermeSamaia, Driver.GulhermeSamaia));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.PedroPiquet, Driver.PedroPiquet));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.FelipeDrugovich, Driver.FelipeDrugovich));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.RobertSchwartzman, Driver.RobertSchwartzman));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.RoyNissany, Driver.RoyNissany));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.MarinoSato, Driver.MarinoSato));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.AidanJackson, Driver.AidanJackson));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.CasperAkkerman, Driver.CasperAkkerman));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.JensonButton, Driver.JensonButton));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.DavidCoulthard, Driver.DavidCoulthard));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>(Codemasters.F1_2021.Driver.NicoRosberg, Driver.NicoRosberg));
                    DriverDict.Add(new KeyValuePair<Codemasters.F1_2021.Driver, Driver>((Codemasters.F1_2021.Driver)255, Driver.PLAYER));
                    foreach (KeyValuePair<Codemasters.F1_2021.Driver, Driver> kvp in DriverDict)
                    {
                        if (pp.FieldParticipantData[i].PilotingDriver == kvp.Key)
                        {
                            OngoingCanvas.FieldData[i].Pilot = kvp.Value;
                        }
                    }

                    //Constructor (team)
                    List<KeyValuePair<Codemasters.F1_2021.Team, Team>> TeamDict = new List<KeyValuePair<Codemasters.F1_2021.Team, Team>>();
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Mercedes, Team.Mercedes));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Ferrari, Team.Ferrari));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.RedBullRacing, Team.RedBullRacing));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Williams, Team.Williams));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.AstonMartin, Team.AstonMartin));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Alpine, Team.Alpine));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.AlphaTauri, Team.AlphaTauri));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Haas, Team.Haas));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.McLaren, Team.McLaren));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.AlfaRomeo, Team.AlfaRomeo));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.ArtGP19, Team.ArtGP19));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Campos19, Team.Campos19));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Carlin19, Team.Carlin19));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.SauberJuniorCharouz19, Team.SauberJuniorCharouz19));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Dams19, Team.Dams19));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.UniVirtuosi19, Team.UniVirtuosi19));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.MPMotorsport19, Team.MPMotorsport19));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Prema19, Team.Prema19));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Trident19, Team.Trident19));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Arden19, Team.Arden19));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.ArtGP20, Team.ArtGP20));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Campos20, Team.Campos20));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Carlin20, Team.Carlin20));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Charouz20, Team.Charouz20));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Dams20, Team.Dams20));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.UniVirtuosi20, Team.UniVirtuosi20));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.MPMotorsport20, Team.MPMotorsport20));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Prema20, Team.Prema20));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Trident20, Team.Trident20));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.BWT20, Team.BWT20));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Hitech20, Team.Hitech20));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Mercedes2020, Team.Mercedes2020));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Ferrari2020, Team.Ferrari2020));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.RedBull2020, Team.RedBull2020));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Williams2020, Team.Williams2020));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.RacingPoint2020, Team.RacingPoint2020));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Renault2020, Team.Renault2020));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.AlphaTauri2020, Team.AlphaTauri2020));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.Haas2020, Team.Haas2020));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.McLaren2020, Team.McLaren2020));
                    TeamDict.Add(new KeyValuePair<Codemasters.F1_2021.Team, Team>(Codemasters.F1_2021.Team.AlfaRomeo2020, Team.AlfaRomeo2020));
                    foreach (KeyValuePair<Codemasters.F1_2021.Team, Team> kvp in TeamDict)
                    {
                        if (pp.FieldParticipantData[i].ManufacturingTeam == kvp.Key)
                        {
                            OngoingCanvas.FieldData[i].Constructor = kvp.Value;
                        }
                    }

                    OngoingCanvas.FieldData[i].RaceNumber = pp.FieldParticipantData[i].CarRaceNumber;
                    OngoingCanvas.FieldData[i].Name = pp.FieldParticipantData[i].Name;

                }
            }
            else if (pt == PacketType.CarTelemetry)
            {
                TelemetryPacket tp = new TelemetryPacket();
                tp.LoadBytes(bytes);

                //Check the #
                OngoingCanvas.InitializeFieldDataIfNeeded(tp.FieldTelemetryData.Length);


                for (int i = 0; i < tp.FieldTelemetryData.Length; i++)
                {
                    OngoingCanvas.FieldData[i].SpeedKph = tp.FieldTelemetryData[i].SpeedKph;
                    OngoingCanvas.FieldData[i].Throttle = tp.FieldTelemetryData[i].Throttle;
                    OngoingCanvas.FieldData[i].Steer = tp.FieldTelemetryData[i].Steer;
                    OngoingCanvas.FieldData[i].Brake = tp.FieldTelemetryData[i].Brake;
                    OngoingCanvas.FieldData[i].Clutch = tp.FieldTelemetryData[i].Clutch;
                    OngoingCanvas.FieldData[i].Gear = tp.FieldTelemetryData[i].Gear;
                    OngoingCanvas.FieldData[i].EngineRpm = tp.FieldTelemetryData[i].EngineRpm;
                    OngoingCanvas.FieldData[i].DrsActive = tp.FieldTelemetryData[i].DrsActive;
                    OngoingCanvas.FieldData[i].RevLightsPercentage = tp.FieldTelemetryData[i].RevLightsPercentage;
                    
                    //brake temperature
                    WheelDataArray wda_BrakeTemperature = new WheelDataArray();
                    wda_BrakeTemperature.FrontLeft = tp.FieldTelemetryData[i].BrakeTemperature.FrontLeft;
                    wda_BrakeTemperature.FrontRight = tp.FieldTelemetryData[i].BrakeTemperature.FrontRight;
                    wda_BrakeTemperature.RearLeft = tp.FieldTelemetryData[i].BrakeTemperature.RearLeft;
                    wda_BrakeTemperature.RearRight = tp.FieldTelemetryData[i].BrakeTemperature.RearRight;
                    OngoingCanvas.FieldData[i].BrakeTemperature = wda_BrakeTemperature;

                    //Tyre surface temperature
                    WheelDataArray wda_TyreSurfaceTemperature = new WheelDataArray();
                    wda_TyreSurfaceTemperature.FrontLeft = tp.FieldTelemetryData[i].TyreSurfaceTemperature.FrontLeft;
                    wda_TyreSurfaceTemperature.FrontRight = tp.FieldTelemetryData[i].TyreSurfaceTemperature.FrontRight;
                    wda_TyreSurfaceTemperature.RearLeft = tp.FieldTelemetryData[i].TyreSurfaceTemperature.RearLeft;
                    wda_TyreSurfaceTemperature.RearRight = tp.FieldTelemetryData[i].TyreSurfaceTemperature.RearRight;
                    OngoingCanvas.FieldData[i].TyreSurfaceTemperature = wda_TyreSurfaceTemperature;

                    //Tyre inner temperature
                    WheelDataArray wda_TyreInnerTemperature = new WheelDataArray();
                    wda_TyreInnerTemperature.FrontLeft = tp.FieldTelemetryData[i].TyreInnerTemperature.FrontLeft;
                    wda_TyreInnerTemperature.FrontRight = tp.FieldTelemetryData[i].TyreInnerTemperature.FrontRight;
                    wda_TyreInnerTemperature.RearLeft = tp.FieldTelemetryData[i].TyreInnerTemperature.RearLeft;
                    wda_TyreInnerTemperature.RearRight = tp.FieldTelemetryData[i].TyreInnerTemperature.RearRight;
                    OngoingCanvas.FieldData[i].TyreInnerTemperature = wda_TyreInnerTemperature;

                    OngoingCanvas.FieldData[i].EngineTemperature = tp.FieldTelemetryData[i].EngineTemperature;
                }
            }
            else if (pt == PacketType.CarStatus)
            {
                CarStatusPacket csp = new CarStatusPacket();
                csp.LoadBytes(bytes);

                //Check the number of common data
                OngoingCanvas.InitializeFieldDataIfNeeded(csp.FieldCarStatusData.Length);

                for (int i = 0; i < csp.FieldCarStatusData.Length; i++)
                {
                    //Fuel mix
                    switch (csp.FieldCarStatusData[i].SelectedFuelMix)
                    {
                        case Codemasters.F1_2021.FuelMix.Lean:
                            OngoingCanvas.FieldData[i].ActiveFuelMix = FuelMix.Lean;
                            break;
                        case Codemasters.F1_2021.FuelMix.Max:
                            OngoingCanvas.FieldData[i].ActiveFuelMix = FuelMix.Max;
                            break;
                        case Codemasters.F1_2021.FuelMix.Rich:
                            OngoingCanvas.FieldData[i].ActiveFuelMix = FuelMix.Rich;
                            break;
                        case Codemasters.F1_2021.FuelMix.Standard:
                            OngoingCanvas.FieldData[i].ActiveFuelMix = FuelMix.Standard;
                            break;
                    }

                    OngoingCanvas.FieldData[i].FuelInTank = csp.FieldCarStatusData[i].FuelLevel;
                    OngoingCanvas.FieldData[i].FuelCapacity = csp.FieldCarStatusData[i].FuelCapacity;
                    OngoingCanvas.FieldData[i].FuelRemainingLaps = csp.FieldCarStatusData[i].FuelRemainingLaps;
                    OngoingCanvas.FieldData[i].DrsAllowed = csp.FieldCarStatusData[i].DrsAllowed;
                    
                    //Equipped tyre compound
                    switch (csp.FieldCarStatusData[i].EquippedTyreCompound)
                    {
                        case Codemasters.F1_2021.TyreCompound.C5:
                            OngoingCanvas.FieldData[i].EquippedTyreCompound = TyreCompound.C5;
                            break;
                        case Codemasters.F1_2021.TyreCompound.C4:
                            OngoingCanvas.FieldData[i].EquippedTyreCompound = TyreCompound.C4;
                            break;
                        case Codemasters.F1_2021.TyreCompound.C3:
                            OngoingCanvas.FieldData[i].EquippedTyreCompound = TyreCompound.C3;
                            break;
                        case Codemasters.F1_2021.TyreCompound.C2:
                            OngoingCanvas.FieldData[i].EquippedTyreCompound = TyreCompound.C2;
                            break;
                        case Codemasters.F1_2021.TyreCompound.C1:
                            OngoingCanvas.FieldData[i].EquippedTyreCompound = TyreCompound.C1;
                            break;
                        case Codemasters.F1_2021.TyreCompound.Inter:
                            OngoingCanvas.FieldData[i].EquippedTyreCompound = TyreCompound.Inter;
                            break;
                        case Codemasters.F1_2021.TyreCompound.Wet:
                            OngoingCanvas.FieldData[i].EquippedTyreCompound = TyreCompound.Wet;
                            break;
                    }

                    OngoingCanvas.FieldData[i].TyreAgeLaps = csp.FieldCarStatusData[i].TyreAgeLaps;
                    OngoingCanvas.FieldData[i].StoredErsEnergy = csp.FieldCarStatusData[i].ErsStoredEnergyJoules;
                    
                    //ERS deploy mode
                    switch (csp.FieldCarStatusData[i].SelectedErsDeployMode)
                    {
                        case Codemasters.F1_2021.ErsDeployMode.HotLap:
                            OngoingCanvas.FieldData[i].ActiveErsDeployMode = ErsDeployMode.Hotlap;
                            break;
                        case Codemasters.F1_2021.ErsDeployMode.Medium:
                            OngoingCanvas.FieldData[i].ActiveErsDeployMode = ErsDeployMode.Medium;
                            break;
                        case Codemasters.F1_2021.ErsDeployMode.None:
                            OngoingCanvas.FieldData[i].ActiveErsDeployMode = ErsDeployMode.None;
                            break;
                        case Codemasters.F1_2021.ErsDeployMode.Overtake:
                            OngoingCanvas.FieldData[i].ActiveErsDeployMode = ErsDeployMode.Overtake;
                            break;
                    }


                }

            }
            else if (pt == PacketType.CarDamage)
            {
                CarDamagePacket cdp = new CarDamagePacket();
                cdp.LoadBytes(bytes);

                //Check the #
                OngoingCanvas.InitializeFieldDataIfNeeded(cdp.FieldCarDamageData.Length);

                for (int i = 0; i < cdp.FieldCarDamageData.Length; i++)
                {
                    //Tyre wear
                    WheelDataArray wda_TyreWear = new WheelDataArray();
                    wda_TyreWear.FrontLeft = cdp.FieldCarDamageData[i].TyreWear.FrontLeft;
                    wda_TyreWear.FrontRight = cdp.FieldCarDamageData[i].TyreWear.FrontRight;
                    wda_TyreWear.RearLeft = cdp.FieldCarDamageData[i].TyreWear.RearLeft;
                    wda_TyreWear.RearRight = cdp.FieldCarDamageData[i].TyreWear.RearRight;
                    OngoingCanvas.FieldData[i].TyreWear = wda_TyreWear;

                    //Tyre damage
                    WheelDataArray wda_TyreDamage = new WheelDataArray();
                    wda_TyreDamage.FrontLeft = cdp.FieldCarDamageData[i].TyreDamage.FrontLeft;
                    wda_TyreDamage.FrontRight = cdp.FieldCarDamageData[i].TyreDamage.FrontRight;
                    wda_TyreDamage.RearLeft = cdp.FieldCarDamageData[i].TyreDamage.RearLeft;
                    wda_TyreDamage.RearRight = cdp.FieldCarDamageData[i].TyreDamage.RearRight;
                    OngoingCanvas.FieldData[i].TyreDamage = wda_TyreDamage;

                    //Brake damage
                    WheelDataArray wda_BrakeDamage = new WheelDataArray();
                    wda_BrakeDamage.FrontLeft = cdp.FieldCarDamageData[i].BrakeDamage.FrontLeft;
                    wda_BrakeDamage.FrontRight = cdp.FieldCarDamageData[i].BrakeDamage.FrontRight;
                    wda_BrakeDamage.RearLeft = cdp.FieldCarDamageData[i].BrakeDamage.RearLeft;
                    wda_BrakeDamage.RearRight = cdp.FieldCarDamageData[i].BrakeDamage.RearRight;
                    OngoingCanvas.FieldData[i].BrakeDamage = wda_BrakeDamage;

                    OngoingCanvas.FieldData[i].FrontLeftWingDamage = cdp.FieldCarDamageData[i].FrontLeftWingDamage;
                    OngoingCanvas.FieldData[i].FrontRightWingDamage = cdp.FieldCarDamageData[i].FrontRightWingDamange;
                    OngoingCanvas.FieldData[i].RearWingDamage = cdp.FieldCarDamageData[i].RearWingDamage;
                }


            }
        }

    }
}