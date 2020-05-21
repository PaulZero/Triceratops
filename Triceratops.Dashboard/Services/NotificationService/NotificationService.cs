using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triceratops.Libraries.Helpers;

namespace Triceratops.Dashboard.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        public const string SessionKey = "triceratops.notification-service";

        private readonly List<Message> _newMessages = new List<Message>();

        private readonly List<Message> _oldMessages = new List<Message>();

        private readonly ISession _session;

        private readonly ILogger _logger;

        public NotificationService(ILogger<INotificationService> logger, ISession session)
        {
            _logger = logger;
            _session = session;

            _oldMessages.AddRange(ReadFromSession());
        }

        public IEnumerable<Message> GetMessages()
        {
            return _oldMessages;
        }

        public void PushMessage(string message, MessageLevel level)
        {
            _newMessages.Add(new Message
            {
                Text = message,
                Level = level
            });

            ClearSession();
            WriteToSession(_newMessages);
        }

        private Message[] ReadFromSession()
        {
            try
            {
                if (_session.TryGetValue(SessionKey, out var bytes) && bytes.Any())
                {
                    var json = Encoding.UTF8.GetString(bytes);

                    return JsonHelper.Deserialise<Message[]>(json) ?? new Message[0];
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Notification service could not read messages from session: {exception.Message}");
            }
            finally
            {
                ClearSession();
            }

            return new Message[0];
        }

        private void WriteToSession(IEnumerable<Message> messages)
        {
            try
            {
                var json = JsonHelper.Serialise(messages);
                var bytes = Encoding.UTF8.GetBytes(json);

                _session.Set(SessionKey, bytes);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Notification service could not write to session: {exception.Message}");
            }
        }

        private void ClearSession()
        {
            try
            {
                _session.Remove(SessionKey);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Notification service could not clear session: {exception.Message}");
            }
        }
    }
}
