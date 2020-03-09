/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.ui;


import android.app.Activity;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;

import com.toast.android.gamebase.base.auth.AuthProvider;
import com.toast.android.gamebase.sample.util.Log;
import com.toast.android.gamebase.sample.GamebaseManager;
import com.toast.android.gamebase.sample.R;
import com.toast.android.gamebase.sample.util.AppUtil;

import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.List;

public class LoginFragment extends Fragment {

    private static final String TAG = LoginFragment.class.getSimpleName();
    private static String mSelectedIDP = "";
    private static View mCurrentView = null;

    public LoginFragment() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        setHasOptionsMenu(true);

        mCurrentView = inflater.inflate(R.layout.fragment_login, container, false);
        Log.d(TAG, "Create Activity");

        if (!GamebaseManager.isLoggedIn() && ((MainActivity) getActivity()).isLastProviderogin()) {
            ((MainActivity) getActivity()).setLastProviderLogin(false);
            AppUtil.showProgressDialog(getActivity(), "Login with latest logged in provider...");
            GamebaseManager.lastProviderLogin(getActivity(), new GamebaseManager.OnLoginWithLastestLoginTypeCallback() {
                @Override
                public void onCallback() {
                    AppUtil.dismissProgressDialog(getActivity());
                }
            });
        }
        setIDPSelectSpinner();
        setManualLoginButton(getActivity());

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


    private void setIDPSelectSpinner() {

        List<String> idpArr = new ArrayList<String>();
        Class tmpClass = AuthProvider.class;
        Field[] field = tmpClass.getDeclaredFields();

        boolean showAllIdPs = true;
        if (showAllIdPs) {
            for (Field f : field) {
                if (f.getType().isAssignableFrom(String.class)) {
                    String val = null;
                    try {
                        val = (String) f.get(tmpClass);
                    } catch (IllegalAccessException e) {
                        e.printStackTrace();
                    }
                    idpArr.add(val);
                }
            }
        } else {
            idpArr.add(GamebaseManager.MANUAL_LOGIN_IDP);
        }

        Spinner spinnerIDP = mCurrentView.findViewById(R.id.spinner_login_idp);
        ArrayAdapter<String> idpAdapter = new ArrayAdapter<>(getContext(), R.layout.spinner_layout, idpArr);
        idpAdapter.setDropDownViewResource(R.layout.spinner_dropdown);
        spinnerIDP.setAdapter(idpAdapter);

        spinnerIDP.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                mSelectedIDP = (String) parent.getItemAtPosition(position);
                Log.d(TAG, "Selected IDP : " + mSelectedIDP);
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });
    }

    private void setManualLoginButton(final Activity activity) {
        Button buttonLogin = mCurrentView.findViewById(R.id.button_login);
        buttonLogin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Log.d(TAG, "--------------------------------------");
                Log.d(TAG, "Clicked Login button: [" + mSelectedIDP + "]");

                GamebaseManager.loginWithIdP(activity, mSelectedIDP);
            }
        });
    }

}
