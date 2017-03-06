using System.Threading.Tasks;
using Plugin.Toasts;
using TressMontage.Client.Core;
using Microsoft.Practices.Unity;

namespace TressMontage.Client.UWP.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IToastNotificator notificator;

        public NotificationService()
        {
            notificator = TressMontage.Client.App.Container.Resolve<IToastNotificator>();
        }

        public async Task<bool> DisplayToastAsync(string title, string message, bool isClickable = false)
        {
            var options = new NotificationOptions()
            {
                Title = title,
                Description = message,
                IsClickable = isClickable
            };

            var result = await notificator.Notify(options);

            return result.Action == NotificationAction.Clicked;
        }
    }
}
