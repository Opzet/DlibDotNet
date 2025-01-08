# Problem with Ms Surface Pro Arm64 Snapdragon 


Processor	Snapdragon(R) X 10-core X1P64100 @ 3.40 GHz   3.42 GHz
Installed RAM	16.0 GB (15.6 GB usable)
Product ID	00356-06300-15600-AAOEM
System type	64-bit operating system, ARM-based processor

Windows 11 Home 26100.2605

Requires DlibDotNetNative.dll but  xxx\.nuget\packages\dlibdotnet\19.21.0.20220724\runtimes doesn't have arm64 for snapdragon

## Trying to build

```
-------------------------------------------------------
-- CMAKE_SYSTEM_INFO_FILE: Platform/Windows
-- CMAKE_SYSTEM_NAME:      Windows
-- CMAKE_SYSTEM_PROCESSOR: ARM64
-- CMAKE_SYSTEM:           Windows-10.0.26100
-- CMAKE_C_COMPILER:       C:/Program Files/Microsoft Visual 
Studio/2022/Community/VC/Tools/MSVC/14.43.34604/bin/Hostarm64/x64/cl.exe
-- CMAKE_CXX_COMPILER:     C:/Program Files/Microsoft Visual 
Studio/2022/Community/VC/Tools/MSVC/14.43.34604/bin/Hostarm64/x64/cl.exe
```

>Powershell
```
pwsh Build.ps1 Release arm 64 desktop
```

```
 .\Build.ps1 Release arm 64 desktop
Build DlibDotNet.Native
Error: This plaform is not support
```


---
Argh.. I suc at cmake

umm https://github.com/takuya-takeuchi/DlibDotNet/tree/light/src/toolchains 
$BuildTargets += [BuildTarget]::new("uwp", "arm", 64, "${OperatingSystem}10-arm64", "" )

md Build
cd Build

> cmake -G "Visual Studio 17 2022" -A x64 ..
> cmake --build . --config Release
