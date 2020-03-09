# Gamebase Authentication Naver Adapter for Android

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
  <artifactId>kotlin-android-extensions-runtime</artifactId>
  <version>1.3.40</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.android.support</groupId>
  <artifactId>appcompat-v7</artifactId>
  <version>28.0.0</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.android.support</groupId>
  <artifactId>support-core-utils</artifactId>
  <version>28.0.0</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.android.support</groupId>
  <artifactId>customtabs</artifactId>
  <version>28.0.0</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.android.support</groupId>
  <artifactId>support-v4</artifactId>
  <version>28.0.0</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.naver.nid</groupId>
  <artifactId>naveridlogin-android-sdk</artifactId>
  <version>4.2.5</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>org.jetbrains.kotlin</groupId>
  <artifactId>kotlin-stdlib-jdk7</artifactId>
  <version>1.3.40</version>
  <scope>compile</scope>
</dependency>
```

### Reference of Naverld Android Developer's guide.

* https://developers.naver.com/docs/login/android/

## Release Note

| Date | Naver Adapter Version | Gamebase SDK Version | NaverIdLogin SDK Version | Description |
| ---- | ---------------------- | -------------------- | --------------------------- | ----------- |
| 2018.02.22 | 1.7.0  | 1.7.0 ~ 1.15.0 | 4.2.0 ~ | - Initial version |
| 2018.09.13 | 1.13.0 | 1.7.0 ~ 1.15.0 | 4.1.4 ~ | - Support NaverLoginSDK 4.1.4 |
| 2019.01.29 | 2.0.0  | 2.0.0          | 4.1.4 ~ | - Update Adapter Version due to Gamebase 2.0.0 Release |
| 2019.04.23 | 2.3.0  | 2.3.0 ~        | 4.1.4 ~ | - Change logout, withdraw interface. Fix a memory leak |
| 2019.05.28 | 2.4.0  | 2.3.0 ~ 2.5.0  | 4.1.4 ~ | - Update naver-login-sdk to 4.2.5 from 4.2.0 |
| 2019.10.29 | 2.6.0  | 2.6.0 ~ 2.6.2  | 4.1.4 ~ | - Update Android support library to 28.0.0 from 27.0.2 |
| 2020.01.21 | 2.7.0  | 2.7.0 ~        | 4.1.4 ~ | - Change the logic that required parameter validate in initialize. |
| 2020.02.25 | 2.7.1  | 2.7.0 ~        | 4.1.4 ~ | - Update comment and copyright. |
