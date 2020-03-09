using System;
using Toast.GamebaseTools.Util.Launching.Internal;

namespace Toast.GamebaseTools.Util.Launching
{
    public static class LaunchingManager
    {
        public static void GetLaunchingInfo<T>(LaunchingConfigurations launchingConfigurations, Action<T> callback)
        {
            LaunchingImplementation.Instance.GetLaunchingInfo<T>(launchingConfigurations, callback);
        }
    }
}
