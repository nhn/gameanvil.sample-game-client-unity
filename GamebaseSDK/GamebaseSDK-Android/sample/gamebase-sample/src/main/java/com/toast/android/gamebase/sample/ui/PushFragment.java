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
import android.widget.CompoundButton;
import android.widget.Switch;
import android.widget.Toast;

import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.GamebaseCallback;
import com.toast.android.gamebase.GamebaseDataCallback;
import com.toast.android.gamebase.base.GamebaseException;
import com.toast.android.gamebase.sample.util.Log;
import com.toast.android.gamebase.base.push.PushConfiguration;
import com.toast.android.gamebase.sample.GamebaseManager;
import com.toast.android.gamebase.sample.R;
import com.toast.android.gamebase.sample.util.AppUtil;


public class PushFragment extends Fragment {

    private static final String TAG = PushFragment.class.getSimpleName();
    private static View mCurrentView = null;

    private static Switch mSwitchAdvertisement = null;
    private static Switch mSwitchAdvertisementNight = null;
    private static Switch mSwitchNotification = null;

    private static boolean mAgreeAdvertisement = false;
    private static boolean mAgreeAdvertisementNight = false;
    private static boolean mEnableNotification = false;

    public PushFragment() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        setHasOptionsMenu(true);

        mCurrentView = inflater.inflate(R.layout.fragment_push, container, false);

        getPushStatus();
        setPushAgreement();

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
        ((MainActivity) getActivity()).actionItemEvent(item);
        return super.onOptionsItemSelected(item);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void setPushAgreement() {
        mSwitchAdvertisement = mCurrentView.findViewById(R.id.switch_adv);
        mSwitchAdvertisementNight = mCurrentView.findViewById(R.id.switch_adv_Night);
        mSwitchNotification = mCurrentView.findViewById(R.id.switch_notification);

        mSwitchAdvertisement.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                mAgreeAdvertisement = mSwitchAdvertisement.isChecked();
                registerPush(mEnableNotification, mAgreeAdvertisement, mAgreeAdvertisementNight, mSwitchAdvertisement);
            }
        });

        mSwitchAdvertisementNight.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                mAgreeAdvertisementNight = mSwitchAdvertisementNight.isChecked();
                registerPush(mEnableNotification, mAgreeAdvertisement, mAgreeAdvertisementNight, mSwitchAdvertisementNight);
            }
        });

        mSwitchNotification.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                mEnableNotification = mSwitchNotification.isChecked();
                registerPush(mEnableNotification, mAgreeAdvertisement, mAgreeAdvertisementNight, mSwitchNotification);
            }
        });

        mSwitchNotification.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                if(isChecked) {
                    mSwitchAdvertisement.setEnabled(true);
                }else {
                    mSwitchAdvertisement.setEnabled(false);
                    mSwitchAdvertisementNight.setEnabled(false);
                }
            }
        });

        mSwitchAdvertisement.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                if(isChecked) {
                    mSwitchAdvertisementNight.setEnabled(true);
                }else {
                    mSwitchAdvertisementNight.setEnabled(false);
                }



            }
        });
    }

    private void getPushStatus() {

        AppUtil.showProgressDialog(getActivity(), "Get Push Agreement Status ...");

        GamebaseManager.queryPush(getActivity(), new GamebaseDataCallback<PushConfiguration>() {
            @Override
            public void onCallback(PushConfiguration data, GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    mAgreeAdvertisement = data.adAgreement;
                    mAgreeAdvertisementNight = data.adAgreementNight;
                    mEnableNotification = data.pushEnabled;

                    mSwitchAdvertisement.setChecked(mAgreeAdvertisement);
                    mSwitchAdvertisementNight.setChecked(mAgreeAdvertisementNight);
                    mSwitchNotification.setChecked(mEnableNotification);

                    if(mEnableNotification == false) {
                        mSwitchAdvertisement.setEnabled(false);
                        mSwitchAdvertisementNight.setEnabled(false);
                    }else if(mAgreeAdvertisement == false) {
                        mSwitchAdvertisementNight.setEnabled(false);
                    }

                    AppUtil.dismissProgressDialog(getActivity());

                    Log.d(TAG, "Agree Advertisement : " + String.valueOf(mAgreeAdvertisement));
                    Log.d(TAG, "Agree Advertisement Night : " + String.valueOf(mAgreeAdvertisementNight));
                    Log.d(TAG, "Enable Notification : " + String.valueOf(mEnableNotification));

                    Gamebase.Util.showToast(getActivity(), "Notification Enable : " + String.valueOf(mEnableNotification) +
                                                       "\nAdvertisement Agreement : " + String.valueOf(mAgreeAdvertisement) +
                                                       "\nNight Advertisement Agreement : " + String.valueOf(mAgreeAdvertisementNight),
                                        Toast.LENGTH_SHORT);
                } else {

                    Gamebase.Util.showToast(getActivity(), "Failed Get AdvertisementAgreement ", Toast.LENGTH_SHORT);
                    Gamebase.Util.showAlert(getActivity(), "Error", exception.toJsonString());
                    AppUtil.dismissProgressDialog(getActivity());

                    Log.d(TAG, "Failed Get AdvertisementAgreement ");
                    Log.d(TAG, "exception Code : " + String.valueOf(exception.getCode()));
                    Log.d(TAG, "exception Message : " + exception.getMessage());
                    Log.d(TAG, "exception Domain : " + exception.getDomain());
                    Log.d(TAG, "exception Detail Code : " + String.valueOf(exception.getDetailCode()));
                    Log.d(TAG, "exception Detail Message: " + exception.getDetailMessage());
                    Log.d(TAG, "exception Detail Domain: " + exception.getDetailDomain());

                }
            }
        });
    }

    private void registerPush(final boolean enable, final boolean agree, final boolean agreeNight, final Switch currentSwitch) {

        AppUtil.showProgressDialog(getActivity(), "Register push ...");

        GamebaseManager.registerPush(getActivity(), enable, agree, agreeNight, new GamebaseCallback() {
            @Override
            public void onCallback(GamebaseException exception) {
                if (Gamebase.isSuccess(exception)) {
                    Gamebase.Util.showToast(getActivity(), "Success Register Push", Toast.LENGTH_SHORT);
                    AppUtil.dismissProgressDialog(getActivity());

                    getPushStatus();
                } else {
                    currentSwitch.setChecked(false);
                    Gamebase.Util.showAlert(getActivity(), "Error", exception.toJsonString());
                    String errorMessage = "Failed Register Push - Notification Enable : " + String.valueOf(enable) + " | Advertisement Agreement : " + String.valueOf(agree) + " | Night Advertisement Agreement : " + String.valueOf(agreeNight);
                    Gamebase.Util.showToast(getActivity(), errorMessage, Toast.LENGTH_SHORT);
                    AppUtil.dismissProgressDialog(getActivity());

                    Log.d(TAG, errorMessage);
                    Log.d(TAG, "exception Code : " + String.valueOf(exception.getCode()));
                    Log.d(TAG, "exception Message : " + exception.getMessage());
                    Log.d(TAG, "exception Domain : " + exception.getDomain());
                    Log.d(TAG, "exception Detail Code : " + String.valueOf(exception.getDetailCode()));
                    Log.d(TAG, "exception Detail Message: " + exception.getDetailMessage());
                    Log.d(TAG, "exception Detail Domain: " + exception.getDetailDomain());
                }
            }
        });
    }

}
