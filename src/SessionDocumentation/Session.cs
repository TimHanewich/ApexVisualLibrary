using System;
using ApexVisual.SessionManagement;

namespace ApexVisual.SessionDocumentation
{
    public class Session
    {
        public UInt64 SessionId {get; set;}
        public Guid Owner {get; set;}
        public CodemastersF1Game Game {get; set;}
        public Track Track {get; set;}
        public SessionType Mode {get; set;}
        public Team Team {get; set;}
        public Driver Driver {get; set;}
        public DateTime UploadedAtUtc {get; set;}
    }
}