using System;
using ApexVisual.SessionManagement;
using System.Collections.Generic;

namespace ApexVisual.LiveSessionManagement
{
    public class LiveSessionManager
    {
        private bool Initialized;
        public LiveDriverSessionData[] LiveDriverData {get; set;}

        public LiveSessionManager()
        {
            Initialized = false;
        }

        public void Update(CommonSessionData csd)
        {
            if (Initialized == false) //We have not set up the array of live driver data yet
            {
                if (csd.FieldData != null) //We wait for the participant to set up
                {
                    List<LiveDriverSessionData> NewData = new List<LiveDriverSessionData>();
                    for (int t = 0; t < csd.FieldData.Length; t++)
                    {
                        LiveDriverSessionData ldsd = new LiveDriverSessionData();
                        ldsd.SelectedDriver = csd.FieldData[t].Pilot;
                        ldsd.TeamColor =  ApexVisualToolkit.GetTeamColorByTeam(csd.FieldData[t].Constructor, csd.Format);
                        ldsd.SelectedTeam = csd.FieldData[t].Constructor;

                        //The driver display name
                        ldsd.DriverDisplayName = ApexVisualToolkit.GetDriverDisplayNameByDriver(csd.FieldData[t].Pilot); //If the driver is not recognized (it is a real player, index 100, 101, 102, etc) this will return "Unknown"
                        if (csd.FieldData[t].IsAiControlled == false) //If it is a player (the above most likely made the displat name 'Unknown', use the player name instead)
                        {
                            ldsd.DriverDisplayName = ApexVisualToolkit.CleanseString(csd.FieldData[t].Name);
                        }

                        NewData.Add(ldsd);
                    }
                    LiveDriverData = NewData.ToArray();
                    Initialized = true;
                }
            }
            else //We already have established a list of live driver session data
            {

                //Update each
                for (int t = 0; t < LiveDriverData.Length; t++)
                {
                    LiveDriverData[t].Update(csd.FieldData[t], csd.SessionTime);
                }


                //Supply the driver ahead distance for all cars (except first place)
                foreach (LiveDriverSessionData ldsd in LiveDriverData)
                {
                    if (ldsd.Position != 1) //Only do this for cars that are NOT in first place
                    {
                        //Find the car that is directly ahead
                        foreach (CommonCarData ld in csd.FieldData)
                        {
                            if (ld.CarPosition == ldsd.Position - 1) //If it is the car ahead
                            {
                                ldsd.SetDriverAheadData(ld.TotalDistanceMeters);
                            }
                        }
                    }
                }

                //Plug the session type into each
                foreach (LiveDriverSessionData ldsd in LiveDriverData)
                {
                    if (csd.ThisSessionType.HasValue)
                    {
                        ldsd.SetSessionType(csd.ThisSessionType.Value);
                    }
                }
            }
        }
    }
}