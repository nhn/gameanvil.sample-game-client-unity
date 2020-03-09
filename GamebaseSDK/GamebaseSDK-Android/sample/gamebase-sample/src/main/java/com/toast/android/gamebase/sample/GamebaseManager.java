/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */
package com.toast.android.gamebase.sample;

import android.app.Activity;
import android.content.Context;
import android.content.DialogInterface;
import android.graphics.Color;

import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.GamebaseCallback;
import com.toast.android.gamebase.GamebaseConfiguration;
import com.toast.android.gamebase.GamebaseDataCallback;
import com.toast.android.gamebase.GamebaseWebViewConfiguration;
import com.toast.android.gamebase.GamebaseWebViewStyle;
import com.toast.android.gamebase.analytics.data.GameUserData;
import com.toast.android.gamebase.analytics.data.LevelUpData;
import com.toast.android.gamebase.auth.data.AuthToken;
import com.toast.android.gamebase.auth.data.BanInfo;
import com.toast.android.gamebase.base.GamebaseError;
import com.toast.android.gamebase.base.GamebaseException;
import com.toast.android.gamebase.base.NetworkManager;
import com.toast.android.gamebase.base.ScreenOrientation;
import com.toast.android.gamebase.base.auth.AuthProvider;
import com.toast.android.gamebase.base.auth.AuthProviderProfile;
import com.toast.android.gamebase.sample.util.Log;
import com.toast.android.gamebase.base.purchase.PurchasableItem;
import com.toast.android.gamebase.base.purchase.PurchasableReceipt;
import com.toast.android.gamebase.base.purchase.PurchaseProvider;
import com.toast.android.gamebase.base.push.PushConfiguration;
import com.toast.android.gamebase.base.push.PushProvider;
import com.toast.android.gamebase.launching.data.LaunchingInfo;
import com.toast.android.gamebase.launching.data.LaunchingStatus;
import com.toast.android.gamebase.observer.Observer;
import com.toast.android.gamebase.observer.ObserverMessage;
import com.toast.android.gamebase.sample.ui.GameCodeManager;
import com.toast.android.gamebase.serverpush.ServerPushEvent;
import com.toast.android.gamebase.serverpush.ServerPushEventMessage;
import com.toast.android.gamebase.toastlogger.LoggerConfiguration;
import com.toast.android.gamebase.toastlogger.data.LogEntry;
import com.toast.android.gamebase.toastlogger.data.LogFilter;
import com.toast.android.gamebase.toastlogger.data.LoggerListener;

