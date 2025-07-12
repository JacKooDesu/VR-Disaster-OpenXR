# Commands

## Recording

.\scrcpy.exe --tcpip=+192.168.1.102 --crop 1730:974:1934:450 --max-fps 30 -b 2M --no-audio -w --print-fps --video-codec=h265 --record=file.mkv --no-playback

## Installing

1. Connect device

```bash
adb devices
```

2. Installing

```bash
adb install <build.apk>
```

WIP
