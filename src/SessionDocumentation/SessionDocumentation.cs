using System;
using System.Collections.Generic;
using TimHanewich.Toolkit;
using System.Linq;

namespace ApexVisual.SessionDocumentation
{
    public class SessionDocumentation
    {
        public OriginalCapture OriginalCapture {get; set;}
        public Session Session {get; set;}
        public Lap[] Laps {get; set;}
        public TelemetrySnapshot[] TelemetrySnapshots {get; set;}
        public WheelDataArray[] WheelDataArrays {get; set;}

        #region "Manipulation"

        public Lap GetLap(byte lap_number)
        {
            foreach (Lap l in Laps)
            {
                if (l.LapNumber == lap_number)
                {
                    return l;
                }
            }
            throw new Exception("Unable to find lap #" + lap_number.ToString());
        }

        public Lap GetLap(Guid id)
        {
            foreach (Lap l in Laps)
            {
                if (l.Id == id)
                {
                    return l;
                }
            }
            throw new Exception("Unable to find lap with Id '" + id.ToString() + "'");
        }
    
        public TelemetrySnapshot[] GetTelemetrySnapshotsFromLap(byte from_lap)
        {
            //Get the lap
            Lap FromLap = GetLap(from_lap);

            List<TelemetrySnapshot> ToReturn = new List<TelemetrySnapshot>();
            foreach (TelemetrySnapshot ts in TelemetrySnapshots)
            {
                if (ts.FromLap == FromLap.Id)
                {
                    ToReturn.Add(ts);
                }
            }
            return ToReturn.ToArray();
        }
    
        public TelemetrySnapshot[] GetTelemetrySnapshotsFromLap(Lap l)
        {
            return GetTelemetrySnapshotsFromLap(l.LapNumber);
        }
    
        public TelemetrySnapshot[] GetTelemetrySnapshotsFromCorner(byte corner_number)
        {
            List<TelemetrySnapshot> ToReturn = new List<TelemetrySnapshot>();
            foreach (TelemetrySnapshot ts in TelemetrySnapshots)
            {
                if (ts.LocationType == TrackLocationType.Corner)
                {
                    if (ts.LocationNumber == corner_number)
                    {
                        ToReturn.Add(ts);
                    }
                }
            }
            return ToReturn.ToArray();
        }
    
        public WheelDataArray GetWheelDataArray(Guid id)
        {
            foreach (WheelDataArray wda in WheelDataArrays)
            {
                if (wda.Id == id)
                {
                    return wda;
                }
            }
            throw new Exception("Unable to find WheelDataArray with Id '" + id.ToString() + "'");
        }
    
        

        public byte NumberOfCorners()
        {
            byte MaxCorner = 0;
            foreach (TelemetrySnapshot ts in TelemetrySnapshots)
            {
                if (ts.LocationType == TrackLocationType.Corner)
                {
                    if (ts.LocationNumber > MaxCorner)
                    {
                        MaxCorner = ts.LocationNumber;
                    }
                }
            }
            return MaxCorner;
        }

        #endregion
    
        #region "Performance Rating"

        public float SpeedInconsistencyRating(byte corner_number)
        {
            TelemetrySnapshot[] CornerSnapshots = GetTelemetrySnapshotsFromCorner(corner_number);

            List<float> Speeds = new List<float>();
            foreach (TelemetrySnapshot ts in CornerSnapshots)
            {
                Speeds.Add(ts.SpeedKph);
            }

            float stdev = MathToolkit.StandardDeviation(Speeds.ToArray());
            float mean = Speeds.ToArray().Average();
            float AsPercent = stdev / mean;

            return AsPercent;
        }

        #endregion
    
    }
}