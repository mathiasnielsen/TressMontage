using System;
using System.Threading.Tasks;

namespace TressMontage.Client.Core
{
    public interface INotificationService
    {
        Task<bool> DisplayToastAsync(string title, string message, bool isClickable = false);
    }
}
