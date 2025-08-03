using System;

namespace Application.Notifications
{
    internal struct NotificationConfig
    {
        public readonly string Title;
        public readonly string Content;
        public DateTime FireTime;

        public NotificationConfig(string title, string content, DateTime fireTime)
        {
            Title = title;
            Content = content;
            FireTime = fireTime;
        }
    }
}