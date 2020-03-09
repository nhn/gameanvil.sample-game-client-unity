/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.ui;


import android.content.DialogInterface;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RadioButton;
import android.widget.Toast;

import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.base.ui.SimpleAlertDialog;
import com.toast.android.gamebase.base.ui.SimpleMultiChoiceDialog;
import com.toast.android.gamebase.base.ui.SimpleSelectItemDialog;
import com.toast.android.gamebase.sample.R;

import java.util.ArrayList;
import java.util.List;

public class UtilFragment extends Fragment {

    private static final String TAG = UtilFragment.class.getSimpleName();
    private static View mCurrentView = null;

    public UtilFragment() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        setHasOptionsMenu(true);

        mCurrentView = inflater.inflate(R.layout.fragment_util, container, false);

        setToastMessage();
        setAlertDialog();

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

    private void setToastMessage() {

        final EditText editTextToastMessage = mCurrentView.findViewById(R.id.editText_ToastMessage);
        final RadioButton radioButtonShort = mCurrentView.findViewById(R.id.radioButton_short);
        final RadioButton radioButtonLong = mCurrentView.findViewById(R.id.radioButton_long);
        Button buttonShowToast = mCurrentView.findViewById(R.id.button_showToast);

        radioButtonShort.setChecked(true);
        editTextToastMessage.setText("Test Toast Message ! ");

        buttonShowToast.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(radioButtonLong.isChecked()) {
                    Gamebase.Util.showToast(getActivity(), editTextToastMessage.getText().toString(), Toast.LENGTH_LONG);
                }else {
                    Gamebase.Util.showToast(getActivity(), editTextToastMessage.getText().toString(), Toast.LENGTH_SHORT);
                }
            }
        });
    }

    private void setAlertDialog() {
        final EditText editTextAlertDialogTitle = mCurrentView.findViewById(R.id.editText_alertTItle);
        final EditText editTextAlertDialogMessage = mCurrentView.findViewById(R.id.editText_alertMessage);

        final RadioButton radioButtonDialog = mCurrentView.findViewById(R.id.radioButton_dialog);
        final RadioButton radioButtonSelectorDialog = mCurrentView.findViewById(R.id.radioButton_selector);
        final RadioButton radioButtonChoiceItemDialog = mCurrentView.findViewById(R.id.radioButton_choiceItem);

        radioButtonDialog.setChecked(true);
        editTextAlertDialogTitle.setText("Test Alert Dialog Title !");
        editTextAlertDialogMessage.setText("Test Alert Dialog Message !");

        Button buttonShowAlertDialog = mCurrentView.findViewById(R.id.button_showAlertDialog);

        buttonShowAlertDialog.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                DialogInterface.OnClickListener positiveButtonEventListener = new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        Gamebase.Util.showToast(getActivity(), "Positive Button", Toast.LENGTH_SHORT);
                    }
                };

                DialogInterface.OnClickListener negativeButtonEventListener = new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        Gamebase.Util.showToast(getActivity(), "Negative Button", Toast.LENGTH_SHORT);
                    }
                };

                DialogInterface.OnCancelListener backkeyEventListener = new DialogInterface.OnCancelListener() {
                    @Override
                    public void onCancel(DialogInterface dialog) {
                        Gamebase.Util.showToast(getActivity(), "Back Key", Toast.LENGTH_SHORT);
                    }
                };

                DialogInterface.OnClickListener selectorEventListener = new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        Gamebase.Util.showToast(getActivity(), "Selector : " + String.valueOf(which), Toast.LENGTH_SHORT);
                    }
                };

                DialogInterface.OnMultiChoiceClickListener multiChoiceClickListener = new DialogInterface.OnMultiChoiceClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which, boolean isChecked) {
                        Gamebase.Util.showToast(getActivity(), String.valueOf(which) + " : " + String.valueOf(isChecked), Toast.LENGTH_SHORT);
                    }
                };

                List<String> items = new ArrayList<String>();
                items.add("item 01");
                items.add("item 02");
                items.add("item 03");

                if(radioButtonDialog.isChecked()) {

                    SimpleAlertDialog.show(getActivity(),
                            editTextAlertDialogTitle.getText().toString(),
                            editTextAlertDialogMessage.getText().toString(),
                            "Positive Button", positiveButtonEventListener,
                            "Negative Button", negativeButtonEventListener,
                            backkeyEventListener, true);

                }else if(radioButtonSelectorDialog.isChecked()) {

                    SimpleSelectItemDialog.show(getActivity(),
                            editTextAlertDialogTitle.getText().toString(),
                            items, selectorEventListener,
                            null, null,
                            backkeyEventListener, true);

                }else if(radioButtonChoiceItemDialog.isChecked()) {

                    SimpleMultiChoiceDialog.show(getActivity(),
                            editTextAlertDialogTitle.getText().toString(),
                            items, multiChoiceClickListener,
                            "Positive Button", positiveButtonEventListener,
                            "Negative Button", negativeButtonEventListener,
                            backkeyEventListener, true);

                }else {
                    Gamebase.Util.showToast(getActivity(), "Unknown Select AlertDialog", Toast.LENGTH_SHORT);
                }
            }
        });
    }
}
