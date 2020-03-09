# Gamebase Push FCM Adapter for Android

## Dependencies

```xml
<dependency>
  <groupId>org.jetbrains.kotlin</groupId>
  <artifactId>kotlin-stdlib-jdk7</artifactId>
  <version>1.3.40</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.toast.android.gamebase</groupId>
  <artifactId>gamebase-adapter-toastpush</artifactId>
  <version>2.6.0</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.toast.android</groupId>
  <artifactId>toast-push-fcm</artifactId>
  <version>0.19.3</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.android.support</groupId>
  <artifactId>support-v4</artifactId>
  <version>28.0.0</version>
  <scope>compile</scope>
</dependency>
```

### Dependencies of Google Play Services libraries

* https://developers.google.com/android/guides/setup#add_google_play_services_to_your_project

### Dependencies of Firebase libraries

* https://firebase.google.com/docs/android/setup#available_libraries

## Release Note

| Date | FCM Adapter Version | Gamebase SDK Version | TOAST Push SDK Version | Description |
| --- | --- | --- | --- | --- |
| 2017.03.09 | 1.0.0 | 1.0.0 ~ 1.1.3.1 | 1.32 | - Initial version |
| 2017.04.28 | 1.1.3.2 | 1.1.3.2 | 1.4.0 | - Update TCPush SDK to 1.4.0 |
| 2017.05.23 | 1.1.4   | 1.1.4 ~ 1.1.4.1 | 1.4.0 | - Update version |
| 2017.06.27 | 1.1.4.2 | 1.1.4.2 ~ 1.5.0 | 1.4.1 ~ 1.4.2 | - Update TCPush SDK to 1.4.1 |
| 2017.07.19 | 1.1.5   | 1.1.5 ~ 1.5.0 | 1.4.1 ~ 1.4.2 | - Update version |
| 2018.02.22 | 1.7.0   | 1.7.0 ~ 2.5.0 | 1.4.1 ~ 1.4.2 | - Update TCPush SDK to 1.4.2<br>- Calling PushAnalytics.onReceived() API |
| 2019.01.29 | 2.0.0   | 2.0.0 ~ 2.5.0 | 1.7.0 | - Update TCPush SDK to 1.7.0 |
| 2019.04.23 | 2.3.0   | 2.0.0 ~ 2.5.0 | 1.7.0 | - Modify the code to match the changed interface. |
| 2019.10.29 | 2.6.0   | 2.6.0 ~       | 0.19.3 ~        | - Update Android support library to 28.0.0 from 27.0.2<br> - Change TCPush SDK to TOAST Push SDK |
| 2020.02.25 | 2.7.1   | 2.6.0 ~       | 0.19.3 ~        | - Update comment and copyright. |
