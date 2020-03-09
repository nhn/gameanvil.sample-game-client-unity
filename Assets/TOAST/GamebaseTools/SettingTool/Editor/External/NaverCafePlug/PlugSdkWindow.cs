using System;
using System.IO;
using Toast.GamebaseTools.Util;
using Toast.GamebaseTools.Util.Launching;
using UnityEditor;
using UnityEngine;

namespace Toast.GamebaseTools.SettingTool
{
    public class PLUGSDKWindow : EditorWindow
    {
        private static PLUGSDKWindow window;
        private static SDKSettingToolWindow settingToolWindow;

        private static string plugLocalPath;

        private const string LAUNCHING_URL = "https://api-lnc.cloud.toast.com/launching";
        private const string LAUNCHING_VERSION = "v3.0";
        private const string LAUNCHING_APP_KEY = "nJurfTWUalNPJCHf";

        private const string ANDROIDMANIFEST_FILE = "AndroidManifest";
        private const string FILE_EXTENSION_XML = "xml";
        private const string ANDROIDMANIFEST_PATH = "/Plugins/Android/";
        
        private static string plugSDKUrl;
        private static string plugAdapterUrl;

        private string downloadFile;
        private int downloadProgress = -1;

        private static string plugSdkPath;

        public static void ShowWindow(SDKSettingToolWindow settingToolWindow, System.Action<PLUGSDKWindow> callback)
        {
            plugLocalPath = Application.dataPath.Replace("Assets", "NaverCafePLUG/");

            LoadLaunching(
                (plugSDKUrl, plugAdapterUrl) =>
                {
                    PLUGSDKWindow.plugSDKUrl = plugSDKUrl;
                    PLUGSDKWindow.plugAdapterUrl = plugAdapterUrl;

                    PLUGSDKWindow.settingToolWindow = settingToolWindow;
                    window = GetWindowWithRect<PLUGSDKWindow>(new Rect(100, 100, 500, 230), true, SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_TITLE"));

                    callback(window);
                });
        }

        private void OnGUI()
        {
            GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
            guiStyle.fontSize = 18;

            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(12f);
                GUILayout.Label(SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_INFOMATION_TITLE"), guiStyle);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            DrawDesc();

            GUILayout.Space(20f);

            DrawButton();
        }

        private void OnDestroy()
        {
            FileManager.StopDownloadFile();
        }

        private void DrawButton()
        {
            GUILayout.Space(10f);

            GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
            guiStyle.fontSize = 18;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(12f);
                GUILayout.Label(SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_DOWNLOAD_PROGRESS_TITLE"), guiStyle);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(12f);

                GUILayout.BeginVertical();
                {
                    guiStyle.fontSize = 12;
                    if (string.IsNullOrEmpty(downloadFile) == false && downloadProgress != -1)
                    {
                        GUILayout.Label(
                            string.Format(
                                "{0} : {1}",
                                SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_DOWNLOAD_FILE_NAME"),
                                Path.GetFileName(downloadFile)),
                            guiStyle);

                        GUILayout.Label(
                            string.Format(
                                "{0} : {1}%",
                                SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_PROGRESS"),
                                downloadProgress),
                            guiStyle);
                    }
                    else
                    {
                        GUILayout.Label(
                           string.Format(
                               "{0} : {1}",
                               SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_DOWNLOAD_FILE_NAME"),
                               "NONE"),
                           guiStyle);

                        GUILayout.Label(
                            string.Format(
                                "{0} :",
                                SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_PROGRESS")),
                            guiStyle);
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(390f);
                GUILayout.BeginVertical();
                {
                    if (GUILayout.Button(SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_INFOMATION_BUTTON"), GUILayout.Width(100), GUILayout.Height(30)) == true)
                    {
                        if (EditorUtility.DisplayDialog(
                                SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_POPUP_TITLE"),
                                SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_POPUP_MESSAGE"),
                                SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_POPUP_BUTTON_OK"),
                                SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_POPUP_BUTTON_CANCEL"))
                            == true)
                        {
                            SetPLUGSDK();
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawDesc()
        {
            GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
            guiStyle.fontSize = 18;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(12f);
                guiStyle.fontSize = 12;
                GUILayout.Label(SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_MESSAGE"), guiStyle, GUILayout.Height(34));                
            }            
            GUILayout.EndHorizontal();
        }

        private static void LoadLaunching(System.Action<string, string> callback)
        {
            LaunchingManager.GetLaunchingInfo<PlugLaunchingInfo>(
                                new LaunchingConfigurations(
                                    LAUNCHING_URL,
                                    LAUNCHING_VERSION,
                                    LAUNCHING_APP_KEY
                                    ),
                                (data) =>
                                {
                                    plugSdkPath = data.launching.settingTool.naverCafePlug.installPath;
                                    callback(                                        
                                        data.launching.settingTool.naverCafePlug.sdkUrl, 
                                        data.launching.settingTool.naverCafePlug.extensionUrl);
                                });
        }

        private void SetPLUGSDK()
        {
            if (Directory.Exists(plugLocalPath) == true)
            {
                FileUtil.DeleteFileOrDirectory(plugLocalPath);                
            }
            
            Directory.CreateDirectory(plugLocalPath);

            DownloadSDK(
                (isSuccessDownloadSDK) =>
                {
                    if (isSuccessDownloadSDK == true)
                    {
                        DownloadAdapter(
                           (isSuccessDownloadAdapter) =>
                           {
                               if (isSuccessDownloadAdapter == true)
                               {
                                   ImportPackage(
                                   (isSuccessImportPackage) =>
                                   {
                                       settingToolWindow.CloseEditor();
                                   });
                               }
                           });
                    }
                });
        }

        private void DownloadSDK(System.Action<bool> callback)
        {
            downloadFile = plugSDKUrl;
            FileManager.DownloadFileToLocal(
                plugSDKUrl,
                string.Format("{0}{1}",plugLocalPath, "GamebaseNaverCafePLUG.unitypackage"),
                (stateCode, msg)=> 
                {
                    downloadFile = null;
                    downloadProgress = -1;
                    window.Repaint();
                    if (stateCode == FileManager.StateCode.SUCCESS)
                    {
                        callback(true);
                    }
                    else
                    {
                        callback(false);
                    }
                },
                (progress) => 
                {
                    downloadProgress = (int)(progress * 100);
                    window.Repaint();
                });
        }

        private void DownloadAdapter(System.Action<bool> callback)
        {
            downloadFile = plugAdapterUrl;
            FileManager.DownloadFileToLocal(
                plugAdapterUrl,
                string.Format("{0}{1}", plugLocalPath, "GamebaseNaverCafePLUGAdapter.unitypackage"),
                (stateCode, msg) =>
                {
                    downloadFile = null;
                    downloadProgress = -1;
                    window.Repaint();
                    if (stateCode == FileManager.StateCode.SUCCESS)
                    {
                        callback(true);
                    }
                    else
                    {
                        callback(false);
                    }
                },
                (progress) =>
                {
                    downloadProgress = (int)(progress * 100);
                    window.Repaint();
                });
        }

        private void ImportPackage(System.Action<bool> callback)
        {
            string plugPath = string.Format("{0}{1}", Application.dataPath, plugSdkPath);

            if (Directory.Exists(plugPath) == true)
            {
                FileUtil.DeleteFileOrDirectory(plugPath);
            }

            string gamebaseExtentionPath = string.Format("{0}{1}", Application.dataPath, "/Gamebase/Extension/NaverCafe");

            if (Directory.Exists(gamebaseExtentionPath) == true)
            {
                FileUtil.DeleteFileOrDirectory(gamebaseExtentionPath);
            }

            AssetDatabase.Refresh();

            string androidManifest = string.Format(
                "{0}{1}{2}.{3}",
                Application.dataPath,
                ANDROIDMANIFEST_PATH,
                ANDROIDMANIFEST_FILE,
                FILE_EXTENSION_XML);

            if (File.Exists(androidManifest) == true)
            {
                string androidManifestBackUp = string.Format(
                "{0}{1}{2}_{3}.{4}",
                Application.dataPath,
                ANDROIDMANIFEST_PATH,
                ANDROIDMANIFEST_FILE,
                string.Format("{0:yyyy_MM_dd_HH_mm_ss}", DateTime.Now),
                FILE_EXTENSION_XML);

                FileUtil.CopyFileOrDirectory(androidManifest, androidManifestBackUp);

                if (EditorUtility.DisplayDialog(
                                SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_ANDROIDMANIFEST_BACKUP_POPUP_TITLE"),
                                string.Format(
                                    "{0}\n\n{1}", 
                                    SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_ANDROIDMANIFEST_BACKUP_POPUP_MESSAGE"),
                                    androidManifestBackUp),
                                SDKSettingToolWindow.GetMultilanguageString("PLUG_SDK_SETTING_POPUP_BUTTON_OK"))
                            == true)
                {
                    
                }
            }

            AssetDatabase.ImportPackage(
                string.Format(
                    "{0}{1}", 
                    plugLocalPath, 
                    "GamebaseNaverCafePLUG.unitypackage"), 
                false);

            AssetDatabase.ImportPackage(
                string.Format(
                    "{0}{1}", 
                    plugLocalPath, 
                    "GamebaseNaverCafePLUGAdapter.unitypackage"), 
                false);

            AssetDatabase.Refresh();

            callback(true);
        }
    }
}
