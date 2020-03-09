﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Toast.Internal;

#if UNITY_2017_2_OR_NEWER
using UnityEngine.Networking;
#endif  // UNITY_2017_2_OR_NEWER

namespace Toast.Core
{

    public class LogTransfer : MonoBehaviour
    {
        private const int MAX_SEND_SIZE = 2097152;
        private const int MAX_COROUTINE_SIZE = 2048;
        private const int MAX_FILE_SIZE = 2048;

        private bool _isStartSender = false;
        private int _couroutineCount = 0;

        private static LogTransfer _instance;
        public static LogTransfer Instance
        {
            get
            {
                _instance = FindObjectOfType(typeof(LogTransfer)) as LogTransfer;
                if (!_instance)
                {
                    var container = GameObject.Find(Constants.LogTransferObjectName);
                    if (container == null)
                    {
                        container = new GameObject(Constants.LogTransferObjectName);
                    }

                    _instance = container.AddComponent<LogTransfer>();
                    DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }

        public void StartSender()
        {
            _isStartSender = true;
        }

        public void StopSender()
        {
            _isStartSender = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!_isStartSender)
            {
                return;
            }

            LogSendQueue.Instance.Enqueue();
            if (LogSendQueue.Instance.Count > 0 && _couroutineCount < MAX_COROUTINE_SIZE)
            {
                LogBulkData bulkLog = LogSendQueue.Instance.Dequeue();
                StartCoroutine(SendReport(bulkLog.LogContents, bulkLog.CreateTime, bulkLog.TransactionId));
            }
        }

        IEnumerator SendReport(string logContents, long createTime, string transactionId)
        {
            _couroutineCount++;

            string url = ToastInstanceLoggerCommonLogic.CollectorUrl;

            string errorString = "";
            string jsonString = "";

            float timeout = 5.0f;
            bool isTimeout = false;

#if UNITY_2017_2_OR_NEWER
            var downloadHandler = new DownloadHandlerBuffer();
            var uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(logContents));

            using (var request = new UnityWebRequest(url,
                UnityWebRequest.kHttpVerbPOST,
                downloadHandler, uploadHandler))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = System.Convert.ToInt32(timeout);

                yield return request.SendWebRequest();

                errorString = request.error;
                if (request.isNetworkError || request.isHttpError)
                {
                    isTimeout = true;
                }
                else
                {
                    jsonString = request.downloadHandler.text;
                }
            }
#else
            float timer = 0f;

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Content-Type", "application/json");
            using (WWW www = new WWW(url, System.Text.Encoding.UTF8.GetBytes(logContents), header))
            {
               do
                {
                    if (timer > timeout)
                    {
                        isTimeout = true;
                        break;
                    }
                    timer += Time.deltaTime;

                    yield return null;
                }
                while (!www.isDone);

                if (isTimeout)
                {
                    www.Dispose();
                }
                else
                {
                    errorString = www.error;
                    jsonString = www.text;
                }
            }
#endif  // UNITY_2017_2_OR_NEWER

#if UNITY_STANDALONE || UNITY_EDITOR
            if (isTimeout == false && string.IsNullOrEmpty(errorString)) // success
            {
                if (ToastFileManager.FileCheck(ToastInstanceLoggerCommonLogic.AppKey, createTime, transactionId))
                {
                    ToastFileManager.FileDelete(ToastInstanceLoggerCommonLogic.AppKey, createTime, transactionId);
                }

                LogSendQueue.Instance.EnqueueInFile();
            }
            else
            {
                if (ToastFileManager.GetProjectFileCount(ToastInstanceLoggerCommonLogic.AppKey) < MAX_FILE_SIZE)
                {
                    ToastFileManager.FileSave(ToastInstanceLoggerCommonLogic.AppKey, createTime, transactionId, logContents);
                }
            }
#endif

            _couroutineCount--;

            yield return null;
        }
    }

}