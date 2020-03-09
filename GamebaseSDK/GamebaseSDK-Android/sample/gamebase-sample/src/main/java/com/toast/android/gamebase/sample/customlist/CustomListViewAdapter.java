/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.customlist;

import android.content.Context;
import android.graphics.drawable.Drawable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.toast.android.gamebase.sample.R;

import java.util.ArrayList;

public class CustomListViewAdapter extends BaseAdapter {

    private ArrayList<CustomListViewItem> mListViewItemList = new ArrayList<CustomListViewItem>();
    public CustomListViewAdapter() {   }

    @Override
    public int getCount() {
        return mListViewItemList.size();
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        final int pos = position;
        final Context context = parent.getContext();

        if (convertView == null) {
            LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            convertView = inflater.inflate(R.layout.listview_item, parent, false);
        }

        ImageView iconImageView = convertView.findViewById(R.id.listItem_imageview_icon) ;
        TextView titleTextView = convertView.findViewById(R.id.listItem_textview_title) ;
        TextView descTextView = convertView.findViewById(R.id.listItem_textview_desc) ;

        CustomListViewItem listViewItem = mListViewItemList.get(position);

        iconImageView.setImageDrawable(listViewItem.getIcon());
        titleTextView.setText(listViewItem.getTitle());
        descTextView.setText(listViewItem.getDesc());

        return convertView;
    }

    @Override
    public long getItemId(int position) {
        return position ;
    }

    @Override
    public Object getItem(int position) {
        return mListViewItemList.get(position) ;
    }

    public void addItem(Drawable icon, String title, String desc) {
        CustomListViewItem item = new CustomListViewItem();

        item.setIcon(icon);
        item.setTitle(title);
        item.setDesc(desc);

        mListViewItemList.add(item);
    }
}
