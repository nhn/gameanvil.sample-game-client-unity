# Gamebase Authentication Facebook Adapter for Android

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
  <groupId>com.android.support</groupId>
  <artifactId>appcompat-v7</artifactId>
  <version>28.0.0</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.android.support</groupId>
  <artifactId>cardview-v7</artifactId>
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
  <groupId>com.facebook.android</groupId>
  <artifactId>facebook-login</artifactId>
  <version>5.1.1</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>org.jetbrains.kotlin</groupId>
  <artifactId>kotlin-stdlib-jdk7</artifactId>
  <version>1.3.40</version>
  <scope>compile</scope>
</dependency>
```

## Release Note

| Date | Facebook Adapter Version | Gamebase SDK Version | Facebook SDK Version | Description |
| --- | --- | --- | --- | --- |
| 2017.03.21 | 1.1.1  | 1.1.0 ~ 1.1.2  | 4.17.0 ~ 4.18.0 | Initial version |
| 2017.04.20 | 1.1.3  | 1.1.3 ~ 1.5.0  | 4.17.0 ~ 4.30.0 | Corrected crash error on initialization in Facebook sdk v4.19.0 or later<br>Change the Interface |
| 2018.02.22 | 1.7.0  | 1.7.0 ~ 1.15.0 | 4.17.0 ~ 4.30.0 | Change the logic that refers from the console setting value. |
| 2018.06.26 | 1.11.0 | 1.7.0 ~ 1.15.0 | 4.17.0 ~ 4.30.0 | Remove java 1.8 define from build settings. |
| 2019.01.29 | 2.0.0  | 2.0.0          | 4.17.0 ~ 4.30.0 | Update Adapter Version due to Gamebase 2.0.0 Release |
| 2019.04.23 | 2.3.0  | 2.3.0 ~ 2.5.0  | 4.17.0 ~ 4.30.0 | Change logout, withdraw interface. Fix a memory leak |
| 2019.10.29 | 2.6.0  | 2.6.0 ~ 2.6.2  | 4.17.0 ~ 5.1.1  | - Update Android support library to 28.0.0 from 27.0.2<br>- Update Facebook sdk to 5.1.1 from 4.30.0 |
| 2020.01.21 | 2.7.0  | 2.7.0 ~        | 4.17.0 ~ 5.1.1  | Change the logic that required parameter validate in initialize. |
| 2020.02.25 | 2.7.1  | 2.7.0 ~        | 4.17.0 ~ 5.1.1  | - Update comment and copyright. |

