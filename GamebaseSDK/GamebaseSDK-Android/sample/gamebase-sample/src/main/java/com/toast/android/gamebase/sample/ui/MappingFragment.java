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
import android.widget.Switch;
import android.widget.TextView;

import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.base.auth.AuthProvider;
import com.toast.android.gamebase.sample.util.Log;
import com.toast.android.gamebase.sample.GamebaseManager;
import com.toast.android.gamebase.sample.R;

import java.util.List;

public class MappingFragment extends Fragment {

    private static final String TAG = MappingFragment.class.getSimpleName();
    private static View mCurrentView = null;
    private static String mUserID = "";

    public MappingFragment() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        setHasOptionsMenu(true);

        mCurrentView = inflater.inflate(R.layout.fragment_mapping, container, false);

        mUserID = Gamebase.getUserID();
        Log.d(TAG, "Create Activity");

        TextView textView_UserID = mCurrentView.findViewById(R.id.textView_userID);
        textView_UserID.setText("User ID : " + mUserID);

        setAuthListUI();
        setLogoutButton();
        setWithdrawButton();

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

    void setAuthListUI() {

        List<String> authList = Gamebase.getAuthMappingList();
        Log.d(TAG, authList.toString());

        final Switch switchGuest = mCurrentView.findViewById(R.id.switch_guest);
        final Switch switchGoogle = mCurrentView.findViewById(R.id.switch_google);
        final Switch switchFacebook = mCurrentView.findViewById(R.id.switch_facebook);
        final Switch switchPayco = mCurrentView.findViewById(R.id.switch_payco);
        final Switch switchNaver = mCurrentView.findViewById(R.id.switch_naver);
        final Switch switchLine = mCurrentView.findViewById(R.id.switch_line);

        switchGuest.setChecked(false);
        switchGoogle.setChecked(false);
        switchFacebook.setChecked(false);
        switchPayco.setChecked(false);
        switchNaver.setChecked(false);
        switchLine.setChecked(false);

        if (authList != null) {
            for (int i = 0; i < authList.size(); i++) {
                String authItem = authList.get(i);
                if (authList.get(i) == null) {
                    continue;
                } else {
                    authItem = authItem.toLowerCase();

                    switch (authItem) {
                        case AuthProvider.GUEST:
                            switchGuest.setChecked(true);
                            break;
                        case AuthProvider.GOOGLE:
                            switchGoogle.setChecked(true);
                            break;
                        case AuthProvider.FACEBOOK:
                            switchFacebook.setChecked(true);
                            break;
                        case AuthProvider.PAYCO:
                            switchPayco.setChecked(true);
                            break;
                        case AuthProvider.NAVER:
                            switchNaver.setChecked(true);
                            break;
                        case AuthProvider.LINE:
                            switchLine.setChecked(true);
                            break;
                    }
                }
            }
        }

        switchGuest.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (switchGuest.isChecked()) {
                    GameCodeManager.addMappingAndChangeAccountUI(getActivity(), AuthProvider.GUEST, switchGuest);
                } else {
                    GameCodeManager.removeMappingAndChangeAccountUI(getActivity(), AuthProvider.GUEST, switchGuest);
                }
            }
        });

        switchGoogle.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (switchGoogle.isChecked()) {
                    GameCodeManager.addMappingAndChangeAccountUI(getActivity(), AuthProvider.GOOGLE, switchGoogle);
                } else {
                    GameCodeManager.removeMappingAndChangeAccountUI(getActivity(), AuthProvider.GOOGLE, switchGoogle);
                }
            }
        });

        switchFacebook.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (switchFacebook.isChecked()) {
                    GameCodeManager.addMappingAndChangeAccountUI(getActivity(), AuthProvider.FACEBOOK, switchFacebook);
                } else {
                    GameCodeManager.removeMappingAndChangeAccountUI(getActivity(), AuthProvider.FACEBOOK, switchFacebook);
                }
            }
        });

        switchPayco.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (switchPayco.isChecked()) {
                    GameCodeManager.addMappingAndChangeAccountUI(getActivity(), AuthProvider.PAYCO, switchPayco);
                } else {
                    GameCodeManager.removeMappingAndChangeAccountUI(getActivity(), AuthProvider.PAYCO, switchPayco);
                }
            }
        });

        switchNaver.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (switchNaver.isChecked()) {
                    GameCodeManager.addMappingAndChangeAccountUI(getActivity(), AuthProvider.NAVER, switchNaver);
                } else {
                    GameCodeManager.removeMappingAndChangeAccountUI(getActivity(), AuthProvider.NAVER, switchNaver);
                }
            }
        });

        switchLine.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (switchLine.isChecked()) {
                    GameCodeManager.addMappingAndChangeAccountUI(getActivity(), AuthProvider.LINE, switchLine);
                } else {
                    GameCodeManager.removeMappingAndChangeAccountUI(getActivity(), AuthProvider.LINE, switchLine);
                }
            }
        });
    }

    private void setLogoutButton() {
        Button btnLogout = mCurrentView.findViewById(R.id.button_logout);
        btnLogout.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                GamebaseManager.logout(getActivity());
            }
        });
    }

    private void setWithdrawButton() {
        Button btnWithdraw = mCurrentView.findViewById(R.id.button_withdraw);
        btnWithdraw.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                GamebaseManager.withdraw(getActivity());
            }
        });
    }

}
