using System;
using System.Threading.Tasks;
using TressMontage.Client.Core;
using TressMontage.Client.Core.Services;
using Xamarin.Forms;

namespace TressMontage.Client.Services
{
    public class DialogService : IDialogService
    {
        private const string DefaultAcceptText = "accept";
        private const string DefaultDeclineText = "cancel";

        public Page MainPage => Application.Current.MainPage;

        public async Task<bool> DisplayAlertAsync(
            string title, 
            string message, 
            string acceptText = "",
            string declineText = "")
        {
            acceptText = UseTextOrDefault(acceptText, DefaultAcceptText);
            declineText = UseTextOrDefault(declineText, DefaultDeclineText);

            var result = await MainPage.DisplayAlert(title, message, acceptText, declineText);
            return result;
        }

        private string UseTextOrDefault(string text, string defaultText)
        { 
            return string.IsNullOrWhiteSpace(text) ? defaultText : text;
        }
    }
}
