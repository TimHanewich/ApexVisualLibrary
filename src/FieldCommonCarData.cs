using System;

namespace ApexVisual
{
    public class FieldCommonCarData
    {
        //Taken from packet header
        public ulong SessionId {get; set;}
        public float SessionTime {get; set;}
        public uint FrameIdentifier {get; set;}
        public byte PlayerCarIndex {get; set;}

        public CommonCarData[] FieldData {get; set;}
    }
}