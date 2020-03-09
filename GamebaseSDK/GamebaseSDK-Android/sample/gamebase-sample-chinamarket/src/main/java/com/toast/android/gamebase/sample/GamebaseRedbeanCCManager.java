/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample;

import android.app.Activity;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Bundle;

import com.redbean.sdk.RBSDKS;
import com.redbean.sdk.listener.RBOnChangeAccountListener;
import com.redbean.sdk.listener.RBOnExitListener;
import com.redbean.sdk.utils.RBParamKeys;
import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.GamebaseConfiguration;
import com.toast.android.gamebase.GamebaseDataCallback;
import com.toast.android.gamebase.base.GamebaseException;
import com.toast.android.gamebase.base.auth.AuthProvider;
import com.toast.android.gamebase.sample.util.Log;
import com.toast.android.gamebase.base.purchase.PurchaseProvider;
import com.toast.android.gamebase.launching.data.LaunchingInfo;
import com.toast.android.gamebase.launching.data.LaunchingStatus;
import com.toast.android.gamebase.sample.ui.GameCodeManager;

import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.atomic.AtomicBoolean;

public class GamebaseRedbeanCCManager {
    private static final String TAG = GamebaseRedbeanCCManager.class.getSimpleName();

    // TODO: [Fix me] App ID issued from TOAST Cloud Project.
    // See http://docs.toast.com/en/Game/Gamebase/en/aos-initialization/#configuration-settings
    private static final String APP_ID = "6ypq5kwa";
    // TODO: [Fix me] See app client version from Gamebase Console
    // See http://docs.toast.com/en/Game/Gamebase/en/oper-app/#client-list
    private static final String APP_VERSION = "1.0.0.CN";

    // TODO: [Fix me] AppKey for the Logger&Crash from TOAST Console > Analytics > Logger & Crash Search
    static final String LOG_AND_CRASH_APPKEY = "jPYSlf6ZDfrXidCUD0hR";

    // TODO: [Fix me] Change to 'false' when release mode.
    private static final boolean DEBUG_MODE = true;
    private static final String STORE_CODE = PurchaseProvider.StoreCode.REDBEANCC;

    // LOGIN ID & Password : gbsample / gbsample
    public static final String MANUAL_LOGIN_IDP = AuthProvider.REDBEANCC;

    // China market SDK
    private static AtomicBoolean hasRBSDKSClass = null;

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Lifecycle
    //
    ////////////////////////////////////////////////////////////////////////////////
    public static void onCreate(final Activity activity, final Bundle savedInstanceState) {
        android.util.Log.d(TAG, "onSaveInstanceState: " +
                (savedInstanceState != null ? savedInstanceState.toString() : "savedInstanceState(null)"));

        if (hasRedbeanCCSDK()) {
            RBSDKS.onCreate(activity, savedInstanceState);
        }
    }

    public static void onSaveInstanceState(final Activity activity, final Bundle savedInstanceState) {
        android.util.Log.d(TAG, "onSaveInstanceState: " +
                (savedInstanceState != null ? savedInstanceState.toString() : "savedInstanceState(null)"));

        if (hasRedbeanCCSDK()) {
            RBSDKS.onCreate(activity, savedInstanceState);
        }
    }

    public static void onStart(final Activity activity) {
        android.util.Log.d(TAG, "onStart");

        if (hasRedbeanCCSDK()) {
            RBSDKS.onStart(activity);
        }
    }

    public static void onResume(final Activity activity) {
        android.util.Log.d(TAG, "onResume");

        if (hasRedbeanCCSDK()) {
            RBSDKS.onResume(activity);
        }
    }

    public static void onPause(final Activity activity) {
        android.util.Log.d(TAG, "onPause");

        if (hasRedbeanCCSDK()) {
            RBSDKS.onPause(activity);
        }
    }

    public static void onStop(final Activity activity) {
        android.util.Log.d(TAG, "onStop");

        if (hasRedbeanCCSDK()) {
            RBSDKS.onStop(activity);
        }
    }

    public static void onDestroy(final Activity activity) {
        android.util.Log.d(TAG, "onDestroy");

        if (hasRedbeanCCSDK()) {
            RBSDKS.onDestroy(activity);
        }
    }

    public static void onNewIntent(final Activity activity, final Intent intent) {
        final String intentLog;
        if (intent == null) {
            intentLog = "intent(null)";
        } else if (intent.getExtras() == null) {
            intentLog = "extras(null)";
        } else {
            intentLog = intent.getExtras().toString();
        }
        android.util.Log.d(TAG, "onNewIntent: " + intentLog);
        if (hasRedbeanCCSDK()) {
            RBSDKS.onNewIntent(activity, intent);
        }
    }

    public static void onConfigurationChanged(final Activity activity, final Configuration newConfig) {
        android.util.Log.d(TAG, "onConfigurationChanged");

        if (hasRedbeanCCSDK()) {
            RBSDKS.onConfigurationChanged(activity, newConfig);
        }
    }

