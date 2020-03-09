[System.Serializable]
public class PlugLaunchingInfo
{
    [System.Serializable]
    public class Header
    {
        public bool isSuccessful;
        public int resultCode;
        public string resultMessage;
    }

    [System.Serializable]
    public class Launching
    {
        [System.Serializable]
        public class SettingToolInfo
        {
            [System.Serializable]
            public class NaverCafePlugInfo
            {
                public string sdkUrl;
                public string installPath;
                public string extensionUrl;
            }

            public NaverCafePlugInfo naverCafePlug;
        }

        public SettingToolInfo settingTool;
    }

    public Header header;
    public Launching launching;
}
