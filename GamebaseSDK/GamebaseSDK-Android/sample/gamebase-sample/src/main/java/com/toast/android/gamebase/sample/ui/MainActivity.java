/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.ui;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.view.Menu;
import android.view.MenuItem;
import android.view.WindowManager;
import android.widget.TextView;
import android.widget.Toast;

import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.sample.util.Log;
import com.toast.android.gamebase.launching.data.LaunchingStatus;
import com.toast.android.gamebase.observer.Observer;
import com.toast.android.gamebase.observer.ObserverMessage;
import com.toast.android.gamebase.sample.GamebaseActivity;
import com.toast.android.gamebase.sample.GamebaseManager;
import com.toast.android.gamebase.sample.R;

import java.util.Map;

public class MainActivity extends GamebaseActivity {

    private static final String TAG = MainActivity.class.getSimpleName();
    private static boolean mLastProviderLoginValue = true;
    private static FragmentManager mFragmentManager = null;

    @Override
    @SuppressWarnings("deprecation")
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // Allows the app to be exposed in the front even when locked.
        // Note that the Toast message may not work.
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_SHOW_WHEN_LOCKED);

        setLaunchingStatusTextView();
        setLaunchingStatusChangeListener();
        setNetworkChangeListener();

        mFragmentManager = getSupportFragmentManager();

        if (savedInstanceState != null) {
            String saveData = savedInstanceState.getString("LastFragment", "empty");

            switch (saveData) {
                case "login":
                    loadLoginFragment();
                    return;
                case "apilist":
                    loadApiListFragment();
                    return;
                case "launchinginfo":
                    loadLaunchingInfoFragment();
                    return;
                case "mapping":
                    loadMappingFragment();
                    return;
                case "purchase":
                    loadPurchaseFragment();
                    return;
                case "push":
                    loadPushFragment();
                    return;
                case "util":
                    loadUtilFragment();
                    return;
                case "network":
                    loadNetworkFragment();
                    return;
                case "log":
                    loadLogFragment();
                    return;
                default:
                    break;
            }
        } else {
            FragmentManager fragmentManager = getSupportFragmentManager();
            FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
            fragmentTransaction.replace(R.id.contentFrame, new LoginFragment(), "login");
            fragmentTransaction.commit();
        }
    }

    @Override
    public void onSaveInstanceState(Bundle savedInstanceState) {
        super.onSaveInstanceState(savedInstanceState);

        Fragment fragment = getSupportFragmentManager().findFragmentById(R.id.contentFrame);
        String tag = fragment.getTag();

        savedInstanceState.putString("LastFragment", tag);
    }

    @Override
    public void onBackPressed() {

        Fragment fragment = getSupportFragmentManager().findFragmentById(R.id.contentFrame);
        String tag = fragment.getTag();

        if (tag.equals("apilist")) {

        } else if (tag.equals("log")) {
            mFragmentManager.popBackStack();

        } else {
            super.onBackPressed();
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Action Bar Item Setting
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.main, menu);

        MenuItem logoutItem = menu.findItem(R.id.action_logout);
        logoutItem.setVisible(false);
        MenuItem closeItem = menu.findItem(R.id.action_activityClose);
        closeItem.setVisible(false);
        MenuItem logOpenItem = menu.findItem(R.id.action_log);
        logOpenItem.setVisible(false);

        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        //actionItemEvent(item, MainActivity.this);
        return super.onOptionsItemSelected(item);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void setLaunchingStatusTextView() {
        TextView textViewStatus = findViewById(R.id.textView_launchingStatus);
        textViewStatus.setText(getStringLaunchingStatus(Gamebase.Launching.getLaunchingStatus()));
    }


    public void setLastProviderLogin(boolean yn) {
        mLastProviderLoginValue = yn;
    }

    public boolean isLastProviderogin() {
        return mLastProviderLoginValue;
    }

    public String getStringLaunchingStatus(int code) {

        String returnData = "Unknown";

        switch (code) {
            case LaunchingStatus.IN_SERVICE:
                returnData = "In Service";
                break;
            case LaunchingStatus.IN_SERVICE_BY_QA_WHITE_LIST:
                returnData = "In Service QA";
                break;
            case LaunchingStatus.TERMINATED_SERVICE:
                returnData = "Closed Service";
                break;
            case LaunchingStatus.INSPECTING_SERVICE:
            case LaunchingStatus.INSPECTING_ALL_SERVICES:
                returnData = "Maintenance";
                break;
            case LaunchingStatus.RECOMMEND_UPDATE:
                returnData = "Upgrade Recommended";
                break;
            case LaunchingStatus.REQUIRE_UPDATE:
                returnData = "Upgrade Required";
                break;
            case LaunchingStatus.BLOCKED_USER:
                returnData = "Blocked BlackList";
                break;
            case LaunchingStatus.INTERNAL_SERVER_ERROR:
                returnData = "Internal Server Error";
                break;
            default:
                returnData = "Unknown";
                break;
        }
        return returnData;
    }

    private void setLaunchingStatusChangeListener() {
        Gamebase.addObserver(new Observer() {
            @Override
            public void onUpdate(ObserverMessage message) {
                if (message.type.equalsIgnoreCase(ObserverMessage.Type.LAUNCHING)) {
                    Map<String, Object> dataMap = message.data;

                    int code = (int) dataMap.get("code");
                    String messageString = (String) dataMap.get("message");

                    TextView textViewLaunchingStatus = findViewById(R.id.textView_launchingStatus);

                    textViewLaunchingStatus.setText(getStringLaunchingStatus(code));

                    Log.d(TAG, "Launching Update Success - Code : " + code);
                    Log.d(TAG, "Launching Update Message : " + messageString);
                    Gamebase.Util.showAlert(MainActivity.this, "Launching", messageString);
                }
            }
        });
    }

    private void setNetworkChangeListener() {
        final TextView textViewNetworkType = findViewById(R.id.textView_networkType);
        int networkType = Gamebase.Network.getType();
        String networkTypeName = Gamebase.Network.getTypeName();
        textViewNetworkType.setText(networkTypeName + " [" + networkType + "]");

        Gamebase.addObserver(new Observer() {
            @Override
            public void onUpdate(ObserverMessage message) {
                if (message.type.equalsIgnoreCase(ObserverMessage.Type.NETWORK)) {
                    // -1 = Type Not
                    //  0 = Mobile
                    //  1 = Wifi
                    //  2 = any

                    String messageString = "";
                    int type = (int) message.data.get("type");
                    switch (type) {
                        case -1:
                            messageString = "Type Not [" + String.valueOf(type) + "]";
                            break;
                        case 0:
                            messageString = "Mobile [" + String.valueOf(type) + "]";
                            break;
                        case 1:
                            messageString = "Wifi [" + String.valueOf(type) + "]";
                            break;
                        case 2:
                            messageString = "Any [" + String.valueOf(type) + "]";
                            break;
                        default:
                            messageString = "Unknown [" + String.valueOf(type) + "]";
                            break;
                    }
                    textViewNetworkType.setText(messageString);
                    Log.d(TAG, messageString);
                    Gamebase.Util.showToast(MainActivity.this, "Changed Network Type : " + message, Toast.LENGTH_SHORT);
                }
            }
        });
    }

    public void actionItemEvent(MenuItem item) {
        int id = item.getItemId();
        if (id == R.id.action_log) {
            Gamebase.Util.showToast(MainActivity.this, "Action Bar item Click - Logger ", Toast.LENGTH_SHORT);
            Log.d(TAG, "Action Bar item Click - Logger ");
            //MainActivity.this.startActivity(new Intent(MainActivity.this, LogActivity.class));
            loadLogFragment();

        } else if (id == R.id.action_logout) {
            Gamebase.Util.showToast(MainActivity.this, "Action Bar item Test - Logout", Toast.LENGTH_SHORT);
            Log.d(TAG, "Action Bar item Test - Logout");

            GamebaseManager.logout(MainActivity.this);
        } else if (id == R.id.action_activityClose) {
            Log.d(TAG, "Action Bar item Test - Close ");
            Gamebase.Util.showToast(MainActivity.this, "Action Bar item Test - Close ", Toast.LENGTH_SHORT);

            Log.d(TAG, "getBackStackEntryCount : " + String.valueOf(mFragmentManager.getBackStackEntryCount()));

            mFragmentManager.popBackStack();

        }
    }


    public void loadLoginFragment() {

        mFragmentManager.beginTransaction()
                .replace(R.id.contentFrame, new LoginFragment(), "login")
                .commit();
    }

    public void loadApiListFragment() {

        mFragmentManager.beginTransaction()
                .replace(R.id.contentFrame, new ApiListFragment(), "apilist")
                .addToBackStack("apilist")
                .commit();

    }


    public void loadLaunchingInfoFragment() {

        mFragmentManager.beginTransaction()
                .replace(R.id.contentFrame, new LaunchingInfoFragment(), "launchinginfo")
                .addToBackStack("launchinginfo")
                .commit();

    }

    public void loadMappingFragment() {

        mFragmentManager.beginTransaction()
                .replace(R.id.contentFrame, new MappingFragment(), "mapping")
                .addToBackStack("mapping")
                .commit();

    }

    public void loadPurchaseFragment() {

        mFragmentManager.beginTransaction()
                .replace(R.id.contentFrame, new PurchaseFragment(), "purchase")
                .addToBackStack("purchase")
                .commit();

    }

    public void loadPushFragment() {

        mFragmentManager.beginTransaction()
                .replace(R.id.contentFrame, new PushFragment(), "push")
                .addToBackStack("push")
                .commit();

    }

    public void loadUtilFragment() {

        mFragmentManager.beginTransaction()
                .replace(R.id.contentFrame, new UtilFragment(), "util")
                .addToBackStack("util")
                .commit();

    }

    public void loadNetworkFragment() {

        mFragmentManager.beginTransaction()
                .replace(R.id.contentFrame, new NetworkFragment(), "network")
                .addToBackStack("network")
                .commit();

    }

    public void loadLogFragment() {

        mFragmentManager.beginTransaction()
                .replace(R.id.contentFrame, new LogFragment(), "log")
                .addToBackStack("log")
                .commit();

    }

}
