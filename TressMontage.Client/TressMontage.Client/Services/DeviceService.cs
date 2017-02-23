using System;
using TressMontage.Client.Core;
using Xamarin.Forms;

namespace TressMontage.Client
{
    public class DeviceService : IDeviceService
    {
        public DeviceType DeviceType
        {
            get
            {
                switch (Device.OS)
                { 
                    case TargetPlatform.Android:
                        return DeviceType.ANDROID;

                    case TargetPlatform.iOS:
                        return DeviceType.IOS;

                    case TargetPlatform.Windows:
                    case TargetPlatform.WinPhone:
                        return DeviceType.WINDOWS;

                    default:
                        return DeviceType.UNKNOWN;
                }
            }
        }
    }
}
