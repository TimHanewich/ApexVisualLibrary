using System;
using System.Collections.Generic;
using ApexVisual;
using ApexVisual.SessionManagement;
using ApexVisual.Analysis;
using Newtonsoft.Json;
using ApexVisual.LiveSessionManagement;

namespace testing
{
    class Program
    {
        static void Main(string[] args)
        {
            List<byte[]> bytes = JsonConvert.DeserializeObject<List<byte[]>>(System.IO.File.ReadAllText(@"C:\Users\tahan\Downloads\spa race.json"));
            
            ApexVisualSessionManager sm = new ApexVisualSessionManager();
            sm.DataUpdateAvailable += Rec;
            foreach (byte[] b in bytes)
            {
                sm.IngestBytes(b);
            }
            System.Threading.Tasks.Task.Delay(3000).Wait();

            LiveSessionManager lsm = new LiveSessionManager();
            
            foreach (CommonSessionData csd in AllData)
            {
                lsm.Update(csd);
            }

            Console.WriteLine(JsonConvert.SerializeObject(lsm.LiveDriverData));
        }

        public static List<CommonSessionData> AllData = new List<CommonSessionData>();

        public static void Rec(CommonSessionData csd)
        {
            AllData.Add(csd);
        }
    }
}
