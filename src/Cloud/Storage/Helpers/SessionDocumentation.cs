using System;
using ApexVisual.SessionDocumentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApexVisual.Cloud.Storage.Helpers
{
    public class SessionDocumentationHelper
    {
        //Private vars
        private OriginalCapture _OriginalCapture;
        private Session _Session;
        private List<Lap> _Laps;
        private List<TelemetrySnapshot> _TelemetrySnapshots;
        private List<WheelDataArray> _WheelDataArray;

        //Public vars
        public OriginalCapture OriginalCapture
        {
            get
            {
                return _OriginalCapture;
            }
        }
        public Session Session
        {
            get
            {
                return _Session;
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
    
        private void Initialize()
        {
            _Laps = new List<Lap>();
            _TelemetrySnapshots = new List<TelemetrySnapshot>();
            _WheelDataArray = new List<WheelDataArray>();
        }

        public SessionDocumentationHelper()
        {
            Initialize();
        }

        public SessionDocumentationHelper(SessionDocumentationEngine sde)
        {
            Initialize();
            _OriginalCapture = sde.OriginalCapture;
            _Session = sde.Session;
            _Laps.AddRange(sde.Laps);
            _TelemetrySnapshots.AddRange(sde.TelemetrySnapshots);
            _WheelDataArray.AddRange(sde.WheelDataArrays);
        }
    
        public static async Task<SessionDocumentationHelper> LoadSessionAsync(ApexVisualManager avm, ulong session_id)
        {
            SessionDocumentationHelper ToReturn = new SessionDocumentationHelper();
            Session s = await avm.DownloadSessionAsync(session_id);
            ToReturn._Session = s;
            return ToReturn;
        }
    }
}