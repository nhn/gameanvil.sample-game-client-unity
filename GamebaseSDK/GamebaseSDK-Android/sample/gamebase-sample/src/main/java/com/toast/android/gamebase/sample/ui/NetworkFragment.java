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
import android.widget.Toast;

import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.sample.util.Log;
import com.toast.android.gamebase.sample.R;


public class NetworkFragment extends Fragment {

    private static final String TAG = NetworkFragment.class.getSimpleName();
    private static View mCurrentView = null;

    public NetworkFragment() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        setHasOptionsMenu(true);

        mCurrentView = inflater.inflate(R.layout.fragment_network, container, false);

        registerGetNetworkTypeEvent();
        registerIsConnectedNetwork();

        return mCurrentView;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Action Bar Item Setting
    @Override
    public void onCreateOptionsMenu(Menu menu, MenuInflater inflater) {
        menu.clear();
        super.onCreateOptionsMenu(menu, inflater);

        inflater.inflate(R.menu.main, menu);

        MenuItem logoutItem = menu.findItem(R.id.action_logout);
        logoutItem.setVisible(false);
        MenuItem closeItem = menu.findItem(R.id.action_activityClose);
        closeItem.setVisible(true);
        MenuItem logOpenItem = menu.findItem(R.id.action_log);
        logOpenItem.setVisible(true);

    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        ((MainActivity)getActivity()).actionItemEvent(item);
        return super.onOptionsItemSelected(item);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void registerGetNetworkTypeEvent() {
        Button buttonNetworkType = mCurrentView.findViewById(R.id.button_networkType);
        buttonNetworkType.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                int networkType = Gamebase.Network.getType();
                String networkTypeName = Gamebase.Network.getTypeName();

                Log.d(TAG, "NetworkType : " + networkTypeName + " [" + String.valueOf(networkType) + "]");
                Gamebase.Util.showToast(getActivity(), "NetworkType : " + networkTypeName + " [" + String.valueOf(networkType) + "]", Toast.LENGTH_SHORT);
            }
        });
    }
    private void registerIsConnectedNetwork() {
        Button buttonIsConnected = mCurrentView.findViewById(R.id.button_isConnected);
        buttonIsConnected.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                boolean isConnected = Gamebase.Network.isConnected();
                Log.d(TAG, "Network isConnected : " + String.valueOf(isConnected));
                Gamebase.Util.showToast(getActivity(), "Network isConnected : " + String.valueOf(isConnected), Toast.LENGTH_SHORT);
            }
        });
    }

}
