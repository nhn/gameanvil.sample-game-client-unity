/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample;

import com.redbean.sdk.app.RBMultiApplication;
import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.toastlogger.LoggerConfiguration;

////////////////////////////////////////////////////////////////////////////////
//
// For use Redbean CC SDK, you need to extend RBApplication or RBMultiApplication.
//
////////////////////////////////////////////////////////////////////////////////
public class GamebaseRedbeanCCApplication extends RBMultiApplication {

    private static final String TAG = GamebaseRedbeanCCApplication.class.getSimpleName();

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
