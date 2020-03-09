using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Toast.GamebaseTools.Util.Indicator.Internal
{
    public class IndicatorSendManager
    {
        public const float LONG_INTERVAL = 1f;
        private Queue<SendData> sendDataQueue = new Queue<SendData>();

        public IndicatorSendManager()
        {
            EditorCoroutine.Start(SendLogPolling());
        }

        public void SendLog(SendData sendData)
        {
            sendDataQueue.Enqueue(sendData);
        }

        private IEnumerator SendLogPolling()
        {
            while (true)
            {
                if (sendDataQueue.Count > 0)
                {
                    SendData sendData = sendDataQueue.Dequeue();

                    if (sendData != null)
                    {
                        yield return EditorCoroutine.Start(
                                        SendHTTPPost(
                                        sendData.url,
                                        sendData.logVersion,
                                        sendData.GetData()));
                    }
                }
                else
                {
                    yield return new WaitForSecondsRealtime(LONG_INTERVAL);
                }

            }
        }

        private IEnumerator SendHTTPPost(string url, string logVersion, string jsonString)
        {
            var encoding = new UTF8Encoding().GetBytes(jsonString);

            var request = UnityWebRequest.Put(string.Format("{0}/{1}/log", url, logVersion), encoding);
            request.method = UnityWebRequest.kHttpVerbPOST;
            var helper = new UnityWebRequestHelper(request);

            yield return EditorCoroutine.Start(helper.SendWebRequest());
        }
    }    
}