    public static void onActivityResult(final Activity activity, final int requestCode, final int resultCode, final Intent data) {
        final String intentLog;
        if (data == null) {
            intentLog = "intent(null)";
        } else if (data.getExtras() == null) {
            intentLog = "extras(null)";
        } else {
            intentLog = data.getExtras().toString();
        }
        android.util.Log.d(TAG, "onActivityResult: " + requestCode + ", " + resultCode + ", " + intentLog);

        if (hasRedbeanCCSDK()) {
            RBSDKS.onActivityResult(activity, requestCode, resultCode, data);
        }
    }

    private static boolean hasRedbeanCCSDK() {
        if (hasRBSDKSClass != null) {
            return hasRBSDKSClass.get();
        } else {
            hasRBSDKSClass = new AtomicBoolean(false);
            try {
                Class.forName("com.redbean.sdk.RBSDKS");
                hasRBSDKSClass.set(true);
                return true;
            } catch (ClassNotFoundException e) {
                return false;
            } catch (Exception e) {
                android.util.Log.w(TAG, "Unexpected Exception : " + e.getMessage());
                return false;
            }
        }
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
    // China Market API
    //
    ////////////////////////////////////////////////////////////////////////////////
    public static void saveRoleInfo(final Activity activity, final Map<String, Object> roleInfo, final String savedState) {
        android.util.Log.d(TAG, "saveRoleInfo: " +
                (roleInfo != null ? roleInfo.toString() : "roleInfo(null)" + ", " + "savedState(" + savedState + ")"));

        if (hasRedbeanCCSDK()) {
            RBSDKS.saveRoleInfo(activity, roleInfo, savedState);
        }
    }

    public static void exit(final Activity activity, final RBOnExitListener listener) {
        android.util.Log.d(TAG, "exit");

        if (hasRedbeanCCSDK()) {
            RBSDKS.exit(activity, new RBOnExitListener() {
                @Override
                public void onExit(String type, String msg) {
                    android.util.Log.d(TAG, "onExit: " + type + ", " + msg);

                    if (type.equalsIgnoreCase(RBSDKS.EXIT_TYPE_GAME)) {
                        // channel has not exit popup , show game exit popup

                    } else if (type.equalsIgnoreCase(RBSDKS.EXIT_TYPE_CHANNEL)) {
                        // channel has exit popup

                    }

                    if (listener != null) {
                        listener.onExit(type, msg);
                    }
                }
            });
        }
    }

    public static void subscribChangedAccount(final Activity activity, final RBOnChangeAccountListener listener) {
        android.util.Log.d(TAG, "subscribChangedAccount");

        if (hasRedbeanCCSDK()) {
            RBSDKS.subscribeChangedAccount(new RBOnChangeAccountListener() {
                @Override
                public void onChangedAccount(boolean isSuccess) {
                    android.util.Log.d(TAG, "onChangedAccount: " + isSuccess);

                    // Goto the Game Title Page and reLogin.
                    GameCodeManager.returnToTitle(activity);

                    if (listener != null) {
                        listener.onChangedAccount(isSuccess);
                    }
                }
            });
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // saveRoleInfo - Sample Code
    //
    ////////////////////////////////////////////////////////////////////////////////
    private static void saveRoleInfoSample(final Activity activity,
                                           final String gameUserId,
                                           final String gameUserName,
                                           final String gameServerId,
                                           final String gameServerName,
                                           final String gamePartyName,
                                           final String gameUserLevel,
                                           final String gameUserVip,
                                           final String gameUserBalance,
                                           final long ctime) {

        final Map<String, Object> roleInfo = new HashMap<>();
        final String savedState = RBSDKS.CREATE_ROLE;     // CREATE_ROLE, LOGIN_ROLE, ROLE_UPGRADE, EXIT

        roleInfo.put(RBParamKeys.STRING_GAME_ROLE_ID, gameUserId);
        roleInfo.put(RBParamKeys.STRING_GAME_ROLE_NAME, gameUserName);
        roleInfo.put(RBParamKeys.STRING_GAME_SERVER_ID, gameServerId);
        roleInfo.put(RBParamKeys.STRING_GAME_SERVER_NAME, gameServerName);
        roleInfo.put(RBParamKeys.STRING_GAME_PARTY_NAME, gamePartyName);
        roleInfo.put(RBParamKeys.STRING_GAME_ROLE_LEVEL, gameUserLevel);
        roleInfo.put(RBParamKeys.STRING_GAME_ROLE_VIP, gameUserVip);
        roleInfo.put(RBParamKeys.STRING_GAME_ROLE_BALANCE, gameUserBalance);
        roleInfo.put(RBParamKeys.LONG_GAME_ROLE_CREATE_TIME, ctime);

        saveRoleInfo(activity, roleInfo, savedState);
    }
}
