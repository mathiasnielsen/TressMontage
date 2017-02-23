using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Microsoft.Practices.Unity;
using Plugin.Toasts;
using TressMontage.Core.IOC;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

namespace TressMontage.Client.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private static IUnityContainer _container;

        public static IUnityContainer Container
        {
            get
            {
                if (_container != null) return _container;

                _container = new UnityContainer();
                _container.AddExtension(new InitializationExtension());

                return _container;
            }
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            var setup = new Setup();
            setup.Bootstrap();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
