/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.customlist;

import android.graphics.drawable.Drawable;

public class CustomListViewItem {
    private Drawable mIconDrawable;
    private String mTitleString;
    private String mDescriptionString;

    public void setIcon(Drawable icon) {
        mIconDrawable = icon;
    }

    public void setTitle(String title) {
        mTitleString = title;
    }

    public void setDesc(String desc) {
        mDescriptionString = desc;
    }

    public Drawable getIcon() {
        return this.mIconDrawable;
    }

    public String getTitle() {
        return this.mTitleString;
    }

    public String getDesc() {
        return this.mDescriptionString;
    }

}
