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
            ushort pformat = BitConverter.ToUInt16(BAM.NextBytes(2));

            //Be sure there is a canvas
            if (OngoingCanvas == null)
            {
                OngoingCanvas = new CommonSessionData();
            }
            
            //Route it based on the game
            if (pformat == 2021)
            {
                Load2021Bytes(bytes);
            }

            //Alert of the newly available data
            TryRaiseDataUpdate(OngoingCanvas);
        }


        #region "Utility functions"

        private void TryRaiseDataUpdate(CommonSessionData fccd)
        {
            try
            {
                DataUpdateAvailable.Invoke(fccd);
            }
            catch
            {

            }
        }

        private uint GetFrameFromHeaderBytes(byte[] bytes)
        {
            //18, 19, 20, 21
            uint Frame = BitConverter.ToUInt32(new byte[] {bytes[18], bytes[19], bytes[20], bytes[21]});
            return Frame;
        }

        #endregion
    }
}