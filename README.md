# VR Disaster OpenXR

Rework version of [VR Disaster XRSpace](https://github.com/JacKooDesu/VR-Disaster-XRSpcae).

## Center Controller Extend

[repo](https://github.com/JacKooDesu/center-controller-rust)

Highly recommend to use the extended version of center controller, which is a rework version with history exporting with udp and format view.

Install with `Powershell`:

```powershell
irm https://raw.githubusercontent.com/JacKooDesu/center-controller-rust/refs/heads/main/ps/installer.ps1 | iex
```

Or download the correct architecture executable from the [release page](https://github.com/JacKooDesu/center-controller-rust/releases/latest/).

## Development

### Commands

#### Recording

```bash adb
.\scrcpy.exe --tcpip=+192.168.1.102 --crop 1730:974:1934:450 --max-fps 30 -b 2M --no-audio -w --print-fps --video-codec=h265 --record=file.mkv --no-playback
```

#### Installing

1. Connect device

```bash
adb devices
```

2. Installing

```bash
adb install <build.apk>
```

#### ADB tcpip (remote debug / scrcpy)

1. Shell ip route check ip

```bash
adb shell ip route
```

2. Start ADB in tcpip

```bash
adb tcpip <port>
adb connect <ipaddress>:<port>
```

WIP
