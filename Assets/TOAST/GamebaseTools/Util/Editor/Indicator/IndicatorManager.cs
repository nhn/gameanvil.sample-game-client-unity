using System.Collections.Generic;
using Toast.GamebaseTools.Util.Indicator.Internal;

namespace Toast.GamebaseTools.Util.Indicator
{
    public static class IndicatorManager
    {
        public static void Initialize(IndicatorConfigurations indicatorConfigurations)
        {
            IndicatorImplementation.Instance.Initialize(indicatorConfigurations);
        }

        public static void Send(Dictionary<string, string> data)
        {
            IndicatorImplementation.Instance.Send(data);
        }
    }
}
