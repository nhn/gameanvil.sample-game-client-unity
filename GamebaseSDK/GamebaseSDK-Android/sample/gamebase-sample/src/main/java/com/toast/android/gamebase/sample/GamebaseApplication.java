/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample;

import android.support.multidex.MultiDexApplication;

import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.base.GamebaseException;
import com.toast.android.gamebase.toastlogger.LoggerConfiguration;
import com.toast.android.gamebase.toastlogger.data.CrashListener;
import com.toast.android.gamebase.toastlogger.data.LogEntry;
import com.toast.android.gamebase.toastlogger.data.LogFilter;
import com.toast.android.gamebase.toastlogger.data.LoggerListener;

import org.jetbrains.annotations.NotNull;

import java.util.Map;

public class GamebaseApplication extends MultiDexApplication {

    private static final String TAG = GamebaseApplication.class.getSimpleName();

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Gamebase SDK Required
    //
    ////////////////////////////////////////////////////////////////////////////////
    @Override
    public void onCreate() {
        super.onCreate();

        // Initialize TOAST Logger
        GamebaseManager.initializeToastLogger(getApplicationContext());
        GamebaseManager.setToastLoggerListener();
    }
}
