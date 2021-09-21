using System;

namespace ApexVisual.SessionDocumentation
{
    public class TelemetrySnapshot
    {
        public Guid Id {get; set;}
        public Guid FromLap {get; set;}
        public float SessionTime {get; set;}
        public TrackLocationType LocationType {get; set;}
        public byte LocationNumber {get; set;}
        public float PositionX {get; set;}
        public float PositionY {get; set;}
        public float PositionZ {get; set;}
        public float CurrentLapTime {get; set;}
        public byte CarPosition {get; set;}
        public bool LapInvalid {get; set;}
        public short SpeedKph {get; set;}
        public byte Throttle {get; set;}
        public short Steer {get; set;}
        public byte Brake {get; set;}
        public Gear Gear {get; set;}
        public bool DrsActive {get; set;}
        public Guid TyreWearPercent {get; set;}
        public Guid TyreDamagePercent {get; set;}
        public float StoredErs {get; set;}
    }
}