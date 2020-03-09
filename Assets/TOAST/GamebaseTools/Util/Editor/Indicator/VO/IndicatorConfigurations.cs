using System.Collections.Generic;

namespace Toast.GamebaseTools.Util.Indicator
{
    public class IndicatorConfigurations
    {
        private class ProjectInfo
        {
            public string projectName;
            public string projectVersion;
            public string logVersion;
            public string body;

            public ProjectInfo(string projectName, string projectVersion, string logVersion, string body)
            {
                this.projectName = projectName;
                this.projectVersion = projectVersion;
                this.logVersion = logVersion;
                this.body = body;
            }
        }

        private string url;
        private ProjectInfo projectInfo;

        private IndicatorConfigurations()
        {
        }

        public IndicatorConfigurations(string url, string projectName, string projectVersion, string logVersion, string body)
        {
            this.url = url;
            projectInfo = new ProjectInfo(projectName, projectVersion, logVersion, body);
        }

        public Dictionary<string, string> GetProjectInfoAsDictionary()
        {
            return new Dictionary<string, string>()
            {
                {"projectName",     projectInfo.projectName},
                {"projectVersion",  projectInfo.projectVersion},
                {"logVersion",      projectInfo.logVersion},
                {"body",            projectInfo.body}
            };
        }

        public string GetUrl()
        {
            return url;
        }

        public string GetLogVersion()
        {
            return projectInfo.logVersion;
        }
    }
}
