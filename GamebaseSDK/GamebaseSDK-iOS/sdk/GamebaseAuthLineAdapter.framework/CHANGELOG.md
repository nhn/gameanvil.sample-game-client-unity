# Change Log

All notable changes to the LINE SDK will be documented in this file.

## 5.0.1 - 2019/04/16

* User logging out now revokes the refresh token and all its corresponding access tokens at the same time.

## 5.0.0 - 2018/10/15

* Added support for LINE Login v2.1, which provide a fine-tuned authorization permissions and more safety authorizing flow. 
* Added support for ID Token with ECDSA verification based on OpenID protocol. It provides a secure way to verify user information.
* You can use a predefined login button to let your users login now.
* Fixed a potential issue which causes authorizing from LINE app may fail on devices with iOS 12.
* The automatically token refreshing should now work properly when receives a token expiring error from LINE Login Server.

> Tokens issued by LINE SDK v4 will be invalid in SDK v5. You need to get authorization from your users again to use LINE APIs.

## 4.2.0 - 2018/07/30

* Added support for Graph API v2 and User Message API.

## 4.1.1 - 2018/04/12

* [LCLC-1696] LineSDKAPI logout but LineSDKLogin still has accessToken.

## 4.1.0 - 2018/01/18

* [LINELD-98] changed to use SFSafariViewController instead of external browser when weblogin.

## 4.0.3 - 2017/08/07

* [LCLC-988] To change to open app2app url with the Universal Link if it's possible on iOS10 or later.
* To separate internal error case for missing ChannelID and missing return url.

## 4.0.2 - 2017/03/21

* Added a module map to the framework.

## 4.0.1 - 2017/01/27

* Fixed a problem where authentication fails when using the Web Login.

## 4.0.0 - 2017/01/24

* Initial Release
