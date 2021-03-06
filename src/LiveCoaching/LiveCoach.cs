using System;
using ApexVisual.Analysis;
using ApexVisual.SessionManagement;
using ApexVisual;

namespace ApexVisual.LiveCoaching
{
    public class LiveCoach
    {
        //Vars
        private TrackDataContainer LoadedTrackData;
        public byte AtCorner {get; set;} //This is the corner number, not the corner index. So Corner #1 would be 1, not 0!
        public CornerStage AtCornerStage {get; set;}
        private bool Calibrating = true; //This means that the user has not yet hit an apex of a corner. Once they hit an apex of one corner (Came within a certain distance of it), we know we are calibrated.
        private float ApexDistanceThreshold = 18f;

        //For change-context
        private CommonSessionData LastReceivedPackets; //We will never receive a packet frame as a whole, but will use this to store each of them
        private bool ApexTelemetryAlreadyBroadcastedForCurrentCorner = false;

        //Events
        public event CornerChangedEventHandler CornerChanged;
        public event CornerStageChangedEventHandler CornerStageChanged;
        public event CornerApexComparisonAvailableHandler ApexTelemetryReceived;

        public LiveCoach(Track track)
        {
            LoadedTrackData = TrackDataContainer.LoadTrack(track);
            LastReceivedPackets = new CommonSessionData();
        }

