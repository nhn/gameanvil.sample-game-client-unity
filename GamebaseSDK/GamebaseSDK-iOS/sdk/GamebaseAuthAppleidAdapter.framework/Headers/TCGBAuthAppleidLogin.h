//
//  TCGBAuthAppleidLogin.h
//  GamebaseAuthAppleidAdapter
//
//  Created by NHNEnt on 14/10/2019.
//  Copyright © 2019 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <Gamebase/Gamebase.h>
#import <AuthenticationServices/AuthenticationServices.h>

NS_ASSUME_NONNULL_BEGIN

typedef void(^AppleLoginCompletion)(id _Nullable credential, TCGBError * _Nullable error);

NS_ENUM(NSInteger, TCGBAuthAppleidLoginErrorDomain) {
    TCGBAuthAppleidLoginError_InvalidVersionFormat = -1
};

@interface TCGBAuthAppleidLogin : NSObject <TCGBAuthAdapterDelegate, ASAuthorizationControllerDelegate>
@property(atomic, strong) AppleLoginCompletion loginCompletionHandler;
@property(atomic, strong) TCGBProviderAuthCredential* authCredential;
@property(atomic, strong) id appleIdCredential;
@property(atomic, strong) NSMutableDictionary*          providerProfile;

- (NSString *)fullName:(NSPersonNameComponents * _Nullable)personName forFormatterStyle:(NSPersonNameComponentsFormatterStyle)formatterStyle;
- (BOOL)availableCheckingWithCurrentVersion:(NSString *)currentVersion error:(NSError **)error;
@end

NS_ASSUME_NONNULL_END
