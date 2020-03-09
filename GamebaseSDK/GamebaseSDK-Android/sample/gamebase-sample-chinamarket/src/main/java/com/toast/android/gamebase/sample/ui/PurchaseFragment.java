/*
 * © NHN Corp. All rights reserved.
 * NHN Corp. PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.toast.android.gamebase.sample.ui;


import android.app.Activity;
import android.content.DialogInterface;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v4.app.Fragment;
import android.support.v4.content.ContextCompat;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Toast;

import com.google.gson.Gson;
import com.toast.android.gamebase.Gamebase;
import com.toast.android.gamebase.GamebaseDataCallback;
import com.toast.android.gamebase.GamebaseSystemInfo;
import com.toast.android.gamebase.base.GamebaseException;
import com.toast.android.gamebase.sample.util.Log;
import com.toast.android.gamebase.base.purchase.PurchasableItem;
import com.toast.android.gamebase.base.purchase.PurchasableReceipt;
import com.toast.android.gamebase.base.ui.SimpleAlertDialog;
import com.toast.android.gamebase.base.ui.SimpleSelectItemDialog;
import com.toast.android.gamebase.sample.GamebaseManager;
import com.toast.android.gamebase.sample.R;
import com.toast.android.gamebase.sample.customlist.CustomListViewAdapter;
import com.toast.android.gamebase.sample.util.AppUtil;

import java.io.BufferedInputStream;
import java.io.Closeable;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

import static com.toast.android.gamebase.launching.data.LaunchingInfo.TCPRODUCT_TYPE_IAP;

public class PurchaseFragment extends Fragment {

    private static final String TAG = PurchaseFragment.class.getSimpleName();
    private static View mCurrentView = null;
    private static List<PurchasableItem> mPurchasableItemList = new ArrayList<>();

    private static Gson gson = new Gson();
    private static final String ZONE_TYPE_REAL = "real";

    public PurchaseFragment() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        setHasOptionsMenu(true);

        mCurrentView = inflater.inflate(R.layout.fragment_purchase, container, false);

        itemList();
        itemListOfNotConsumed();
        getSubscriptions();
        setPurchasableList();

        return mCurrentView;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Action Bar Item Setting
    @Override
    public void  onCreateOptionsMenu(Menu menu, MenuInflater inflater) {
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


    private void setPurchasableList() {

        AppUtil.showProgressDialog(getActivity(), "get Purchasable Item List...");
        //AppUtil.dismissProgressDialog(PurchaseActivity.this);

        final ListView listViewPurchasable = mCurrentView.findViewById(R.id.listview_purchase);
        final CustomListViewAdapter adapter = new CustomListViewAdapter();

        GamebaseManager.requestItemList(getActivity(), new GamebaseDataCallback<List<PurchasableItem>>() {
            @Override
            public void onCallback(List<PurchasableItem> result, GamebaseException exception) {

                if (Gamebase.isSuccess(exception)) {

                    mPurchasableItemList = result;
                    for (int i = 0; i < mPurchasableItemList.size(); i++) {
                        PurchasableItem item = mPurchasableItemList.get(i);
                        String itemName = item.itemName;
                        float itemPrice = item.price;
                        String itemCurrency = item.currency;
                        String marketItemId = item.marketItemId;
                        long itemSeq = item.itemSeq;
                        adapter.addItem(ContextCompat.getDrawable(getActivity(), R.drawable.item_001), itemName, itemCurrency + " " + String.valueOf(itemPrice));
                    }
                    listViewPurchasable.setAdapter(adapter);
                    AppUtil.dismissProgressDialog(getActivity());
                } else {
                    Gamebase.Util.showAlert(getActivity(), "Error", exception.toJsonString());
                    AppUtil.dismissProgressDialog(getActivity());

                    Log.d(TAG, exception.toJsonString());
                    Log.d(TAG, exception.getDomain());
                    Log.d(TAG, String.valueOf(exception.getCode()));
                    Log.d(TAG, exception.getMessage());
                    Log.d(TAG, exception.getDetailDomain());
                    Log.d(TAG, String.valueOf(exception.getDetailCode()));
                    Log.d(TAG, exception.getDetailMessage());
                }
            }
        });


        listViewPurchasable.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView parent, View v, int position, long id) {

                PurchasableItem purchasableItem = mPurchasableItemList.get(position);
                Gamebase.Util.showToast(getActivity(), purchasableItem.toJsonString(), Toast.LENGTH_SHORT);

                AppUtil.showProgressDialog(getActivity(), "Purchase : [" + purchasableItem.itemName + "]");

                GamebaseManager.requestPurchase(getActivity(), purchasableItem.itemSeq, new GamebaseDataCallback<PurchasableReceipt>() {
                    @Override
                    public void onCallback(PurchasableReceipt data, GamebaseException exception) {

                        if (Gamebase.isSuccess(exception)) {

                            Gamebase.Util.showToast(getActivity(), "Success Purchase : " + data.toString(), Toast.LENGTH_SHORT);
                            AppUtil.dismissProgressDialog(getActivity());

                            Log.d(TAG, data.paymentSeq);
                            Log.d(TAG, data.currency);
                            Log.d(TAG, data.purchaseToken);
                            Log.d(TAG, String.valueOf(data.itemSeq));
                            Log.d(TAG, String.valueOf(data.price));
                        } else {

                            Gamebase.Util.showAlert(getActivity(), "Error", exception.toJsonString());
                            AppUtil.dismissProgressDialog(getActivity());

                            Log.d(TAG, exception.toJsonString());
                            Log.d(TAG, exception.getDomain());
                            Log.d(TAG, String.valueOf(exception.getCode()));
                            Log.d(TAG, exception.getMessage());
                            Log.d(TAG, exception.getDetailDomain());
                            Log.d(TAG, String.valueOf(exception.getDetailCode()));
                            Log.d(TAG, exception.getDetailMessage());

                        }
                    }
                });
            }
        });
    }

    private void itemList() {

        Button buttonItemList = mCurrentView.findViewById(R.id.button_IAPItemList);

        buttonItemList.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                AppUtil.showProgressDialog(getActivity(), "Get Item List... ");

                GamebaseManager.requestItemList(getActivity(), new GamebaseDataCallback<List<PurchasableItem>>() {
                    @Override
                    public void onCallback(List<PurchasableItem> data, GamebaseException exception) {

                        if (Gamebase.isSuccess(exception)) {

                            Log.d(TAG, data.toString());

                            List<String> viewItemList = new ArrayList<String>();

                            if (data.size() > 0) {
                                for (int i = 0; i < data.size(); i++) {
                                    PurchasableItem item = data.get(i);

                                    StringBuilder sb = new StringBuilder();
                                    sb.append(item.itemName);
                                    sb.append("(");
                                    sb.append(String.valueOf(item.itemSeq));
                                    sb.append(")");
                                    sb.append(" : ");
                                    sb.append(String.valueOf(item.price));
                                    sb.append(" " + item.currency);
                                    sb.append(" [" + String.valueOf(item.marketItemId) + "]");

                                    viewItemList.add(sb.toString());
                                }
                            } else {
                                viewItemList.add("Empty");
                            }

                            SimpleSelectItemDialog.show(getActivity(), "IAP Item List", viewItemList, null, null, null);
                            AppUtil.dismissProgressDialog(getActivity());
                        } else {
                            Gamebase.Util.showAlert(getActivity(), "requestItemListPurchasable error", exception.toJsonString());
                            AppUtil.dismissProgressDialog(getActivity());

                            Log.d(TAG, exception.toJsonString());
                        }
                    }
                });
            }
        });
    }

    private void itemListOfNotConsumed() {

        Button buttonItemListOfNotConsumed = mCurrentView.findViewById(R.id.button_notConsumed);

        buttonItemListOfNotConsumed.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                AppUtil.showProgressDialog(getActivity(), "Get NotConsumed Item List ... ");

                GamebaseManager.requestNotConsumedItems(getActivity(), new GamebaseDataCallback<List<PurchasableReceipt>>() {
                    @Override
                    public void onCallback(List<PurchasableReceipt> data, GamebaseException exception) {

                        if (Gamebase.isSuccess(exception)) {

                            Log.d(TAG, data.toString());

                            final List<String> viewItemList = new ArrayList<>();
                            final List<String> paymentSeqs = new ArrayList<>();
                            final List<String> tokens = new ArrayList<>();
                            if (data.size() > 0) {
                                for (int i = 0; i < data.size(); i++) {
                                    viewItemList.add(purchasableReceiptToString(data.get(i)));
                                    paymentSeqs.add(data.get(i).paymentSeq);
                                    tokens.add(data.get(i).purchaseToken);
                                }
                            } else {
                                viewItemList.add("Empty");
                            }

                            SimpleSelectItemDialog.show(getActivity(), "Item List Of Not Consumed", viewItemList, new DialogInterface.OnClickListener() {
                                @Override
                                public void onClick(DialogInterface dialog, final int selectedIndex) {
                                    if (paymentSeqs.size() > 0) {
                                        SimpleAlertDialog.show(getActivity(), "Consume",
                                                "== Do not call consume API from client! ==\nThis is for the test.\n\nWill you consume this item?",
                                                "OK", new DialogInterface.OnClickListener() {
                                                    @Override
                                                    public void onClick(DialogInterface dialog, int which) {
                                                        consume(getActivity(), paymentSeqs.get(selectedIndex), tokens.get(selectedIndex));
                                                    }
                                                }, "Cancel", null, false);
                                    }
                                }
                            }, null, null);
                            AppUtil.dismissProgressDialog(getActivity());
                        } else {
                            Log.d(TAG, exception.toJsonString());
                            Gamebase.Util.showAlert(getActivity(), "requestItemListOfNotConsumed error", exception.toJsonString());
                            AppUtil.dismissProgressDialog(getActivity());
                        }
                    }
                });
            }
        });
    }

    private void getSubscriptions() {

        Button buttonSubscriptions = mCurrentView.findViewById(R.id.button_subscriptions);

        buttonSubscriptions.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                AppUtil.showProgressDialog(getActivity(), "Get Subscriptions ... ");

                GamebaseManager.requestSubscriptions(getActivity(), new GamebaseDataCallback<List<PurchasableReceipt>>() {
                    @Override
                    public void onCallback(List<PurchasableReceipt> data, GamebaseException exception) {

                        if (Gamebase.isSuccess(exception)) {

                            Log.d(TAG, data.toString());

                            final List<String> viewItemList = new ArrayList<>();
                            final List<String> paymentSeqs = new ArrayList<>();
                            final List<String> tokens = new ArrayList<>();
                            if (data.size() > 0) {
                                for (int i = 0; i < data.size(); i++) {
                                    viewItemList.add(purchasableReceiptToString(data.get(i)));
                                    paymentSeqs.add(data.get(i).paymentSeq);
                                    tokens.add(data.get(i).purchaseToken);
                                }
                            } else {
                                viewItemList.add("Empty");
                            }
                            SimpleSelectItemDialog.show(getActivity(), "List of subscriptions", viewItemList, null, null, null);
                            AppUtil.dismissProgressDialog(getActivity());
                        } else {
                            Log.d(TAG, exception.toJsonString());
                            Gamebase.Util.showAlert(getActivity(), "requestSubscriptions error", exception.toJsonString());
                            AppUtil.dismissProgressDialog(getActivity());
                        }
                    }
                });
            }
        });
    }

    private String purchasableReceiptToString(PurchasableReceipt item) {

        StringBuilder sb = new StringBuilder();
        sb.append(item.purchaseToken);
        sb.append(" : " + item.itemSeq);
        sb.append(" : " + item.price);
        sb.append(" " + item.currency);

        return sb.toString();
    }

    ////////////////////////////////////////////////////////////////////////////////
    //
    // Consume.
    // WARNING: DO NOT CALL THIS API FROM CLIENT!
    //
    ////////////////////////////////////////////////////////////////////////////////
    private static void consume(@NonNull final Activity activity, @NonNull final String paymentSeq, @NonNull final String accessToken) {
        String zoneType = GamebaseSystemInfo.getInstance().getZoneType();
        String apiPrefix = "https://";
        if (!zoneType.equalsIgnoreCase(ZONE_TYPE_REAL)) {
            apiPrefix = apiPrefix + zoneType.toLowerCase() + "-";
        }
        final String apiUrl = apiPrefix + "api-iap.cloud.toast.com/v1/service/consume";
        final Map<String, Object> parameters = new HashMap<>();
        parameters.put("paymentSeq", paymentSeq);
        parameters.put("accessToken", accessToken);
        final Map<String, String> headers = new HashMap<>();
        try {
            String tcIapAppKey = Gamebase.Launching.getLaunchingInformations().getAppKey(TCPRODUCT_TYPE_IAP);
            headers.put("X-NHN-TCIAP-AppKey", tcIapAppKey);
        } catch (Exception e) {
        }

        AppUtil.showProgressDialog(activity, "Consume ... ");
        new Thread((new Runnable() {
            @Override
            public void run() {
                String response = getResponse(apiUrl, parameters, headers);
                Log.d(TAG, "consume result : " + response);
                AppUtil.dismissProgressDialog(activity);
                Gamebase.Util.showToast(activity, "Item consumed : " + response, Toast.LENGTH_SHORT);
            }
        })).start();
    }

    private static String getResponse(final String apiUrl, final Map<String, Object> parameters, final Map<String, String> headers) {
        try {
            URL url = new URL(apiUrl);
            HttpURLConnection urlConnection = (HttpURLConnection) url.openConnection();
            urlConnection.setRequestProperty("Content-Type", "application/json");
            if (headers != null) {
                for (String key : headers.keySet()) {
                    urlConnection.setRequestProperty(key, headers.get(key));
                }
            }

            urlConnection.setDoOutput(true);
            urlConnection.setChunkedStreamingMode(0);

            OutputStream out = null;
            InputStream in = null;
            try {
                long requestTime = System.currentTimeMillis();
                urlConnection.setRequestMethod("POST");
                out = urlConnection.getOutputStream();
                out.write(parametersToByteValue(BodyType.JSON, parameters));
                out.close();

                int responseCode = urlConnection.getResponseCode();
                long duration = System.currentTimeMillis() - requestTime;

                Log.d(TAG, "------------------------------ HTTP Request -------------------------------------------------------");
                Log.d(TAG, "URL: " + apiUrl);
                Log.d(TAG, "Method: POST");
                Log.d(TAG, "[RequestBody]\n" + new String(parametersToByteValue(BodyType.JSON, parameters), "utf-8"));
                Log.d(TAG, "---------------------------------------------------------------------------------------------------");
                if ((responseCode != 200) && (responseCode != 201)) {
                    IOException exception = new IOException(
                            "HTTP Response - " + urlConnection.getResponseCode() +
                                    " " + urlConnection.getResponseMessage() +
                                    "\n[HEADERS]\n" + getArrangedHeaders(urlConnection.getHeaderFields()) +
                                    "[BODY]\n" + readStreamToString(urlConnection.getErrorStream()));
                    Log.w(TAG, exception);
                    return exception.getMessage();
                }
                in = urlConnection.getInputStream();
                String response = readStreamToString(in);

                Log.d(TAG, "------------------------------ HTTP Response ------------------------------------------------------");
                Log.d(TAG, "responseCode: " + Integer.valueOf(responseCode) + ", responseTime:" + "m/s" + Long.valueOf(duration));
                Log.d(TAG, "[ResponseBody]\n" + response);
                Log.d(TAG, "---------------------------------------------------------------------------------------------------");
                urlConnection.disconnect();
                return response;
            } finally {
                closeQuietly(out);
                closeQuietly(in);
            }
        } catch (Exception e) {
            Log.e(TAG, Log.getStackTraceString(e));
            return e.getMessage();
        }
    }

    private static String getArrangedHeaders(Map<String, List<String>> headers) {
        StringBuilder sb = new StringBuilder();
        Set<String> keys = headers.keySet();
        for (String key : keys) {
            if (key != null) {
                sb.append(key).append(":");
                List<String> values = headers.get(key);
                for (String value : values) {
                    if (value != null) {
                        sb.append(value);
                    }
                }
                sb.append("\n");
            }
        }
        return sb.toString();
    }

    private static void closeQuietly(Closeable closeable) {
        try {
            if (closeable != null) {
                closeable.close();
            }
        } catch (IOException e) {
            Log.w(TAG, e.getMessage());
        }
    }

    private static byte[] parametersToByteValue(final BodyType bodyType, Map<String, Object> parameters)
            throws UnsupportedEncodingException {
        String params = null;
        if (bodyType.equals(BodyType.NORMAL)) {
            params = encodeURLParameters(parameters);
        } else {
            params = gson.toJson(parameters);
        }
        return params.getBytes("utf-8");
    }

    private static String readStreamToString(InputStream inputStream)
            throws IOException {
        BufferedInputStream bufferedInputStream = null;
        InputStreamReader reader = null;
        try {
            bufferedInputStream = new BufferedInputStream(inputStream);
            reader = new InputStreamReader(bufferedInputStream);
            StringBuilder stringBuilder = new StringBuilder();

            int bufferSize = 8192;
            char[] buffer = new char[bufferSize];

            int n = 0;
            while ((n = reader.read(buffer)) != -1) {
                stringBuilder.append(buffer, 0, n);
            }
            return stringBuilder.toString();
        } finally {
            closeQuietly(bufferedInputStream);
            closeQuietly(reader);
        }
    }

    private static String encodeURLParameters(Map<String, Object> params)
            throws UnsupportedEncodingException {
        if ((params == null) || (params.isEmpty())) {
            return "";
        }
        StringBuilder sb = new StringBuilder();
        boolean isFirst = true;
        for (String key : params.keySet()) {
            if (!isFirst) {
                sb.append('&');
            } else {
                isFirst = false;
            }
            sb.append(RFC3986Encoder(key));
            sb.append('=');
            sb.append(RFC3986Encoder(params.get(key).toString()));
        }
        return sb.toString();
    }

    private static String RFC3986Encoder(String normal)
            throws UnsupportedEncodingException {
        if (normal == null) {
            return "";
        }
        return URLEncoder.encode(normal, "UTF-8").replace("+", "%20").replace("*", "%2A").replace("%7E", "~");
    }

    public enum BodyType {
        NORMAL, JSON;

        BodyType() {
        }
    }
}
