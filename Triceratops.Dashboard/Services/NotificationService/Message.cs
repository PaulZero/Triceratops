using System;

namespace Triceratops.Dashboard.Services.NotificationService
{
    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Text { get; set; }

        public MessageLevel Level { get; set; }
    }
}
