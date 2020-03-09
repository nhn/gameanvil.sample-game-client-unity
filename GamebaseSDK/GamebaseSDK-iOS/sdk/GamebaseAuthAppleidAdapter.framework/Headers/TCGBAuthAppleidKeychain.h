//
//  TCGBAuthAppleidKeychain.h
//  GamebaseAuthAppleidAdapter
//
//  Created by NHNEnt on 15/10/2019.
//  Copyright © 2019 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TCGBAuthAppleidKeychain : NSObject
@property(atomic, strong) NSString*             service;
@property(atomic, strong) NSString*             account;
@property(atomic, strong) NSString*             accessGroup;

- (instancetype)initWithService:(NSString *)service account:(NSString *)account accessGroup:(NSString * _Nullable)accessGroup;
- (NSString *)readItem;
- (void)saveItemWithUserId:(NSString *)password;
- (void)deleteItem;

@end

NS_ASSUME_NONNULL_END
