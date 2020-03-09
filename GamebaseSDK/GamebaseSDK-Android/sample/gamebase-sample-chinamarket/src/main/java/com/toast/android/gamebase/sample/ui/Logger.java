/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.ui;

import com.toast.android.gamebase.base.log.Loggable;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;


interface LogUpdateEventListener {
    void onLogUpdateEvent();
}

public class Logger implements Loggable {

    private volatile static Logger uniqueInstance = null;
    private String mLogString = "";
    private static boolean mLastProviderLoginValue = true;
    private LogUpdateEventListener mLogUpdateEventListener = null;

    private Logger() {}

    public static Logger getInstance() {
        if(uniqueInstance == null) {
            synchronized (Logger.class) {
                if(uniqueInstance == null) {
                    uniqueInstance = new Logger();

                    com.toast.android.gamebase.base.log.Logger.addLoggable("LogView", uniqueInstance);

                    SimpleDateFormat sfd = new SimpleDateFormat("yyyy-MM-dd", Locale.KOREA);
                    String currentTime = sfd.format(new Date());
                    uniqueInstance.mLogString = "[ Logger Start Time : " + currentTime + " ]";
                }
            }
        }
        return uniqueInstance;
    }

    @Override
    public void setMessagePrefix(String prefix) {

    }

    @Override
    public String getMessagePrefix() {
        return null;
    }

    @Override
    public boolean isLoggable(int level, int priority) {
        return true;
    }

    @Override
    public void println(int priority, String tag, String msg, Throwable tr) {
        String priorityString;
        switch (priority) {
            case com.toast.android.gamebase.base.log.Logger.VERBOSE:
                priorityString = "[V]";
                break;
            case com.toast.android.gamebase.base.log.Logger.DEBUG:
                priorityString = "[D]";
                break;
            case com.toast.android.gamebase.base.log.Logger.INFO:
                priorityString = "[I]";
                break;
            case com.toast.android.gamebase.base.log.Logger.WARN:
                priorityString = "[W]";
                break;
            case com.toast.android.gamebase.base.log.Logger.ERROR:
                priorityString = "[E]";
                break;
            case com.toast.android.gamebase.base.log.Logger.ASSERT:
                priorityString = "[A]";
                break;
            default:
                priorityString = "";
                break;
        }

        String exceptionString = null;
        if (tr != null) {
            exceptionString = android.util.Log.getStackTraceString(tr);
        }

        final StringBuilder outputBuilder = new StringBuilder();
        String delimiter = "\t";
        appendIfNotNull(outputBuilder, tag, delimiter);
        appendIfNotNull(outputBuilder, priorityString, delimiter);
        appendIfNotNull(outputBuilder, msg, delimiter);
        appendIfNotNull(outputBuilder, exceptionString, delimiter);

        appendToLog(outputBuilder.toString());

    }
    /** Takes a string and adds to it, with a separator, if the bit to be added isn't null. Since
     * the logger takes so many arguments that might be null, this method helps cut out some of the
     * agonizing tedium of writing the same 3 lines over and over.
     * @param source StringBuilder containing the text to append to.
     * @param addStr The String to append
     * @param delimiter The String to separate the source and appended strings. A tab or comma,
     *                  for instance.
     * @return The fully concatenated String as a StringBuilder
     */
    private StringBuilder appendIfNotNull(StringBuilder source, String addStr, String delimiter) {
        if (addStr != null) {
            if (addStr.length() == 0) {
                delimiter = "";
            }

            return source.append(addStr).append(delimiter);
        }
        return source;
    }

    /** Outputs the string as a new line of log data in the LogView. */
    public void appendToLog(String s) {
        mLogString = mLogString + s + "\n" ;

        if(mLogUpdateEventListener != null) {
            mLogUpdateEventListener.onLogUpdateEvent();
        }
    }


    public void setLastProviderLogin(boolean yn) {
        mLastProviderLoginValue = yn;
    }
    public boolean isLastProviderLogin() {
        return mLastProviderLoginValue;
    }

    public void clearLogText() {
        mLogString = "";
    }
    public String getLogText() {
        return mLogString;
    }


    public void setOnLogUpdateEvent(LogUpdateEventListener listener) {
        mLogUpdateEventListener = listener;
    }

}
