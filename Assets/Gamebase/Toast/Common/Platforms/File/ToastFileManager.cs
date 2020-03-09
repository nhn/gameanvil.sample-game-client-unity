#if UNITY_STANDALONE || UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Toast.Internal
{
    public static class ToastFileManager
    {
        private static string EncryptProjectKey(string projectKey)
        {
            string encryptedKey = ToastAES.AESEncrypt256(projectKey, ToastApplicationInfo.GetEncrypKey());

#if UNITY_2017_3_OR_NEWER
            return UnityEngine.Networking.UnityWebRequest.EscapeURL(encryptedKey);
#else
            return WWW.EscapeURL(encryptedKey);
#endif
        }

        private static string EncryptBulkLog(string planeTxt)
        {
            return ToastAES.AESEncrypt256(planeTxt, ToastApplicationInfo.GetEncrypKey());
        }

        private static string DecryptBulkLog(string encryptedTxt)
        {
            return ToastAES.AESDecrypt256(encryptedTxt, ToastApplicationInfo.GetEncrypKey());
        }

        private static void FileDelete(string fileName)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.Delete(filePath);
        }

        public static void FileDelete(string projectKey, long createTime, string transactionId)
        {
            string encryptProjectKey = EncryptProjectKey(projectKey);
            string fileName = encryptProjectKey + "/" + createTime.ToString() + "_" + transactionId;
            FileDelete(fileName);
        }

        private static void FileAppendText(string fileName, string text)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            fileStream.Close();
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.WriteLine(text);
            streamWriter.Flush();
            streamWriter.Close();            
        }

        private static void FileSave(string fileName, string text)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fileStream.Close();
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.WriteLine(EncryptBulkLog(text));
            streamWriter.Flush();
            streamWriter.Close();
        }

        public static void FileSave(string projectKey, long createTime, string transactionId, string text)
        {
            string encryptProjectKey = EncryptProjectKey(projectKey);
            string folderPath = Path.Combine(Application.persistentDataPath, encryptProjectKey);
            Directory.CreateDirectory(folderPath);

            string fileName = encryptProjectKey + "/" + createTime.ToString() + "_" + transactionId;
            FileSave(fileName, text);
        }

        public static void SettingFileSave(string projectKey, string text)
        {
            string encryptProjectKey = EncryptProjectKey(projectKey);
            string folderPath = Path.Combine(Application.persistentDataPath, encryptProjectKey + "_settings");
            Directory.CreateDirectory(folderPath);

            string fileName = encryptProjectKey + "_settings/" + "settings";            
            FileSave(fileName, text);
        }

        public static string SettingFileLoad(string projectKey)
        {
            string encryptProjectKey = EncryptProjectKey(projectKey);
            string folderPath = Path.Combine(Application.persistentDataPath, encryptProjectKey + "_settings");
            Directory.CreateDirectory(folderPath);

            string fileName = encryptProjectKey + "_settings/" + "settings";
            if (FileExist(fileName))
            {
                return FileLoad(fileName);
            }
            else
            {
                return "";
            }
        }

        public static string GetFirstFile(string projectKey)
        {
            string encryptProjectKey = EncryptProjectKey(projectKey);
            if (!DirectoryExist(encryptProjectKey))
            {
                return "";
            }

            string folderPath = Path.Combine(Application.persistentDataPath, encryptProjectKey);
            string[] files = Directory.GetFiles(folderPath);
            if (files.Length > 0)
            {
                return files[0];
            }
            else
            {
                return "";
            }
        }

        public static int GetProjectFileCount(string projectKey)
        {
            string encryptProjectKey = EncryptProjectKey(projectKey);
            if (!DirectoryExist(encryptProjectKey))
            {
                return 0;
            }
            
            string folderPath = Path.Combine(Application.persistentDataPath, encryptProjectKey);
            string[] files = Directory.GetFiles(folderPath);

            return files.Length;
        }

        private static string FileLoad(string fileName)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(filePath);

            string text = "";
            while (true)
            {
                string readText = streamReader.ReadLine();
                if (readText == null)
                {
                    break;
                }

                text += readText;
            }

            streamReader.Close();
            fileStream.Close();

            return DecryptBulkLog(text);
        }

        public static string FileLoad(string projectKey, long createTime, string transactionId)
        {
            string encryptProjectKey = EncryptProjectKey(projectKey);
            if (!DirectoryExist(encryptProjectKey))
            {
                return "";
            }            
            
            string fileName = encryptProjectKey + "/" + createTime.ToString() + "_" + transactionId;
            return FileLoad(fileName);
        }

        public static bool FileCheck(string projectKey, long createTime, string transactionId)
        {
            string encryptProjectKey = EncryptProjectKey(projectKey);
            string folderPath = Path.Combine(Application.persistentDataPath, encryptProjectKey);
            if (!Directory.Exists(folderPath))
            {
                return false;
            }
            
            string fileName = encryptProjectKey + "/" + createTime.ToString() + "_" + transactionId;
            return FileExist(fileName);
        }

        private static bool FileExist(string fileName)
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            return File.Exists(filePath);            
        }

        private static bool DirectoryExist(string dir)
        {           
            string folderPath = Path.Combine(Application.persistentDataPath, dir);
            return Directory.Exists(folderPath);
        }

    }
}

#endif