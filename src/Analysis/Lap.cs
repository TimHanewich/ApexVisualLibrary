using System;
using ApexVisual;
using ApexVisual.SessionManagement;

namespace ApexVisual.Analysis
{
    public class Lap
    {
        public byte LapNumber {get; set;}
        public TelemetrySnapshot[] Corners {get; set;} 
        public float Sector1Time {get; set;}
        public float Sector2Time {get; set;}
        public float Sector3Time {get; set;}
        public bool LapInvalid {get; set;}
        public float FuelConsumed {get; set;}
        public float PercentOnThrottle {get; set;}
        public float PercentOnBrake {get; set;}
        public float PercentCoasting {get; set;}
        public float PercentThrottleBrakeOverlap {get; set;}
        public float PercentOnMaxThrottle {get; set;}
        public float PercentOnMaxBrake {get; set;}
        public int GearChanges {get; set;}
        public ushort TopSpeedKph {get; set;}
        public TyreCompound EquippedTyreCompound {get; set;}

        //Incremental Tyre Wear
        public WheelDataArray IncrementalTyreWear {get; set;}

        //Beginning tyre wear (snapshot)
        public WheelDataArray BeginningTyreWear {get; set;}

        public float LapTime()
        {
            return Sector1Time + Sector2Time + Sector3Time;
        }
    }
}