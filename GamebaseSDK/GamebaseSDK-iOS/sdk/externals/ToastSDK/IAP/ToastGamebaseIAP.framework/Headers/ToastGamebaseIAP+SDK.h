//
//  ToastGamebaseIAP+SDK.h
//  ToastGamebaseIAP
//
//  Created by Hyup on 16/04/2019.
//  Copyright © 2019 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "ToastGamebaseIAPConfiguration.h"
#import "ToastGamebaseProduct.h"
#import "ToastGamebaseProductsResponse.h"


@class ToastGamebasePurchase;

typedef void (^ToastGamebasePurchaseHandler)(BOOL isSuccess,
                                             ToastGamebasePurchase *_Nullable purchase,
                                             NSString *_Nonnull productID,
                                             NSError* _Nullable error);

@protocol ToastGamebaseInAppPurchaseDelegate <NSObject>

- (void)didReceivePurchaseResult:(ToastGamebasePurchase *_Nullable)purchase
                       isSuccess:(BOOL)isSuccess
                       productID:(NSString * _Nonnull)productID
                           error:(NSError *_Nullable)error;
@end

NS_ASSUME_NONNULL_BEGIN

@interface ToastGamebaseIAP : NSObject

#pragma mark - initialize
+ (void)setDelegate:(id<ToastGamebaseInAppPurchaseDelegate>)delegate;

+ (void)initWithConfiguration:(ToastGamebaseIAPConfiguration *)configuration;

+ (void)initWithConfiguration:(ToastGamebaseIAPConfiguration *)configuration
                     delegate:(id<ToastGamebaseInAppPurchaseDelegate>)delegate;

#pragma mark - products
+ (void)requestProductsWithCompletionHandler:(nullable void (^)(ToastGamebaseProductsResponse * _Nullable response, NSError * _Nullable error))completionHandler;

#pragma mark - consumable
+ (void)requestConsumablePurchasesWithCompletionHandler:(nullable void (^)(NSArray<ToastGamebasePurchase *> * _Nullable purchases, NSError * _Nullable error))completionHandler;

#pragma mark - purchase
+ (void)purchaseWithProduct:(ToastGamebaseProduct *)product completionHandler:(ToastGamebasePurchaseHandler)completionHandler;

+ (void)purchaseWithProductID:(NSString *)productID completionHandler:(ToastGamebasePurchaseHandler)completionHandler;

#pragma mark - purchase - payload (payload is ToastIAP only)
+ (void)purchaseWithProductID:(NSString *)productID
                      payload:(nullable NSString *)payload
            completionHandler:(ToastGamebasePurchaseHandler)completionHandler;

+ (void)purchaseWithProduct:(ToastGamebaseProduct *)product
                    payload:(nullable NSString *)payload
          completionHandler:(ToastGamebasePurchaseHandler)completionHandler;

#pragma mark - purchase - payload, extra (payload(gamebasePayload) is ToastIAP only)
+ (void)purchaseWithProductID:(NSString *)productID
                      payload:(nullable NSString *)payload
              gamebasePaylaod:(nullable NSString *)gamebasePayload
            completionHandler:(ToastGamebasePurchaseHandler)completionHandler;

+ (void)purchaseWithProduct:(ToastGamebaseProduct *)product
                    payload:(nullable NSString *)payload
            gamebasePaylaod:(nullable NSString *)gamebasePayload
          completionHandler:(ToastGamebasePurchaseHandler)completionHandler;

#pragma mark - restore (ToastIAP Only)
+ (void)restoreWithCompletionHandler:(nullable void (^)(NSArray<ToastGamebasePurchase *> * _Nullable purchases, NSError * _Nullable error))completionHandler;

#pragma mark - activatedPurchase (ToastIAP Only)
+ (void)requestActivatedPurchasesWithCompletionHandler:(nullable void (^)(NSArray<ToastGamebasePurchase *> * _Nullable purchases, NSError * _Nullable error))completionHandler;

#pragma mark - processes incomplete purchases  (ToastIAP Only - TCIAP -> ToastIAP)
+ (void)processesIncompletePurchasesWithCompletionHandler:(nullable void (^)(NSArray <ToastGamebasePurchase *> * _Nullable results, NSError * _Nullable error))completionHandler;

#pragma mark - Support Utils
+ (void)consumeWithPurchase:(ToastGamebasePurchase *)purchase
          completionHandler:(nullable void (^)(NSError * _Nullable error))completionHandler;

+ (void)setDebugMode:(BOOL)debugMode;

+ (NSString *)version;
@end

NS_ASSUME_NONNULL_END
