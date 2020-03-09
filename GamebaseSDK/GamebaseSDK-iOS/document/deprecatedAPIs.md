# Deprecated APIs

| Class |API Name |Added Version |Deprecated Version |Removed Version |Replacement API Name |
| --- | --- | --- | --- | --- | --- | 
|Gamebase.Webview|ShowWebBrowser(string url)|1.4.0|1.5.0||ShowWebView(string url, GamebaseRequest.Webview.GamebaseWebViewConfiguration configuration = null, GamebaseCallback.ErrorDelegate closeCallback = null, List<string> schemeList = null, GamebaseCallback.GamebaseDelegate<string> schemeEvent = null)|
|Gamebase.Webview|ShowWebView(GamebaseRequest.Webview.GamebaseWebViewConfiguration configuration)|1.4.0|1.5.0||ShowWebView(string url, GamebaseRequest.Webview.GamebaseWebViewConfiguration configuration = null, GamebaseCallback.ErrorDelegate closeCallback = null, List<string> schemeList = null, GamebaseCallback.GamebaseDelegate<string> schemeEvent = null)|
|Gamebase.Util|ShowToast(string message, int duration)|1.4.0|1.5.0||ShowToast(string message, GamebaseUIToastType type)|
|Gamebase.Launching|AddUpdateStatusListener(GamebaseCallback.DataDelegate<GamebaseResponse.Launching.LaunchingStatus> callback)|1.4.0|1.8.0||AddObserver(GamebaseCallback.DataDelegate<GamebaseResponse.SDK.ObserverMessage> observer)|
|Gamebase.Launching|RemoveUpdateStatusListener(GamebaseCallback.DataDelegate<GamebaseResponse.Launching.LaunchingStatus> callback)|1.4.0|1.8.0||RemoveObserver(GamebaseCallback.DataDelegate<GamebaseResponse.SDK.ObserverMessage> observer)|
|Gamebase.Network|AddOnChangedStatusListener(GamebaseCallback.DataDelegate<GamebaseNetworkType> callback)|1.4.0|1.8.0||AddObserver(GamebaseCallback.DataDelegate<GamebaseResponse.SDK.ObserverMessage> observer)|
|Gamebase.Network|RemoveOnChangedStatusListener(GamebaseCallback.DataDelegate<GamebaseNetworkType> callback)|1.4.0|1.8.0||RemoveObserver(GamebaseCallback.DataDelegate<GamebaseResponse.SDK.ObserverMessage> observer)|
<!--stackedit_data:
eyJoaXN0b3J5IjpbMTAzNDM5MDEyNl19
-->