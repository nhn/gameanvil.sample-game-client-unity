[System.Serializable]
public class LaunchingInfo
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
        public class ZoneInfo
        {
            [System.Serializable]
            public class Zone
            {
                public string logVersion;
                public string appKey;
                public string url;
                public string activation;
            }

            public Zone alpha;
            public Zone real;
        }

        public ZoneInfo settingTool;
    }

    public Header header;
    public Launching launching;
}
