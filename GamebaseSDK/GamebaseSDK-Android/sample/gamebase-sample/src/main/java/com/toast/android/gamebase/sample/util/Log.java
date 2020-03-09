/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.util;

public class Log {
    public static String getStackTraceString(final Throwable tr) {
        return android.util.Log.getStackTraceString(tr);
    }

    public static void v(final String tag, final String msg) {
        android.util.Log.v(tag, msg);
    }

    public static void d(final String tag, final String msg) {
        android.util.Log.d(tag, msg);
    }

    public static void i(final String tag, final String msg) {
        android.util.Log.i(tag, msg);
    }

    public static void w(final String tag, final String msg) {
        android.util.Log.w(tag, msg);
    }

    public static void w(final String tag, final Throwable tr) {
        android.util.Log.w(tag, tr);
    }

    public static void e(final String tag, final String msg) {
        android.util.Log.e(tag, msg);
    }

    public static void wtf(final String tag, final String msg) {
        android.util.Log.wtf(tag, msg);
    }
}
