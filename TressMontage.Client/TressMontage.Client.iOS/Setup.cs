using Plugin.Toasts;
using TressMontage.Client.Core;
using TressMontage.Client.Extensions;
using UserNotifications;
using Xamarin.Forms;

namespace TressMontage.Client.iOS
{
    public class Setup
    {
        public void Bootstrap()
        {
            Register();
            Initialize();
            RequestAuthorization();
        }

        private void Register()
        {
            App.Container.RegisterSingleton<INotificationService, NotificationService>();
            App.Container.RegisterSingleton<IToastNotificator, ToastNotification>();
        }

        private void Initialize()
        {
            ToastNotification.Init();
        }

        private void RequestAuthorization()
        {
            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | 
                UNAuthorizationOptions.Badge | 
                UNAuthorizationOptions.Sound,
                (granted, error) =>
                {
                      // Do something if needed
                });
        }
    }
}
