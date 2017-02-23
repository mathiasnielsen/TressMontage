using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Plugin.Toasts;
using TressMontage.Client.Core;

namespace TressMontage.Client.iOS
{
    /// <summary>
    /// Pluging from source: https://xamarinhelp.com/toast-notifications-xamarin-forms/
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IToastNotificator notificator;

        public NotificationService()
        {
            notificator = App.Container.Resolve<IToastNotificator>();
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
