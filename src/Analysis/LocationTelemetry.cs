using System;

namespace ApexVisual.Analysis
{
    public class LocationTelemetry
    {
        //Optimums
        public float SpeedMph {get; set;} //Probably a major point of interest
        public sbyte Gear {get; set;} //Probably a major point of interest
        public float Steer {get; set;}
        public float Throttle {get; set;}
        public float Brake {get; set;}
    }
}