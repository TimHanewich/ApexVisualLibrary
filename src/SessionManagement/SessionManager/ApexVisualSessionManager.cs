using System;
using System.Collections.Generic;
using TimHanewich.Toolkit;

namespace ApexVisual.SessionManagement
{
    public partial class ApexVisualSessionManager
    {
        //Events
        public event CommonSessionDataHandler DataUpdateAvailable;

        //Ongoing canvas
        private CommonSessionData OngoingCanvas;


        public void IngestBytes(byte[] bytes)
        {
            ByteArrayManager BAM = new ByteArrayManager(bytes);

            //Get the packet format (game)
            ushort pformat = BitConverter.ToUInt16(BAM.NextBytes(2), 0);

            //Be sure there is a canvas
            if (OngoingCanvas == null)
            {
                OngoingCanvas = new CommonSessionData();
            }
            
            //Route it based on the game
            bool UpdateApplied = true; //Default to assume is yes, changes were made.
            if (pformat == 2021)
            {
                UpdateApplied = Load2021Bytes(bytes);
            }
            else if (pformat == 2020)
            {
                UpdateApplied = Load2020Bytes(bytes);
            }
            else if (pformat == 2019)
            {
                UpdateApplied = Load2019Bytes(bytes);
            }
            else
            {
                throw new Exception("Telemetry format '" + pformat.ToString() + "' not supported.");
            }

            //Alert of the newly available data
            if (UpdateApplied)
            {
                TryRaiseDataUpdate(OngoingCanvas);
            }
        }

        #region "All in one bulk convert"

        private static List<CommonSessionData> ForBulkConverting;

        public static CommonSessionData[] BulkConvert(List<byte[]> all_bytes)
        {
            ApexVisualSessionManager sm = new ApexVisualSessionManager();
            ForBulkConverting = new List<CommonSessionData>();
            ForBulkConverting.Clear();
            sm.DataUpdateAvailable += SaveUpdate;
            foreach (byte[] b in all_bytes)
            {
                sm.IngestBytes(b);
            }
            return ForBulkConverting.ToArray();
        }

        private static void SaveUpdate(CommonSessionData csd)
        {
            ForBulkConverting.Add(csd);
        }

        #endregion

        #region "Utility functions"

        private void TryRaiseDataUpdate(CommonSessionData fccd)
        {
            if (DataUpdateAvailable != null)
            {
                try
                {
                    DataUpdateAvailable.Invoke(fccd.Copy());
                }
                catch
                {

                }
            }
        }

        private uint GetFrameFromHeaderBytes(byte[] bytes)
        {
            //18, 19, 20, 21
            uint Frame = BitConverter.ToUInt32(new byte[] {bytes[18], bytes[19], bytes[20], bytes[21]}, 0);
            return Frame;
        }

        #endregion
    }
}