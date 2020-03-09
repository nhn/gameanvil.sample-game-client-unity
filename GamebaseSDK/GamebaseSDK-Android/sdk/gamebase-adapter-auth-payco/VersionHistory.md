# Gamebase Authentication Payco Adapter for Android

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
  <groupId>com.payco.android</groupId>
  <artifactId>payco-login</artifactId>
  <version>1.4.1</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.google.android.gms</groupId>
  <artifactId>play-services-ads-identifier</artifactId>
  <version>16.0.0</version>
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
```

## Release Note

| Date | Payco Adapter Version | Gamebase SDK Version | Payco Login SDK Version | Description |
| ---- | --------------------- | -------------------- | ----------------------- | ----------- |
| 2017.03.21 | 1.1.1   | 1.1.0 ~ 1.1.2   | 1.2.8 ~ 1.4.1 | - Initial version |
| 2017.04.20 | 1.1.3   | 1.1.3 ~ 1.1.3.2 | 1.2.8 ~ 1.4.1 | - Change the Interface |
| 2017.04.28 | 1.1.3.2 | 1.1.3 ~ 1.1.3.2 | 1.2.8 ~ 1.4.1 | - Update Payco SDK to 1.2.9 |
| 2017.05.02 | 1.1.3.3 | 1.1.3.3         | 1.2.8 ~ 1.4.1 | - In the Gamebase sandbox environment, set the payco's operating environment to DEMO. |
| 2017.05.23 | 1.1.4   | 1.1.4 ~ 1.5.0   | 1.2.8 ~ 1.4.1 | - Update version |
| 2017.06.30 | 1.1.4.2 | 1.1.4.2 ~ 1.5.0 | 1.2.8 ~ 1.4.1 | - If it is a SANDBOX environment or the Gamebase service zone is not REAL, it will try to authenticate in Payco's DEMO environment. |
| 2017.07.19 | 1.1.5   | 1.1.5 ~ 1.5.0   | 1.2.8 ~ 1.4.1 | - Update version |
| 2017.09.15 | 1.2.0   | 1.1.5 ~ 1.5.0   | 1.2.8 ~ 1.4.1 | - Change zone type by TOAST Cloud project zone. Now BETA project will authenticate to the REAL NEOID. |
| 2018.02.22 | 1.7.0   | 1.7.0 ~ 1.15.0  | 1.2.8 ~ 1.4.1 | - Change the logic that refers from the console setting value.<br> - Update Payso SDK to 1.3.2 |
| 2019.01.29 | 2.0.0   | 2.0.0           | 1.2.8 ~ 1.4.1 | - Update Adapter Version due to Gamebase 2.0.0 Release |
| 2019.04.23 | 2.3.0   | 2.3.0 ~ 2.5.0   | 1.2.8 ~ 1.4.1 | - Change logout, withdraw interface. Fix a memory leak |
| 2019.10.29 | 2.6.0   | 2.6.0 ~ 2.6.2   | 1.2.8 ~ 1.4.1 | - Update Android support library to 28.0.0 from 27.0.2<br> - Update Payco SDK to 1.4.2 from 1.3.2 |
| 2020.01.21 | 2.7.0   | 2.7.0 ~         | 1.2.8 ~ 1.4.1 | - Change the logic that required parameter validate in initialize. |
| 2020.02.25 | 2.7.1   | 2.7.0 ~         | 1.2.8 ~ 1.4.1 | - Update comment and copyright. |
