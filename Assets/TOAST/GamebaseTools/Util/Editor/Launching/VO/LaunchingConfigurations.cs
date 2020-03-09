namespace Toast.GamebaseTools.Util.Launching
{
    public class LaunchingConfigurations
    {
        public string uri;
        public string version;
        public string appKey;

        public LaunchingConfigurations(string uri, string version, string appKey)
        {
            this.uri = uri;
            this.version = version;
            this.appKey = appKey;
        }
    }
}
