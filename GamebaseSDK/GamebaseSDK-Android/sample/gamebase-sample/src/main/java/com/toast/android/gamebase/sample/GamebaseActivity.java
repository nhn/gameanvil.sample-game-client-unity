/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */
package com.toast.android.gamebase.sample;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;

import com.toast.android.gamebase.Gamebase;

public class GamebaseActivity extends AppCompatActivity {

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Gamebase SDK Required
    //
    ////////////////////////////////////////////////////////////////////////////////
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        Gamebase.onActivityResult(requestCode, resultCode, data);
    }
}