using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TressMontage.Client.Services
{
    public class NavigationBase
    {
        public NavigationBase()
        {
        }

        private Dictionary<string, Type> pages { get; }
            = new Dictionary<string, Type>();

        public Page MainPage => Application.Current.MainPage;

        public void Configure(string key, Type pageType) => pages[key] = pageType;

        public void GoBack() => MainPage.Navigation.PopAsync();

        public void NavigateTo(string pageKey, Dictionary<string, string> parameter = null,
            HistoryBehavior historyBehavior = HistoryBehavior.Default)
        {
            Type pageType;
            if (pages.TryGetValue(pageKey, out pageType))
            {
                var displayPage = (Page)Activator.CreateInstance(pageType);
                displayPage.SetNavigationArgs(parameter);

                if (historyBehavior == HistoryBehavior.ClearHistory)
                {
                    MainPage.Navigation.InsertPageBefore(displayPage,
                        MainPage.Navigation.NavigationStack[0]);

                    var existingPages = MainPage.Navigation.NavigationStack.ToList();
                    for (int i = 1; i < existingPages.Count; i++)
                        MainPage.Navigation.RemovePage(existingPages[i]);
                }
                else
                {
                    MainPage.Navigation.PushAsync(displayPage);
                }
            }
            else
            {
                throw new ArgumentException($"No such page: {pageKey}.",
                    nameof(pageKey));
            }
        }
    }

    public static class NavigationExtensions
    {
        private static ConditionalWeakTable<Page, Dictionary<string, string>> arguments
            = new ConditionalWeakTable<Page, Dictionary<string, string>>();

        public static Dictionary<string, string> GetNavigationArgs(this Page page)
        {
            Dictionary<string, string> argument = null;
            arguments.TryGetValue(page, out argument);

            return argument ?? new Dictionary<string, string>();
        }

        public static void SetNavigationArgs(this Page page, Dictionary<string, string> args)
            => arguments.Add(page, args);
    }
}
