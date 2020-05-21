using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Dashboard.Services.NotificationService
{
    public interface INotificationService
    {
        void PushMessage(string message, MessageLevel level);

        IEnumerable<Message> GetMessages();
    }
}
