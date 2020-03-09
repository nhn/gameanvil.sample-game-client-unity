using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Toast.GamebaseTools.Util;
using Toast.GamebaseTools.Util.Ad;
using Toast.GamebaseTools.Util.Launching;
using Toast.GamebaseTools.Util.Indicator;
using Toast.GamebaseTools.Util.Multilanguage;

namespace Toast.GamebaseTools.SettingTool
{
    [InitializeOnLoad]
    public class SDKSettingToolWindow : EditorWindow
    {
        public const string VERSION = "v1.5.0";

        private enum PlatformType
        {
            UNITY,
            ANDROID,
            IOS
        }

        private enum SDK_STATE
        {
            NONE,
            NOT_FILE,
            DOWNLOAD,
            EXTRACT,
        }

        private enum UPDATE_STATE
        {
            NONE,
            MANDATORY,
            OPTIONAL,
            ERROR
        }

        private static class GameConstants
        {
            public const string LANGUAGE_KOREAN = "ko";
            public const string LANGUAGE_ENGLISH = "en";
        }

        public const string SERVICE_NAME = "GamebaseSettingTool";
        private const string MULTILANGUAGE_FILE_PATH = "/TOAST/GamebaseTools/SettingTool/Editor/Language.xml";

        private static string downloadPath;
#if GAMEBASE_SETTINGTOOL_ALPHA_ZONE
        private const string DOWNLOAD_URL = "http://127.0.0.1/toastcloud/sdk_download/gamebase/";
#else
        private const string DOWNLOAD_URL = "http://static.toastoven.net/toastcloud/sdk_download/gamebase/";
#endif
        private const string XML_FILE_NAME = "GamebaseSDK.xml";
        private const string VERSION_CHECK_XML_NAME = "SupportedSettingToolVersion.xml";
        public const string TOAST_PATH = "https://docs.toast.com/ko/Download/#game-gamebase";
        public const string ASSET_STORE_URL = "http://u3d.as/12WT";
        private const string IOS_FILE_NAME = "GamebaseSDK-iOS.zip";
        private const string ANDROID_FILE_NAME = "GamebaseSDK-Android.zip";
        private const string UNITY_FILE_NAME = "GamebaseSDK-Unity.zip";
        private const string LOCAL_SETTING_INFOMATION_XML = "/TOAST/GamebaseTools/SettingTool/Editor/LocalSettingInfomation.xml";

        private const string LANGUAGE_SAVE_KEY = "GAMEBASE_SETTING_TOOL_LANGUAGE_SAVE_KEY";

        private const int OFFSET = 244;
        private const int WINDOW_WIDTH = 191;
        private const int WINDOW_HEIGHT = 64;
        private const int SPACE_TOP = 260;
        private const int SPACE_WIDTH = 25;
        private const int SPACE_HEIGHT = 15;
        private const int OFFSET_SDK_SETTING = 108;
#if GAMEBASE_SETTINGTOOL_ALPHA_ZONE
        private const string ADVERTISEMENT_URL = "http://127.0.0.1/gamebase/GamebaseSettingTool/Ad/";
#else
        private const string ADVERTISEMENT_URL = "http://static.toastoven.net/toastcloud/sdk_download/gamebase/GamebaseSettingTool/Ad/";
#endif
        private const string ADVERTISEMENT_DOWNLOAD_PATH = "TOAST/GamebaseTools/SettingTool/Ad/";
        private const string ADVERTISEMENT_XML_NAME = "Advertisement.xml";

        private const string LAUNCHING_URL = "https://api-lnc.cloud.toast.com/launching";
        private const string LAUNCHING_VERSION = "v3.0";
        private const string LAUNCHING_APP_KEY = "nJurfTWUalNPJCHf";
        
        private static SDKSettingToolWindow window;
        private static FileStream fileStream = null;

        private static PLUGSDKWindow plugWindow;

        private Rect scrollBGRect = new Rect(10, 311, 1002, 447);
        private Rect scrollRect = new Rect(11, 311, 1000, 445);

        private static SDK_STATE sdkState = SDK_STATE.NONE;

        private static string downloadProgress = string.Empty;
        private static string extractProgress = string.Empty;

        private static int tapIndex = 0;

        private static string downloadFileName = string.Empty;
        private static string extractFileName = string.Empty;

        private static List<string> settingsPakageFiles = new List<string>();
        private static Dictionary<string, string> settingsLibsFiles = new Dictionary<string, string>();
        private static List<string> settingsDirectories = new List<string>();
        private static List<string> settingsExternalsDirectories = new List<string>();

        private static List<string> platforms = new List<string>();

        private static SettingToolVO.GamebaseSDKInfo gamebaseSDKInfo;
        private static SettingToolVO.GamebaseSDKInfo gamebaseLocalSettings;
        private static SettingToolVO.GamebaseSDKInfo drawSDKInfo;

        private EditorCoroutine extractCoroutine = null;

        private static Vector2 scrollPos;

        private static int scrollMaxCountUnity;
        private static int scrollMaxCountIOS;
        private static int scrollMaxCountAndroid;

        private object lockObject = new object();
        private static UPDATE_STATE updateState = UPDATE_STATE.NONE;

        private static int selectedLanguageIndex;
        private static string[] languages;

        private static bool isinitialize = false;

        public static void ShowWindow()
        {
            MultilanguageManager.Load(
                SERVICE_NAME,
                MULTILANGUAGE_FILE_PATH,
                (result, resultMessage) =>
                {
                    if (result == MultilanguageResultCode.SUCCESS || result == MultilanguageResultCode.ALREADY_LOADED)
                    {
                        languages = MultilanguageManager.GetSupportLanguages(SERVICE_NAME, true);

                        CheckVersion((state) =>
                        {
                            if (UPDATE_STATE.ERROR == state)
                            {
                                return;
                            }

                            updateState = state;

                            if (null != window)
                            {
                                window.Close();
                                window = null;
                            }

                            sdkState = SDK_STATE.NONE;

                            LoadSDKSettingFromLocal();

                            window = GetWindowWithRect<SDKSettingToolWindow>(new Rect(100, 100, 1024, 778), true, "Gamebase Settings");

                            string[] languages = MultilanguageManager.GetSupportLanguages(SERVICE_NAME, false);

                            LaunchingManager.GetLaunchingInfo<LaunchingInfo>(
                                new LaunchingConfigurations(
                                    LAUNCHING_URL,
                                    LAUNCHING_VERSION,
                                    LAUNCHING_APP_KEY
                                    ),
                                (data) =>
                                {
                                    if (data != null)
                                    {
#if GAMEBASE_SETTINGTOOL_ALPHA_ZONE
                                        LaunchingInfo.Launching.ZoneInfo.Zone zone = data.launching.settingTool.alpha;
#else
                                        LaunchingInfo.Launching.ZoneInfo.Zone zone = data.launching.settingTool.real;
#endif

                                        IndicatorManager.Initialize(
                                            new IndicatorConfigurations(
                                                zone.url,
                                                zone.appKey,
                                                VERSION,
                                                zone.logVersion,
                                                "SettingTool"
                                                    ));

                                    }

                                    Advertisement.Initialize(
                                        window,
                                        new Rect(754, 14, 256, 144),
                                        new AdvertisementConfigurations(
                                            ADVERTISEMENT_URL,
                                            string.Format(
                                                "{0}/{1}",
                                                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                ADVERTISEMENT_DOWNLOAD_PATH
                                                ),
                                            ADVERTISEMENT_XML_NAME,
                                            languages
                                            ),
                                        LanguageCode);

                                    Advertisement.SetSelectAdvertisementInfoCallback(
                                        (adName, link) =>
                                        {
                                            IndicatorManager.Send(
                                                new Dictionary<string, string>()
                                                {
                                                    { "ACTION",             "Ad" },
                                                    { "ACTION_DETAIL_1",    adName },
                                                    { "ACTION_DETAIL_2",    link },
                                                });
                                        });

                                    string selectLanguage = LanguageCode;

                                    for (int i = 0 ; i< languages.Length ; i++)
                                    {
                                        string language = languages[i];
                                        if (language.Equals(selectLanguage) == true)
                                        {
                                            selectedLanguageIndex = i;
                                            break;
                                        }
                                    }

                                    MultilanguageManager.SelectLanguageByNativeName(SERVICE_NAME, selectLanguage,
                                        (resultSelectLanguage, resultMessageSelectLanguage) =>
                                        {
                                            if (resultSelectLanguage == MultilanguageResultCode.SUCCESS)
                                            {
                                                Advertisement.SetLanguageCode(MultilanguageManager.GetSupportLanguages(SERVICE_NAME, false)[selectedLanguageIndex]);

                                                return;
                                            }
                                        });
                                    isinitialize = true;
                                });
                        });
                    }
                });

        }

