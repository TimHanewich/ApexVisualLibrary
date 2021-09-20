using System;
using System.Collections.Generic;
using ApexVisual;
using ApexVisual.SessionManagement;
using ApexVisual.Analysis;
using Newtonsoft.Json;
using ApexVisual.LiveSessionManagement;
using Newtonsoft.Json.Linq;
using Codemasters.F1_2021;
using ApexVisual.LiveCoaching;
using ApexVisual.SessionDocumentation;

namespace testing
{
    class Program
    {
        static void Main(string[] args)
        {
            List<byte[]> bytes = JsonConvert.DeserializeObject<List<byte[]>>(System.IO.File.ReadAllText(@"C:\Users\tahan\Downloads\Sample Telemetry Data\2021\spa race.json"));
            Console.Write("Converting... ");
            CommonSessionData[] AllData = ApexVisualSessionManager.BulkConvert(bytes);
            Console.WriteLine(AllData.Length.ToString("#,##0"));

            Console.Write("Feeding... ");
            SessionDocumentationEngine sde = new SessionDocumentationEngine();
            foreach (CommonSessionData csd in AllData)
            {
                sde.Update(csd);
            }
            Console.WriteLine("Done");

            Console.WriteLine(sde.Sessions.Length.ToString() + " sessions");
            Console.WriteLine(sde.Laps.Length.ToString() + " laps");
            Console.WriteLine(sde.TelemetrySnapshots.Length.ToString() + " telemetry snapshot");
            Console.WriteLine(sde.WheelDataArrays.Length.ToString() + " wheel data arrays");

            

        }

        public static UInt64 DecodeFromSql(Int64 value)
        {
            return Convert.ToUInt64(value + Int64.MaxValue) + 1;
        }

        public static Int64 EncodeForSql(UInt64 value)
        {
            if (value > Int64.MaxValue)
            {
                UInt64 ToConvert = value - Convert.ToUInt64(Int64.MaxValue) - 1;
                return Convert.ToInt64(ToConvert);
            }
            else
            {
                Int64 ToPullDown = Convert.ToInt64(value);
                return ToPullDown - Int64.MaxValue - 1;
            }
        }

        public static void SaveIt(CommonSessionData csd)
        {

        }
    }
}
