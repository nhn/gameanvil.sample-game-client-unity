using System;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Toast.GamebaseTools.Util
{
    public class FileManager
    {
        public enum StateCode
        {
            SUCCESS,
            FILE_NOT_FOUND_ERROR,
            WEB_REQUEST_ERROR,
            UNKNOWN_ERROR,
        }

        private static List<EditorCoroutine> coroutineList = new List<EditorCoroutine>();

        public static void DownloadFileToLocal(string remoteFilename, string localFilename, Action<StateCode, string> callback, Action<float> callbackProgress = null)
        {
            EditorCoroutine downloadFileCoroutine = null;

            downloadFileCoroutine = EditorCoroutine.Start(
                DownloadFile(
                    remoteFilename,
                    (stateCode, message, data) =>
                    {
                        if(StateCode.SUCCESS == stateCode)
                        {
                            try
                            {
                                File.WriteAllBytes(localFilename, data);
                                callback(StateCode.SUCCESS, null);
                            }                            
                            catch (Exception e)
                            {
                                callback(StateCode.UNKNOWN_ERROR, e.Message);
                            }
                        }
                        else
                        {
                            callback(stateCode, message);
                        }

                        if (downloadFileCoroutine != null)
                        {
                            coroutineList.Remove(downloadFileCoroutine);
                            downloadFileCoroutine = null;
                        }
                    },
                    callbackProgress
                    )
                );
            coroutineList.Add(downloadFileCoroutine);
        }

        public static void DownloadFileToString(string remoteFilename, Action<StateCode, string, string> callback, Action<float> callbackProgress = null)
        {
            EditorCoroutine downloadFileCoroutine = null;
            downloadFileCoroutine = EditorCoroutine.Start(
                DownloadFile(
                    remoteFilename,
                    (stateCode, message, data) =>
                    {
                        string encoding = null;
                        if(data != null)
                        {
                            encoding = System.Text.Encoding.Default.GetString(data);
                        }

                        callback(stateCode, message, encoding);

                        if (downloadFileCoroutine != null)
                        {
                            coroutineList.Remove(downloadFileCoroutine);
                            downloadFileCoroutine = null;
                        }
                    },
                    callbackProgress
                    )
                );
            coroutineList.Add(downloadFileCoroutine);
        }

        public static void StopDownloadFile()
        {
            foreach (EditorCoroutine coroutine in coroutineList)
            {
                if (coroutine != null)
                {
                    coroutine.Stop();
                }
            }

            coroutineList.Clear();
        }

        private static IEnumerator DownloadFile(string remoteFilename, Action<StateCode, string, byte[]> callback, Action<float> callbackProgress = null)
        {
            UnityWebRequest www = UnityWebRequest.Get(remoteFilename);
#if UNITY_2017_2_OR_NEWER
            yield return www.SendWebRequest();
#else
            yield return www.Send();
#endif

            while (true)
            {
                if (true == www.isDone)
                {
                    if (200 == www.responseCode)
                    {
#if UNITY_2017_2_OR_NEWER
                        if (true == www.isNetworkError)
#else
                        if (true == www.isError)
#endif
                        {
                            callback(StateCode.WEB_REQUEST_ERROR, www.error, null);
                            yield break;
                        }
                        else
                        {
                            try
                            {
                                callback(StateCode.SUCCESS, null, www.downloadHandler.data);
                            }
                            catch (Exception e)
                            {
                                callback(StateCode.UNKNOWN_ERROR, e.Message, null);
                            }
                            yield break;
                        }
                    }
                    else
                    {
                        switch (www.responseCode)
                        {
                            case 404:
                                {
                                    callback(StateCode.FILE_NOT_FOUND_ERROR, remoteFilename, null);
                                    break;
                                }
                            default:
                                {
#if UNITY_2017_2_OR_NEWER
                                    if (true == www.isNetworkError)
#else
                                    if (true == www.isError)
#endif
                                    {
                                        callback(StateCode.WEB_REQUEST_ERROR, www.error, null);
                                    }
                                    else
                                    {
                                        callback(StateCode.UNKNOWN_ERROR, www.responseCode.ToString(), null);
                                    }

                                    break;
                                }
                        }
                        yield break;
                    }
                }
                else
                {
                    if (null != callbackProgress)
                    {
                        callbackProgress(www.downloadProgress);
                    }
                }

                yield return null;
            }
        }
    }
}