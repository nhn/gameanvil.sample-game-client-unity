using UnityEditor;
using UnityEngine;

namespace Toast.GamebaseTools.SettingTool
{
    public class SettingToolMenu
    {
        [MenuItem("Tools/TOAST/Gamebase/Setting Tool/Settings")]
        public static void GamebaseSDKSetting()
        {
            SDKSettingToolWindow.ShowWindow();
        }

        [MenuItem("Tools/TOAST/Gamebase/Setting Tool/Guide")]
        public static void GamebaseSDKGuide()
        {
            Application.OpenURL("http://docs.TOAST.com/en/Game/Gamebase/en/unity-started/#using-the-setting-tool");
        }
    }
}