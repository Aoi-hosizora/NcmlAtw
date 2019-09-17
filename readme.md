# NeteaseLyric_Mobile2Desktop
+ Project the android netease music app lyric to desktop

### Development Environment
+ Android
    + `Android 8.0.0`
    + `Netease Music V6.4.0.680721`
    + `Android Studio 3.4.1`
+ Window
    + `Visual C# 2010`
    + `.NET Framework 4.0`

### Depedencies
+ Android
    + `butterknife 8.8.1`
    + `LibZXing v1.1.2`
+ Windows
    + `Newtonsoft.Json 12.0.0.0`
    + `QRCoder 1.3.6.0`

### Usage Tips
+ Before start service you should open netease music first, for `MediaController` could not find the session.
+ Before start service you should listen the currect port in windows first, for the socket in android will ping the port in LAN when starting service.

### Screenshot

<p align="center">
<img src="https://raw.githubusercontent.com/Aoi-hosizora/NeteaseLyric_Mobile2Desktop/master/assets/Android.jpg"></img>
</p>
<p align="center">
<img src="https://raw.githubusercontent.com/Aoi-hosizora/NeteaseLyric_Mobile2Desktop/master/assets/Windows.jpg"></img>
</p>
<p align="center">
<img src="https://raw.githubusercontent.com/Aoi-hosizora/NeteaseLyric_Mobile2Desktop/master/assets/Setting.jpg"></img>
</p>

### Delelopment Tips
+ Get netease music song metadata and playstate (Android)
    + See [MainService.java](https://github.com/Aoi-hosizora/NeteaseLyric_Mobile2Desktop/blob/master/NeteaseM2DClient/app/src/main/java/com/aoihosizora/neteasem2dclient/MainService.java)
    + Use `NotificationListenerService` and `MediaController` to subscribe notification.
    + Inherit from `MediaController.Callback` and override `onPlaybackStateChanged` and `onMetadataChanged` to handle notification.
    + Use `java.net.Socket` to send Json through TCP socket in LAN
+ Allow to subscribe the applications notification (Android)
    + See functions `onCreate()` and `isNotificationListenersEnabled()` in [MainActivity.java](https://github.com/Aoi-hosizora/NeteaseLyric_Mobile2Desktop/blob/master/NeteaseM2DClient/app/src/main/java/com/aoihosizora/neteasem2dclient/MainActivity.java)
    + Open intent `android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS`
    + Allow to run in background to prevent being killed by system. See screenshot.
+ Cross over the window (Windows)
    + See functon `setWindowCrossOver()` in [CommonUtil.cs](https://github.com/Aoi-hosizora/NeteaseLyric_Mobile2Desktop/blob/master/NeteaseM2DServer/NeteaseM2DServer/Src/Util/CommonUtil.cs)
    + Use windows API `SetWindowLong` and `SetLayeredWindowAttributes` and set attr to `WS_EX_TRANSPARENT | WS_EX_LAYERED` or `WS_EX_LAYERED`
+ Search song in netease music (Windows)
    + See function `Search()` in [MainForm.cs](https://github.com/Aoi-hosizora/NeteaseLyric_Mobile2Desktop/blob/master/NeteaseM2DServer/NeteaseM2DServer/Src/UI/MainForm.cs) and [NeteaseMuiscApi](https://github.com/GEEKiDoS/NeteaseMuiscApi)
    + Check song info consistency through check `searchRet.Al.Name` and `searchRet.Name` in `checkSearchResult(Song, Metadata)`
    + Search strategies see functions `Search()` and `checkContinueResult(SearchResult, Metadata)`
+ Lyric parse and animation (Windows)
    + See functions `timerLyric_Tick()` and `timerLabelText_Tick()` in [LyricForm.cs](https://github.com/Aoi-hosizora/NeteaseLyric_Mobile2Desktop/blob/master/NeteaseM2DServer/NeteaseM2DServer/Src/UI/LyricForm.cs)

### References
+ [Using MediaController on Android 5](https://stackoverflow.com/questions/27107212/using-mediacontroller-on-android-5)
+ [NeteaseMuiscApi](https://github.com/GEEKiDoS/NeteaseMuiscApi)