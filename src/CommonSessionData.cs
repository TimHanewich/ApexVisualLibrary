using System;

namespace ApexVisual
{
    public class CommonSessionData
    {
        public WeatherCondition CurrentWeather {get; set;}
        public byte TrackTemperatureCelsius {get; set;}
        public byte AirTemperatureCelsius {get; set;}
        public byte LapsInRace {get; set;}
        public SessionType ThisSessionType {get; set;}
        public Track SessionTrack {get; set;}
        public ushort SessionTimeLeftSeconds {get; set;}
        public ushort SessionDurationSeconds {get; set;}
        public SafetyCarStatus CurrentSafetyCarStatus {get; set;}
    }
}