using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Toast.GamebaseTools.Util.Launching.Internal
{
    public class LaunchingImplementation
    {
        private static readonly LaunchingImplementation instance = new LaunchingImplementation();
        public static LaunchingImplementation Instance
        {
            get { return instance; }
        }

        public void GetLaunchingInfo<T>(LaunchingConfigurations launchingConfigurations, Action<T> callback)
        {
            var request = UnityWebRequest.Get(
                string.Format("{0}/{1}/appkeys/{2}/configurations",
                launchingConfigurations.uri,
                launchingConfigurations.version,
                launchingConfigurations.appKey));
            request.method = UnityWebRequest.kHttpVerbGET;

            var helper = new UnityWebRequestHelper(request);

            EditorCoroutine.Start(
                helper.SendWebRequest(
                    (result) =>
                    {
                        var launchingInfo = JsonUtility.FromJson<T>(result.downloadHandler.text);
                        callback(launchingInfo);
                    }));

        }
    }
}
