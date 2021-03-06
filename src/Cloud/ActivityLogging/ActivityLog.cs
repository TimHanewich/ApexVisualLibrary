using System;

namespace ApexVisual.Cloud.ActivityLogging
{
    public class ActivityLog
    {
        public Guid SessionId {get; set;} //A unique identifier for this session. A new one is created when launched and then the remainder follow suit.
        public Guid ByUser {get; set;}
        public DateTimeOffset TimeStamp {get; set;}
        public ApplicationType ApplicationId {get; set;}
        public ActivityType ActivityId {get; set;}
        public PackageVersion PackageVersion {get; set;}
        public string Note {get; set;}

        public ActivityLog(Guid? use_id = null)
        {
            TimeStamp = DateTimeOffset.Now;
            if (use_id != null)
            {
                SessionId = use_id.Value;
            }
            else
            {
                SessionId = Guid.NewGuid();
            }
        }

    }
}