using System;

namespace ApexVisual
{
    public class FieldCommonCarData
    {
        //Taken from packet header
        public ulong SessionId {get; set;}
        public float SessionTime {get; set;}
        public uint FrameIdentifier {get; set;}
        public byte PlayerCarIndex {get; set;}

        //Taken from session packet
        public WeatherCondition CurrentWeather {get; set;}
        public byte TrackTemperatureCelsius {get; set;}
        public byte AirTemperatureCelsius {get; set;}
        public byte LapsInRace {get; set;}
        public SessionType ThisSessionType {get; set;}
        public Track SessionTrack {get; set;}
        public ushort SessionTimeLeftSeconds {get; set;}
        public ushort SessionDurationSeconds {get; set;}
        public SafetyCarStatus CurrentSafetyCarStatus {get; set;}

        public CommonCarData[] FieldData {get; set;}
    }
}