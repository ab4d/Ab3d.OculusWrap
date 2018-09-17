# Ab3d.OculusWrap
Ab3d.OculusWrap is a .Net wrapper for Oculus Rift SDK.

![Oculus Rift with Ab3d.DXEngine DirectX 11 rendering](https://github.com/ab4d/Ab3d.OculusWrap/wiki/images/DXEngine-OculusWrap-890.jpg)

The current version of Ab3d.OculusWrap is built for Oculus SDK 1.17.0 (2017-08-10).

The library is also available as a NuGet package: Ab3d.OculusWrap (https://www.nuget.org/packages/Ab3d.OculusWrap/)

The project is based on the OculusWrap library that was created by MortInfinite (https://oculuswrap.codeplex.com/).
Because the original OculusWrap library was discontinued on April 2016 (with support for Oculus SDK 1.3.2), 
I have decided to update and cleanup the library.


The Ab3d.OculusWrap library provides managed classes, structs, enums and function calls that are required to work with native Oculus LibOVR library.
The Ab3d.OculusWrap provides raw implementation of the Oculus library and does not define any special helper classes. This means that converting existing C++ sample code into .Net code should be easy.
Because the Ab3d.OculusWrap do not required a reference to a native library, it can be used for Any CPU, x86 or x64 target platform.

The library comes with simple DirectX 11 demo project that uses SharpDX.
It also comes with a sample project that shows how to use the library with the Ab3d.DXEngine library (DirectX 11 rendering engine for .Net).
See below for more info.



### Major changes from the original OculusWrap library

- No more safe and unsafe implementation. All calls to native library now use SuppressUnmanagedCodeSecurity attribute. This provides substantial performance savings because .Net runtime does not need to perform stack walk security checks before each call to native library. Also, using the LibOVR library do not provide a security threat because it does not allow the called to do some unintended actions – for example access to file system or script execution. The same attribute is also used by the SharpDX library.
- Improved definition of some structs and classes with using more standard types – for example bool instead of byte.
- Renamed some names to provide more common naming convention style – for example renames some upper case enum values or removed “_out” suffixes.
- Removed MirrorTexture and TextureSwapChain helper classes from the wrapper library into the demo project.


### First time usage guide

1. Connect Oculus Rift and install Oculus runtime.
2. Make sure that you have "Unknown Sources" enabled in Oculus - Settings - General. This will allow starting applications that are not downloaded from Oculus Store.
3. Open Ab3d.OculusWrap solution in Visual Studio
4. Compile the solution. If you do not have Ab3d.DXEngine and Ab3d.PowerToys installed, you will not be able to compile the Ab3d.DXEngine.OculusWrap and Ab3d.DXEngine.OculusWrap.Sample projects. But you can still start the Ab3d.OculusWrap.DemoDX11 project with right clicking on the project and then selecting Debug -> Start new instance.
5. To try the Ab3d.DXEngine.OculusWrap.Sample, you need to install the Ab3d.DXEngine and Ab3d.PowerToys libraries. This can be done with downloading an evaluation installer from https://www.ab4d.com/Downloads.aspx. After that check that the references to Ab3d.DXEngine and Ab3d.PowerToys are correct and recompile the solution.


### Projects in the Ab3d.OculusWrap solution

1) Core projects:

- **Ab3d.OculusWrap**  
  .Net wrapper for Oculus OVR library (no third-party library required)

- **Ab3d.OculusWrap.DemoDX11**  
  Simple DirectX 11 demo of OculusWrap (using SharpDX)

- **Ab3d.OculusWrap.UnitTests**  
  UnitTests for Ab3d.OculusWrap library (using SharpDX)

The core projects are based on the OculusWrap library that was created by MortInfinite (https://oculuswrap.codeplex.com/)

The projects are compiled for .Net 4.5 and are using SharpDX 4.2.

2) Projects that work with Ab3d.DXEngine and Ab3d.PowerToys libraries:

- **Ab3d.DXEngine.OculusWrap**  
  Defines OculusWrapVirtualRealityProvider that can be used to enable rendering for Oculus Rift.
	
- **Ab3d.DXEngine.OculusWrap.Sample**  
  This sample allows you to use XBOX controller to walk around a simple 3D structure. The sample shows how easy is to create 3D content with Ab3d.DXEngine and Ab3d.PowerToys librariesto and that enabling Oculus Rift rendering required only a few additional lines of code.

The following screenshot is showing the Ab3d.DXEngine.OculusWrap.Sample. And as you can see from the the window title and Oculus Performance overlay it has no problems with rendering at 90 FPS and still having plently of performance headroom.

[Ab3d.DXEngine](https://www.ab4d.com/DXEngine.aspx) is a super fast 3D rendering engine that uses DirectX 11 and SharpDX and can be used in .Net Desktop applications.
Ab3d.DXEngine is very easy to use. You can use WPF 3D objects to define the 3D scene, cameras and lights.
To make working with 3D graphics even easier, the Ab3d.DXEngine also fully support the Ab3d.PowerToys library (see below).
More information about Ab3d.DXEngine can be read on https://www.ab4d.com/DXEngine.aspx

[Ab3d.PowerToys](https://www.ab4d.com/PowerToys.aspx) library is the ultimate helper library for WPF 3D graphics.
The library contains many helper classes that greatly simply work with 3D graphics.
It also comes with many samples that can be used to jump-start development.
More information about the Ab3d.PowerToys library can be read on https://www.ab4d.com/PowerToys.aspx.

To compile the Ab3d.DXEngine.OculusWrap and Ab3d.DXEngine.OculusWrap.Sample projects, the Ab3d.DXEngine and Ab3d.PowerToys libraries needs to be installed.
A 60-day evaluation version can be downloaded from https://www.ab4d.com/Downloads.aspx.
