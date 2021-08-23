using System;
using System.Collections.Generic;
using ApexVisual;
using ApexVisual.SessionManagement;
using ApexVisual.Analysis;
using Newtonsoft.Json;
using ApexVisual.LiveSessionManagement;
using Newtonsoft.Json.Linq;

namespace testing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Give me the raw data: ");
            string val = Console.ReadLine();

            LocationTelemetry[] tele = OldToNewOptima(val);

            Console.WriteLine();
            Console.WriteLine(JsonConvert.SerializeObject(tele).Replace("\"", "\\\""));

        }

        public static LocationTelemetry[] OldToNewOptima(string old)
        {
            string toconvjson = old.Replace("\\", "");
            JObject mjo = JObject.Parse(toconvjson);
            JArray ja = JArray.Parse(mjo.Property("Corners").Value.ToString());

            List<LocationTelemetry> ToReturn = new List<LocationTelemetry>();
            foreach (JObject jo in ja)
            {
                LocationTelemetry lt = new LocationTelemetry();

                lt.SpeedMph = Convert.ToSingle(jo.Property("OptimalSpeedMph").Value.ToString());
                lt.Gear = Convert.ToSByte(jo.Property("OptimalGear").Value.ToString());
                lt.Steer = Convert.ToSingle(jo.Property("OptimalSteer").Value.ToString());
                lt.Throttle = Convert.ToSingle(jo.Property("OptimalThrottle").Value.ToString());
                lt.Brake = Convert.ToSingle(jo.Property("OptimalBrake").Value.ToString());

                ToReturn.Add(lt);
            }

            return ToReturn.ToArray();
        }

    }
}
