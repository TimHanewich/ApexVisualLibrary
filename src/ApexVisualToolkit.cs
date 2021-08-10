using System;
using ApexVisual.SessionManagement;
using TimHanewich.Csv;

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
            csv.AddNewRow();

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


            foreach (CommonSessionData csd in session_data)
            {
                DataRow ndr = csv.AddNewRow();

                ndr.Values.Add(csd.SessionTime.ToString());
                ndr.Values.Add(csd.FrameIdentifier.ToString());
                ndr.Values.Add(csd.CurrentWeather.ToString());
                ndr.Values.Add(csd.TrackTemperatureCelsius.ToString());
                ndr.Values.Add(csd.AirTemperatureCelsius.ToString());
                ndr.Values.Add(csd.SessionTimeLeftSeconds.ToString());
                ndr.Values.Add(csd.CurrentSafetyCarStatus.ToString());
                ndr.Values.Add(csd.FieldData[for_index].PositionX.ToString());
                ndr.Values.Add(csd.FieldData[for_index].PositionY.ToString());
                ndr.Values.Add(csd.FieldData[for_index].PositionZ.ToString());
                ndr.Values.Add(csd.FieldData[for_index].gForceLateral.ToString());
                ndr.Values.Add(csd.FieldData[for_index].gForceLongitudinal.ToString());
                ndr.Values.Add(csd.FieldData[for_index].gForceVertical.ToString());
                ndr.Values.Add(csd.FieldData[for_index].FrontWheelAngle.ToString());
                ndr.Values.Add(csd.FieldData[for_index].CurrentLapTimeSeconds.ToString());
                ndr.Values.Add(csd.FieldData[for_index].LapDistanceMeters.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TotalDistanceMeters.ToString());
                ndr.Values.Add(csd.FieldData[for_index].CarPosition.ToString());
                ndr.Values.Add(csd.FieldData[for_index].CurrentLapNumber.ToString());
                ndr.Values.Add(csd.FieldData[for_index].CurrentPitStatus.ToString());
                ndr.Values.Add(csd.FieldData[for_index].CurrentSector.ToString());
                ndr.Values.Add(csd.FieldData[for_index].CurrentLapInvalid.ToString());
                ndr.Values.Add(csd.FieldData[for_index].CurrentDriverStatus.ToString());
                ndr.Values.Add(csd.FieldData[for_index].SpeedKph.ToString());
                ndr.Values.Add(csd.FieldData[for_index].Throttle.ToString());
                ndr.Values.Add(csd.FieldData[for_index].Steer.ToString());
                ndr.Values.Add(csd.FieldData[for_index].Brake.ToString());
                ndr.Values.Add(csd.FieldData[for_index].Clutch.ToString());
                ndr.Values.Add(csd.FieldData[for_index].Gear.ToString());
                ndr.Values.Add(csd.FieldData[for_index].EngineRpm.ToString());
                ndr.Values.Add(csd.FieldData[for_index].DrsActive.ToString());
                ndr.Values.Add(csd.FieldData[for_index].BrakeTemperature.FrontLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].BrakeTemperature.FrontRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].BrakeTemperature.RearLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].BrakeTemperature.RearRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreSurfaceTemperature.FrontLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreSurfaceTemperature.FrontRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreSurfaceTemperature.RearLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreSurfaceTemperature.RearRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreInnerTemperature.FrontLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreInnerTemperature.FrontRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreInnerTemperature.RearLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreInnerTemperature.RearRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].EngineTemperature.ToString());
                ndr.Values.Add(csd.FieldData[for_index].ActiveFuelMix.ToString());
                ndr.Values.Add(csd.FieldData[for_index].FuelInTank.ToString());
                ndr.Values.Add(csd.FieldData[for_index].DrsAllowed.ToString());
                ndr.Values.Add(csd.FieldData[for_index].EquippedTyreCompound.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreAgeLaps.ToString());
                ndr.Values.Add(csd.FieldData[for_index].StoredErsEnergy.ToString());
                ndr.Values.Add(csd.FieldData[for_index].ActiveErsDeployMode.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreWear.FrontLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreWear.FrontRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreWear.RearLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreWear.RearRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreDamage.FrontLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreDamage.FrontRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreDamage.RearLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].TyreDamage.RearRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].BrakeDamage.FrontLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].BrakeDamage.FrontRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].BrakeDamage.RearLeft.ToString());
                ndr.Values.Add(csd.FieldData[for_index].BrakeDamage.RearRight.ToString());
                ndr.Values.Add(csd.FieldData[for_index].FrontLeftWingDamage.ToString());
                ndr.Values.Add(csd.FieldData[for_index].FrontRightWingDamage.ToString());
                ndr.Values.Add(csd.FieldData[for_index].RearWingDamage.ToString());
            }


            return csv.GenerateAsCsvFileContent();
        }
    }
}