        private void OnDestroy()
        {
            if (plugWindow != null)
            {
                plugWindow.Close();
            }

            Advertisement.OnDestroy();

            MultilanguageManager.Unload(
                SERVICE_NAME,
                (result, resultMessage) =>
                {

                });

            drawSDKInfo = null;

            FileManager.StopDownloadFile();

            if (null != extractCoroutine)
            {
                extractCoroutine.Stop();
            }

            if (SDK_STATE.DOWNLOAD == sdkState || SDK_STATE.EXTRACT == sdkState)
            {
                if (null != fileStream)
                {
                    fileStream.Close();
                    fileStream = null;
                }

                DeleteDownloadSDK();
            }
        }

        public void CloseEditor()
        {
            lock (lockObject)
            {
                AssetDatabase.Refresh();
                window.Close();

                if (plugWindow != null)
                {
                    plugWindow.Close();
                }

                EditorGUIUtility.ExitGUI();
            }
        }

        private void OnGUI()
        {
            if (window == null)
            {
                return;
            }

            lock (lockObject)
            {
                GUILayout.Space(10f);

                GUILayout.BeginHorizontal();
                {
                    DrawSettingToolUpdate();
                    DrawDownloadSDK();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(18f);
                DrawExternalSDK();
                GUILayout.Space(18f);
                DrawSDKSettings();
                DrawCopyright();
            }

            if(isinitialize == true)
            {
                DrawLanguage();
            }            

            DrawSampleAppLink();

            Advertisement.Draw();
        }

        private void DrawSampleAppLink()
        {
            Color oldColor = GUI.contentColor;
            int oldfontSize = GUI.skin.button.fontSize;
            FontStyle oldFontStyle = GUI.skin.button.fontStyle;
            Texture2D oldNormal = GUI.skin.button.normal.background;
            Texture2D oldHover = GUI.skin.button.normal.background;
            Texture2D oldActive = GUI.skin.button.active.background;

            GUI.contentColor = new Color(17f / 255f, 164f / 255f, 251f / 255f, 1f);
            GUI.skin.button.fontSize = 10;
            GUI.skin.button.fontStyle = FontStyle.Bold;
            GUI.skin.button.normal.background = null;
            GUI.skin.button.hover.background = null;
            GUI.skin.button.active.background = null;

            if (GUI.Button(new Rect(792, 190, 220, 16), "Link to Gamebase SampleApp GitHub") == true)
            {
                Application.OpenURL("https://github.com/nhn/toast.gamebase.unity.sample");
            }

            GUI.contentColor = oldColor;
            GUI.skin.button.fontSize = oldfontSize;
            GUI.skin.button.fontStyle = oldFontStyle;
            GUI.skin.button.normal.background = oldNormal;
            GUI.skin.button.hover.background = oldHover;
            GUI.skin.button.active.background = oldActive;
        }

        private void DrawExternalSDK()
        {
            GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
            guiStyle.fontSize = 18;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(12f);
                GUILayout.Label(GetMultilanguageString("EXTERNAL_SDK_SETTING_TITLE"), guiStyle);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(12f);

                if (GUILayout.Button(GetMultilanguageString("PLUG_SDK_SETTING_BUTTON"), GUILayout.Width(120), GUILayout.Height(30)) == true)
                {
                    PLUGSDKWindow.ShowWindow(
                        window,
                        (plugWindow) =>
                        {
                            SDKSettingToolWindow.plugWindow = plugWindow;
                        });
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawLanguage()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Space(446f);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(900f);

                    EditorGUI.BeginChangeCheck();
                    {                        
                        selectedLanguageIndex = EditorGUILayout.Popup(selectedLanguageIndex, languages);
                        GUILayout.Space(8f);
                    }

                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        MultilanguageManager.SelectLanguageByNativeName(SERVICE_NAME, GetSelectLanguageCode(),
                            (result, resultMessage) =>
                            {
                                if (result == MultilanguageResultCode.SUCCESS)
                                {
                                    Advertisement.SetLanguageCode(MultilanguageManager.GetSupportLanguages(SERVICE_NAME, false)[selectedLanguageIndex]);

                                    return;
                                }
                            });
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        public static string GetSelectLanguageCode()
        {
            if (selectedLanguageIndex >= languages.Length)
            {
                return string.Empty;
            }

            string language = languages[selectedLanguageIndex];
            LanguageCode = language;

            return language;
        }

        #region DrawGUI
        private void DrawSettingToolUpdate()
        {
            GUILayout.BeginVertical();
            {
                GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
                guiStyle.fontSize = 18;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(12f);

                    GUILayout.Label(GetMultilanguageString("UI_TEXT_SETTING_TOOL_TITLE"), guiStyle, GUILayout.Width(110));

                    GUILayout.Space(10f);
                    guiStyle.alignment = TextAnchor.LowerLeft;
                    guiStyle.fontSize = 12;
                    GUILayout.Label(VERSION, guiStyle, GUILayout.Width(180), GUILayout.Height(18));
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(12f);

                DrawUpdateButton();
            }
            GUILayout.EndVertical();
        }

        private void DrawDownloadSDK()
        {
            GUILayout.BeginVertical();
            {
                Color oldColor;
                GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
                guiStyle.fontSize = 18;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(2f);
                    GUILayout.Label(GetMultilanguageString("POPUP_DOWNLOAD_SDK_TITLE"), guiStyle, GUILayout.Width(760));
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(12f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(4f);
                    guiStyle.fontSize = 12;
                    oldColor = guiStyle.normal.textColor;

                    GUILayout.Label(
                        string.Format("{0} : ", GetMultilanguageString("UI_TEXT_DOWNLOAD_PATH")),
                        guiStyle,
                        GUILayout.Width(110));
                    GUILayout.BeginVertical();
                    {
                        guiStyle.normal.textColor = oldColor;
                        GUILayout.Label(string.Format("{0}/{1}", "{PROJECT_PATH}", "GamebaseSDK"), guiStyle, GUILayout.Width(904));
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                {
                    switch (updateState)
                    {
                        case UPDATE_STATE.NONE:
                            {
                                GUILayout.BeginVertical();
                                {
                                    DrawDownloadSDKButton();
                                }
                                GUILayout.EndVertical();
                                break;
                            }
                        case UPDATE_STATE.MANDATORY:
                            {
                                break;
                            }
                        case UPDATE_STATE.OPTIONAL:
                            {
                                GUILayout.BeginVertical();
                                {
                                    DrawDownloadSDKButton();
                                }
                                GUILayout.EndVertical();
                                break;
                            }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void DrawDownloadSDKButton()
        {
            GUILayout.BeginHorizontal();
            {
                if (SDK_STATE.DOWNLOAD == sdkState || SDK_STATE.EXTRACT == sdkState)
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button(GetMultilanguageString("POPUP_DOWNLOAD_SDK_TITLE"), GUILayout.Width(150), GUILayout.Height(30)) == true)
                {
                    if (CheckDownloadPopup() == false)
                    {
                        DownloadSDK(
                        () =>
                        {
                            ExtractSDK(
                                () =>
                                {
                                    LoadSDKSettingFromLocal();
                                    sdkState = SDK_STATE.NONE;
                                });
                        });
                    }
                }

                if (SDK_STATE.DOWNLOAD == sdkState || SDK_STATE.EXTRACT == sdkState)
                {
                    GUI.enabled = true;
                }

                GUILayout.BeginVertical();
                {
                    GUILayout.Space(10);

                    GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
                    guiStyle.fontSize = 11;
                    guiStyle.normal.textColor = Color.cyan;
                    guiStyle.alignment = TextAnchor.MiddleLeft;
                    switch (sdkState)
                    {
                        case SDK_STATE.NONE:
                            {
                                break;
                            }
                        case SDK_STATE.NOT_FILE:
                            {
                                GUILayout.Label(GetMultilanguageString("UI_TEXT_SDK_DOWNLOAD_REQUIRED"), guiStyle, GUILayout.Width(400));
                                break;
                            }
                        case SDK_STATE.DOWNLOAD:
                            {
                                GUILayout.Label(
                                    string.Format("{0} {1} : {2}", downloadFileName, GetMultilanguageString("UI_TEXT_DOWNLOADING"), downloadProgress),
                                    guiStyle,
                                    GUILayout.Width(400));
                                break;
                            }
                        case SDK_STATE.EXTRACT:
                            {
                                GUILayout.Label(
                                    string.Format("{0} {1} : {2}", extractFileName, GetMultilanguageString("UI_TEXT_EXTRACTING"), extractProgress),
                                     guiStyle,
                                     GUILayout.Width(400));
                                break;
                            }
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSDKSettings()
        {
            if (drawSDKInfo == null)
            {
                GUILayout.Space(OFFSET_SDK_SETTING);
                return;
            }

            if (SDK_STATE.EXTRACT == sdkState || SDK_STATE.DOWNLOAD == sdkState)
            {
                GUILayout.Space(OFFSET_SDK_SETTING);
                return;
            }

            GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
            guiStyle.fontSize = 18;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(12f);
                GUILayout.Label(GetMultilanguageString("UI_TEXT_SDK_SETTING"), guiStyle);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10f);
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(10f);
                    GUILayout.Label(GetMultilanguageString("UI_TEXT_SELECT_ADDITIONAL_PLATFORMS"), GUILayout.Width(400));

                    GUILayout.BeginHorizontal();
                    {
                        drawSDKInfo.useAndroidPlatform = GUILayout.Toggle(
                            drawSDKInfo.useAndroidPlatform,
                            GetMultilanguageString("UI_TEXT_USE_ANDROID_PLATFORM"),
                            GUILayout.Width(180));

                        drawSDKInfo.useIOSPlatform = GUILayout.Toggle(
                            drawSDKInfo.useIOSPlatform,
                            GetMultilanguageString("UI_TEXT_USE_IOS_PLATFORM"),
                            GUILayout.Width(180));
                    }
                    GUILayout.EndHorizontal();

                }
                GUILayout.EndVertical();


                GUILayout.BeginVertical();
                {
                    GUILayout.Space(6f);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(240f);
                        GUILayout.Space(148f);

                        if (gamebaseSDKInfo == null)
                        {
                            GUILayout.Space(104f);
                        }

                        if (gamebaseLocalSettings == null)
                        {
                            GUILayout.Space(104f);
                        }

                        if (null != gamebaseSDKInfo)
                        {
                            if (GUILayout.Button(GetMultilanguageString("UI_BUTTON_SETTINGS"), GUILayout.Width(100), GUILayout.Height(42)) == true)
                            {
                                if (CheckDownloadPopup() == false)
                                {
                                    SettingSDKPopup();
                                }
                            }
                        }

                        if (null != gamebaseLocalSettings)
                        {
                            if (GUILayout.Button(GetMultilanguageString("UI_BUTTON_REMOVE"), GUILayout.Width(100), GUILayout.Height(42)) == true)
                            {
                                if (CheckDownloadPopup() == false)
                                {
                                    RemoveSDKPopup();
                                }
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10f);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                int maxCount = platforms.Count;

                platforms.Clear();

                platforms.Add("Unity");

                if (drawSDKInfo.useAndroidPlatform == true)
                {
                    platforms.Add("Android");
                }

                if (drawSDKInfo.useIOSPlatform == true)
                {
                    platforms.Add("iOS");
                }

                if (maxCount != platforms.Count)
                {
                    tapIndex = 0;
                }

                if (0 != platforms.Count)
                {
                    int index = GUILayout.Toolbar(tapIndex, platforms.ToArray(), GUILayout.Width(1002));
                    if (index != tapIndex)
                    {
                        tapIndex = index;
                    }
                }
            }
            GUILayout.EndHorizontal();

            DrawScrollBG();

            if (0 != platforms.Count)
            {
                switch (platforms[tapIndex])
                {
                    case "Unity":
                        {
                            DrawAdapterList(PlatformType.UNITY);
                            break;
                        }
                    case "Android":
                        {
                            DrawAdapterList(PlatformType.ANDROID);
                            break;
                        }
                    case "iOS":
                        {
                            DrawAdapterList(PlatformType.IOS);
                            break;
                        }
                }
            }
        }
        private void DrawCopyright()
        {
            GUIStyle guiStyle = new GUIStyle(GUI.skin.label);

            guiStyle.alignment = TextAnchor.MiddleCenter;
            guiStyle.fontSize = 10;
            GUI.Label(new Rect(0, 760, 1024, 14), "ⓒ NHN Corp. All rights reserved.", guiStyle);
        }

        private void DrawUpdateButton()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(12);

                GUILayout.BeginVertical();
                {
                    GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
                    guiStyle.fontSize = 12;
                    guiStyle.alignment = TextAnchor.MiddleLeft;

                    switch (updateState)
                    {
                        case UPDATE_STATE.NONE:
                            {
                                GUILayout.Label(GetMultilanguageString("UI_TEXT_NOT_UPDATE"), guiStyle, GUILayout.Width(220));
                                break;
                            }
                        case UPDATE_STATE.MANDATORY:
                            {
                                guiStyle.normal.textColor = Color.red;
                                GUILayout.Label(GetMultilanguageString("UI_TEXT_MANDATORY_UPDATE"), guiStyle, GUILayout.Width(220));
                                break;
                            }
                        case UPDATE_STATE.OPTIONAL:
                            {
                                guiStyle.normal.textColor = Color.green;
                                GUILayout.Label(GetMultilanguageString("UI_TEXT_OPTIONAL_UPDATE"), guiStyle, GUILayout.Width(220));
                                break;
                            }
                    }

                    GUILayout.Space(10);

                    if (UPDATE_STATE.NONE == updateState)
                    {
                        GUI.enabled = false;
                    }

                    if (GUILayout.Button(GetMultilanguageString("UI_TEXT_DOWNLOAD_SETTINGTOOL"), GUILayout.Width(150), GUILayout.Height(30)) == true)
                    {
                        Application.OpenURL(TOAST_PATH);
                    }

                    GUI.enabled = true;

                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawScrollBG()
        {
            GUI.Box(
                scrollBGRect,
                GUIContent.none);
        }

        private void DrawAdapterList(PlatformType type)
        {
            int provider = 0;
            int purchase = 0;
            int push = 0;
            int etc = 0;

            scrollPos = GUI.BeginScrollView(
                scrollRect,
                scrollPos,
                new Rect(10, 253, 984, GetScrollHeight(type)));
            {
                BeginWindows();

                for (int i = 0; i < drawSDKInfo.adapters.Count; i++)
                {
                    SettingToolVO.AdapterInfo adapter = drawSDKInfo.adapters[i];

                    SettingToolVO.PlatformInfo platform = GetPlatformInfo(type, adapter);

                    if (null != platform)
                    {
                        string path = string.Empty;

                        switch (type)
                        {
                            case PlatformType.UNITY:
                                {
                                    path = downloadPath + "/" + UNITY_FILE_NAME.Replace(".zip", "") + "/sdk/" + adapter.unity.directory;
                                    if (string.IsNullOrEmpty(path) == false && Directory.Exists(path) == false)
                                    {
                                        continue;
                                    }

                                    if (drawSDKInfo.useUnityAdapter == false && adapter.isUnityPriority == true)
                                    {
                                        continue;
                                    }
                                    break;
                                }
                            case PlatformType.ANDROID:
                                {
                                    path = downloadPath + "/" + ANDROID_FILE_NAME.Replace(".zip", "") + "/sdk/" + adapter.android.directory;
                                    if (string.IsNullOrEmpty(path) == false && Directory.Exists(path) == false)
                                    {
                                        continue;
                                    }

                                    if (drawSDKInfo.useUnityAdapter == true && adapter.isUnityPriority == true)
                                    {
                                        continue;
                                    }
                                    break;
                                }
                            case PlatformType.IOS:
                                {
                                    path = downloadPath + "/" + IOS_FILE_NAME.Replace(".zip", "") + "/sdk/" + adapter.ios.directory;
                                    if (string.IsNullOrEmpty(path) == false && Directory.Exists(path) == false)
                                    {
                                        continue;
                                    }

                                    if (drawSDKInfo.useUnityAdapter == true && adapter.isUnityPriority == true)
                                    {
                                        continue;
                                    }
                                    break;
                                }
                        }

                        int indexY = 0;
                        int indexX = 0;

                        switch (adapter.category)
                        {
                            case "Authentication":
                                {
                                    indexY = provider;
                                    indexX = 0;
                                    provider++;
                                    break;
                                }
                            case "Purchase":
                                {
                                    indexY = purchase;
                                    indexX = 1;
                                    purchase++;
                                    break;
                                }
                            case "Push":
                                {
                                    indexY = push;
                                    indexX = 2;
                                    push++;
                                    break;
                                }
                            case "ETC":
                                {
                                    indexY = etc;
                                    indexX = 3;
                                    etc++;
                                    break;
                                }
                        }

                        GUILayout.Window(
                            i,
                            new Rect(
                                20 + OFFSET * indexX + SPACE_WIDTH / 2,
                                SPACE_TOP + (WINDOW_HEIGHT + SPACE_HEIGHT) * indexY,
                                WINDOW_WIDTH,
                                WINDOW_HEIGHT),
                            DrawAdapterCell,
                            adapter.category);
                    }
                }

                EndWindows();
            }
            GUI.EndScrollView();

            List<int> maxCount = new List<int>();
            maxCount.Add(provider);
            maxCount.Add(purchase);
            maxCount.Add(push);
            maxCount.Add(etc);

            switch (type)
            {
                case PlatformType.UNITY:
                    {
                        scrollMaxCountUnity = maxCount.Max();
                        break;
                    }
                case PlatformType.ANDROID:
                    {
                        scrollMaxCountAndroid = maxCount.Max();
                        break;
                    }
                case PlatformType.IOS:
                    {
                        scrollMaxCountIOS = maxCount.Max();
                        break;
                    }
            }
        }

        private void DrawAdapterCell(int index)
        {
            SettingToolVO.AdapterInfo adapter = drawSDKInfo.adapters[index];
            switch (platforms[tapIndex])
            {
                case "Unity":
                    {
                        DrawAdapterCellUnity(adapter);
                        break;
                    }
                case "Android":
                    {
                        DrawAdapterCellAndroid(adapter);
                        break;
                    }
                case "iOS":
                    {
                        DrawAdapterCellIOS(adapter);
                        break;
                    }
            }
        }

        private void DrawAdapterCellUnity(SettingToolVO.AdapterInfo adapter)
        {
            if (adapter.unity == null)
            {
                return;
            }

            bool isCheck = EditorGUILayout.BeginToggleGroup(adapter.name, adapter.unity.useAdapter);
            {
                if (isCheck != adapter.unity.useAdapter)
                {
                    adapter.unity.useAdapter = isCheck;
                    if (adapter.unity.useAdapter == true)
                    {
                        if (adapter.unityOrNativeParallelUseDisable == true)
                        {
                            bool iosVerification = false;
                            if (adapter.ios != null && adapter.ios.useAdapter == true)
                            {
                                iosVerification = true;
                            }

                            bool androidVerification = false;
                            if (adapter.android != null && adapter.android.useAdapter == true)
                            {
                                androidVerification = true;
                            }

                            if (iosVerification == true || androidVerification == true)
                            {
                                if (EditorUtility.DisplayDialog(
                                    GetMultilanguageString("POPUP_DEPENDENCIES_TITLE"),
                                    GetMultilanguageString("POPUP_001_MESSAGE"),
                                    GetMultilanguageString("POPUP_OK"),
                                    GetMultilanguageString("POPUP_CANCEL")) == true)
                                {
                                    VerificationAdapterToggle(adapter.name, PlatformType.UNITY);

                                    if (string.IsNullOrEmpty(adapter.unity.desc) == false)
                                    {
                                        if (EditorUtility.DisplayDialog(
                                            adapter.unity.title,
                                            adapter.unity.desc,
                                            adapter.unity.button_ok,
                                            adapter.unity.button_close) == true)
                                        {
                                            if (string.IsNullOrEmpty(adapter.unity.link) == false)
                                            {
                                                Application.OpenURL(adapter.unity.link);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    adapter.unity.useAdapter = !adapter.unity.useAdapter;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(adapter.unity.desc) == false)
                                {
                                    if (EditorUtility.DisplayDialog(
                                        adapter.unity.title,
                                        adapter.unity.desc,
                                        adapter.unity.button_ok,
                                        adapter.unity.button_close) == true)
                                    {
                                        if (string.IsNullOrEmpty(adapter.unity.link) == false)
                                        {
                                            Application.OpenURL(adapter.unity.link);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(adapter.unity.desc) == false)
                            {
                                if (EditorUtility.DisplayDialog(
                                    adapter.unity.title,
                                    adapter.unity.desc,
                                    adapter.unity.button_ok,
                                    adapter.unity.button_close) == true)
                                {
                                    if (string.IsNullOrEmpty(adapter.unity.link) == false)
                                    {
                                        Application.OpenURL(adapter.unity.link);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndToggleGroup();
        }

        private void DrawAdapterCellAndroid(SettingToolVO.AdapterInfo adapter)
        {
            if (adapter.android == null)
            {
                return;
            }
            adapter.android.useAdapter = EditorGUILayout.BeginToggleGroup(adapter.name, adapter.android.useAdapter);
            {
                if (adapter.android.useAdapter == true)
                {
                    if (adapter.groupBySelectOnlyOneForCategory == true)
                    {
                        for (int i = 0; i < gamebaseSDKInfo.adapters.Count; i++)
                        {
                            SettingToolVO.AdapterInfo info = gamebaseSDKInfo.adapters[i];

                            if (info.name.Equals(adapter.name) == false)
                            {
                                if (null != info.android && info.category == adapter.category)
                                {
                                    info.android.useAdapter = false;
                                }
                            }
                        }
                    }

                    if (adapter.unityOrNativeParallelUseDisable == true)
                    {
                        if (adapter.unity != null && adapter.unity.useAdapter == true)
                        {
                            if (EditorUtility.DisplayDialog(
                                GetMultilanguageString("POPUP_DEPENDENCIES_TITLE"),
                                GetMultilanguageString("POPUP_002_MESSAGE"),
                                GetMultilanguageString("POPUP_OK"),
                                GetMultilanguageString("POPUP_CANCEL")) == true)
                            {
                                VerificationAdapterToggle(adapter.name, PlatformType.ANDROID);
                            }
                            else
                            {
                                adapter.android.useAdapter = !adapter.android.useAdapter;
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndToggleGroup();

            string path = downloadPath + "/" + ANDROID_FILE_NAME.Replace(".zip", "") + "/sdk/" + adapter.android.directory;

            if (Directory.Exists(path) == true)
            {
                if (GUILayout.Button(GetMultilanguageString("UI_BUTTON_DEPENDENCIES_DETAIL")) == true)
                {
                    string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    StringBuilder fileList = new StringBuilder();
                    foreach (string file in files)
                    {
                        if (file.ToLower().Contains(".jar") == true)
                        {
                            string[] fileName = file.Split(Path.DirectorySeparatorChar);
                            fileList.AppendLine(fileName[fileName.Length - 1]);
                        }
                        else if (file.ToLower().Contains(".aar") == true)
                        {
                            string[] fileName = file.Split(Path.DirectorySeparatorChar);
                            fileList.AppendLine(fileName[fileName.Length - 1]);

                        }
                    }

                    if (EditorUtility.DisplayDialog(GetMultilanguageString("POPUP_DEPENDENCIES_TITLE"), fileList.ToString().Substring(0, fileList.ToString().Length - 2), GetMultilanguageString("POPUP_OK")) == true)
                    {
                    }
                }

            }
        }

        private void DrawAdapterCellIOS(SettingToolVO.AdapterInfo adapter)
        {
            if (adapter.ios == null)
            {
                return;
            }

            adapter.ios.useAdapter = EditorGUILayout.BeginToggleGroup(adapter.name, adapter.ios.useAdapter);
            {
                if (adapter.ios.useAdapter == true)
                {
                    if (adapter.unityOrNativeParallelUseDisable == true)
                    {
                        if (adapter.unity != null && adapter.unity.useAdapter == true)
                        {
                            if (EditorUtility.DisplayDialog(
                                GetMultilanguageString("POPUP_DEPENDENCIES_TITLE"),
                                GetMultilanguageString("POPUP_003_MESSAGE"),
                                GetMultilanguageString("POPUP_OK"),
                                GetMultilanguageString("POPUP_CANCEL")) == true)
                            {
                                VerificationAdapterToggle(adapter.name, PlatformType.IOS);
                            }
                            else
                            {
                                adapter.ios.useAdapter = !adapter.ios.useAdapter;
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndToggleGroup();

            if (adapter.ios.externals != null)
            {
                if (GUILayout.Button(GetMultilanguageString("UI_BUTTON_DEPENDENCIES_DETAIL")) == true)
                {
                    if (EditorUtility.DisplayDialog(GetMultilanguageString("POPUP_DEPENDENCIES_TITLE"), adapter.ios.externals, GetMultilanguageString("POPUP_OK")) == true)
                    {
                    }
                }
            }
        }
        #endregion

        private int GetScrollHeight(PlatformType type)
        {
            int scrollHeight = 0;

            switch (type)
            {
                case PlatformType.UNITY:
                    {
                        scrollHeight = (WINDOW_HEIGHT + SPACE_HEIGHT) * scrollMaxCountUnity;
                        break;
                    }
                case PlatformType.ANDROID:
                    {
                        scrollHeight = (WINDOW_HEIGHT + SPACE_HEIGHT) * scrollMaxCountAndroid;
                        break;
                    }
                case PlatformType.IOS:
                    {
                        scrollHeight = (WINDOW_HEIGHT + SPACE_HEIGHT) * scrollMaxCountIOS;
                        break;
                    }
            }

            return scrollHeight;
        }

        private SettingToolVO.PlatformInfo GetPlatformInfo(PlatformType type, SettingToolVO.AdapterInfo adapter)
        {
            SettingToolVO.PlatformInfo platform = null;

            switch (type)
            {
                case PlatformType.UNITY:
                    {
                        platform = adapter.unity;
                        break;
                    }
                case PlatformType.ANDROID:
                    {
                        platform = adapter.android;
                        break;
                    }
                case PlatformType.IOS:
                    {
                        platform = adapter.ios;
                        break;
                    }
            }

            return platform;
        }

        private static void CheckVersion(Action<UPDATE_STATE> callback)
        {
            FileManager.DownloadFileToString(
                DOWNLOAD_URL + VERSION_CHECK_XML_NAME,
                (stateCodeFile, messageFile, data) =>
                {
                    if (FileManager.StateCode.SUCCESS != stateCodeFile)
                    {
                        CheckFileManagerError(stateCodeFile, messageFile, null);
                        return;
                    }

                    XMLManager.LoadXMLFromText<SettingToolVO.SupportedSettingToolVersion>(
                        data,
                        (stateCodeXML, dataXML, messageXML) =>
                        {
                            if (XMLManager.ResponseCode.SUCCESS != stateCodeXML)
                            {
                                CheckXMLManagerError(
                                    stateCodeXML,
                                    messageXML,
                                     () =>
                                     {
                                     });
                                return;
                            }

                            SettingToolVO.SupportedSettingToolVersion vo = dataXML;

                            if (vo == null || vo.newestVersion == null || vo.compatibleVersion == null)
                            {
                                callback(UPDATE_STATE.MANDATORY);
                                return;
                            }

                            if (vo.newestVersion.Equals(VERSION) == true)
                            {
                                callback(UPDATE_STATE.NONE);
                                return;
                            }

                            if (vo.compatibleVersion.Equals(VERSION) == true)
                            {
                                callback(UPDATE_STATE.OPTIONAL);
                                return;
                            }

                            UPDATE_STATE state = UPDATE_STATE.NONE;

                            string[] compatibleVersion = vo.compatibleVersion.Replace("v", "").Split('.');
                            string[] newestVersion = vo.newestVersion.Replace("v", "").Split('.');
                            string[] localVersion = VERSION.Replace("v", "").Split('.');

                            for (int i = 0; i < newestVersion.Length; i++)
                            {
                                int newest = int.Parse(newestVersion[i]);
                                int local = int.Parse(localVersion[i]);

                                if (newest > local)
                                {
                                    state = UPDATE_STATE.OPTIONAL;
                                    break;
                                }
                            }

                            for (int i = 0; i < compatibleVersion.Length; i++)
                            {
                                int compatible = int.Parse(compatibleVersion[i]);
                                int local = int.Parse(localVersion[i]);

                                if (compatible > local)
                                {
                                    state = UPDATE_STATE.MANDATORY;
                                    break;
                                }
                            }

                            callback(state);
                        });
                }, null);
        }

        private void VerificationAdapterToggle(string adapterName, PlatformType type)
        {
            foreach (SettingToolVO.AdapterInfo adapter in drawSDKInfo.adapters)
            {
                if (adapter.name.Equals(adapterName) == true)
                {
                    if (PlatformType.UNITY == type)
                    {
                        if (adapter.ios != null)
                        {
                            adapter.ios.useAdapter = false;
                        }

                        if (adapter.android != null)
                        {
                            adapter.android.useAdapter = false;
                        }
                    }
                    else
                    {
                        if (adapter.unity != null)
                        {
                            adapter.unity.useAdapter = false;
                        }
                    }
                    break;
                }
            }
        }

        #region DownloadSDK
        private void DownloadSDK(Action callback)
        {
            if (drawSDKInfo == null
                || EditorUtility.DisplayDialog(
                        GetMultilanguageString("POPUP_DOWNLOAD_SDK_TITLE"),
                        GetMultilanguageString("POPUP_006_MESSAGE"),
                        GetMultilanguageString("POPUP_OK"),
                        GetMultilanguageString("POPUP_CANCEL")) == true)
            {
                sdkState = SDK_STATE.DOWNLOAD;

                DeleteDownloadSDK();

                CreateDirectory(downloadPath);

                downloadFileName = XML_FILE_NAME;
                DownloadSDKFile(
                    XML_FILE_NAME,
                    () =>
                    {
                        downloadFileName = UNITY_FILE_NAME;
                        DownloadSDKFile(
                            UNITY_FILE_NAME,
                            () =>
                            {
                                downloadFileName = IOS_FILE_NAME;
                                DownloadSDKFile(
                                    IOS_FILE_NAME,
                                    () =>
                                    {
                                        downloadFileName = ANDROID_FILE_NAME;
                                        DownloadSDKFile(
                                            ANDROID_FILE_NAME,
                                            () =>
                                            {
                                                callback();
                                            });
                                    });
                            });
                    });
            }
        }

        private void DownloadSDKFile(string fileName, Action callback)
        {
            downloadFileName = fileName;
            downloadProgress = "0";
            Repaint();

            string url = string.Format("{0}{1}", DOWNLOAD_URL, fileName);
            string downloadFile = string.Format("{0}/{1}", downloadPath, fileName);

            FileManager.DownloadFileToLocal(
                url,
                downloadFile,
                (stateCode, message) =>
                {
                    if (FileManager.StateCode.SUCCESS != stateCode)
                    {
                        CheckFileManagerError(
                            stateCode,
                            message,
                            () =>
                            {
                                sdkState = SDK_STATE.NOT_FILE;
                                window.Repaint();
                                DeleteDownloadSDK();
                            });
                        return;
                    }

                    callback();
                    downloadProgress = "";
                    Repaint();
                },
                (progress) =>
                {
                    downloadProgress = ((int)(progress * 100f)).ToString() + "%";
                    Repaint();
                });
        }
        #endregion DownloadSDK

        #region ExtractSDK
        private void ExtractSDK(Action callback)
        {
            sdkState = SDK_STATE.EXTRACT;

            extractFileName = UNITY_FILE_NAME;
            ExtractSDKFile(
                UNITY_FILE_NAME,
                (isSuccessUnity) =>
                {
                    if (isSuccessUnity == false)
                    {
                        callback();
                        return;
                    }
                    fileStream = null;
                    extractFileName = IOS_FILE_NAME;
                    ExtractSDKFile(
                        IOS_FILE_NAME,
                        (isSuccessIOS) =>
                        {
                            if (isSuccessIOS == false)
                            {
                                callback();
                                return;
                            }
                            fileStream = null;
                            extractFileName = ANDROID_FILE_NAME;
                            ExtractSDKFile(
                                ANDROID_FILE_NAME,
                                (isSuccessAndroid) =>
                                {
                                    fileStream = null;
                                    callback();
                                    Repaint();
                                },
                                (progress) =>
                                {
                                    extractProgress = ((int)(progress * 100f)).ToString() + "%";
                                    Repaint();
                                });
                        },
                        (progress) =>
                        {
                            extractProgress = ((int)(progress * 100f)).ToString() + "%";
                            Repaint();
                        });
                },
                (progress) =>
                {
                    extractProgress = ((int)(progress * 100f)).ToString() + "%";
                    Repaint();
                });
        }

        private void ExtractSDKFile(string fileName, Action<bool> callback, Action<float> progressCallback)
        {
            string downloadFile = string.Format("{0}/{1}", downloadPath, fileName);

            extractCoroutine = EditorCoroutine.Start(
                ZipManager.Extract(
                    downloadFile,
                    downloadFile.Replace(".zip", ""),
                    (stateCode, message) =>
                    {
                        if (ZipManager.StateCode.SUCCESS != stateCode)
                        {
                            CheckZipManagerError(
                                stateCode,
                                message,
                                () =>
                                {
                                });
                            callback(false);
                            return;
                        }
                        callback(true);
                    },
                    (stream) => { fileStream = stream; },
                    progressCallback,
                    null,
                    true));
        }
        #endregion

        #region LoadSDKData
        private static void LoadSDKSettingFromLocal()
        {
            tapIndex = 0;

            downloadPath = Application.dataPath.Replace("Assets", "GamebaseSDK");

            string gamebaseSettingInfoPath = Application.dataPath + LOCAL_SETTING_INFOMATION_XML;

            gamebaseSDKInfo = null;
            gamebaseLocalSettings = null;

            if (File.Exists(gamebaseSettingInfoPath) == true)
            {
                XMLManager.LoadXMLFromFile<SettingToolVO.GamebaseSDKInfo>(
                    gamebaseSettingInfoPath,
                    (stateCodeInfo, dataInfo, messageInfo) =>
                    {
                        gamebaseLocalSettings = dataInfo;
                        LoadSDKSettingFromDownload();
                    });
            }
            else
            {
                LoadSDKSettingFromDownload();
            }
        }

        private static void LoadSDKSettingFromDownload()
        {
            string path = string.Format("{0}/{1}", downloadPath, XML_FILE_NAME);
            if (File.Exists(path) == true)
            {
                XMLManager.LoadXMLFromFile<SettingToolVO.GamebaseSDKInfo>(
                    path,
                    (stateCodeSDKInfo, dataSDKInfo, messageSDKInfo) =>
                    {
                        if (XMLManager.ResponseCode.SUCCESS != stateCodeSDKInfo)
                        {
                            CheckXMLManagerError(
                                stateCodeSDKInfo,
                                messageSDKInfo,
                                () =>
                                {
                                    DeleteDownloadSDK();
                                });
                        }

                        gamebaseSDKInfo = dataSDKInfo;

                        drawSDKInfo = null;

                        if (gamebaseSDKInfo != null)
                        {
                            if (null != gamebaseLocalSettings)
                            {
                                LoadPlatformInformationFromSDK();
                            }

                            drawSDKInfo = gamebaseSDKInfo;
                        }
                        else
                        {
                            if (gamebaseLocalSettings != null)
                            {
                                drawSDKInfo = gamebaseLocalSettings;
                            }
                        }

                        if (gamebaseSDKInfo == null)
                        {
                            sdkState = SDK_STATE.NOT_FILE;
                        }
                    });
            }
            else
            {
                sdkState = SDK_STATE.NOT_FILE;
            }
        }

        private static void LoadPlatformInformationFromSDK()
        {
            gamebaseSDKInfo.useUnityAdapter = gamebaseLocalSettings.useUnityAdapter;
            gamebaseSDKInfo.useAndroidPlatform = gamebaseLocalSettings.useAndroidPlatform;
            gamebaseSDKInfo.useIOSPlatform = gamebaseLocalSettings.useIOSPlatform;

            Dictionary<string, SettingToolVO.AdapterInfo> adapterDictionary = gamebaseLocalSettings.adapters.ToDictionary(
                (adapter) =>
                {
                    return adapter.name.ToLower();
                });

            foreach (SettingToolVO.AdapterInfo adapter in gamebaseSDKInfo.adapters)
            {

                SettingToolVO.AdapterInfo adapterLocal;
                adapterDictionary.TryGetValue(adapter.name.ToLower(), out adapterLocal);

                if (adapterLocal != null)
                {
                    if (adapter.unity != null && adapterLocal.unity != null)
                    {
                        adapter.unity.useAdapter = adapterLocal.unity.useAdapter;
                    }
                    if (adapter.android != null && adapterLocal.android != null)
                    {
                        adapter.android.useAdapter = adapterLocal.android.useAdapter;
                    }
                    if (adapter.ios != null && adapterLocal.ios != null)
                    {
                        adapter.ios.useAdapter = adapterLocal.ios.useAdapter;
                    }
                }
            }

            downloadPath = Application.dataPath.Replace("Assets", "GamebaseSDK");
        }
        #endregion

        #region Popup
        private bool CheckDownloadPopup()
        {
            if (string.IsNullOrEmpty(downloadPath) == true && drawSDKInfo == null)
            {
                if (EditorUtility.DisplayDialog(
                    GetMultilanguageString("POPUP_NOTICE_TITLE"),
                    GetMultilanguageString("POPUP_007_MESSAGE"),
                    GetMultilanguageString("POPUP_OK")) == true)
                {
                    if (window != null)
                    {
                        window.Close();
                        window = null;
                    }
                }
                return true;
            }

            return false;
        }

        private void SettingSDKPopup()
        {
            if (EditorUtility.DisplayDialog(
                GetMultilanguageString("POPUP_SETTING_TITLE"),
                GetMultilanguageString("POPUP_009_MESSAGE"),
                GetMultilanguageString("POPUP_OK"),
                GetMultilanguageString("POPUP_CANCEL")) == true)
            {
                SettingSDK();

                CloseEditor();
            }
        }

        private void RemoveSDKPopup()
        {
            if (EditorUtility.DisplayDialog(
                GetMultilanguageString("POPUP_REMOVE_TITLE"),
                GetMultilanguageString("POPUP_010_MESSAGE"),
                GetMultilanguageString("POPUP_OK"),
                GetMultilanguageString("POPUP_CANCEL")) == true)
            {
                RemoveSDK();

                CloseEditor();
            }
        }

        private void NotFoundFilePopup(string fileName)
        {
            EditorUtility.DisplayDialog(
                GetMultilanguageString("POPUP_NOT_FOUND_TITLE"),
                GetMultilanguageString("POPUP_011_MESSAGE"),
                GetMultilanguageString("POPUP_OK"));

            DeleteDownloadSDK();
            RemoveSDK();
        }

        private static void FileIOErrorPopup(string message)
        {
            EditorUtility.DisplayDialog(
                GetMultilanguageString("POPUP_FILE_IO_TITLE"),
                message,
                GetMultilanguageString("POPUP_OK"));
        }
        #endregion

        #region Error
        private static void CheckXMLManagerError(XMLManager.ResponseCode stateCode, string message, Action callback)
        {
            string title = GetMultilanguageString("POPUP_XML_TITLE");
            string msg = string.Empty;

            switch (stateCode)
            {
                case XMLManager.ResponseCode.FILE_NOT_FOUND_ERROR:
                    {
                        msg = string.Format(GetMultilanguageString("POPUP_013_MESSAGE"), message);
                        break;
                    }
                case XMLManager.ResponseCode.DATA_IS_NULL_ERROR:
                    {
                        msg = GetMultilanguageString("POPUP_014_MESSAGE");
                        break;
                    }
                case XMLManager.ResponseCode.PATH_IS_NULL_ERROR:
                    {
                        msg = GetMultilanguageString("POPUP_015_MESSAGE");
                        break;
                    }
                case XMLManager.ResponseCode.UNKNOWN_ERROR:
                    {
                        msg = string.Format(GetMultilanguageString("POPUP_016_MESSAGE"), message);
                        break;
                    }
            }

            if (EditorUtility.DisplayDialog(title, msg, GetMultilanguageString("POPUP_OK")) == true)
            {
                if (null != callback)
                {
                    callback();
                }
            }
        }

        private static void CheckFileManagerError(FileManager.StateCode stateCode, string message, Action callback)
        {
            string title = GetMultilanguageString("POPUP_FILE_DOWNLOAD_TITLE");
            string msg = string.Empty;

            switch (stateCode)
            {
                case FileManager.StateCode.FILE_NOT_FOUND_ERROR:
                    {
                        msg = string.Format(GetMultilanguageString("POPUP_017_MESSAGE"), message);
                        break;
                    }
                case FileManager.StateCode.WEB_REQUEST_ERROR:
                    {
                        msg = string.Format(GetMultilanguageString("POPUP_018_MESSAGE"), message);
                        break;
                    }
                case FileManager.StateCode.UNKNOWN_ERROR:
                    {
                        msg = string.Format(GetMultilanguageString("POPUP_019_MESSAGE"), message);
                        break;
                    }
            }

            if (EditorUtility.DisplayDialog(title, msg, GetMultilanguageString("POPUP_OK")) == true)
            {
                if (null != callback)
                {
                    callback();
                }
            }
        }

        private static void CheckZipManagerError(ZipManager.StateCode stateCode, string message, Action callback)
        {
            string title = GetMultilanguageString("POPUP_EXTRACT_TITLE");
            string msg = string.Empty;

            switch (stateCode)
            {
                case ZipManager.StateCode.FILE_NOT_FOUND_ERROR:
                    {
                        msg = string.Format(GetMultilanguageString("POPUP_020_MESSAGE"), message);
                        break;
                    }
                case ZipManager.StateCode.FILE_PATH_NULL:
                    {
                        msg = string.Format(GetMultilanguageString("POPUP_021_MESSAGE"), message);
                        break;
                    }
                case ZipManager.StateCode.FOLDER_PATH_NULL:
                    {
                        msg = string.Format(GetMultilanguageString("POPUP_022_MESSAGE"), message);
                        break;
                    }
                case ZipManager.StateCode.UNKNOWN_ERROR:
                    {
                        msg = string.Format(GetMultilanguageString("POPUP_023_MESSAGE"), message);
                        break;
                    }
            }

            if (EditorUtility.DisplayDialog(title, msg, GetMultilanguageString("POPUP_OK")) == true)
            {
                if (null != callback)
                {
                    callback();
                }
            }
        }
        #endregion

        private void SettingSDK()
        {
            string settingPath;
            string from;
            string to;

            settingsPakageFiles.Clear();
            settingsLibsFiles.Clear();
            settingsDirectories.Clear();

            foreach (string baseDirectory in drawSDKInfo.baseInfo.unity.directories)
            {
                string path = downloadPath + "/" + UNITY_FILE_NAME.Replace(".zip", "/sdk/") + baseDirectory;
                if (Directory.Exists(path) == false)
                {
                    path = downloadPath + "/" + UNITY_FILE_NAME.Replace(".zip", "/") + UNITY_FILE_NAME.Replace(".zip", "/sdk/") + baseDirectory;
                    if (Directory.Exists(path) == false)
                    {
                        NotFoundFilePopup(path);
                        return;
                    }
                }
                string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (file.ToLower().Contains(".unitypackage") == true)
                    {
                        settingsPakageFiles.Add(ReplaceDirectorySeparator(file));
                    }
                }
            }

            foreach (string baseDirectory in drawSDKInfo.baseInfo.android.directories)
            {
                string path = downloadPath + "/" + ANDROID_FILE_NAME.Replace(".zip", "/sdk/") + baseDirectory;
                if (Directory.Exists(path) == false)
                {
                    path = downloadPath + "/" + ANDROID_FILE_NAME.Replace(".zip", "/") + ANDROID_FILE_NAME.Replace(".zip", "/sdk/") + baseDirectory;
                    if (Directory.Exists(path) == false)
                    {
                        NotFoundFilePopup(path);
                        return;
                    }
                }
                string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    if (file.ToLower().Contains(".aar") == true || file.ToLower().Contains(".jar") == true)
                    {
                        string fileName = GetFileOrDirectoryName(file);
                        if (settingsLibsFiles.ContainsKey(fileName) == false)
                        {
                            settingsLibsFiles.Add(fileName, file);
                        }
                    }
                }
            }

            foreach (string baseDirectory in drawSDKInfo.baseInfo.ios.directories)
            {
                string path = downloadPath + "/" + IOS_FILE_NAME.Replace(".zip", "/sdk/") + baseDirectory;
                if (Directory.Exists(path) == false)
                {
                    path = downloadPath + "/" + IOS_FILE_NAME.Replace(".zip", "/") + IOS_FILE_NAME.Replace(".zip", "/sdk/") + baseDirectory;
                    if (Directory.Exists(path) == false)
                    {
                        NotFoundFilePopup(path);
                        return;
                    }
                }
                settingsDirectories.Add(path);
            }

            foreach (SettingToolVO.AdapterInfo adapter in drawSDKInfo.adapters)
            {
                if (adapter.unity != null && adapter.unity.useAdapter == true)
                {
                    string path = downloadPath + "/" + UNITY_FILE_NAME.Replace(".zip", "/sdk/") + adapter.unity.directory;
                    if (Directory.Exists(path) == false)
                    {
                        path = downloadPath + "/" + UNITY_FILE_NAME.Replace(".zip", "/") + UNITY_FILE_NAME.Replace(".zip", "/sdk/") + adapter.unity.directory;
                        if (Directory.Exists(path) == false)
                        {
                            NotFoundFilePopup(path);
                            return;
                        }
                    }
                    string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        if (file.ToLower().Contains(".unitypackage") == true)
                        {
                            settingsPakageFiles.Add(ReplaceDirectorySeparator(file));
                        }
                    }
                }

                if (adapter.android != null && adapter.android.useAdapter == true)
                {
                    string path = downloadPath + "/" + ANDROID_FILE_NAME.Replace(".zip", "/sdk/") + adapter.android.directory;
                    if (Directory.Exists(path) == false)
                    {
                        path = downloadPath + "/" + ANDROID_FILE_NAME.Replace(".zip", "/") + ANDROID_FILE_NAME.Replace(".zip", "/sdk/") + adapter.android.directory;
                        if (Directory.Exists(path) == false)
                        {
                            NotFoundFilePopup(path);
                            return;
                        }
                    }
                    string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        if (file.ToLower().Contains(".aar") == true || file.ToLower().Contains(".jar") == true)
                        {
                            string fileName = GetFileOrDirectoryName(file);
                            if (settingsLibsFiles.ContainsKey(fileName) == false)
                            {
                                settingsLibsFiles.Add(fileName, file);
                            }
                        }
                    }
                }

                if (adapter.ios != null && adapter.ios.useAdapter == true)
                {
                    string path = downloadPath + "/" + IOS_FILE_NAME.Replace(".zip", "/sdk/") + adapter.ios.directory;
                    if (Directory.Exists(path) == false)
                    {
                        path = downloadPath + "/" + IOS_FILE_NAME.Replace(".zip", "/") + IOS_FILE_NAME.Replace(".zip", "/sdk/") + adapter.ios.directory;
                        if (Directory.Exists(path) == false)
                        {
                            NotFoundFilePopup(path);
                            return;
                        }
                    }
                    settingsDirectories.Add(path);
                    if (string.IsNullOrEmpty(adapter.ios.externals) == false)
                    {
                        path = downloadPath + "/" + IOS_FILE_NAME.Replace(".zip", "/sdk/externals/") + adapter.ios.externals;
                        if (Directory.Exists(path) == false)
                        {
                            path = downloadPath + "/" + IOS_FILE_NAME.Replace(".zip", "/") + IOS_FILE_NAME.Replace(".zip", "/sdk/externals/") + adapter.ios.externals;
                            if (Directory.Exists(path) == false)
                            {
                                NotFoundFilePopup(path);
                                return;
                            }
                        }
                        settingsExternalsDirectories.Add(path);
                    }
                }
            }

            #region save xml
            string settingsXML = Application.dataPath + LOCAL_SETTING_INFOMATION_XML;

            DeleteFile(settingsXML);

            XMLManager.SaveXMLToFile(
                settingsXML,
                drawSDKInfo,
                (stateCode, message) =>
                {
                    if (XMLManager.ResponseCode.SUCCESS != stateCode)
                    {
                        CheckXMLManagerError(
                            stateCode,
                            message,
                            () =>
                            {
                            });
                    }
                });
            #endregion

            #region copy android
            settingPath = Application.dataPath + "/Plugins/Android/libs/Gamebase/";
            DeleteDirectory(settingPath);

            if (gamebaseSDKInfo.useAndroidPlatform == true)
            {
                if (0 < settingsLibsFiles.Count)
                {
                    settingPath = Application.dataPath + "/Plugins/Android/libs/Gamebase/";
                    CreateDirectory(settingPath);
                    foreach (string file in settingsLibsFiles.Values)
                    {
                        from = ReplaceDirectorySeparator(file);
                        to = ReplaceDirectorySeparator(settingPath + GetFileOrDirectoryName(file));
                        CopyFile(from, to);
                    }
                }
            }
            #endregion

            #region copy iOS
            settingPath = Application.dataPath + "/Plugins/IOS/Gamebase/";
            DeleteDirectory(settingPath);
            settingPath = Application.dataPath + "/Plugins/IOS/Gamebase/externals/";
            DeleteDirectory(settingPath);

            if (gamebaseSDKInfo.useIOSPlatform == true)
            {
                if (0 < settingsDirectories.Count)
                {
                    settingPath = Application.dataPath + "/Plugins/IOS/Gamebase/";
                    CreateDirectory(settingPath);
                    foreach (string directory in settingsDirectories)
                    {
                        from = ReplaceDirectorySeparator(directory);
                        to = ReplaceDirectorySeparator(settingPath + GetFileOrDirectoryName(directory));

                        CopyDirectory(from, to);
                    }
                }
                if (0 < settingsExternalsDirectories.Count)
                {
                    settingPath = Application.dataPath + "/Plugins/IOS/Gamebase/externals/";
                    CreateDirectory(settingPath);
                    foreach (string directory in settingsExternalsDirectories)
                    {
                        from = ReplaceDirectorySeparator(directory);
                        to = ReplaceDirectorySeparator(settingPath + GetFileOrDirectoryName(directory));

                        CopyDirectory(from, to);
                    }
                }
            }
            #endregion

            #region impore unitypackage
            DeleteDirectory(Application.dataPath + "/Gamebase");

            AssetDatabase.Refresh();

            if (0 < settingsPakageFiles.Count)
            {
                foreach (string pakageFile in settingsPakageFiles)
                {
                    if (File.Exists(pakageFile) == false)
                    {
                        NotFoundFilePopup(pakageFile);
                        return;
                    }

                    AssetDatabase.ImportPackage(pakageFile, false);
                }
            }
            #endregion
        }

        private void RemoveSDK()
        {
            string path = string.Empty;

            path = Application.dataPath + LOCAL_SETTING_INFOMATION_XML;
            DeleteFile(path);

            path = Application.dataPath + "/Gamebase";
            DeleteDirectory(path);

            path = Application.dataPath + "/Plugins/Android/libs/Gamebase";
            DeleteDirectory(path);

            path = Application.dataPath + "/Plugins/IOS/Gamebase";
            DeleteDirectory(path);

            path = Application.dataPath + "/StreamingAssets/Gamebase";
            DeleteDirectory(path);
        }

        private static void DeleteDownloadSDK()
        {
            DeleteDirectory(downloadPath);
#if UNITY_EDITOR_WIN
            string[] directoryies = ReplaceDirectorySeparator(downloadPath).Split(Path.DirectorySeparatorChar);
            string newPathRoot = null;
            if (null != directoryies && 0 < directoryies.Length)
            {
                newPathRoot = directoryies[0] + Path.DirectorySeparatorChar + "GamebaseSettingsToolTemp";
                DeleteDirectory(newPathRoot);
            }
#elif UNITY_EDITOR_OSX
#endif
        }

        private static void CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                FileIOErrorPopup(e.Message);
            }
        }

        private static void DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path) == true)
                {
                    FileUtil.DeleteFileOrDirectory(path);
                }

                if (File.Exists(path + ".meta") == true)
                {
                    File.Delete(path + ".meta");
                }
            }
            catch (Exception e)
            {
                FileIOErrorPopup(e.Message);
            }
        }

        private static void DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path) == true)
                {
                    File.Delete(path);
                }

                if (File.Exists(path + ".meta") == true)
                {
                    File.Delete(path + ".meta");
                }
            }
            catch (Exception e)
            {
                FileIOErrorPopup(e.Message);
            }
        }

        private static void CopyFile(string from, string to)
        {
            try
            {
                FileUtil.CopyFileOrDirectory(from, to);
            }
            catch (Exception e)
            {
                FileIOErrorPopup(e.Message);
            }
        }

        private static void CopyDirectory(string from, string to)
        {
            try
            {
                FileUtil.CopyFileOrDirectory(from, to);
            }
            catch (Exception e)
            {
                FileIOErrorPopup(e.Message);
            }
        }

        private string GetFileOrDirectoryName(string name)
        {
            string[] values = ReplaceDirectorySeparator(name).Split(Path.DirectorySeparatorChar);
            return values[values.Length - 1];
        }

        public static string ReplaceDirectorySeparator(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }

        public static string GetMultilanguageString(string key)
        {
            return MultilanguageManager.GetString(SERVICE_NAME, key);
        }

        private static string LanguageCode
        {
            get
            {
                if (EditorPrefs.HasKey(LANGUAGE_SAVE_KEY) == false)
                {
                    string language = string.Empty;
                    switch (Application.systemLanguage)
                    {
                        case SystemLanguage.Korean:
                            {
                                language = GameConstants.LANGUAGE_KOREAN;
                                break;
                            }
                        default:
                            {
                                language = GameConstants.LANGUAGE_ENGLISH;
                                break;
                            }
                    }

                    LanguageCode = language;

                    return language;
                }

                return EditorPrefs.GetString(LANGUAGE_SAVE_KEY);
            }
            set
            {
                EditorPrefs.SetString(LANGUAGE_SAVE_KEY, value);
            }
        }
    }
}