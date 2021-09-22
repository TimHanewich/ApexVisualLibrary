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

        #region "Comprehensive"

        public async Task ComprehensiveUploadAsync(ApexVisualManager avm, SessionDocumentationEngine sde, bool update_percent = true)
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

        public async Task ComprehensiveDeleteAsync(ApexVisualManager avm, ulong session_id, bool update_percent = false)
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
            try
            {
                ProcessingPercentCompleteUpdate.Invoke(percent);
            }
            catch
            {

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

        #endregion
    
    }
}