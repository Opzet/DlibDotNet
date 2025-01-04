echo off

ech Fix: Error MSB3030 Could not copy the file "C:\Users\david\.nuget\packages\dlibdotnet\19.21.0.20210302\runtimes\win-AnyCPU\native\DlibDotNetNativeDnn.dll" because it was not found.

echo Looking for C:\Users\%username%

IF EXIST C:\Users\%username%\ (
GOTO usernameOK 
) ELSE (
GOTO MissingUser 
)

:usernameOK
Echo C:\Users\%username% EXIST
IF EXIST "C:\Users\%username%\.nuget\packages\dlibdotnet\19.21.0.20210302\runtimes\win-AnyCPU\" ( 
	ECHO folder anyCPU already exists
) ELSE ( 
ECHO AnyCPU Foldlder does not exist, copying x64 to anyCPU
 xcopy "C:\Users\%username%\.nuget\packages\dlibdotnet\19.21.0.20210302\runtimes\win-x64\"  "C:\Users\%username%\.nuget\packages\dlibdotnet\19.21.0.20210302\runtimes\win-AnyCPU\" /E /I /Y )

GOTO end

:MissingUser
Echo Missing User %username% - using predefined
IF EXIST C:\Users\David\.nuget\packages\dlibdotnet\19.21.0.20210302\runtimes\win-AnyCPU\ (
ECHO folder C:\Users\David\ ... anyCPU already exists
) ELSE ( 
xcopy "C:\Users\David\.nuget\packages\dlibdotnet\19.21.0.20210302\runtimes\win-x64\" "C:\Users\David\.nuget\packages\dlibdotnet\19.21.0.20210302\runtimes\win-AnyCPU\" /E /I /Y )


:end

Echo Success, AnyCPU setup for dlibdotnet\19.21.0.20210230
Pause 