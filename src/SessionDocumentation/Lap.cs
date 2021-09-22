using System;
using ApexVisual.SessionManagement;

namespace ApexVisual.SessionDocumentation
{
    public class Lap
    {
        public Guid Id {get; set;}
        public UInt64 FromSession {get; set;}
        public byte LapNumber {get; set;}
        public float Sector1Time {get; set;}
        public float Sector2Time {get; set;}
        public float Sector3Time {get; set;}
        public float EndingFuel {get; set;}
        public byte PercentOnThrottle {get; set;}
        public byte PercentOnBrake {get; set;}
        public byte PercentCoasting {get; set;}
        public byte PercentOnMaxThrottle {get; set;}
        public byte PercentOnMaxBrake {get; set;}
        public float EndingErs {get; set;}
        public short GearChanges {get; set;}
        public TyreCompound EquippedTyreCompound {get; set;}
        public Guid EndingTyreWear {get; set;}

        public float LapTime()
        {
            return Sector1Time + Sector2Time + Sector3Time;
        }
    }
}