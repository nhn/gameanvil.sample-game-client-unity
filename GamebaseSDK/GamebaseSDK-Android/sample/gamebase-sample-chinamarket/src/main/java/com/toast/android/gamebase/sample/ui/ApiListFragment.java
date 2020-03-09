/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.ui;


import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AlertDialog;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.EditText;
import android.widget.ListView;

import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.GamebaseCallback;
import com.toast.android.gamebase.GamebaseDataCallback;
import com.toast.android.gamebase.GamebaseWebViewConfiguration;
import com.toast.android.gamebase.base.GamebaseException;
import com.toast.android.gamebase.sample.util.Log;
import com.toast.android.gamebase.sample.R;
import com.toast.android.gamebase.sample.customlist.CustomListViewAdapter;
import com.toast.android.gamebase.sample.customlist.CustomListViewItem;

import java.util.ArrayList;
import java.util.List;

public class ApiListFragment extends Fragment {

    private static final String TAG = ApiListFragment.class.getSimpleName();
    private static View mCurrentView = null;

    public ApiListFragment() {
        // Required empty public constructor
    }


    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        setHasOptionsMenu(true);

        mCurrentView = inflater.inflate(R.layout.fragment_api_list, container, false);

        setAPIListView();

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
        logoutItem.setVisible(true);
        MenuItem closeItem = menu.findItem(R.id.action_activityClose);
        closeItem.setVisible(false);
        MenuItem logOpenItem = menu.findItem(R.id.action_log);
        logOpenItem.setVisible(true);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        ((MainActivity) getActivity()).actionItemEvent(item);

        return super.onOptionsItemSelected(item);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////


    private void setAPIListView() {

        CustomListViewAdapter adapter = new CustomListViewAdapter();
        ListView apiListView = mCurrentView.findViewById(R.id.listview_apiList);
        apiListView.setAdapter(adapter);

        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.gamebase_sample_info), "Info", "Application Info Data");
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.gamebase_sample_mapping), "Mapping", "IDP Mapping ");
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.gamebase_sample_purchase), "Purchase", "Purchase Test");
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.gamebase_sample_push), "Push", "Push Setting");
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.gamebase_sample_webview), "WebView", "WebView Test");
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.gamebase_sample_util), "Util", "Toast, Alert Message");
        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.gamebase_sample_network), "Network", "Network Test");

        apiListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {

            @Override
            public void onItemClick(AdapterView parent, View v, int position, long id) {

                CustomListViewItem item = (CustomListViewItem) parent.getItemAtPosition(position);
                String titleStr = item.getTitle();
                Log.d(TAG, "Selected ListViewItem: " + titleStr);

                Intent intent;

                switch (titleStr) {
                    case "Info":
                        ((MainActivity) getActivity()).loadLaunchingInfoFragment();
                        break;

                    case "Mapping":
                        ((MainActivity) getActivity()).loadMappingFragment();
                        break;

                    case "Purchase":
                        ((MainActivity) getActivity()).loadPurchaseFragment();
                        break;

                    case "Push":
                        ((MainActivity) getActivity()).loadPushFragment();
                        break;

                    case "WebView":
                        showWebView();
                        break;

                    case "Util":
                        ((MainActivity) getActivity()).loadUtilFragment();
                        break;

                    case "Network":
                        ((MainActivity) getActivity()).loadNetworkFragment();
                        break;
                }
            }
        });
    }

    private void showWebView() {
        AlertDialog.Builder inputDialog = new AlertDialog.Builder(getActivity());
        inputDialog.setTitle("Show Browser");
        inputDialog.setMessage("Input Open URL");

        View layout = getActivity().getLayoutInflater().inflate(R.layout.url_input_dialog, (ViewGroup) getActivity().findViewById(R.id.popup_root));
        inputDialog.setView(layout);

        final EditText input = layout.findViewById(R.id.editText_inputUrl);
        input.setText("https://baidu.com"); // setting default url

        inputDialog.setPositiveButton("Go", new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int whichButton) {
                String url = input.getText().toString();
                List<String> schmeList = new ArrayList<>();
                schmeList.add(url);
                schmeList.add("customscheme://myscheme");
                Gamebase.WebView.showWebView(getActivity(), url,
                        new GamebaseWebViewConfiguration.Builder().build(),
                        new GamebaseCallback() {
                            @Override
                            public void onCallback(GamebaseException exception) {
                                if (!Gamebase.isSuccess(exception)) {
                                    Log.w(TAG, "showWebView exception" + exception.toString());
                                }
                                Log.d(TAG, "showWebView onCallback");
                            }
                        }, schmeList,
                        new GamebaseDataCallback<String>() {
                            @Override
                            public void onCallback(String data, GamebaseException exception) {
                                Log.d(TAG, "onSchemeEvent : " + data);
                            }
                        }
                );
            }
        });

        inputDialog.setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int whichButton) {
                // Canceled
                return;
            }
        });
        inputDialog.show();
    }
}
