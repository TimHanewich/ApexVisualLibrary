using System;

namespace ApexVisual.SessionManagement
{
    public class CommonCarData
    {
        
        //From motion packet
        public float PositionX {get; set;}
        public float PositionY {get; set;}
        public float PositionZ {get; set;}
        public float gForceLateral {get; set;}
        public float gForceLongitudinal {get; set;}
        public float gForceVertical {get; set;}

        //From motion packet - Players car ONLY
        public WheelDataArray WheelSpeed {get; set;}
        public float FrontWheelAngle {get; set;} //Front wheel angle in radians

        //From lap data packet
        public float LastLapTimeSeconds {get; set;}
        public float CurrentLapTimeSeconds {get; set;}
        public float Sector1TimeSeconds {get; set;}
        public float Sector2TimeSeconds {get; set;}
        public float LapDistanceMeters {get; set;}
        public float TotalDistanceMeters {get; set;}
        public byte CarPosition {get; set;}
        public byte CurrentLapNumber {get; set;}
        public PitStatus CurrentPitStatus {get; set;}
        public Sector CurrentSector {get; set;}
        public bool CurrentLapInvalid {get; set;}
        public byte Penalties {get; set;} //Total time in seconds of penalties so far.
        public byte StartingGridPosition {get; set;}
        public DriverStatus CurrentDriverStatus {get; set;}

        //Taken from Telemetry packet
        public ushort SpeedKph {get; set;}
        public float Throttle {get; set;}
        public float Steer {get; set;}
        public float Brake {get; set;}
        public byte Clutch {get; set;}
        public sbyte Gear {get; set;}
        public ushort EngineRpm {get; set;}
        public bool DrsActive {get; set;}
        public byte RevLightsPercentage {get; set;}
        public WheelDataArray BrakeTemperature {get; set;}
        public WheelDataArray TyreSurfaceTemperature {get; set;}
        public WheelDataArray TyreInnerTemperature {get; set;}
        public ushort EngineTemperature {get; set;}
        
        //Taken from Car status packet
        public FuelMix ActiveFuelMix {get; set;}
        public float FuelInTank {get; set;}
        public float FuelCapacity {get; set;}
        public float FuelRemainingLaps {get; set;}
        public bool DrsAllowed {get; set;}
        public TyreCompound EquippedTyreCompound {get; set;}
        public byte TyreAgeLaps {get; set;} //Tyre age in laps
        public float StoredErsEnergy {get; set;}
        public ErsDeployMode ActiveErsDeployMode {get; set;}

        //Taken from participants packet
        public bool IsAiControlled {get; set;}
        public Driver Pilot {get; set;}
        public Team Constructor {get; set;}
        public byte RaceNumber {get; set;}
        public string Name {get; set;}

        //Taken from car damage packet
        public WheelDataArray TyreWear {get; set;}
        public WheelDataArray TyreDamage {get; set;}
        public WheelDataArray BrakeDamage {get; set;}
        public byte FrontLeftWingDamage {get; set;}
        public byte FrontRightWingDamage {get; set;}
        public byte RearWingDamage {get; set;}

    }
}