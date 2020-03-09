/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample;

import android.content.Intent;
import android.content.res.Configuration;
import android.os.Bundle;

public class GamebaseRedbeanCCActivity extends GamebaseActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        GamebaseRedbeanCCManager.onCreate(this, savedInstanceState);
    }

    @Override
    public void onSaveInstanceState(Bundle savedInstanceState) {
        super.onSaveInstanceState(savedInstanceState);
        GamebaseRedbeanCCManager.onSaveInstanceState(this, savedInstanceState);
    }

    @Override
    protected void onStart() {
        super.onStart();
        GamebaseRedbeanCCManager.onStart(this);
    }

    @Override
    protected void onResume() {
        super.onResume();
        GamebaseRedbeanCCManager.onResume(this);
    }

    @Override
    protected void onPause() {
        super.onPause();
        GamebaseRedbeanCCManager.onPause(this);
    }

    @Override
    protected void onStop() {
        super.onStop();
        GamebaseRedbeanCCManager.onStop(this);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        GamebaseRedbeanCCManager.onDestroy(this);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        GamebaseRedbeanCCManager.onNewIntent(this, intent);
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        GamebaseRedbeanCCManager.onConfigurationChanged(this, newConfig);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        GamebaseRedbeanCCManager.onActivityResult(this, requestCode, resultCode, data);
    }
}
