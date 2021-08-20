using System;
using ApexVisual.SessionManagement;
using TimHanewich.Csv;
using ApexVisual.Analysis;
using System.Drawing;
using System.Collections.Generic;

namespace ApexVisual
{
    public class ApexVisualToolkit
    {
        public static string GenerateSessionAsCsv(CommonSessionData[] session_data, byte for_index)
        {
            CsvFile csv = new CsvFile();

            //Some opening info about
            DataRow dr1 = csv.AddNewRow();
            dr1.Values.Add("Pilot");
            dr1.Values.Add(session_data[session_data.Length-1].FieldData[session_data[session_data.Length-1].PlayerCarIndex].Pilot.ToString());
            DataRow dr2 = csv.AddNewRow();
            dr2.Values.Add("Constructor");
            dr2.Values.Add(session_data[session_data.Length-1].FieldData[session_data[session_data.Length-1].PlayerCarIndex].Constructor.ToString());
            DataRow dr3 = csv.AddNewRow();
            dr3.Values.Add("Is AI Controlled");
            dr3.Values.Add(session_data[session_data.Length-1].FieldData[session_data[session_data.Length-1].PlayerCarIndex].IsAiControlled.ToString());
            DataRow dr4 = csv.AddNewRow();
            dr4.Values.Add("Name");
            dr4.Values.Add(session_data[session_data.Length-1].FieldData[session_data[session_data.Length-1].PlayerCarIndex].Name);

            //Add a blank row
            csv.AddNewRow().Values.Add("");

            //Headers
            DataRow dr = csv.AddNewRow();
            dr.Values.Add("Session Time");
            dr.Values.Add("Frame");
            dr.Values.Add("Weather");
            dr.Values.Add("Track Temperature (C)");
            dr.Values.Add("Air Temperature (C)");
            dr.Values.Add("Session Time Remaining");
            dr.Values.Add("Safety Car Status");
            dr.Values.Add("Position X");
            dr.Values.Add("Position Y");
            dr.Values.Add("Position Z");
            dr.Values.Add("Lateral G Force");
            dr.Values.Add("Longitudinal G Force");
            dr.Values.Add("Vertical G Force");
            dr.Values.Add("Front Wheel Angle");
            dr.Values.Add("Current Lap Time");
            dr.Values.Add("Lap Distance (Meters)");
            dr.Values.Add("Total Distance (Meters)");
            dr.Values.Add("Car Position");
            dr.Values.Add("Current Lap Number");
            dr.Values.Add("Pit Status");
            dr.Values.Add("Sector");
            dr.Values.Add("Current Lap Invalid");
            dr.Values.Add("Driver Status");
            dr.Values.Add("Speed KPH");
            dr.Values.Add("Throttle");
            dr.Values.Add("Steer");
            dr.Values.Add("Brake");
            dr.Values.Add("Clutch");
            dr.Values.Add("Gear");
            dr.Values.Add("Engine RPM");
            dr.Values.Add("DRS Active");
            dr.Values.Add("Brake Temperature - Front Left");
            dr.Values.Add("Brake Temperature - Front Right");
            dr.Values.Add("Brake Temperature - Rear Left");
            dr.Values.Add("Brake Temperature - Rear Right");
            dr.Values.Add("Tyre Surface Temperature - Front Left");
            dr.Values.Add("Tyre Surface Temperature - Front Right");
            dr.Values.Add("Tyre Surface Temperature - Rear Left");
            dr.Values.Add("Tyre Surface Temperature - Rear Right");
            dr.Values.Add("Tyre Inner Temperature - Front Left");
            dr.Values.Add("Tyre Inner Temperature - Front Right");
            dr.Values.Add("Tyre Inner Temperature - Rear Left");
            dr.Values.Add("Tyre Inner Temperature - Rear Right");
            dr.Values.Add("Engine Temperature");
            dr.Values.Add("Fuel Mix");
            dr.Values.Add("Fuel In Tank");
            dr.Values.Add("DRS Allowed");
            dr.Values.Add("Equipped Tyre Compound");
            dr.Values.Add("Tyre Age in Laps");
            dr.Values.Add("Stored ERS Energy");
            dr.Values.Add("ERS Deploy Mode");
            dr.Values.Add("Tyre Wear - Front Left");
            dr.Values.Add("Tyre Wear - Front Right");
            dr.Values.Add("Tyre Wear - Rear Left");
            dr.Values.Add("Tyre Wear - Rear Right");
            dr.Values.Add("Tyre Damage - Front Left");
            dr.Values.Add("Tyre Damage - Front Right");
            dr.Values.Add("Tyre Damage - Rear Left");
            dr.Values.Add("Tyre Damage - Rear Right");
            dr.Values.Add("Brake Damage - Front Left");
            dr.Values.Add("Brake Damage - Front Right");
            dr.Values.Add("Brake Damage - Rear Left");
            dr.Values.Add("Brake Damage - Rear Right");
            dr.Values.Add("Front Left Wing Damage");
            dr.Values.Add("Front Right Wing Damage");
            dr.Values.Add("Rear Wing Damage");

            //Print them
            CommonSessionData LastSeenCommonSessionData = session_data[0];
            foreach (CommonSessionData csd in session_data)
            {
                if (csd.FrameIdentifier != LastSeenCommonSessionData.FrameIdentifier) //We are on a new frame now. So print the most complete version of the last frame!
                {
                    if (LastSeenCommonSessionData.FieldData != null)
                    {
                        DataRow ndr = csv.AddNewRow();

                        ndr.Values.Add(LastSeenCommonSessionData.SessionTime.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FrameIdentifier.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.CurrentWeather.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.TrackTemperatureCelsius.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.AirTemperatureCelsius.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.SessionTimeLeftSeconds.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.CurrentSafetyCarStatus.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].PositionX.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].PositionY.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].PositionZ.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].gForceLateral.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].gForceLongitudinal.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].gForceVertical.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].FrontWheelAngle.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].CurrentLapTimeSeconds.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].LapDistanceMeters.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TotalDistanceMeters.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].CarPosition.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].CurrentLapNumber.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].CurrentPitStatus.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].CurrentSector.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].CurrentLapInvalid.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].CurrentDriverStatus.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].SpeedKph.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].Throttle.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].Steer.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].Brake.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].Clutch.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].Gear.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].EngineRpm.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].DrsActive.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].BrakeTemperature.FrontLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].BrakeTemperature.FrontRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].BrakeTemperature.RearLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].BrakeTemperature.RearRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreSurfaceTemperature.FrontLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreSurfaceTemperature.FrontRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreSurfaceTemperature.RearLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreSurfaceTemperature.RearRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreInnerTemperature.FrontLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreInnerTemperature.FrontRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreInnerTemperature.RearLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreInnerTemperature.RearRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].EngineTemperature.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].ActiveFuelMix.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].FuelInTank.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].DrsAllowed.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].EquippedTyreCompound.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreAgeLaps.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].StoredErsEnergy.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].ActiveErsDeployMode.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreWear.FrontLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreWear.FrontRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreWear.RearLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreWear.RearRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreDamage.FrontLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreDamage.FrontRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreDamage.RearLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].TyreDamage.RearRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].BrakeDamage.FrontLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].BrakeDamage.FrontRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].BrakeDamage.RearLeft.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].BrakeDamage.RearRight.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].FrontLeftWingDamage.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].FrontRightWingDamage.ToString());
                        ndr.Values.Add(LastSeenCommonSessionData.FieldData[for_index].RearWingDamage.ToString());
                    }
                }

                //Update last seens
                LastSeenCommonSessionData = csd; 
            }


            return csv.GenerateAsCsvFileContent();
        }
    
        public static float DistanceBetweenTwoPoints(TrackLocation loc1, TrackLocation loc2)
        {
            float x_2 = (float)Math.Pow(loc2.PositionX - loc1.PositionX, 2);
            float y_2 = (float)Math.Pow(loc2.PositionY - loc1.PositionY, 2);
            float z_2 = (float)Math.Pow(loc2.PositionZ - loc1.PositionZ, 2);
            float dist = (float)Math.Sqrt(x_2 + y_2 + z_2);
            return dist;
        }
    
        public static Color GetTeamColorByTeam(Team t, ushort year)
        {
            if (year == 2019)
            {
                if (t == Team.Mercedes)
                {
                    return Color.FromArgb(255, 0, 210, 190);
                }
                else if (t == Team.Haas)
                {
                    return Color.FromArgb(255, 189, 158, 87);
                }
                else if (t == Team.McLaren)
                {
                    return Color.FromArgb(255, 255, 135, 0);
                }
                else if (t == Team.AlfaRomeo)
                {
                    return Color.FromArgb(255, 155, 0, 0);
                }
                else if (t == Team.RedBullRacing)
                {
                    return Color.FromArgb(255, 30, 65, 255);
                }
                else if (t == Team.Renault)
                {
                    return Color.FromArgb(255, 255, 245, 0);
                }
                else if (t == Team.Ferrari)
                {
                    return Color.FromArgb(255, 220, 0, 0);
                }
                else if (t == Team.ToroRosso)
                {
                    return Color.FromArgb(255, 70, 155, 255);
                }
                else if (t == Team.Williams)
                {
                    return Color.FromArgb(255, 255, 255, 255);
                }
                else if (t == Team.RacingPoint)
                {
                    return Color.FromArgb(255, 245, 150, 200);
                }
            }
            else if (year == 2020)
            {
                if (t == Team.Mercedes)
                {
                    return Color.FromArgb(255, 0, 210, 190);
                }
                else if (t == Team.Haas)
                {
                    return Color.FromArgb(255, 120, 120, 120);
                }
                else if (t == Team.McLaren)
                {
                    return Color.FromArgb(255, 255, 135, 0);
                }
                else if (t == Team.AlfaRomeo)
                {
                    return Color.FromArgb(255, 155, 0, 0);
                }
                else if (t == Team.RedBullRacing)
                {
                    return Color.FromArgb(255, 6, 0, 239);
                }
                else if (t == Team.Renault)
                {
                    return Color.FromArgb(255, 255, 245, 0);
                }
                else if (t == Team.Ferrari)
                {
                    return Color.FromArgb(255, 220, 0, 0);
                }
                else if (t == Team.AlphaTauri)
                {
                    return Color.FromArgb(255, 255, 255, 255);
                }
                else if (t == Team.Williams)
                {
                    return Color.FromArgb(255, 0, 130, 250);
                }
                else if (t == Team.RacingPoint)
                {
                    return Color.FromArgb(255, 245, 150, 200);
                }
            }
            else if (year == 2021)
            {
                if (t == Team.Mercedes)
                {
                    return Color.FromArgb(255, 0, 210, 190);
                }
                else if (t == Team.Haas)
                {
                    return Color.FromArgb(255, 255, 255, 255);
                }
                else if (t == Team.McLaren)
                {
                    return Color.FromArgb(255, 255, 152, 0);
                }
                else if (t == Team.AlfaRomeo)
                {
                    return Color.FromArgb(255, 144, 0, 0);
                }
                else if (t == Team.RedBullRacing)
                {
                    return Color.FromArgb(255, 6, 0, 239);
                }
                else if (t == Team.Alpine)
                {
                    return Color.FromArgb(255, 0, 144, 255);
                }
                else if (t == Team.Ferrari)
                {
                    return Color.FromArgb(255, 220, 0, 0);
                }
                else if (t == Team.AlphaTauri)
                {
                    return Color.FromArgb(255, 43, 69, 98);
                }
                else if (t == Team.Williams)
                {
                    return Color.FromArgb(255, 0, 90, 255);
                }
                else if (t == Team.AstonMartin)
                {
                    return Color.FromArgb(255, 0, 111, 98);
                }
            }

            throw new Exception("Team colors unavailable for format year '" + year.ToString() + "' or for the specified team.");
        }

        public static string GetDriverDisplayNameByDriver(Driver d)
        {
            if (d == Driver.LewisHamilton)
            {
                return "L. Hamilton";
            }
            else if (d == Driver.ValtteriBottas)
            {
                return "V. Bottas";
            }
            else if (d == Driver.RomainGrosjean)
            {
                return "R. Grosjean";
            }
            else if (d == Driver.KevinMagnussen)
            {
                return "K. Magnussen";
            }
            else if (d == Driver.ValtteriBottas)
            {
                return "V. Bottas";
            }
            else if (d == Driver.RobertKubica)
            {
                return "R. Kubica";
            }
            else if (d == Driver.SergioPerez)
            {
                return "S. Perez";
            }
            else if (d == Driver.LanceStroll)
            {
                return "L. Stroll";
            }
            else if (d == Driver.CarlosSainz)
            {
                return "C. Sainz";
            }
            else if (d == Driver.LandoNorris)
            {
                return "L. Norris";
            }
            else if (d == Driver.KimiRaikkonen)
            {
                return "K. Raikkonen";
            }
            else if (d == Driver.AntonioGiovinazzi)
            {
                return "A. Giovinazzi";
            }
            else if (d == Driver.MaxVerstappen)
            {
                return "M. Verstappen";
            }
            else if (d == Driver.AlexanderAlbon)
            {
                return "A. Albon";
            }
            else if (d == Driver.DanielRicciardo)
            {
                return "D. Ricciardo";
            }
            else if (d == Driver.NicoHulkenburg)
            {
                return "N. Hulkenburg";
            }
            else if (d == Driver.SebastianVettel)
            {
                return "S. Vettel";
            }
            else if (d == Driver.CharlesLeclerc)
            {
                return "C. Leclerc";
            }
            else if (d == Driver.PierreGasly)
            {
                return "P. Gasly";
            }
            else if (d == Driver.DaniilKvyat)
            {
                return "D. Kvyat";
            }
            else if (d == Driver.GeorgeRussell)
            {
                return "G. Russell";
            }
            else if (d == Driver.NicholasLatifi)
            {
                return "N. Latifi";
            }
            else if (d == Driver.EstebanOcon)
            {
                return "E. Ocon";
            }
            else if (d == Driver.FernandoAlonso)
            {
                return "F. Alonso";
            }
            else if (d == Driver.FelipeMassa)
            {
                return "F. Massa";
            }
            else if (d == Driver.NikitaMazepin)
            {
                return "N. Mazepin";
            }
            else if (d == Driver.MickSchumacher)
            {
                return "M. Schumacher";
            }
            else if (d == Driver.YukiTsunoda)
            {
                return "Y. Tsunoda";
            }

            return d.ToString();
        }

        public static string CleanseString(string original, string allowed_characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_ ")
        {
            string ToReturn = "";
            foreach (char c in original)
            {
                int num = Convert.ToInt32(c);
                if (num == 160) //160 is a non breaking space.
                {
                    ToReturn = ToReturn + " ";
                }
                else
                {
                    if (allowed_characters.Contains(c.ToString()))
                    {
                        ToReturn = ToReturn + c.ToString();
                    }
                } 
            }
            return ToReturn;
        }
    
        public static string GetLapTimeDisplayFromSeconds(float seconds)
        {
            int number_of_minutes = (int)Math.Floor(seconds / 60);
            float remaining = seconds - (number_of_minutes * 60);
            string s = number_of_minutes.ToString() + ":" + remaining.ToString("#00.000");
            return s;
        }
    
        public static string GetDriverStatusFriendlyName(DriverStatus ds)
        {
            switch (ds)
            {
                case DriverStatus.InGarage:
                    return "In Garage";
                case DriverStatus.FlyingLap:
                    return "Flying Lap";
                case DriverStatus.InLap:
                    return "In Lap";
                case DriverStatus.OutLap:
                    return "Out Lap";
                case DriverStatus.OnTrack:
                    return "On Track";
                default:
                    return ds.ToString();
            }
        }
    
        public static string GetDriverThreeLetters(Driver d)
        {
            List<KeyValuePair<Driver, string>> dict = new List<KeyValuePair<Driver, string>>();
            dict.Add(new KeyValuePair<Driver, string>(Driver.KimiRaikkonen, "RAI"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.AntonioGiovinazzi, "GIO"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.PierreGasly, "GAS"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.YukiTsunoda, "TSU"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.FernandoAlonso, "ALO"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.EstebanOcon, "OCO"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.SebastianVettel, "VET"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.LanceStroll, "STR"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.CharlesLeclerc, "LEC"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.CarlosSainz, "SAI"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.NikitaMazepin, "MAZ"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.MickSchumacher, "MSC"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.DanielRicciardo, "RIC"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.LandoNorris, "NOR"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.LewisHamilton, "HAM"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.ValtteriBottas, "BOT"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.SergioPerez, "PER"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.MaxVerstappen, "VER"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.NicholasLatifi, "LAT"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.GeorgeRussell, "RUS"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.DaniilKvyat, "KVY"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.NicoHulkenburg, "HUL"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.AlexanderAlbon, "ALB"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.JackAitken, "AIT"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.RomainGrosjean, "GRO"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.KevinMagnussen, "MAG"));
            dict.Add(new KeyValuePair<Driver, string>(Driver.RobertKubica, "KUB"));

            foreach (KeyValuePair<Driver, string> kvp in dict)
            {
                if (kvp.Key == d)
                {
                    return kvp.Value;
                }
            }

            return d.ToString();
        }
    }
}