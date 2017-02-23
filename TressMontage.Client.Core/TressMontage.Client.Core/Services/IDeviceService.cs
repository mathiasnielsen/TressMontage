using System;
namespace TressMontage.Client.Core
{
    public interface IDeviceService
    {
        DeviceType DeviceType { get; }
    }

    public enum DeviceType
    { 
        UNKNOWN = 0,
        IOS,
        ANDROID,
        WINDOWS
    }
}
