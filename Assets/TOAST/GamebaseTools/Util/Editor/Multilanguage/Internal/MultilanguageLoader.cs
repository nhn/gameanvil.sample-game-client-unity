using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Toast.GamebaseTools.Util.Multilanguage.Internal
{
    internal class MultilanguageLoader
    {
        private const string STREAMING_ASSETS_DIRECTORY_NAME = "/StreamingAssets/";
        private const string RESOURCE_DIRECTORY_NAME = "/Resources/";

        public enum LoadType
        {
            LOCAL_FILE,
            DOWNLOAD_FILE,
            STREAMING_ASSET,
            RESOURCE_ASSET
        }

        private MonoBehaviour monoObject;

        public void Load(string filepath, Action<MultilanguageResultCode, MulitlanguageXml, string> callback)
        {
            LoadType loadType = LoadType.LOCAL_FILE;
            if (IsWebPath(filepath) == true)
            {
                loadType = LoadType.DOWNLOAD_FILE;
            }
            else
            {
                if (filepath.StartsWith("/", StringComparison.Ordinal) == false)
                {
                    filepath = string.Format("/{0}", filepath);
                }

                if (filepath.StartsWith(STREAMING_ASSETS_DIRECTORY_NAME, StringComparison.Ordinal) == true)
                {
                    loadType = LoadType.STREAMING_ASSET;
                    filepath = filepath.Replace(STREAMING_ASSETS_DIRECTORY_NAME, "");
                }
                else if (filepath.Contains(RESOURCE_DIRECTORY_NAME) == true)
                {
                    loadType = LoadType.RESOURCE_ASSET;

                    int startIndex = filepath.LastIndexOf(RESOURCE_DIRECTORY_NAME, StringComparison.Ordinal) + RESOURCE_DIRECTORY_NAME.Length - 1;
                    filepath = filepath.Substring(startIndex);

                    int commaLastIndex = filepath.LastIndexOf(".", StringComparison.Ordinal);
                    if (commaLastIndex > -1)
                    {
                        filepath = filepath.Substring(0, commaLastIndex);
                    }
                }

                filepath = filepath.TrimStart('/');
            }

            switch (loadType)
            {
                case LoadType.LOCAL_FILE:
                    {
                        string localPath = Path.Combine(Application.dataPath, filepath);
                        LoadLocalFile(localPath, callback);
                        break;
                    }
                case LoadType.DOWNLOAD_FILE:
                    {
                        LoadDownloadFile(filepath, false, callback);
                        break;
                    }
                case LoadType.STREAMING_ASSET:
                    {
                        string streamingAssetsPath = string.Empty;

#if UNITY_ANDROID && !UNITY_EDITOR
                        streamingAssetsPath = string.Format("jar:file://{0}!/assets/{1}", Application.dataPath, filepath);
#else
                        streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, filepath);
#endif

                        if (IsWebPath(streamingAssetsPath) == true)
                        {
                            LoadDownloadFile(streamingAssetsPath, true, callback);
                        }
                        else
                        {
                            LoadLocalFile(streamingAssetsPath, callback);
                        }

                        break;
                    }
                case LoadType.RESOURCE_ASSET:
                    {
                        LoadResourceAsset(filepath, callback);
                        break;
                    }
            }
        }

#region Load Process

        private void LoadLocalFile(string localPath, Action<MultilanguageResultCode, MulitlanguageXml, string> callback)
        {
            XMLManager.LoadXMLFromFile<MulitlanguageXml>(
                localPath,
                (xmlResultCode, xmlData, xmlResultMessage) =>
                {
                    callback(GetResultCode(xmlResultCode), xmlData, xmlResultMessage);
                });
        }

        private void LoadResourceAsset(string assetName, Action<MultilanguageResultCode, MulitlanguageXml, string> callback)
        {
            TextAsset asset = Resources.Load<TextAsset>(assetName);
            if (asset == null)
            {
                callback(MultilanguageResultCode.FILE_NOT_FOUND, null, null);
                return;
            }

            XMLManager.LoadXMLFromText<MulitlanguageXml>(
                asset.text,
                (xmlResultCode, xmlData, xmlResultMessage) =>
                {
                    callback(GetResultCode(xmlResultCode), xmlData, xmlResultMessage);
                });
        }

        private void LoadDownloadFile(string url, bool isStreamingAsset, Action<MultilanguageResultCode, MulitlanguageXml, string> callback)
        {
            Action<MultilanguageResultCode, string, string> requestCallback =
                (result, loadText, resultMessage) =>
                {
                    if (result != MultilanguageResultCode.SUCCESS)
                    {
                        callback(result, null, resultMessage);
                        return;
                    }

                    if (string.IsNullOrEmpty(loadText) == true)
                    {
                        callback(MultilanguageResultCode.FILE_PARSING_ERROR, null, null);
                        return;
                    }

                    XMLManager.LoadXMLFromText<MulitlanguageXml>(
                        loadText,
                        (xmlResultCode, xmlData, xmlResultMessage) =>
                        {
                            callback(GetResultCode(xmlResultCode), xmlData, xmlResultMessage);
                        });
                };

            IEnumerator loadEnumerator = null;
#if !UNITY_2017_1_OR_NEWER && !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            if (isStreamingAsset == true)
            {
                loadEnumerator = LoadStreamingAssetFile(url, requestCallback);
            }
            else
#endif
            {
                loadEnumerator = DownloadFile(url, requestCallback);
            }

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying == false)
            {
                EditorCoroutine.Start(loadEnumerator);
            }
            else
#endif
            {
                if (monoObject == null)
                {
                    monoObject = GameObjectContainer.GetGameObject(MultilanguageManager.SERVICE_NAME).GetComponent<MonoBehaviour>();
                }

                monoObject.StartCoroutine(loadEnumerator);
            }
        }

