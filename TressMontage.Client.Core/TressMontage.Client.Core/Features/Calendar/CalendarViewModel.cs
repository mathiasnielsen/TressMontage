using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TressMontage.Client.Core.Features.Base;

namespace TressMontage.Client.Core.Features.Calendar
{
    public class CalendarViewModel : BindableViewModelBase
    {
        private readonly INotificationService notificationService;
        private readonly IDeviceService deviceService;

        public CalendarViewModel(INotificationService notificationService, IDeviceService deviceService)
        {
            this.notificationService = notificationService;
            this.deviceService = deviceService;
        }

        public override async Task OnViewInitialized(Dictionary<string, string> navigationParameters)
        {
            if (deviceService.DeviceType == DeviceType.IOS)
            { 
                await notificationService.DisplayToastAsync("Hrumpf!", "If on iOS, calendar is not working");
            }
        }
    }
}
