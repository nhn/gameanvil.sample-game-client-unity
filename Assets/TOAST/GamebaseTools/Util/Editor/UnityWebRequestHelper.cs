using System;
using System.Collections;
using UnityEngine.Networking;

namespace Toast.GamebaseTools.Util
{
    public class UnityWebRequestHelper
    {
        private const int TIMEOUT = 10;

        private UnityWebRequest request;

        public UnityWebRequestHelper(UnityWebRequest request)
        {
            this.request = request;
        }

        public IEnumerator SendWebRequest(Action<UnityWebRequest> callback = null)
        {
            request.timeout = TIMEOUT;
            request.SetRequestHeader("Content-Type", "application/json");

#if UNITY_2017_2_OR_NEWER
            yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif
            while (request.isDone == false)
            {
                yield return null;
            }
            
            if (callback != null)
            {
                callback(request);
            }
        }

        private bool IsSuccess()
        {
            if (request.responseCode != 200)
            {
                return false;
            }

#if UNITY_2017_1_OR_NEWER
            if (request.isNetworkError == true)
#else
            if (request.isError == true)
#endif
            {
                return false;
            }

            if (string.IsNullOrEmpty(request.downloadHandler.text) == true)
            {
                return false;
            }

            return true;
        }
    }
}