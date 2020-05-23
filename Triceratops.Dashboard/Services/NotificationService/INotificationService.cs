using System.Collections.Generic;

namespace Triceratops.Dashboard.Services.NotificationService
{
    public interface INotificationService
    {
        void PushMessage(string message, MessageLevel level);

        IEnumerable<Message> GetMessages();
    }
}
