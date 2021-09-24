using System;
using ApexVisual.SessionDocumentation;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApexVisual.LiveCoaching;

namespace ApexVisual.Cloud.Storage.Helpers
{
    public class SessionDocumentationHelper
    {
        
        public event PercentUpdate ProcessingPercentCompleteUpdate;
        public event IntHandler ResourcesDownloadedUpdate;

        #region "Comprehensive"

        public async Task ComprehensiveUploadAsync(ApexVisualManager avm, ApexVisual.SessionDocumentation.SessionDocumentation sde, bool update_percent = true)
        {   
        
            //Get the total # of resources to upload.
            int TotalCountToUpload = 1 + sde.Laps.Length + sde.TelemetrySnapshots.Length + sde.WheelDataArrays.Length; //The # to upload. Add 1 for the session (1 session)
            int Uploaded = 0;

            //Attempt the delete
            await ComprehensiveDeleteAsync(avm, sde.Session.SessionId, false);

            //Upload the original capture if needed
            bool OriginalCaptureAlreadyExists = await avm.OriginalCaptureExistsAsync(sde.Session.SessionId);
            if (OriginalCaptureAlreadyExists == false)
            {
                await avm.UploadOriginalCaptureAsync(sde.OriginalCapture);
            }

            //Upload session
            await avm.UploadSessionAsync(sde.Session);
            Uploaded = Uploaded + 1;
            UpdatePercentComplete(Uploaded, TotalCountToUpload, update_percent);

            //Upload laps
            foreach (Lap l in sde.Laps)
            {
                await avm.UploadLapAsync(l);
                Uploaded = Uploaded + 1;
                UpdatePercentComplete(Uploaded, TotalCountToUpload, update_percent);
            }

            //upload telemetry snapshots
            foreach (TelemetrySnapshot ts in sde.TelemetrySnapshots)
            {
                await avm.UploadTelemetrySnapshotAsync(ts);
                Uploaded = Uploaded + 1;
                UpdatePercentComplete(Uploaded, TotalCountToUpload, update_percent);
            }

            //upload wheel data arrays
            foreach (WheelDataArray wda in sde.WheelDataArrays)
            {
                await avm.UploadWheelDataArrayAsync(wda);
                Uploaded = Uploaded + 1;
                UpdatePercentComplete(Uploaded, TotalCountToUpload, update_percent);
            }
        }

        public async Task<SessionDocumentation.SessionDocumentation> ComprehensiveDownloadAsync(ApexVisualManager avm, ulong session_id, bool update_resources_downloaded = true)
        {
            SessionDocumentation.SessionDocumentation ToReturn = new SessionDocumentation.SessionDocumentation();

            int ResourcesDownloaded = 0;

            //Get the original capture
            bool OC_Exists = await avm.OriginalCaptureExistsAsync(session_id);
            if (OC_Exists)
            {
                ToReturn.OriginalCapture = await avm.DownloadOriginalCaptureAsync(session_id);
                ResourcesDownloaded = ResourcesDownloaded + 1;
                UpdateResourcesDownloaded(ResourcesDownloaded, update_resources_downloaded);
            }

            //Get the session
            ToReturn.Session = await avm.DownloadSessionAsync(session_id);
            ResourcesDownloaded = ResourcesDownloaded + 1;
            UpdateResourcesDownloaded(ResourcesDownloaded, update_resources_downloaded);

            //Get the laps
            ToReturn.Laps = await avm.DownloadLapsFromSessionAsync(session_id);
            ResourcesDownloaded = ResourcesDownloaded + ToReturn.Laps.Length;
            UpdateResourcesDownloaded(ResourcesDownloaded, update_resources_downloaded);

            //Get the telemetry snapshots
            ToReturn.TelemetrySnapshots = await avm.DownloadTelemetrySnapshotsAsync(session_id);
            ResourcesDownloaded = ResourcesDownloaded + ToReturn.TelemetrySnapshots.Length;
            UpdateResourcesDownloaded(ResourcesDownloaded, update_resources_downloaded);

            //Get the Wheel data arrays
            ToReturn.WheelDataArrays = await avm.DownloadWheelDataArraysAsync(session_id);
            ResourcesDownloaded = ResourcesDownloaded + ToReturn.WheelDataArrays.Length;
            UpdateResourcesDownloaded(ResourcesDownloaded, update_resources_downloaded);

            return ToReturn;
        }

        public async Task ComprehensiveDeleteAsync(ApexVisualManager avm, ulong session_id, bool update_percent = true)
        {
            //Intentionally do NOT delete the OriginalCapture. This is meant to be permanent.

            UpdatePercentComplete(0f);

            //Delte the wheel data arrays first
            await avm.DeleteWheelDataArraysAsync(session_id);
            UpdatePercentComplete(0.25f, update_percent);
            
            //Now delete the telemetry snapshots
            await avm.DeleteTelemetrySnapshotsAsync(session_id);
            UpdatePercentComplete(0.50f, update_percent);

            //Now delete  the laps
            await avm.DeleteLapsAsync(session_id);
            UpdatePercentComplete(0.75f, update_percent);

            //Now delete the session itself
            await avm.DeleteSessionAsync(session_id);
            UpdatePercentComplete(1f, update_percent);
        }

        #endregion


        #region  "UTILITY"

        //between 0 and 1
        private void UpdatePercentComplete(float percent)
        {
            if (ProcessingPercentCompleteUpdate != null)
            {
                try
                {
                    ProcessingPercentCompleteUpdate.Invoke(percent);
                }
                catch
                {

                }
            }
        }

        private void UpdatePercentComplete(float percent, bool ActuallyDoIt)
        {
            if (ActuallyDoIt)
            {
                UpdatePercentComplete(percent);
            }
        }

        private void UpdatePercentComplete(int complete, int out_of)
        {
            float percent = Convert.ToSingle(complete) / Convert.ToSingle(out_of);
            UpdatePercentComplete(percent);
        }

        private void UpdatePercentComplete(int complete, int out_of, bool ActuallyDoIt)
        {
            if (ActuallyDoIt)
            {
                UpdatePercentComplete(complete, out_of);
            }
        }

        private void UpdateResourcesDownloaded(int value, bool ActuallyDoIt)
        {
            if (ActuallyDoIt)
            {
                if (ResourcesDownloadedUpdate != null)
                {
                    ResourcesDownloadedUpdate.Invoke(value);
                }
            }
        }

        #endregion
    
    }
}