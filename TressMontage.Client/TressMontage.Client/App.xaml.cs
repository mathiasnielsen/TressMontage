using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using TressMontage.Client.Core.Services;
using TressMontage.Client.Extensions;
using TressMontage.Client.Features.Common;
using TressMontage.Client.Features.Home;
using TressMontage.Client.Services;
using TressMontage.Core.IOC;
using Xamarin.Forms;

namespace TressMontage.Client
{
    public partial class App : Application
    {
        private static IUnityContainer _container;

        public static IUnityContainer Container
        {
            get
            {
                if (_container != null) return _container;

                _container = new UnityContainer();
                _container.AddExtension(new InitializationExtension());
                RegisterCoreTypes();

                return _container;
            }
        }

        public App()
        {
            InitializeComponent();

            var content = new HomeView();
            MainPage = new MasterNavigationPage(content);
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

        /// <summary>
        /// Creates IoC type mappings for interfaces which are implemented in the Core proejct.
        /// Platform-specific types are registered in the Setup class on each platform.
        /// </summary>
        private static void RegisterCoreTypes()
        {
            // Multi-instance objects
            //_container.RegisterType<IBonjourAdapter, BonjourAdapter>();

            // Singleton objects
            _container.RegisterSingleton<INavigationService, NavigationService>();
            _container.RegisterSingleton<IStorageService, StorageService>();
            _container.RegisterSingleton<IFileInfoManager, FileInfoManager>();
        }
    }
}
