using System;

namespace ApexVisual.SessionDocumentation
{
    public class SessionDocumentation
    {
        public OriginalCapture OriginalCapture {get; set;}
        public Session Session {get; set;}
        public Lap[] Laps {get; set;}
        public TelemetrySnapshot[] TelemetrySnapshots {get; set;}
        public WheelDataArray[] WheelDataArrays {get; set;}
    }
}