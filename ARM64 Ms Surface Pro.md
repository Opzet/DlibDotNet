# ARM64 Ms Surface Pro

Start here : https://github.com/Opzet/DlibDotNet/examples/WinForms/FaceDetection/FaceDetectionDemo.csproj
![image](https://github.com/user-attachments/assets/697079a9-6ec9-402d-94d7-8a5589443e66)


## WindowsLibraryLoader Error 

DlibDotNet.ShapePredictor.Deserialize(String path)

Issue with ***WindowsLibraryLoader*** Dlib .NET wrapper written in C++ and C# for Windows, MacOS, Linux and iOS - Opzet/DlibDotNet

![image](https://github.com/user-attachments/assets/391d4cc0-1d03-4a2d-a1c6-689e200c0422)

## NativeLibrary DlibDotNetNative.dll
NativeLibrary source (\DlibDotNet\src\dlib) -> ***DlibDotNetNative.dll***
WindowsLibraryLoader (odd..  {"ARM", "WinCE"}) 
 how compiled  \DlibDotNet\src\dlib c++ for  Surface laptop powered by ARM-based Snapdragon X series processors, require ARM64?
```csharp
internal sealed class WindowsLibraryLoader
{
    #region Fields
    private const string ProcessorArchitecture = "PROCESSOR_ARCHITECTURE";
    private const string DllFileExtension = ".dll";
    private const string DllDirectory = "dll";
    private readonly Dictionary<string, int> _ProcessorArchitectureAddressWidthPlatforms = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
         {
            {"x86", 4},
            {"AMD64", 8},
            {"IA64", 8},
            {"ARM", 4}
        };
 ```
 
 ```csharp
 
    internal sealed partial class NativeMethods
    {
         #region Fields
 
        /// <summary>
        /// Native library file name.
        /// If Linux, it will be converted to  libDlibDotNetNative.so
        /// If MacOSX, it will be converted to  libDlibDotNetNative.dylib
        /// If Windows, it will be available after call LoadLibrary.
        /// And this file name must not contain period. If it does,
        /// CLR does not add extension (.dll) and CLR fails to load library
        /// </summary>

#if LIB_STATIC
        public const string NativeLibrary = "__Internal";
        public const string NativeDnnLibrary = "__Internal";
#else
        public const string NativeLibrary = "DlibDotNetNative";
        public const string NativeDnnLibrary = "DlibDotNetNativeDnn";
#endif
        public const CallingConvention CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl;
        private static readonly WindowsLibraryLoader WindowsLibraryLoader = new WindowsLibraryLoader();
#endregion
 
#region Constructors
        static NativeMethods()
        {
            WindowsLibraryLoader.LoadLibraries(new[]
            {
                $"{NativeLibrary}",
                $"{NativeDnnLibrary}"
            });
```

 
