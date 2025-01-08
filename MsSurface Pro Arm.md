
# Problem with Ms Surface Pro Arm64 Snapdragon 

Requires DlibDotNetNative.dll but  xxx\.nuget\packages\dlibdotnet\19.21.0.20220724\runtimes doesn't have arm64 for snapdragon

## Trying to build


umm https://github.com/takuya-takeuchi/DlibDotNet/tree/light/src/toolchains 
$BuildTargets += [BuildTarget]::new("uwp", "arm", 64, "${OperatingSystem}10-arm64", "" )

md Build
cd Build

> cmake -G "Visual Studio 17 2022" -A x64 ..
> cmake --build . --config Release
