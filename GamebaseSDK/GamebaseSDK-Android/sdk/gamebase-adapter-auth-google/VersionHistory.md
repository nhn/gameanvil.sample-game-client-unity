# Gamebase Authentication Google Adapter for Android

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
  <artifactId>support-v4</artifactId>
  <version>28.0.0</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>org.jetbrains.kotlin</groupId>
  <artifactId>kotlin-stdlib-jdk7</artifactId>
  <version>1.3.40</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.google.android.gms</groupId>
  <artifactId>play-services-auth</artifactId>
  <version>16.0.1</version>
  <scope>compile</scope>
</dependency>
```

### Dependencies of Google Play Services libraries

* https://developers.google.com/android/guides/setup#add_google_play_services_to_your_project

## Release Note

| Date | Google Adapter Version | Gamebase SDK Version | Google Play Service Version | Description |
| ---- | ---------------------- | -------------------- | --------------------------- | ----------- |
| 2017.03.21 | 1.1.1 | 1.1.0 ~ 1.1.2  | 10.0.1 ~ 11.8.0 | - Initial version |
| 2017.04.20 | 1.1.3 | 1.1.3 ~ 1.5.0  | 10.0.1 ~ 11.8.0 | - Change the Interface |
| 2017.09.18 | 1.2.0 | 1.1.3 ~ 1.5.0  | 10.0.1 ~ 11.8.0 | - Fix wrong null check<br> - Fix NullPointerException by forced restart. |
| 2018.02.22 | 1.7.0 | 1.7.0 ~ 1.15.0 | 10.0.1 ~ 11.8.0 | - Change the logic that refers from the console setting value. |
| 2018.05.03 | 1.9.0 | 1.7.0 ~ 1.15.0 | 10.0.1 ~ 11.8.0 | - Add safe logic when auth code was null. |
| 2019.01.29 | 2.0.0 | 2.0.0          | 10.0.1 ~ 11.8.0 | - Update Adapter Version due to Gamebase 2.0.0 Release |
| 2019.04.23 | 2.3.0 | 2.3.0 ~ 2.5.0  | 10.0.1 ~ 11.8.0 | - Change logout, withdraw interface. Fix a memory leak |
| 2019.10.29 | 2.6.0 | 2.6.0 ~ 2.6.2  | 16.0.1 ~        | - Update Android support library to 28.0.0 from 27.0.2<br> - Update play-services-auth to 16.0.1 from 11.8.0 |
| 2020.01.21 | 2.7.0 | 2.7.0 ~        | 16.0.1 ~        | - Change the logic that required parameter validate in initialize. |
| 2020.02.25 | 2.7.1 | 2.7.0 ~        | 16.0.1 ~        | - Update comment and copyright. |
