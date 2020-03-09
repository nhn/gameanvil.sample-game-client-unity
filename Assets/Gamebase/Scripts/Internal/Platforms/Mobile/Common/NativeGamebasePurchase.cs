﻿#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
using System.Collections.Generic;
using Toast.Gamebase.LitJson;

namespace Toast.Gamebase.Internal.Mobile
{
    public class NativeGamebasePurchase : IGamebasePurchase
    {
        protected class GamebasePurchase
        {
            public const string PURCHASE_API_REQUEST_PURCHASE_SEQ                   = "gamebase://requestPurchaseSeq";
            public const string PURCHASE_API_REQUEST_PURCHASE                       = "gamebase://requestPurchase";
            public const string PURCHASE_API_REQUEST_ITEM_LIST_OF_NOT_CONSUMED      = "gamebase://requestItemListOfNotConsumed";
            public const string PURCHASE_API_REQUEST_RETYR_TRANSACTION              = "gamebase://requestRetryTransaction";
            public const string PURCHASE_API_REQUEST_ITEM_LIST_PURCHASABLE          = "gamebase://requestItemListPurchasable";
            public const string PURCHASE_API_REQUEST_ITEM_LIST_AT_AP_CONSOLE        = "gamebase://requestItemListAtIAPConsole";
            public const string PURCHASE_API_SET_PROMOTION_IAP_HANDLER              = "gamebase://setPromotionIAPHandler";
            public const string PURCHASE_API_SET_STORE_CODE                         = "gamebase://setStoreCode";
            public const string PURCHASE_API_GET_STORE_CODE                         = "gamebase://getStoreCode";
            public const string PURCHASE_API_REQUEST_ACTIVATED_PURCHASES            = "gamebase://requestActivatedPurchases";
        }

        protected INativeMessageSender  messageSender   = null;
        protected string                CLASS_NAME      = string.Empty;

        public NativeGamebasePurchase()
        {
            Init();
        }

        virtual protected void Init()
        {
            messageSender.Initialize(CLASS_NAME);

            DelegateManager.AddDelegate(GamebasePurchase.PURCHASE_API_REQUEST_PURCHASE_SEQ,                 DelegateManager.SendGamebaseDelegateOnce<GamebaseResponse.Purchase.PurchasableReceipt>);
            DelegateManager.AddDelegate(GamebasePurchase.PURCHASE_API_REQUEST_PURCHASE,                     DelegateManager.SendGamebaseDelegateOnce<GamebaseResponse.Purchase.PurchasableReceipt>);
            DelegateManager.AddDelegate(GamebasePurchase.PURCHASE_API_REQUEST_ITEM_LIST_OF_NOT_CONSUMED,    DelegateManager.SendGamebaseDelegateOnce<List<GamebaseResponse.Purchase.PurchasableReceipt>>);
            DelegateManager.AddDelegate(GamebasePurchase.PURCHASE_API_REQUEST_RETYR_TRANSACTION,            DelegateManager.SendGamebaseDelegateOnce<GamebaseResponse.Purchase.PurchasableRetryTransactionResult>);
            DelegateManager.AddDelegate(GamebasePurchase.PURCHASE_API_REQUEST_ITEM_LIST_PURCHASABLE,        DelegateManager.SendGamebaseDelegateOnce<List<GamebaseResponse.Purchase.PurchasableItem>>);
            DelegateManager.AddDelegate(GamebasePurchase.PURCHASE_API_REQUEST_ITEM_LIST_AT_AP_CONSOLE,      DelegateManager.SendGamebaseDelegateOnce<List<GamebaseResponse.Purchase.PurchasableItem>>);
            DelegateManager.AddDelegate(GamebasePurchase.PURCHASE_API_SET_PROMOTION_IAP_HANDLER,            DelegateManager.SendGamebaseDelegateOnce<GamebaseResponse.Purchase.PurchasableReceipt>);
            DelegateManager.AddDelegate(GamebasePurchase.PURCHASE_API_REQUEST_ACTIVATED_PURCHASES,          DelegateManager.SendGamebaseDelegateOnce<List<GamebaseResponse.Purchase.PurchasableReceipt>>);
        }