import org.jetbrains.annotations.NotNull;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public class GamebaseManager {
    private static final String TAG = GamebaseManager.class.getSimpleName();

    // TODO: [Fix me] App ID issued from TOAST Project.
    // See http://docs.toast.com/en/Game/Gamebase/en/aos-initialization/#configuration-settings
    private static final String APP_ID = "6ypq5kwa";
    // TODO: [Fix me] See app client version from Gamebase Console
    // See http://docs.toast.com/en/Game/Gamebase/en/oper-app/#client-list
    private static final String APP_VERSION = "1.0.0";

    // TODO: [Fix me] AppKey for the Logger&Crash from TOAST Console > Analytics > Logger & Crash Search
    static final String LOG_AND_CRASH_APPKEY = "jPYSlf6ZDfrXidCUD0hR";

    // TODO: [Fix me] Change to 'false' when release mode.
    private static final boolean DEBUG_MODE = true;

    // TODO: [Fix me] Value for the analytics data of the game.
    private static final int USER_CURRENT_GAME_LEVEL = 10;
    private static final String ANALYTICS_CHANNEL_ID = "channel sample";
    private static final String ANALYTICS_CHARACTER_ID = "character human";

    public static final String MANUAL_LOGIN_IDP = AuthProvider.GUEST;
    private static final String STORE_CODE = PurchaseProvider.StoreCode.GOOGLE;

    private static void printGamebaseError(final GamebaseException exception) {
        Log.w(TAG, "Login Exception");
        Log.w(TAG, "Login Exception - Domain : " + exception.getDomain());
        Log.w(TAG, "Login Exception - Message : " + exception.getMessage());
        Log.w(TAG, "Login Exception - Code : " + exception.getCode());
        Log.w(TAG, "Login Exception - Detail : " + exception.toString());
        Log.w(TAG, "--------------------------------------");
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Initialization
    //
    ////////////////////////////////////////////////////////////////////////////////
    public static void initialize(final Activity activity) {
        Gamebase.setDebugMode(DEBUG_MODE);

        GamebaseConfiguration configuration =
                GamebaseConfiguration.newBuilder(APP_ID, APP_VERSION, STORE_CODE)
                        .enablePopup(true)
                        .enableBanPopup(true)
                        .enableLaunchingStatusPopup(true)
                        .build();

        Gamebase.initialize(activity, configuration, new GamebaseDataCallback<LaunchingInfo>() {
            @Override
            public void onCallback(LaunchingInfo launchingInfo, GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    boolean canPlay = true;
                    String errorLog = "";
                    switch (launchingInfo.getStatus().getCode()) {
                        case LaunchingStatus.IN_SERVICE:
                            break;
                        case LaunchingStatus.RECOMMEND_UPDATE:
                            Log.d(TAG, "There is a new version of this application.");
                            break;
                        case LaunchingStatus.IN_SERVICE_BY_QA_WHITE_LIST:
                        case LaunchingStatus.IN_TEST:
                        case LaunchingStatus.IN_REVIEW:
                            Log.d(TAG, "You logged in because you are developer.");
                            break;
                        case LaunchingStatus.REQUIRE_UPDATE:
                            canPlay = false;
                            errorLog = "You have to update this application.";
                            break;
                        case LaunchingStatus.BLOCKED_USER:
                            canPlay = false;
                            errorLog = "You are blocked user!";
                            break;
                        case LaunchingStatus.TERMINATED_SERVICE:
                            canPlay = false;
                            errorLog = "Game is closed!";
                            break;
                        case LaunchingStatus.INSPECTING_SERVICE:
                        case LaunchingStatus.INSPECTING_ALL_SERVICES:
                            canPlay = false;
                            errorLog = "Under maintenance.";
                            break;
                        case LaunchingStatus.INTERNAL_SERVER_ERROR:
                        default:
                            canPlay = false;
                            errorLog = "Unknown internal error.";
                            break;
                    }
                    if (canPlay) {
                        GameCodeManager.moveToLoginScene(activity);
                    } else {
                        GameCodeManager.showErrorAndReturnToTitle(activity,
                                "Launching Failed",
                                errorLog);
                    }
                } else {
                    Log.w(TAG, "Launching Exception : " + exception.toJsonString());

                    GameCodeManager.showErrorAndReturnToTitle(activity,
                            "Launching Exception",
                            exception.toJsonString());
                }
            }
        });
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Authentication
    //
    ////////////////////////////////////////////////////////////////////////////////
    public static void lastProviderLogin(final Activity activity, final OnLoginWithLastestLoginTypeCallback callback) {
        final String lastLoggedInProvider = Gamebase.getLastLoggedInProvider();
        if (isChinaMarketBuild(lastLoggedInProvider)) {
            // China SDK should not call loginForLastLoggedInProvider.
            loginWithIdP(activity, lastLoggedInProvider);
            if (callback != null) {
                callback.onCallback();
            }
            return;
        }
        Log.d(TAG, "Last Logged in Provider : " + lastLoggedInProvider);

        Gamebase.loginForLastLoggedInProvider(activity, new GamebaseDataCallback<AuthToken>() {
            @Override
            public void onCallback(AuthToken result, GamebaseException exception) {
                boolean triggerCallback = true;
                if (Gamebase.isSuccess(exception)) {
                    onLoginWithLastLoggedInProviderSucceeded(activity, result);
                } else {
                    if (isNetworkError(exception)) {
                        triggerCallback = false;
                        GameCodeManager.showError(activity, "Network Error", "Check your network.");
                        retryWithInterval(new Runnable() {
                            @Override
                            public void run() {
                                lastProviderLogin(activity, callback);
                            }
                        }, 2);
                    } else if (isBannedUser(exception)) {
                        // Do nothing because you set the 'enableBanPopup' true.
                        // Gamebase will show ban-popup automatically.

                        // Obtaining Ban Information
                        BanInfo banInfo = Gamebase.getBanInfo();
                        printBanInfo(banInfo);
                    } else {
                        if (lastLoggedInProvider != null) {
                            loginWithIdP(activity, lastLoggedInProvider);
                        } else {
                            // Do nothing. User should select IDP from Game UI.
                        }
                    }
                }

                if (triggerCallback && callback != null) {
                    callback.onCallback();
                }
            }
        });
    }

    private static boolean isChinaMarketBuild(final String provider) {
        if (AuthProvider.REDBEANCC.equalsIgnoreCase(provider)) {
            return true;
        }
        return false;
    }

    public interface OnLoginWithLastestLoginTypeCallback {
        void onCallback();
    }

    public static void loginWithIdP(final Activity activity, final String provider) {
        Log.d(TAG, "Logger with provider : " + provider);

        Gamebase.login(activity, provider, null, new GamebaseDataCallback<AuthToken>() {
            @Override
            public void onCallback(AuthToken result, GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    onLoginWithIdPSucceeded(activity, provider, result);
                } else {
                    if (isNetworkError(exception)) {
                        GameCodeManager.showError(activity, "Network Error", "Check your network.");
                        retryWithInterval(new Runnable() {
                            @Override
                            public void run() {
                                loginWithIdP(activity, provider);
                            }
                        }, 2);
                    } else if (isBannedUser(exception)) {
                        // Do nothing because you set the 'enableBanPopup' true.
                        // Gamebase will show ban-popup automatically.

                        // Obtaining Ban Information
                        BanInfo banInfo = Gamebase.getBanInfo();
                        printBanInfo(banInfo);
                    } else {
                        onLoginFailed(activity, exception);
                    }
                }
            }
        });
    }

    private static boolean isNetworkError(final GamebaseException exeption) {
        final int errorCode = exeption.getCode();
        if (errorCode == GamebaseError.SOCKET_ERROR ||
                errorCode == GamebaseError.SOCKET_RESPONSE_TIMEOUT) {
            return true;
        }
        return false;
    }

    private static boolean isBannedUser(final GamebaseException exeption) {
        if (exeption.getCode() == GamebaseError.BANNED_MEMBER) {
            return true;
        }
        return false;
    }

    private static void printBanInfo(final BanInfo banInfo) {
        Log.d(TAG, "Ban Info");
        Log.d(TAG, "Ban Info - User ID : " + banInfo.userId);
        Log.d(TAG, "Ban Info - Ban Type : " + banInfo.banType);
        Log.d(TAG, "Ban Info - Message : " + banInfo.message);
        Log.d(TAG, "Ban Info - Begin Date : " + banInfo.beginDate);
        Log.d(TAG, "Ban Info - End Date : " + banInfo.endDate);
        Log.d(TAG, "--------------------------------------");
    }

    private static void retryWithInterval(final Runnable runnable, final int intervalSecond) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    Thread.sleep(intervalSecond * 1000);
                    runnable.run();
                } catch (InterruptedException e) {
                }
            }
        }).start();
    }

    private static void onLoginFailed(final Activity activity, GamebaseException exception) {
        printGamebaseError(exception);
        GameCodeManager.showError(activity, "Login Failed!", exception.toJsonString());
    }

    private static void onLoginSucceeded(final Activity activity, AuthToken result) {
        Log.i(TAG, "Gamebase User Id : " + result.getUserId());
        Log.i(TAG, "Gamebase Access Token : " + result.getAccessToken());
        Log.i(TAG, "LastLoggedInProvider : " + Gamebase.getLastLoggedInProvider());
        Log.d(TAG, "--------------------------------------");

        // Handles Server Push Messages from Gamebase server.
        addServerPushEvent(activity);

        // Receive and process status change events of Gamebase.
        addObserver(activity);

        if (hasPushAdapterInProject()) {
            if (GameCodeManager.hasPushSettingsData(activity.getApplicationContext())) {
                // Register Push from saved values.
                PushConfiguration configuration = GameCodeManager.loadPushSettings(activity.getApplicationContext());
                if (configuration != null) {
                    registerPush(activity, configuration.pushEnabled, configuration.adAgreement, configuration.adAgreementNight, null);
                }
            } else {
                // Show push settings UI to user.
                GameCodeManager.showPushUI(activity, new GameCodeManager.PushUICallback() {
                    @Override
                    public void onClosed(PushConfiguration configuration) {
                        if (configuration != null) {
                            registerPush(activity, configuration.pushEnabled, configuration.adAgreement, configuration.adAgreementNight, null);
                        }
                    }
                });
            }
        }

        if (hasPurchaseAdapterInProject()) {
            // Check not consumed item
            requestNotConsumedItems(activity, new GamebaseDataCallback<List<PurchasableReceipt>>() {
                @Override
                public void onCallback(List<PurchasableReceipt> purchasableReceipts, GamebaseException notComsumeListException) {
                    if (Gamebase.isSuccess(notComsumeListException)) {
                        if (purchasableReceipts != null && purchasableReceipts.size() > 0) {
                            // There are purchased items that require consume.
                            // Game client needs to send a request to the game server to consume,
                            // so that items can be provided.
                        }
                    }
                }
            });
        }

        // Initialize Gamebase Analytics
        initializeAnalytics(USER_CURRENT_GAME_LEVEL, ANALYTICS_CHANNEL_ID, ANALYTICS_CHARACTER_ID);

        GameCodeManager.startGame(activity);
    }

    private static void onLoginWithLastLoggedInProviderSucceeded(final Activity activity, AuthToken result) {
        Log.d(TAG, "Login with Last Logged In Provider Success");
        Log.i(TAG, "LastLoggedInProvider : " + Gamebase.getLastLoggedInProvider());

        onLoginSucceeded(activity, result);
    }

    private static void onLoginWithIdPSucceeded(final Activity activity, String provider, AuthToken result) {
        Log.d(TAG, "Login with IdP(" + provider + ") Success");
        Log.i(TAG, provider + " User Id : " + Gamebase.getAuthProviderUserID(provider));
        Log.i(TAG, provider + " Access Token : " + Gamebase.getAuthProviderAccessToken(provider));
        AuthProviderProfile profile = Gamebase.getAuthProviderProfile(provider);
        Log.i(TAG, provider + " Profile : " + (profile != null ? profile.toJsonString() : "null"));

        onLoginSucceeded(activity, result);
    }

    public static boolean isLoggedIn() {
        final String userId = Gamebase.getUserID();
        if (userId == null || userId.equalsIgnoreCase("")) {
            return false;
        }
        return true;
    }

    public static void logout(final Activity activity) {
        Gamebase.logout(activity, new GamebaseCallback() {
            @Override
            public void onCallback(GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    GameCodeManager.returnToLoginScene(activity);
                } else {
                    GameCodeManager.showError(activity, "Logout Failed !", exception.toJsonString());
                }
            }
        });
    }

    public static void withdraw(final Activity activity) {
        Gamebase.withdraw(activity, new GamebaseCallback() {
            @Override
            public void onCallback(GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    GameCodeManager.returnToLoginScene(activity);
                } else {
                    GameCodeManager.showError(activity, "Withdraw Failed !", exception.toJsonString());
                }
            }
        });
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Account Mapping
    //
    ////////////////////////////////////////////////////////////////////////////////
    public static void addMapping(final Activity activity, final String providerName, final GamebaseDataCallback<AuthToken> callback) {
        Gamebase.addMapping(activity, providerName, null, new GamebaseDataCallback<AuthToken>() {
            @Override
            public void onCallback(AuthToken data, final GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    Log.d(TAG, "Add Mapping Success");
                    Log.i(TAG, "User Id : " + data.getUserId());
                    Log.i(TAG, "Gamebase Access Token : " + data.getAccessToken());
                    Log.d(TAG, "--------------------------------------");

                    callback.onCallback(data, exception);
                } else {
                    printGamebaseError(exception);
                    GameCodeManager.showError(activity, "Add Mapping Failed!", exception.toJsonString());
                    callback.onCallback(data, exception);
                }
            }
        });
    }

    public static void removeMapping(final Activity activity, final String providerName, final GamebaseCallback callback) {
        Gamebase.removeMapping(activity, providerName, new GamebaseCallback() {
            @Override
            public void onCallback(GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    callback.onCallback(exception);
                } else {
                    printGamebaseError(exception);
                    GameCodeManager.showError(activity, "Remove Mapping Failed!", exception.toJsonString());
                    callback.onCallback(exception);
                }
            }
        });
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Purchase
    //
    ////////////////////////////////////////////////////////////////////////////////
    private static boolean hasPurchaseAdapterInProject() {
        List<String> purchaseProviders = new ArrayList<>();
        purchaseProviders.add(PurchaseProvider.StoreCode.GOOGLE);
        purchaseProviders.add(PurchaseProvider.StoreCode.ONESTORE);
        for (String provider : purchaseProviders) {
            try {
                final String lowerProvider = provider.toLowerCase();
                Class.forName("com.toast.android.gamebase.purchase." + lowerProvider +
                        ".Purchase" + lowerProvider.substring(0, 1).toUpperCase() + lowerProvider.substring(1));
                return true;
            } catch (Exception ignored) {
            }
        }
        return false;
    }

    public static void requestNotConsumedItems(final Activity activity, final GamebaseDataCallback<List<PurchasableReceipt>> callback) {
        Gamebase.Purchase.requestItemListOfNotConsumed(activity, new GamebaseDataCallback<List<PurchasableReceipt>>() {
            @Override
            public void onCallback(List<PurchasableReceipt> purchasableReceipts, GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    // Succeeded.
                } else {
                    // Failed.
                    Log.w(TAG, "Request purchases failed : " + exception.toString());
                }

                if (callback != null) {
                    callback.onCallback(purchasableReceipts, exception);
                }
            }
        });
    }

    public static void requestSubscriptions(final Activity activity, final GamebaseDataCallback<List<PurchasableReceipt>> callback) {
        Gamebase.Purchase.requestActivatedPurchases(activity, new GamebaseDataCallback<List<PurchasableReceipt>>() {
            @Override
            public void onCallback(List<PurchasableReceipt> purchasableReceipts, GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    // Succeeded.
                } else {
                    // Failed.
                    Log.w(TAG, "Request purchases failed : " + exception.toString());
                }

                if (callback != null) {
                    callback.onCallback(purchasableReceipts, exception);
                }
            }
        });
    }

    public static void requestItemList(final Activity activity, final GamebaseDataCallback<List<PurchasableItem>> callback) {
        Gamebase.Purchase.requestItemListPurchasable(activity, new GamebaseDataCallback<List<PurchasableItem>>() {
            @Override
            public void onCallback(List<PurchasableItem> purchasableItems, GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    // Succeeded.
                } else {
                    // Failed.
                    Log.w(TAG, "Request item list failed : " + exception.toString());
                }

                if (callback != null) {
                    callback.onCallback(purchasableItems, exception);
                }
            }
        });
    }

    public static void requestPurchase(final Activity activity, final long itemSeq, final GamebaseDataCallback<PurchasableReceipt> callback) {
        Gamebase.Purchase.requestPurchase(activity, itemSeq, new GamebaseDataCallback<PurchasableReceipt>() {
            @Override
            public void onCallback(PurchasableReceipt data, GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    // Succeeded.
                } else if (exception.getCode() == GamebaseError.PURCHASE_USER_CANCELED) {
                    // User canceled.
                } else {
                    // To Purchase Item Failed cause of the error
                }

                if (callback != null) {
                    callback.onCallback(data, exception);
                }
            }
        });
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Push
    //
    ////////////////////////////////////////////////////////////////////////////////
    private static boolean hasPushAdapterInProject() {
        List<String> pushProviders = new ArrayList<>();
        pushProviders.add(PushProvider.Type.FCM);
        pushProviders.add(PushProvider.Type.TENCENT);
        for (String provider : pushProviders) {
            try {
                final String lowerProvider = provider.toLowerCase();
                Class.forName("com.toast.android.gamebase.push." + lowerProvider +
                        ".Push" + lowerProvider.substring(0, 1).toUpperCase() + lowerProvider.substring(1));
                return true;
            } catch (Exception ignored) {
            }
        }
        return false;
    }

    public static void queryPush(final Activity activity, final GamebaseDataCallback<PushConfiguration> callback) {
        Gamebase.Push.queryPush(activity, new GamebaseDataCallback<PushConfiguration>() {
            @Override
            public void onCallback(PushConfiguration pushConfiguration, GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    Log.d(TAG, "Query Push Success");
                    Log.i(TAG, "enablePush : " + pushConfiguration.pushEnabled);
                    Log.i(TAG, "enableAdPush : " + pushConfiguration.adAgreement);
                    Log.i(TAG, "enableAdNightPush : " + pushConfiguration.adAgreementNight);
                    Log.d(TAG, "--------------------------------------");
                } else {
                    Log.e(TAG, "Query push failed : " + exception.toString());
                }
                if (callback != null) {
                    callback.onCallback(pushConfiguration, exception);
                }
            }
        });
    }

    public static void registerPush(final Activity activity,
                                    final boolean enablePush,
                                    final boolean enableAdPush,
                                    final boolean enableAdPushNight,
                                    final GamebaseCallback callback) {
        final PushConfiguration configuration = new PushConfiguration(enablePush, enableAdPush, enableAdPushNight);
        Gamebase.Push.registerPush(activity, configuration, new GamebaseCallback() {
            @Override
            public void onCallback(GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    // Succeeded.
                    GameCodeManager.savePushSettings(activity, configuration);
                } else {
                    Log.e(TAG, "Register push failed : " + exception.toString());
                }
                if (callback != null) {
                    callback.onCallback(exception);
                }
            }
        });
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // WebView
    //
    ////////////////////////////////////////////////////////////////////////////////
    // Show WebView.
    public static void showWebView(final Activity activity, final String urlString) {
        Gamebase.WebView.showWebView(activity, urlString);
    }

    public static void showWebView(final Activity activity,
                                   final String url,
                                   final GamebaseWebViewConfiguration configuration) {
        Gamebase.WebView.showWebView(activity, url, configuration);
    }

    public static void showWebView(final Activity activity,
                                   final String url,
                                   final GamebaseWebViewConfiguration configuration,
                                   final GamebaseCallback onCloseCallback,
                                   final List<String> schemeList,
                                   final GamebaseDataCallback<String> onEvent) {
        Gamebase.WebView.showWebView(activity, url, configuration, onCloseCallback, schemeList, onEvent);
    }

    // Close currently displayed WebView.
    public static void closeWebView(final Activity activity) {
        Gamebase.WebView.closeWebView(activity);
    }

    // Open with External Web Browser.
    public static void openExternalBrowser(final Activity activity, final String urlString) {
        Gamebase.WebView.openWebBrowser(activity, urlString);
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // WebView - Example Code
    //
    ////////////////////////////////////////////////////////////////////////////////
    private static void webviweExample(final Activity activity,
                                       final String urlString,
                                       final String title) {
        GamebaseWebViewConfiguration configuration
                = new GamebaseWebViewConfiguration.Builder()
                .setStyle(GamebaseWebViewStyle.BROWSER)             // BROWSER, POPUP
                .setTitleText(title)                                // Set Title
                .setScreenOrientation(ScreenOrientation.PORTRAIT)   // PORTRAIT, LANDSCAPE, LANDSCAPE_SENSOR
                .setNavigationBarColor(Color.RED)                   // Set Navigation Bar Color
                .setNavigationBarHeight(40)                         // Set Navigation Bar Height
                .setBackButtonVisible(true)                         // Set Back Button Visible
//                .setBackButtonImageResource(R.id.back_button)       // Set Back Button Image ID of resource
//                .setCloseButtonImageResource(R.id.close_button)     // Set Close Button Image ID of resource
                .build();
        showWebView(activity, urlString, configuration, null, null, null);
    }

    private static void webviewExampleWithSchemeEvent(final Activity activity,
                                                      final String urlString,
                                                      final String title) {
        GamebaseWebViewConfiguration configuration
                = new GamebaseWebViewConfiguration.Builder()
                .setStyle(GamebaseWebViewStyle.BROWSER)             // BROWSER, POPUP
                .setTitleText(title)                                // Set Title
                .build();
        List<String> schemeList = new ArrayList<>();
        schemeList.add("mygame://");
        schemeList.add("closemywebview://");
        showWebView(activity, urlString, configuration,
                new GamebaseCallback() {
                    @Override
                    public void onCallback(GamebaseException exception) {
                        // When closed WebView, this callback will be called.
                    }
                },
                schemeList,
                new GamebaseDataCallback<String>() {
                    @Override
                    public void onCallback(String fullUrl, GamebaseException exception) {
                        if (Gamebase.isSuccess(exception)) {
                            if (fullUrl.contains("mygame://")) {
                                // do something
                            } else if (fullUrl.contains("closemywebview://")) {
                                closeWebView(activity);
                            }
                        } else {
                            GameCodeManager.showError(activity, "Exception from WebView Callback", exception.toString());
                        }
                    }
                });
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // ServerPush
    //
    ////////////////////////////////////////////////////////////////////////////////
    public static void addServerPushEvent(final Activity activity) {
        Gamebase.addServerPushEvent(new ServerPushEvent() {
            @Override
            public void onReceive(ServerPushEventMessage serverPushEventMessage) {
                if (serverPushEventMessage.type.equals(ServerPushEventMessage.Type.APP_KICKOUT)) {
                    // Kicked out from Gamebase server.(Maintenance, banned or etc..)
                    // Return to title and initialize Gamebase again.
                    GameCodeManager.returnToTitle(activity);
                } else if (serverPushEventMessage.type.equals(ServerPushEventMessage.Type.TRANSFER_KICKOUT)) {
                    // If the user wants to move the guest account to another device,
                    // if the account transfer is successful,
                    // the login of the previous device is released,
                    // so go back to the title and try to log in again.
                    GameCodeManager.returnToTitle(activity);
                }
            }
        });
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Observer
    //
    ////////////////////////////////////////////////////////////////////////////////
    public static void addObserver(final Activity activity) {
        Observer launchingStatusObserver = new Observer() {
            @Override
            public void onUpdate(ObserverMessage observerMessage) {
                Map<String, Object> dataMap = observerMessage.data;
                if (observerMessage.type.equalsIgnoreCase(ObserverMessage.Type.LAUNCHING)) {
                    int launchingStatusCode = Integer.parseInt((String) dataMap.get("code"));
                    String messageString = (String) dataMap.get("message");
                    Log.d(TAG, "Update launching status to " + launchingStatusCode + ", " + messageString);

                    boolean canPlay = true;
                    String errorLog = "";
                    switch (launchingStatusCode) {
                        case LaunchingStatus.IN_SERVICE:
                            break;
                        case LaunchingStatus.RECOMMEND_UPDATE:
                            Log.d(TAG, "There is a new version of this application.");
                            break;
                        case LaunchingStatus.IN_SERVICE_BY_QA_WHITE_LIST:
                        case LaunchingStatus.IN_TEST:
                        case LaunchingStatus.IN_REVIEW:
                            Log.d(TAG, "You logged in because you are developer.");
                            break;
                        case LaunchingStatus.REQUIRE_UPDATE:
                            canPlay = false;
                            errorLog = "You have to update this application.";
                            break;
                        case LaunchingStatus.BLOCKED_USER:
                            canPlay = false;
                            errorLog = "You are blocked user!";
                            break;
                        case LaunchingStatus.TERMINATED_SERVICE:
                            canPlay = false;
                            errorLog = "Game is closed!";
                            break;
                        case LaunchingStatus.INSPECTING_SERVICE:
                        case LaunchingStatus.INSPECTING_ALL_SERVICES:
                            canPlay = false;
                            errorLog = "Under maintenance.";
                            break;
                        case LaunchingStatus.INTERNAL_SERVER_ERROR:
                        default:
                            canPlay = false;
                            errorLog = "Unknown internal error.";
                            break;
                    }
                    if (canPlay) {
                        // Now, user can play the game.
                        Log.d(TAG, "Changed Status : You can play game, now!");
                    } else {
                        // User cannot play the game.
                        Log.e(TAG, "Changed Status : " + errorLog);
                        GameCodeManager.returnToTitle(activity);
                    }
                }
            }
        };

        Observer userStatusObserver = new Observer() {
            @Override
            public void onUpdate(ObserverMessage observerMessage) {
                Map<String, Object> dataMap = observerMessage.data;
                if (observerMessage.type.equalsIgnoreCase(ObserverMessage.Type.HEARTBEAT)) {
                    int errorCode = Integer.parseInt((String) dataMap.get("code"));
                    Log.d(TAG, "Heartbeat changing : " + dataMap);

                    switch (errorCode) {
                        case GamebaseError.INVALID_MEMBER:
                            // You can check the invalid user session in here.
                            // ex) After transferred account to another device.
                            break;
                        case GamebaseError.BANNED_MEMBER:
                            // You can check the banned user session in here.
                            break;
                    }
                }
            }
        };

        Observer networkStatusObserver = new Observer() {
            @Override
            public void onUpdate(ObserverMessage observerMessage) {
                Map<String, Object> dataMap = observerMessage.data;
                if (observerMessage.type.equalsIgnoreCase(ObserverMessage.Type.NETWORK)) {
                    int networkTypeCode = Integer.parseInt((String) dataMap.get("code"));
                    Log.d(TAG, "Network changing : " + dataMap);

                    // You can check the changed network status in here.
                    if (networkTypeCode == NetworkManager.TYPE_NOT) {
                        // Network disconnected.
                    } else {
                        // Network connected.
                    }
                }
            }
        };

        Gamebase.addObserver(launchingStatusObserver);
        Gamebase.addObserver(userStatusObserver);
        Gamebase.addObserver(networkStatusObserver);
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Gamebase Analytics
    //
    ////////////////////////////////////////////////////////////////////////////////
    public static void initializeAnalytics(int userLevel, String channelId, String characterId) {
        GameUserData gameUserData = new GameUserData(userLevel);

        // Optional data
        gameUserData.channelId = channelId;     // Optional
        gameUserData.characterId = characterId; // Optional

        Gamebase.Analytics.setGameUserData(gameUserData);
    }

    public static void onGameUserLevelUp(int userLevel, long levelUpTime) {
        LevelUpData levelUpData = new LevelUpData(userLevel, levelUpTime);
        Gamebase.Analytics.traceLevelUp(levelUpData);
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // TOAST Logger (Logger&Crash)
    //
    ////////////////////////////////////////////////////////////////////////////////
    public static void initializeToastLogger(final Context context) {
        Log.d(TAG, "Initialize TOAST Logger");
        final LoggerConfiguration.Builder configBuilder = LoggerConfiguration.newBuilder(
                LOG_AND_CRASH_APPKEY, true);
        Gamebase.Logger.initialize(context, configBuilder.build());
    }

    public static void setToastLoggerListener() {
        Gamebase.Logger.setLoggerListener(new LoggerListener() {
            @Override
            public void onSuccess(@NotNull LogEntry logEntry) {
            }

            @Override
            public void onFilter(@NotNull LogEntry logEntry, @NotNull LogFilter logFilter) {
                Log.i(TAG, "SendLog.onFilter(" + logEntry.toString() + ") : " + logFilter.toString());
            }

            @Override
            public void onSave(@NotNull LogEntry logEntry) {
            }

            @Override
            public void onError(@NotNull LogEntry logEntry, @NotNull GamebaseException exception) {
                Log.w(TAG, "SendLog.onError(" + logEntry.toString() + ") : " + exception.toString());
            }
        });
    }

    public static void sendLogDebug(final String message, final Map<String, Object> userField) {
        Gamebase.Logger.debug(message, userField);
    }

    public static void sendLogInfo(final String message, final Map<String, Object> userField) {
        Gamebase.Logger.info(message, userField);
    }

    public static void sendLogWarn(final String message, final Map<String, Object> userField) {
        Gamebase.Logger.warn(message, userField);
    }

    public static void sendLogError(final String message, final Map<String, Object> userField) {
        Gamebase.Logger.error(message, userField);
    }

    public static void sendLogFatal(final String message, final Map<String, Object> userField) {
        Gamebase.Logger.fatal(message, userField);
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // UI Helper
    //
    ////////////////////////////////////////////////////////////////////////////////
    public static void showSimpleAlertDialog(final Activity activity, final String title, final String message) {
        Gamebase.Util.showAlert(activity, title, message);
    }

    public static void showAlertDialogWithListener(final Activity activity,
                                                   final String title,
                                                   final String message,
                                                   final DialogInterface.OnClickListener clickListener) {
        Gamebase.Util.showAlert(activity, title, message, clickListener);
    }

    public static void showToast(final Activity activity, final String message, final int duration) {
        Gamebase.Util.showToast(activity, message, duration);
    }
}
