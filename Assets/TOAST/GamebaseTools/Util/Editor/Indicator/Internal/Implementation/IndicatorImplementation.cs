using System.Collections.Generic;

namespace Toast.GamebaseTools.Util.Indicator.Internal
{
    public class IndicatorImplementation
    {
        private static readonly IndicatorImplementation instance = new IndicatorImplementation();
        public static IndicatorImplementation Instance
        {
            get { return instance; }
        }

        private IndicatorConfigurations indicatorConfigurations;
        private IndicatorSendManager indicatorSendManager = new IndicatorSendManager();
        private bool isInitialized = false;
        public void Initialize(IndicatorConfigurations indicatorConfigurations)
        {
            this.indicatorConfigurations = indicatorConfigurations;
            isInitialized = true;
        }

        public void Send(Dictionary<string, string> data, bool ignoreActivation = false)
        {
            if(isInitialized == false)
            {
                return;
            }

            Dictionary<string, string> sendData = new Dictionary<string, string>(data);
            Dictionary<string, string> projectInfo = indicatorConfigurations.GetProjectInfoAsDictionary();
            MergeDictionary(ref sendData, projectInfo);

            indicatorSendManager.SendLog(
                new SendData(
                    indicatorConfigurations.GetUrl(),
                    indicatorConfigurations.GetLogVersion(),
                    sendData
                    ));
        }

        private void MergeDictionary(ref Dictionary<string, string> originalData, Dictionary<string, string> additionalData)
        {
            if (additionalData == null)
            {
                return;
            }

            if (originalData == null)
            {
                originalData = new Dictionary<string, string>();
            }

            foreach (string key in additionalData.Keys)
            {
                if (originalData.ContainsKey(key) == false)
                {
                    originalData.Add(key, additionalData[key]);
                }
                else
                {
                    originalData[key] = additionalData[key];
                }
            }
        }        
    }
}
