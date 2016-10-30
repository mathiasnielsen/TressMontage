using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TressMontage.Client.Features.Home;
using Xamarin.Forms;

namespace TressMontage.Client
{
    public class App : Application
    {
        ////private static IUnityContainer _container;

        ////public static IUnityContainer Container
        ////{
        ////    get
        ////    {
        ////        if (_container != null) return _container;

        ////        _container = new UnityContainer();
        ////        _container.AddExtension(new InitializationExtension());
        ////        RegisterCoreTypes();

        ////        return _container;
        ////    }
        ////}

        public App()
        {
            var content = new HomeView();
            MainPage = new NavigationPage(content);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
