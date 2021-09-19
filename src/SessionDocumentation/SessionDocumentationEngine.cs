using System;
using System.Collections.Generic;
using ApexVisual.SessionManagement;

namespace ApexVisual.SessionDocumentation
{
    public class SessionDocumentationEngine
    {
        //private Variables
        private List<Session> _Sessions;
        private List<Lap> _Laps;
        private List<TelemetrySnapshot> _TelemetrySnapshots;
        private List<WheelDataArray> _WheelDataArray;

        //Publicly accessing variables
        public Session[] Sessions
        {
            get
            {
                return _Sessions.ToArray();
            }
        }
        public Lap[] Laps
        {
            get
            {
                return _Laps.ToArray();
            }
        }
        public TelemetrySnapshot[] TelemetrySnapshots
        {
            get
            {
                return _TelemetrySnapshots.ToArray();
            }
        }
        public WheelDataArray[] WheelDataArrays
        {
            get
            {
                return _WheelDataArray.ToArray();
            }
        }
        
        public SessionDocumentationEngine()
        {
            _Sessions = new List<Session>();
            _Laps = new List<Lap>();
            _TelemetrySnapshots = new List<TelemetrySnapshot>();
            _WheelDataArray = new List<WheelDataArray>();
        }

    }
}