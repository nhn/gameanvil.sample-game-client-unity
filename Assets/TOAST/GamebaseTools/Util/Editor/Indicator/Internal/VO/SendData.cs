using System.Collections.Generic;
using System.Text;

namespace Toast.GamebaseTools.Util.Indicator.Internal
{
    public class SendData
    {
        public string url;
        public string logVersion;
        private Dictionary<string, string> data;

        private SendData()
        {
        }

        public SendData(string url, string logVersion, Dictionary<string, string> data)
        {
            this.url = url;
            this.logVersion = logVersion;
            this.data = data;
        }

        public string GetData()
        {
            return DictionaryToJsonString(data);
        }

        private string DictionaryToJsonString(Dictionary<string, string> data)
        {
            if(data == null || data.Count == 0)
            {
                return string.Empty;
            }

            List<string> dataList = new List<string>();
                        
            foreach(var key in data.Keys)
            {
                dataList.Add(string.Format("\"{0}\":\"{1}\"", key, data[key]));
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i < dataList.Count; i++)
            {
                sb.Append(dataList[i]);
                if (i < dataList.Count - 1)
                {
                    sb.Append(",");
                }                
            }
            sb.Append("}");

            string jsonString = sb.ToString();
            return jsonString;
        }
    }
}
