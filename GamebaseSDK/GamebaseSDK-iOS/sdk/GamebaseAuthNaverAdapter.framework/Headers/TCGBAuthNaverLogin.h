//
//  TCGBAuthNaverLogin.h
//  GamebaseAuthNaverAdapter
//
//  Created by NHN on 2018. 1. 23..
//  © NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <Gamebase/Gamebase.h>
#import <NaverThirdPartyLogin/NaverThirdPartyLogin.h>

#define GamebaseAuthNaverAdapterVersion @"2.0.2"

@interface TCGBAuthNaverLogin : NSObject <TCGBAuthAdapterDelegate, NaverThirdPartyLoginConnectionDelegate>

typedef NS_ENUM(NSInteger, TCGBAuthNaverLoginErrorCode)
{
    TCGBAuthNaverLoginNoViewControllerError = 0,
    TCGBAuthNaverLoginNoAdditionalInfoError,
    TCGBAuthNaverLoginNoConsumerKeyOrSecretError,
    TCGBAuthNaverLoginNoCallbackURLSchemeError,
    TCGBAuthNaverLoginUserCanceledError,
    TCGBAuthNaverLoginOAuthError,
    TCGBAuthNaverLoginUnknownError = -1
};


@property (nonatomic, strong) NSString *appId;
@property (nonatomic, strong) NSString *appSecret;
@property (nonatomic, strong) NSString *appName;
@property (nonatomic, strong) NSString *urlScheme;
@property (nonatomic, strong) TCGBProviderAuthCredential* credential;
@property (nonatomic, strong) UIViewController* loginViewController;

@end
