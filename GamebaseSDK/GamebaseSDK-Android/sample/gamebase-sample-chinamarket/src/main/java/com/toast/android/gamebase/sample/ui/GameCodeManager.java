/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */
package com.toast.android.gamebase.sample.ui;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.SharedPreferences;
import android.support.annotation.UiThread;
import android.widget.Switch;

import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.GamebaseCallback;
import com.toast.android.gamebase.GamebaseDataCallback;
import com.toast.android.gamebase.auth.data.AuthToken;
import com.toast.android.gamebase.base.GamebaseException;
import com.toast.android.gamebase.base.push.PushConfiguration;
import com.toast.android.gamebase.sample.GamebaseManager;

public class GameCodeManager {
    private static final String PREF_NAME = "gamebase.sample.preference";
    private static final String PREF_KEY_HAS_HISTORY_OF_PUSH_SETTINGS = "history.push.settings";
    private static final String PREF_KEY_PUSH_ENABLED = "push.enabled";
    private static final String PREF_KEY_PUSH_ENABLED_AD = "push.enabled.ad";
    private static final String PREF_KEY_PUSH_ENABLED_AD_NIGHT = "push.enabled.ad.night";
    private static final String PREF_KEY_PUSH_DISPLAY_LANGUAGE_CODE = "push.displaylanguage.code";

    public static void showErrorAndReturnToTitle(final Activity activity, final String title, final String message) {
        final DialogInterface.OnClickListener returnToTitle = new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                returnToTitle(activity);
            }
        };
        Gamebase.Util.showAlert(activity, title, message, returnToTitle);
    }

    public static void returnToTitle(final Activity activity) {
        activity.finish();
    }

    public static void showError(final Activity activity, final String title, final String message) {
        Gamebase.Util.showAlert(activity, title, message);
    }

    public static void moveToLoginScene(final Activity activity) {
        SplashActivity.LoadLoginActivity(activity);
    }

    public static void returnToLoginScene(final Activity activity) {
        ((MainActivity) activity).loadLoginFragment();
    }

    public static void startGame(final Activity activity) {
        ((MainActivity) activity).loadApiListFragment();
    }

    public static void addMappingAndChangeAccountUI(final Activity activity, final String providerName, final Switch mappingSwitch) {
        GamebaseManager.addMapping(activity, providerName, new GamebaseDataCallback<AuthToken>() {
            @Override
            public void onCallback(AuthToken authToken, GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    changeAccountSwitchUI(activity, mappingSwitch, true);
                } else {
                    changeAccountSwitchUI(activity, mappingSwitch, false);
                }
            }
        });
    }

    public static void removeMappingAndChangeAccountUI(final Activity activity, final String providerName, final Switch mappingSwitch) {
        GamebaseManager.removeMapping(activity, providerName, new GamebaseCallback() {
            @Override
            public void onCallback(GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    // succeeded
                } else {
                    changeAccountSwitchUI(activity, mappingSwitch, true);
                }
            }
        });
    }

    private static void changeAccountSwitchUI(final Activity activity, final Switch changeSwitch, final boolean flag) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                changeSwitch.setChecked(flag);
            }
        });
    }

    private static SharedPreferences getSharedPreference(final Context context) {
        SharedPreferences preferences = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        return preferences;
    }

    private static SharedPreferences.Editor getSharedPreferenceEditor(final Context context) {
        SharedPreferences preferences = getSharedPreference(context);
        if (preferences != null) {
            return preferences.edit();
        }

        return null;
    }

    public static boolean hasPushSettingsData(final Context context) {
        SharedPreferences preferences = getSharedPreference(context);
        if (preferences == null) {
            return false;
        }

        return preferences.contains(PREF_KEY_PUSH_ENABLED) &&
                preferences.contains(PREF_KEY_PUSH_ENABLED_AD) &&
                preferences.contains(PREF_KEY_PUSH_ENABLED_AD_NIGHT);
    }

    public static PushConfiguration loadPushSettings(final Context context) {
        SharedPreferences preferences = getSharedPreference(context);
        if (preferences == null) {
            return new PushConfiguration(false, false, false);
        }

        return new PushConfiguration(preferences.getBoolean(PREF_KEY_PUSH_ENABLED, false),
                preferences.getBoolean(PREF_KEY_PUSH_ENABLED_AD, false),
                preferences.getBoolean(PREF_KEY_PUSH_ENABLED_AD_NIGHT, false));
    }

    public static void savePushSettings(final Context context, final PushConfiguration configuration) {
        SharedPreferences.Editor editor = getSharedPreferenceEditor(context);
        if (editor == null) {
            return;
        }

        editor.putBoolean(PREF_KEY_PUSH_ENABLED, configuration.pushEnabled);
        editor.putBoolean(PREF_KEY_PUSH_ENABLED_AD, configuration.adAgreement);
        editor.putBoolean(PREF_KEY_PUSH_ENABLED_AD_NIGHT, configuration.adAgreementNight);
        if (configuration.displayLanguageCode != null && !configuration.displayLanguageCode.equals("")) {
            editor.putString(PREF_KEY_PUSH_DISPLAY_LANGUAGE_CODE, configuration.displayLanguageCode);
        }
        editor.apply();
    }

    public static void showPushUI(final Activity activity, final PushUICallback callback) {
        if (activity == null) {
            return;
        }
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                AlertDialog.Builder builder = new AlertDialog.Builder(activity);
                final String[] labels = new String[]{"Enable Notification", "Agree AD Notification", "Agree AD-Night Notification"};
                final boolean[] checked = new boolean[]{true, true, true};
                builder.setMultiChoiceItems(labels, checked, new DialogInterface.OnMultiChoiceClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i, boolean b) {
                        checked[i] = b;
                    }
                });
                builder.setPositiveButton("OK", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        if (callback != null) {
                            PushConfiguration configuration = new PushConfiguration(checked[0], checked[1], checked[2]);
                            callback.onClosed(configuration);
                        }
                    }
                });
                builder.setNegativeButton("Cancel", null)
                        .setTitle("Register Push Notification")
                        .setCancelable(true)
                        .show();
            }
        });
    }

    public interface PushUICallback {
        void onClosed(PushConfiguration configuration);
    }
}
