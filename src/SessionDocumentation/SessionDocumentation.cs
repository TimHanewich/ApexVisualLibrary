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

        public float ConsistencyRating(byte corner_number)
        {
            //Weightings
            //Must all add up to 1!!
            float Weight_Speed = 0.5f;
            float Weight_Gear = 0.3f;
            float Weight_Steer = 0.2f;

            //Get the speed inconsistency rating
            float InconsistencyRating_Speed = SpeedInconsistencyRating(corner_number);
            float InconsistencyRating_Gear = GearInconsistencyRating(corner_number);
            float InconsistencyRating_Steer = SteerInconsistencyRating(corner_number);

            //Return NaN if any of the three are NaN
            if (InconsistencyRating_Speed == float.NaN || InconsistencyRating_Gear == float.NaN || InconsistencyRating_Steer == float.NaN)
            {
                return float.NaN;
            }

            //Get a weighted inconsistency rating
            float WeightedInconsistencyRating = 0;
            WeightedInconsistencyRating = WeightedInconsistencyRating + (Weight_Speed * InconsistencyRating_Speed);
            WeightedInconsistencyRating = WeightedInconsistencyRating + (Weight_Gear * InconsistencyRating_Gear);
            WeightedInconsistencyRating = WeightedInconsistencyRating + (Weight_Steer * InconsistencyRating_Steer);

            //Reverse it to make it a CONSISTENCY rating. i.e. 100% means perfectly consistent.
            float ToReturn = 1 - WeightedInconsistencyRating;
            return ToReturn;
        }

        public float SpeedInconsistencyRating(byte corner_number)
        {
            TelemetrySnapshot[] CornerSnapshots = GetTelemetrySnapshotsFromCorner(corner_number);
            if (CornerSnapshots.Length == 0)
            {
                return float.NaN;
            }

            List<float> Speeds = new List<float>();
            foreach (TelemetrySnapshot ts in CornerSnapshots)
            {
                Speeds.Add(ts.SpeedKph);
            }

            float stdev = MathToolkit.StandardDeviation(Speeds.ToArray());
            float mean = Speeds.ToArray().Average();
            float AsPercent = stdev / mean;
            AsPercent = Math.Abs(AsPercent);

            return AsPercent;
        }

        public float GearInconsistencyRating(byte corner_number)
        {
            TelemetrySnapshot[] CornerSnapshots = GetTelemetrySnapshotsFromCorner(corner_number);
            if (CornerSnapshots.Length == 0)
            {
                return float.NaN;
            }

            List<float> Gears = new List<float>();
            foreach (TelemetrySnapshot ts in CornerSnapshots)
            {
                float GearToAdd = 0;
                if (ts.Gear == Gear.Gear1)
                {
                    GearToAdd = 1;
                }
                else if (ts.Gear == Gear.Gear2)
                {
                    GearToAdd = 2;
                }
                else if (ts.Gear == Gear.Gear3)
                {
                    GearToAdd = 3;
                }
                else if (ts.Gear == Gear.Gear4)
                {
                    GearToAdd = 4;
                }
                else if (ts.Gear == Gear.Gear5)
                {
                    GearToAdd = 5;
                }
                else if (ts.Gear == Gear.Gear6)
                {
                    GearToAdd = 6;
                }
                else if (ts.Gear == Gear.Gear7)
                {
                    GearToAdd = 7;
                }
                else if (ts.Gear == Gear.Gear8)
                {
                    GearToAdd = 8;
                }
                Gears.Add(GearToAdd);
            }

            float stdev = MathToolkit.StandardDeviation(Gears.ToArray());
            float mean = Gears.ToArray().Average();
            float AsPercent = stdev / mean;
            AsPercent = Math.Abs(AsPercent);

            return AsPercent;
        }

        public float SteerInconsistencyRating(byte corner_number)
        {
            TelemetrySnapshot[] CornerSnapshots = GetTelemetrySnapshotsFromCorner(corner_number);
            if (CornerSnapshots.Length == 0)
            {
                return float.NaN;
            }

            List<float> Steers = new List<float>();
            foreach (TelemetrySnapshot ts in CornerSnapshots)
            {
                Steers.Add(ts.Steer);
            }

            float stdev = MathToolkit.StandardDeviation(Steers.ToArray());
            float mean = Steers.ToArray().Average();
            float AsPercent = stdev / mean;
            AsPercent = Math.Abs(AsPercent);

            return AsPercent;
        }

        #endregion
    
    }
}