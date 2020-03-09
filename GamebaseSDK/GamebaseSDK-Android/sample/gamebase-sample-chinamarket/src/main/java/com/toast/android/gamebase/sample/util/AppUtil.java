/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.util;

import android.app.Activity;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.widget.Toast;

import com.toast.android.gamebase.Gamebase;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;

@SuppressWarnings("deprecation")
public class AppUtil {

    public static String getCurrentTime() {
        SimpleDateFormat sfd = new SimpleDateFormat("yyMMdd HH:mm:ss", Locale.KOREA);
        String currentTime = sfd.format(new Date());
        return currentTime;
    }

    public static void copyClipBoard(final Activity activity, String string ) {

        Context context = activity.getApplicationContext();

        ClipboardManager clipboardManager = (ClipboardManager)context.getSystemService(context.CLIPBOARD_SERVICE);
        ClipData clipData = ClipData.newPlainText("text", string );
        clipboardManager.setPrimaryClip(clipData);

        ClipData data = clipboardManager.getPrimaryClip();

        Gamebase.Util.showToast(activity, "Copy Clipboard : " + data.getItemAt(0).getText().toString(), Toast.LENGTH_SHORT);

    }


    private static android.app.ProgressDialog mProgressDialog;

    public static void showProgressDialog(final Activity activity, final String message) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                mProgressDialog = android.app.ProgressDialog.show(activity, "", message);
                mProgressDialog.show();
            }
        });
    }

    public static void dismissProgressDialog(final Activity activity) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (mProgressDialog != null && mProgressDialog.isShowing()) {
                    mProgressDialog.dismiss();
                }
                mProgressDialog = null;
            }
        });
    }

}
