# Gamebase Authentication Twitter Adapter for Android

## Dependencies

```xml
<dependency>
  <groupId>com.toast.android.gamebase</groupId>
  <artifactId>gamebase-sdk</artifactId>
  <version>2.6.0</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>org.jetbrains.kotlin</groupId>
  <artifactId>kotlin-stdlib-jdk7</artifactId>
  <version>1.3.40</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>oauth.signpost</groupId>
  <artifactId>signpost-core</artifactId>
  <version>1.2.1.2</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.google.android.gms</groupId>
  <artifactId>play-services-auth</artifactId>
  <version>16.0.1</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.android.support</groupId>
  <artifactId>support-v4</artifactId>
  <version>28.0.0</version>
  <scope>compile</scope>
</dependency>
```

### Reference of Twitter Android Developer's guide.

* https://dev.twitter.com/web/sign-in/implementing
* https://developer.twitter.com/en/docs/basics/authentication/overview/oauth

## Release Note

| Date | Twitter Adapter Version | Gamebase SDK Version  | Description |
| ---- | ----------------------- | --------------------- |------------ |
| 2018.06.26 | 1.11.0 | 1.11.0 ~ 1.15.0 | - Initial version |
| 2018.08.30 | 1.12.2 | 1.11.0 ~ 1.15.0 | - Support for Android 9.0 |
| 2019.01.29 | 2.0.0  | 2.0.0           | - Update Adapter Version due to Gamebase 2.0.0 Release |
| 2019.04.23 | 2.3.0  | 2.3.0 ~ 2.5.0   | - Change logout, withdraw interface. Fix a memory leak |
| 2019.05.16 | 2.3.1  | 2.3.0 ~ 2.5.0   | - Fix a login bug; the login callback was set to null before finishing login process |
| 2019.10.29 | 2.6.0  | 2.6.0 ~ 2.6.2   | - Update Android support library to 28.0.0 from 27.0.2<br> - Fixed internal logic(about TLS support) |
| 2020.01.21 | 2.7.0  | 2.7.0 ~         | - Change the logic that required parameter validate in initialize. |
| 2020.02.25 | 2.7.1  | 2.7.0 ~         | - Update comment and copyright. |
