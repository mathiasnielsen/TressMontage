using System;
using System.Threading.Tasks;

namespace TressMontage.Client.Core.Services
{
    public interface IDialogService
    {
        Task<bool> DisplayAlertAsync(string title, string message, string acceptText = "", string declineText = "");
    }
}
