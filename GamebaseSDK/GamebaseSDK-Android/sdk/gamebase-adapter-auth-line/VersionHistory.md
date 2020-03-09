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
  <artifactId>customtabs</artifactId>
  <version>28.0.0</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>com.linecorp</groupId>
  <artifactId>linesdk</artifactId>
  <version>4.0.8</version>
  <scope>compile</scope>
</dependency>
<dependency>
  <groupId>org.jetbrains.kotlin</groupId>
  <artifactId>kotlin-stdlib-jdk7</artifactId>
  <version>1.3.40</version>
  <scope>compile</scope>
</dependency>
```

### Reference of Line Android Developer's guide.

* https://developers.line.me/en/docs/line-login/android/integrate-line-login/

## Release Note

| Date | Line Adapter Version | Gamebase SDK Version | Line SDK Version | Description |
| ---- | -------------------- | -------------------- | ---------------- | ----------- |
| 2018.06.26 | 1.11.0 | 1.11.0 ~ 1.15.0 | 4.0.8 | - Initial version |
| 2019.01.29 | 2.0.0  | 2.0.0           | 4.0.8 | - Update Adapter Version due to Gamebase 2.0.0 Release |
| 2019.04.23 | 2.3.0  | 2.3.0  ~ 2.5.0  | 4.0.8 | - Change logout, withdraw interface. Fix a memory leak |
| 2019.10.29 | 2.6.0  | 2.6.0  ~ 2.6.2  | 4.0.8 | - Update Android support library to 28.0.0 from 27.0.2 |
| 2020.01.21 | 2.7.0  | 2.7.0  ~        | 4.0.8 | - Change the logic that required parameter validate in initialize. |
| 2020.02.25 | 2.7.1  | 2.7.0 ~         | 4.0.8 | - Update comment and copyright. |
