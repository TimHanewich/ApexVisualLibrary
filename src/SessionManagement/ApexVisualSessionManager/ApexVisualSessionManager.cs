using System;
using System.Collections.Generic;

namespace ApexVisual.SessionManagement
{
    public partial class ApexVisualSessionManager
    {
        //Events
        public event CommonSessionDataHandler CommonSessionDataAvailable;
        public event CommonCarDataHandler CommonCarDataAvailable;

        //Repository of data
        private CommonSessionData[] CommonSessionDataBackup;
        private CommonCarData[] CommonCarDataBackup;

        #region "Utility functions"

        private void TryRaiseSessionData(CommonSessionData csd)
        {
            try
            {
                CommonSessionDataAvailable.Invoke(csd);
            }
            catch
            {

            }
        }

        private void TryRaiseCarData(CommonCarData ccd)
        {
            try
            {
                CommonCarDataAvailable.Invoke(ccd);
            }
            catch
            {

            }
        }

        private void AddToCommonSessionDataBackup(CommonSessionData csd)
        {
            List<CommonSessionData> ToAddTo = new List<CommonSessionData>();
            ToAddTo.AddRange(CommonSessionDataBackup);
            ToAddTo.Add(csd);
            CommonSessionDataBackup = ToAddTo.ToArray();
        }

        private void AddToCommonCarDataBackup(CommonCarData ccd)
        {
            List<CommonCarData> ToAddTo = new List<CommonCarData>();
            ToAddTo.AddRange(CommonCarDataBackup);
            ToAddTo.Add(ccd);
            CommonCarDataBackup = ToAddTo.ToArray();
        }

        #endregion
    }
}