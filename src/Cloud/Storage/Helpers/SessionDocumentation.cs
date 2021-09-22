using System;
using ApexVisual.SessionDocumentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApexVisual.Cloud.Storage.Helpers
{
    public class SessionDocumentationHelper
    {
        //For cloud retrieval (if needed)
        private ApexVisualManager avm;

        //Private vars
        private OriginalCapture _OriginalCapture;
        private Session _Session;
        private List<Lap> _Laps;
        private List<TelemetrySnapshot> _TelemetrySnapshots;
        private List<WheelDataArray> _WheelDataArray;

        //Public vars
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
    
        #region "Construction"

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
            ToReturn.SetAuthenticatedApexVisualManager(avm);
            Session s = await avm.DownloadSessionAsync(session_id);
            ToReturn._Session = s;
            return ToReturn;
        }
    
        #endregion
    
        public void SetAuthenticatedApexVisualManager(ApexVisualManager apex_viusal_manager)
        {
            avm = apex_viusal_manager;
        }


        #region "Retrieval"

        public async Task<OriginalCapture> OriginalCaptureAsync()
        {
            if (_OriginalCapture == null)
            {
                ThrowExceptionIfAvmNotProvided();
                _OriginalCapture = await avm.DownloadOriginalCaptureAsync(Session.SessionId);
            }
            return _OriginalCapture;
        }

        public async Task<Lap> LapAsync(byte lap_number)
        {
            foreach (Lap l in _Laps)
            {
                if (l.LapNumber == lap_number)
                {
                    return l;
                }
            }
            Lap lll = await avm.DownloadLapAsync(_Session.SessionId, lap_number);
            _Laps.Add(lll);
            return lll;
        }

        public async Task<Lap[]> AllLapsAsync()
        {
            Lap[] laps = await avm.DownloadLapsFromSessionAsync(Session.SessionId);
            _Laps.Clear();
            _Laps.AddRange(laps);
            return laps;
        }

        public async Task<byte[]> AvailableLapsAsync()
        {
            ThrowExceptionIfAvmNotProvided();
            byte[] ToReturn = await avm.AvailableLapsAsync(Session.SessionId);
            return ToReturn;
        }

        #endregion

        #region "Comprehensive"

        public async Task ComprehensiveRetrievalAsync()
        {
            ThrowExceptionIfAvmNotProvided();

            //Get original capture
            _OriginalCapture = await avm.DownloadOriginalCaptureAsync(Session.SessionId);

            //Get laps
            Lap[] ls = await avm.DownloadLapsFromSessionAsync(Session.SessionId);
            _Laps.Clear();
            _Laps.AddRange(ls);

            //Telemetry Snapshots
            TelemetrySnapshot[] snapshots = await avm.DownloadTelemetrySnapshotsAsync(Session.SessionId);
            _TelemetrySnapshots.Clear();
            _TelemetrySnapshots.AddRange(snapshots);

            //Now assemble all of the wheel data arrays
            List<Guid> ToDownloadWheelDataArrays = new List<Guid>();
            foreach (Lap l in _Laps)
            {
                ToDownloadWheelDataArrays.Add(l.EndingTyreWear);
            }
            foreach (TelemetrySnapshot ts in _TelemetrySnapshots)
            {
                ToDownloadWheelDataArrays.Add(ts.TyreWearPercent);
                ToDownloadWheelDataArrays.Add(ts.TyreDamagePercent);
            }
            _WheelDataArray.Clear();
            foreach (Guid g in ToDownloadWheelDataArrays)
            {
                WheelDataArray wda = await avm.DownloadWheelDataArrayAsync(g);
                _WheelDataArray.Add(wda);
            }

        }

        #endregion


        #region  "UTILITY"

        private void ThrowExceptionIfAvmNotProvided()
        {
            if (avm == null)
            {
                throw new Exception("The resource that was requested requires an authenticated instance of ApexVisualManager. One was not provided. Please use the 'SetAuthenticatedApexVisualManager' method.");
            }
        }

        #endregion
    
    }
}