#if !UNITY_2017_1_OR_NEWER && !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        private IEnumerator LoadStreamingAssetFile(string url, Action<MultilanguageResultCode, string, string> callback)
        {
            WWW www = new WWW(url);
            yield return www;
            while (www.isDone == false)
            {
                yield return null;
            }

            if (string.IsNullOrEmpty(www.error) == false)
            {
                callback(MultilanguageResultCode.FILE_LOAD_FAILED, null, www.error);
                yield break;
            }

            callback(MultilanguageResultCode.SUCCESS, www.text, null);
        }
#endif

        private IEnumerator DownloadFile(string url, Action<MultilanguageResultCode, string, string> callback)
        {
            string dataText = string.Empty;
            MultilanguageResultCode resultCode = MultilanguageResultCode.SUCCESS;
            string resultMessage = null;

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
#if UNITY_2017_2_OR_NEWER
                yield return request.SendWebRequest();
#else
                yield return request.Send();
#endif

                while (request.isDone == false)
                {
                    yield return null;
                }

#if UNITY_2017_1_OR_NEWER
                if (request.isNetworkError == true)
#else
                if (request.isError == true)
#endif
                {                
                    resultCode = MultilanguageResultCode.FILE_LOAD_FAILED;
                    resultMessage = request.error + " " + request.responseCode + " " + url;
                    callback(resultCode, dataText, resultMessage);
                    yield break;
                }

                switch (request.responseCode)
                {
                    case 200:
                        {
                            dataText = request.downloadHandler.text;
                            break;
                        }
                    case 404:
                        {
                            resultCode = MultilanguageResultCode.FILE_NOT_FOUND;
                            break;
                        }
                    default:
                        {
                            resultCode = MultilanguageResultCode.FILE_LOAD_FAILED;
                            resultMessage = string.Format("Response Code: {0}", request.responseCode);
                            break;
                        }
                }

                callback(resultCode, dataText, resultMessage);
            }
        }
#endregion

        private bool IsWebPath(string path)
        {
            return path.Contains("://") == true || path.Contains(":///") == true;
        }

        protected MultilanguageResultCode GetResultCode(XMLManager.ResponseCode xmlResponseCode)
        {
            MultilanguageResultCode resultCode;

            switch (xmlResponseCode)
            {
                case XMLManager.ResponseCode.SUCCESS:
                    {
                        resultCode = MultilanguageResultCode.SUCCESS;
                        break;
                    }
                case XMLManager.ResponseCode.FILE_NOT_FOUND_ERROR:
                    {
                        resultCode = MultilanguageResultCode.FILE_NOT_FOUND;
                        break;
                    }
                default:
                    {
                        resultCode = MultilanguageResultCode.FILE_PARSING_ERROR;
                        break;
                    }
            }

            return resultCode;
        }
    }
}
