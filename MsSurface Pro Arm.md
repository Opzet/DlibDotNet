# Problem with Ms Surface Pro Arm64 Snapdragon 

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


umm https://github.com/takuya-takeuchi/DlibDotNet/tree/light/src/toolchains 
$BuildTargets += [BuildTarget]::new("uwp", "arm", 64, "${OperatingSystem}10-arm64", "" )

md Build
cd Build

> cmake -G "Visual Studio 17 2022" -A x64 ..
> cmake --build . --config Release
