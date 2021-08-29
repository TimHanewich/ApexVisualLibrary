using System;

namespace ApexVisual.Cloud.DeveloperMessaging
{
    public class MessageSubmission
    {
        public string Username {get; set;}
        public string Email {get; set;}
        public string Body {get; set;}
        public MessageType MessageType {get; set;}
        public DateTimeOffset CreatedAt {get; set;}

        public MessageSubmission()
        {
            CreatedAt = DateTimeOffset.Now;
        }
    }
}