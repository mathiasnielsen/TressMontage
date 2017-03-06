using Plugin.Toasts;
using Plugin.Toasts.UWP;
using TressMontage.Client.Core;
using TressMontage.Client.Extensions;
using TressMontage.Client.UWP.Services;

namespace TressMontage.Client.UWP
{
    public class Setup
    {
        public void Bootstrap()
        {
            Register();
            Initialize();
        }

        private void Register()
        {
            TressMontage.Client.App.Container.RegisterSingleton<INotificationService, NotificationService>();
            TressMontage.Client.App.Container.RegisterSingleton<IToastNotificator, ToastNotification>();
        }

        private void Initialize()
        {
            ToastNotification.Init();
        }
    }
}
