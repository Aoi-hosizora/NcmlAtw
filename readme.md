# NeteaseLyric_Mobile2Desktop

+ Subscribe netease music application in Android, and show the current music information and lyric on Windows.
+ Development environment: `Netease Music V6.4.0.680721` `Android Studio 3.4.1` `Visual C# 2010` `.NET Framework 4.0`.

### Depedencies

+ Android
    + `butterknife 8.8.1`
    + `LibZXing 1.1.2`
+ Windows
    + `Newtonsoft.Json 12.0.0.0`
    + `QRCoder 1.3.6.0`

### Usage Tips

+ Before start service you should open netease music first, because `MediaController` could not find the session.

### Screenshot

|[!android](./assets/Android.jpg)|[!windows](./assets/Windows.jpg)|[!setting](./assets/Setting.jpg)|
|---|---|---|

### References

+ [Using MediaController on Android 5](https://stackoverflow.com/questions/27107212/using-mediacontroller-on-android-5)
+ [NeteaseMuiscApi](https://github.com/GEEKiDoS/NeteaseMuiscApi)