        public void Update(CommonSessionData csd)
        {

            //Coaching
            //In order for this to happen, a few things must happen... (i will make the asssumption that the packets are UP TO DATE!)
            //This has to be a motion packet (this contains the X,Y, and Z position)
            //We have to already have seen the previous motion packet (this is used to see direction)
            //We have to already have seen the previous lap packet (this is used to see what sector we are in)

            if (csd.FieldData != null)
            {
                TrackLocation my_loc = new TrackLocation();
                my_loc.PositionX = csd.FieldData[csd.PlayerCarIndex].PositionX;
                my_loc.PositionY = csd.FieldData[csd.PlayerCarIndex].PositionY;
                my_loc.PositionZ = csd.FieldData[csd.PlayerCarIndex].PositionZ;
            
                if (Calibrating == true) //This is a new instance and we have to wait until the user gets to within a certain distancce of ANY corner to know what corner he is on
                {
                    //Find out what corner we are closest to
                    byte nearest_corner = 0;
                    float nearest_corner_distance = float.MaxValue;
                    for (int i = 0; i < LoadedTrackData.Corners.Length; i++) //Loop through each corner, measure the distance, and if this one is closest save it
                    {
                        TrackLocation tl = LoadedTrackData.Corners[i];

                        //Get my sector
                        byte mysector = 0;
                        switch (csd.FieldData[csd.PlayerCarIndex].CurrentSector)
                        {
                            case Sector.Sector1:
                                mysector = 1;
                                break;
                            case Sector.Sector2:
                                mysector = 2;
                                break;
                            case Sector.Sector3:
                                mysector = 3;
                                break;
                        }

                        if (tl.Sector == mysector) //We will only evaluate corners that are in the current sector we are in.
                        {
                            float this_distance = ApexVisualToolkit.DistanceBetweenTwoPoints(my_loc, tl);
                            if (this_distance < nearest_corner_distance)
                            {
                                nearest_corner_distance = this_distance;
                                nearest_corner = (byte)(i + 1); //Plus one because we want the corner number, not the index of the corner.
                            }
                        }
                    }

                    //If the corner is within the threshold, mark this as an apex hit and mark the corner # we are on
                    if (nearest_corner_distance <= ApexDistanceThreshold)
                    {
                        AtCorner = nearest_corner;
                        AtCornerStage = CornerStage.Apex;
                        Calibrating = false; //We are no longer calibrating

                        //Invoke the events
                        if (CornerChanged != null)
                        {
                            try
                            {
                                CornerChanged.Invoke(nearest_corner);
                            }
                            catch
                            {

                            }
                        }
                        if (CornerStageChanged != null)
                        {
                            try
                            {
                                CornerStageChanged.Invoke(CornerStage.Apex);
                            }
                            catch
                            {

                            }
                        }
                         
                    }
                }
                else //We are no longer calibrating
                {
                    
                    if (AtCornerStage == CornerStage.Apex) //If the mode is currently "at apex", then wait until the user leaves the threshold and then mark it as "leaving"
                    {
                        //Measure the distance in between the car and that corner
                        TrackLocation at_corner = LoadedTrackData.Corners[AtCorner-1]; 
                        float distance_to_corner = ApexVisualToolkit.DistanceBetweenTwoPoints(at_corner, my_loc);
                        if (distance_to_corner > ApexDistanceThreshold)
                        {
                            AtCornerStage = CornerStage.Exit;
                            if (CornerStageChanged != null)
                            {
                                try
                                {
                                    CornerStageChanged.Invoke(CornerStage.Exit);
                                }
                                catch
                                {

                                }
                            }
                        }
                        else //If we are still inside the threshold, try to pinpoint if we are moving toward the apex or away from it. If we are move away from it (we have passed it), invoke the corner apex telemetry received received.
                        {
                            if (ApexTelemetryAlreadyBroadcastedForCurrentCorner == false) //We don't want to broadcast it twice, so we check to see if we already broadcasted it.
                            {
                                TrackLocation last_loc = new TrackLocation();
                                last_loc.PositionX = LastReceivedPackets.FieldData[LastReceivedPackets.PlayerCarIndex].PositionX;
                                last_loc.PositionY = LastReceivedPackets.FieldData[LastReceivedPackets.PlayerCarIndex].PositionY;
                                last_loc.PositionZ = LastReceivedPackets.FieldData[LastReceivedPackets.PlayerCarIndex].PositionZ;
                                float last_distance_to_corner = ApexVisualToolkit.DistanceBetweenTwoPoints(last_loc, at_corner);

                                //If this distance is GREATER THAN the last distance, it means we are moving away from it (we passed it). So trigger the telemetry
                                if (distance_to_corner >= last_distance_to_corner)
                                {
                                    if (ApexTelemetryReceived != null)
                                    {
                                        try
                                        {
                                            ApexTelemetryReceived.Invoke(LastReceivedPackets.FieldData[LastReceivedPackets.PlayerCarIndex], at_corner);
                                        }
                                        catch
                                        {

                                        } 
                                    }
                                    ApexTelemetryAlreadyBroadcastedForCurrentCorner = true; //Flip this to true so we dont broadcast it again in this corner. This will be flipped back to false once we leave this corner.
                                }
                            }
                        }
                    }
                    else if (AtCornerStage == CornerStage.Exit) //If We are in the corner exit: check if we are closer to the previous corner that we are exiting or the next corner. If we are closer to the next corner, flip it to entry for that corner.
                    {

                        //Get the previous corner index and the next corner index
                        int Previous_Corner_Index = AtCorner - 1;
                        int Next_Corner_Index; //This is a bit more complicated because if we are at the LAST corner of the lap, the next corner index would be 1!
                        if (AtCorner == LoadedTrackData.Corners.Length) //If the current corner is the LAST corner in the track data location
                        {
                            Next_Corner_Index = 0;
                        }
                        else
                        {
                            Next_Corner_Index = Previous_Corner_Index + 1;
                        }

                        //Get the previous and next corners
                        TrackLocation Previous_Corner = LoadedTrackData.Corners[Previous_Corner_Index]; //The current corner that we are exiting from (reduce by 1 for an index)
                        TrackLocation Next_Corner = LoadedTrackData.Corners[Next_Corner_Index]; //The NEXT corner that we are now approaching.

                        //Check which one we are closer to 
                        float distance_to_prev = ApexVisualToolkit.DistanceBetweenTwoPoints(Previous_Corner, my_loc);
                        float distance_to_next = ApexVisualToolkit.DistanceBetweenTwoPoints(Next_Corner, my_loc);

                        //If we are closer to next corner, flip it to that corner entry
                        if (distance_to_next <= distance_to_prev)
                        {
                            AtCorner = (byte)(Next_Corner_Index + 1); //Flip the corner to the new corner
                            AtCornerStage = CornerStage.Entry; //Set it to entry

                            //Raise the events
                            if (CornerChanged != null)
                            {
                                try
                                {
                                    CornerChanged.Invoke((byte)(Next_Corner_Index + 1));
                                }
                                catch
                                {
                                    
                                }
                            }
                            if (CornerStageChanged != null)
                            {
                                try
                                {
                                    CornerStageChanged.Invoke(CornerStage.Entry);
                                }
                                catch
                                {

                                }
                            }
                            
                            

                            //Flip the Already broadccasted telemetry for this apex corner to false
                            ApexTelemetryAlreadyBroadcastedForCurrentCorner = false;
                        }
                    }
                    else if (AtCornerStage == CornerStage.Entry) //If we are at the corner entry stage, check to see if we are within the threshold to call it an apex hit
                    {
                        //Get this corner
                        TrackLocation This_Corner = LoadedTrackData.Corners[AtCorner-1];

                        //Measure the distance to that corner
                        float distance_to_corner = ApexVisualToolkit.DistanceBetweenTwoPoints(This_Corner, my_loc);

                        //If the distance is within the threshold, call it an apex hit
                        if (distance_to_corner <= ApexDistanceThreshold)
                        {
                            AtCornerStage = CornerStage.Apex;

                            //Raise the events
                            if (CornerStageChanged != null)
                            {
                                try
                                {
                                    CornerStageChanged.Invoke(CornerStage.Apex);
                                }
                                catch
                                {

                                }
                            }
                            
                        }
                    }
                }
            }

            
                



            #region "Save this packet"

            LastReceivedPackets = csd;

            #endregion
        }

    }
}