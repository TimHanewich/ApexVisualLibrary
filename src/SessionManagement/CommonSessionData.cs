using System;
using System.Collections.Generic;

namespace ApexVisual.SessionManagement
{
    public class CommonSessionData
    {
        //Taken from packet header
        public ulong SessionId {get; set;}
        public float SessionTime {get; set;}
        public uint FrameIdentifier {get; set;}
        public byte PlayerCarIndex {get; set;}
        public ushort Format {get; set;} //Year of the F1 game. For example, '2021'

        //Taken from session packet
        public WeatherCondition CurrentWeather {get; set;}
        public byte TrackTemperatureCelsius {get; set;}
        public byte AirTemperatureCelsius {get; set;}
        public byte LapsInRace {get; set;}
        public SessionType? ThisSessionType {get; set;}
        public Track? SessionTrack {get; set;}
        public ushort SessionTimeLeftSeconds {get; set;}
        public ushort SessionDurationSeconds {get; set;}
        public SafetyCarStatus CurrentSafetyCarStatus {get; set;}

        //Taken from participants packet
        public byte NumberOfActiveCars {get; set;} //Number of cars in this data.

        public CommonCarData[] FieldData {get; set;}

        public void InitializeFieldDataIfNeeded(int number_needed)
        {
            bool NeedToDo = false;
            if (FieldData == null)
            {
                NeedToDo = true;
            }
            else
            {
                if (FieldData.Length != number_needed)
                {
                    NeedToDo = true;
                }
            }

            if (NeedToDo)
            {
                List<CommonCarData> ToAdd = new List<CommonCarData>();
                for (int i = 0; i < number_needed; i++)
                {
                    ToAdd.Add(new CommonCarData());
                }
                FieldData = ToAdd.ToArray();
            }

        }
    
        public CommonSessionData Copy()
        {
            CommonSessionData ToReturn = new CommonSessionData();

            ToReturn.SessionId = SessionId;
            ToReturn.SessionTime = SessionTime;
            ToReturn.FrameIdentifier = FrameIdentifier;
            ToReturn.PlayerCarIndex = PlayerCarIndex;
            ToReturn.Format = Format;
            ToReturn.CurrentWeather = CurrentWeather;
            ToReturn.TrackTemperatureCelsius = TrackTemperatureCelsius;
            ToReturn.AirTemperatureCelsius = AirTemperatureCelsius;
            ToReturn.LapsInRace = LapsInRace;
            ToReturn.ThisSessionType = ThisSessionType;
            ToReturn.SessionTrack = SessionTrack;
            ToReturn.SessionTimeLeftSeconds = SessionTimeLeftSeconds;
            ToReturn.SessionDurationSeconds = SessionDurationSeconds;
            ToReturn.CurrentSafetyCarStatus = CurrentSafetyCarStatus;
            ToReturn.NumberOfActiveCars = NumberOfActiveCars;

            //Copy each car data
            if (FieldData != null)
            {
                List<CommonCarData> CopiedCarData = new List<CommonCarData>();
                foreach (CommonCarData ccd in FieldData)
                {
                    CopiedCarData.Add(ccd.Copy());
                }
                ToReturn.FieldData = CopiedCarData.ToArray();
            }
            

            return ToReturn;
        }
    }
}