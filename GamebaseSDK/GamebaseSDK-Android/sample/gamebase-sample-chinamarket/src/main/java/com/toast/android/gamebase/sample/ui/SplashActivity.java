/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.ui;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.KeyEvent;

import com.toast.android.gamebase.sample.GamebaseManager;
import com.toast.android.gamebase.sample.GamebaseRedbeanCCActivity;
import com.toast.android.gamebase.sample.R;

public class SplashActivity extends GamebaseRedbeanCCActivity {
    private static final String TAG = SplashActivity.class.getSimpleName();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splash);

        android.util.Log.d(TAG, "SplashActivity Init");

        GamebaseManager.initialize(this);
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        return keyCode != KeyEvent.KEYCODE_BACK && super.onKeyDown(keyCode, event);
    }

    public static void LoadLoginActivity(final Activity activity) {

        Thread thread = new Thread() {
            @Override
            public void run() {
                try {
                    Thread.sleep(3000);
                } catch (Exception e) {
                    e.printStackTrace();
                }

                activity.runOnUiThread(new Runnable() {
                    @Override
                    public void run() {

                        //startActivity(new Intent(SplashActivity.this, LoginActivity.class));
                        activity.startActivity(new Intent(activity, MainActivity.class));
                        activity.overridePendingTransition(android.R.anim.fade_in, android.R.anim.fade_out);
                        activity.finish();
                    }
                });
            }
        };
        thread.start();
    }

}