        virtual public void RequestPurchase(long itemSeq, int handle)
        {
            NativeRequest.Purchase.PurchaseItemSeq vo = new NativeRequest.Purchase.PurchaseItemSeq();
            vo.itemSeq = itemSeq;

            string jsonData = JsonMapper.ToJson(
                new UnityMessage(
                    GamebasePurchase.PURCHASE_API_REQUEST_PURCHASE_SEQ,
                    jsonData: JsonMapper.ToJson(vo),
                    handle:handle
                    ));
            messageSender.GetAsync(jsonData);
        }

        public void RequestPurchase(string marketItemId, int handle)
        {
            NativeRequest.Purchase.PurchaseMarketItemId vo = new NativeRequest.Purchase.PurchaseMarketItemId();
            vo.marketItemId = marketItemId;

            string jsonData = JsonMapper.ToJson(
                new UnityMessage(
                    GamebasePurchase.PURCHASE_API_REQUEST_PURCHASE,
                    jsonData: JsonMapper.ToJson(vo),
                    handle: handle
                    ));
            messageSender.GetAsync(jsonData);
        }

        virtual public void RequestItemListOfNotConsumed(int handle)
        {
            string jsonData = JsonMapper.ToJson(
                new UnityMessage(
                    GamebasePurchase.PURCHASE_API_REQUEST_ITEM_LIST_OF_NOT_CONSUMED,
                    handle: handle
                    ));
            messageSender.GetAsync(jsonData);
        }

        virtual public void RequestRetryTransaction(int handle)
        {
            string jsonData = JsonMapper.ToJson(
                new UnityMessage(GamebasePurchase.PURCHASE_API_REQUEST_RETYR_TRANSACTION,
                handle: handle
                ));
            messageSender.GetAsync(jsonData);
        }

        virtual public void RequestItemListPurchasable(int handle)
        {
            string jsonData = JsonMapper.ToJson(
                new UnityMessage(GamebasePurchase.PURCHASE_API_REQUEST_ITEM_LIST_PURCHASABLE,
                handle: handle
                ));
            messageSender.GetAsync(jsonData);
        }

        virtual public void RequestItemListAtIAPConsole(int handle)
        {
            string jsonData = JsonMapper.ToJson(
                new UnityMessage(
                    GamebasePurchase.PURCHASE_API_REQUEST_ITEM_LIST_AT_AP_CONSOLE,
                    handle: handle
                    ));
            messageSender.GetAsync(jsonData);
        }

        virtual public void SetPromotionIAPHandler(int handle)
        {
            string jsonData = JsonMapper.ToJson(
                new UnityMessage(
                    GamebasePurchase.PURCHASE_API_SET_PROMOTION_IAP_HANDLER,
                    handle: handle
                    ));

            messageSender.GetAsync(jsonData);
        }

        virtual public void SetStoreCode(string storeCode)
        {
            string jsonData = JsonMapper.ToJson(
                new UnityMessage(
                    GamebasePurchase.PURCHASE_API_SET_STORE_CODE,
                    jsonData: storeCode
                    ));
            messageSender.GetSync(jsonData);
        }

        virtual public string GetStoreCode()
        {
            string jsonData = JsonMapper.ToJson(new UnityMessage(GamebasePurchase.PURCHASE_API_GET_STORE_CODE));
            return messageSender.GetSync(jsonData);
        }

        public void RequestActivatedPurchases(int handle)
        {
            string jsonData = JsonMapper.ToJson(
                new UnityMessage(
                    GamebasePurchase.PURCHASE_API_REQUEST_ACTIVATED_PURCHASES,
                    handle: handle
                    ));

            messageSender.GetAsync(jsonData);
        }
    }
}
#endif