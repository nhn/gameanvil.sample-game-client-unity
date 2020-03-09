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
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.SimpleAdapter;

import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.GamebaseSystemInfo;
import com.toast.android.gamebase.sample.util.Log;
import com.toast.android.gamebase.sample.R;
import com.toast.android.gamebase.sample.util.AppUtil;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class LaunchingInfoFragment extends Fragment {

    private static final String TAG = LaunchingInfoFragment.class.getSimpleName();
    private static View mCurrentView = null;

    public LaunchingInfoFragment() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        setHasOptionsMenu(true);

        mCurrentView = inflater.inflate(R.layout.fragment_launching_info, container, false);

        setLaunchingInfoListView();

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


    private void setLaunchingInfoListView() {
        ListView listView = mCurrentView.findViewById(R.id.listview_launchingInfo);
        ArrayList<HashMap<String, String>> itemList = new ArrayList<HashMap<String, String>>();

        settingData(itemList);
        Log.d(TAG, itemList.toString());

        SimpleAdapter adapter = new SimpleAdapter(getActivity(),
                itemList,
                android.R.layout.simple_list_item_2,
                new String[]{"title", "desc"},
                new int[]{android.R.id.text1, android.R.id.text2});

        listView.setAdapter(adapter);
        listView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView parent, View v, int position, long id) {
                @SuppressWarnings("unchecked")
                HashMap<String , String > data = (HashMap<String , String> ) parent.getItemAtPosition(position);
                String desc = data.get("desc");
                AppUtil.copyClipBoard(getActivity(), desc);
            }
        }) ;
    }

    private void addItem(ArrayList<HashMap<String, String>> list, String title, String desc) {

        HashMap<String , String> item = new HashMap<String, String>();
        item.put("title", title);
        if(desc == null || desc.equals("")) {
            item.put("desc", "-");
        }else {
            item.put("desc", desc);
        }
        list.add(item);
    }

    private void settingData(ArrayList<HashMap<String, String>> list) {
        list.clear();
        addItem(list, "LaunchingStatus", String.valueOf(Gamebase.Launching.getLaunchingStatus()));
        addItem(list, "ServiceZone", GamebaseSystemInfo.getInstance().getZoneType());
        addItem(list, "SDKVersion", Gamebase.getSDKVersion());
        addItem(list, "AppID", GamebaseSystemInfo.getInstance().getAppId());
        addItem(list, "AppName", GamebaseSystemInfo.getInstance().getAppName());
        addItem(list, "AppVersion", GamebaseSystemInfo.getInstance().getAppVersion());
        addItem(list, "UserID", Gamebase.getUserID());
        addItem(list, "AccessToken", Gamebase.getAccessToken());
        addItem(list, "DeviceLanguage", Gamebase.getDeviceLanguageCode());
        addItem(list, "DisplayLanguage", Gamebase.getDisplayLanguageCode());
        addItem(list, "CarrierCode", Gamebase.getCarrierCode());
        addItem(list, "CarrierName", Gamebase.getCarrierName());
        addItem(list, "CountryCode", Gamebase.getCountryCode());
        addItem(list, "CountryCode(USIM)", Gamebase.getCountryCodeOfUSIM());
        addItem(list, "CountryCode(DEVICE)", Gamebase.getCountryCodeOfDevice());
        addItem(list, "LastLoggedinProvider", Gamebase.getLastLoggedInProvider());

        //Auth List
        List<String> authList = Gamebase.getAuthMappingList();

        if(authList != null) {
            if(authList.size() == 0) {
                addItem(list, "AuthList", "-");
            }else  {
                for(int i=0; i<authList.size(); i++) {
                    addItem(list, "AuthList - " +  String.valueOf(i), authList.get(i));
                }
            }
        }
    }

}
