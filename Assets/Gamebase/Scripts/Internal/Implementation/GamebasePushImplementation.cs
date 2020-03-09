﻿#if !UNITY_EDITOR && UNITY_ANDROID
using Toast.Gamebase.Internal.Mobile.Android;
#elif !UNITY_EDITOR && UNITY_IOS
using Toast.Gamebase.Internal.Mobile.IOS;
#elif !UNITY_EDITOR && UNITY_WEBGL
using Toast.Gamebase.Internal.Single.WebGL;
#else
using Toast.Gamebase.Internal.Single.Standalone;
#endif

namespace Toast.Gamebase.Internal
{
    public sealed class GamebasePushImplementation
    {
        private static readonly GamebasePushImplementation instance = new GamebasePushImplementation();
        
        public static GamebasePushImplementation Instance
        {
            get { return instance; }
        }

        IGamebasePush push;

        private GamebasePushImplementation()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            push = new AndroidGamebasePush();
#elif !UNITY_EDITOR && UNITY_IOS
            push = new IOSGamebasePush();
#elif !UNITY_EDITOR && UNITY_WEBGL
            push = new WebGLGamebasePush();
#else
            push = new StandaloneGamebasePush();
#endif
        }

        public void RegisterPush(GamebaseRequest.Push.PushConfiguration pushConfiguration, GamebaseCallback.ErrorDelegate callback)
        {
            int handle = GamebaseCallbackHandler.RegisterCallback(callback);
            push.RegisterPush(pushConfiguration, handle);
        }
        
        public void QueryPush(GamebaseCallback.GamebaseDelegate<GamebaseResponse.Push.PushConfiguration> callback)
        {
            int handle = GamebaseCallbackHandler.RegisterCallback(callback);
            push.QueryPush(handle);
        }

        public void SetSandboxMode(bool isSandbox)
        {
            push.SetSandboxMode(isSandbox);
        }
    }
}