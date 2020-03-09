/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.ui;


import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ScrollView;
import android.widget.TextView;

import com.toast.android.gamebase.sample.R;

public class LogFragment extends Fragment {

    private TextView mTextViewLog;
    private static View mCurrentView = null;
    private static Logger mLogger = null;

    public LogFragment() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        setHasOptionsMenu(true);

        mCurrentView = inflater.inflate(R.layout.fragment_log, container, false);

        mLogger = Logger.getInstance();

        setClearLogsButton();

        /*******************************************************************************************/
        mTextViewLog = mCurrentView.findViewById(R.id.textView_Log);
        mTextViewLog.setText(mLogger.getLogText());
        setAutoScroll();

        mLogger.setOnLogUpdateEvent(new LogUpdateEventListener() {
            @Override
            public void onLogUpdateEvent() {
                getActivity().runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        mTextViewLog.setText(mLogger.getLogText());
                        setAutoScroll();
                    }
                });
            }
        });
        /*******************************************************************************************/

        return mCurrentView;

    }


    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Action Bar Item Setting
    @Override
    public void onCreateOptionsMenu(Menu menu, MenuInflater inflater) {

        menu.clear();

        inflater.inflate(R.menu.main, menu);

        MenuItem logoutItem = menu.findItem(R.id.action_logout);
        logoutItem.setVisible(false);
        MenuItem closeItem = menu.findItem(R.id.action_activityClose);
        closeItem.setVisible(true);
        MenuItem logOpenItem = menu.findItem(R.id.action_log);
        logOpenItem.setVisible(false);

    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        mLogger.setOnLogUpdateEvent(null);
        ((MainActivity)getActivity()).actionItemEvent(item);
        return super.onOptionsItemSelected(item);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////



    private  void setClearLogsButton () {

        Button buttonClearLog = mCurrentView.findViewById(R.id.button_clearLog);
        buttonClearLog.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                mLogger.clearLogText();
                mTextViewLog.setText(mLogger.getLogText());
            }
        });
    }

    private void setAutoScroll() {

        final ScrollView scrollViewLog = mCurrentView.findViewById(R.id.scrollView_Log);

        getActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {

                scrollViewLog.post(new Runnable() {
                    @Override
                    public void run() {
                        scrollViewLog.scrollTo(0, mTextViewLog.getHeight());
                    }
                });

            }
        });

    }

}
