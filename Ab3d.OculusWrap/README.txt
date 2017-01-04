Ab3d.OculusWrap solution contains the following projects:

1) Core projects:

- Ab3d.OculusWrap
  .Net wrapper for Oculus OVR library

- Ab3d.OculusWrap.DemoDX11
  Simple DirectX 11 demo of OculusWrap (using SharpDX)

- Ab3d.OculusWrap.UnitTests
  UnitTests for Ab3d.OculusWrap library

The core projects are based on the OculusWrap library that was created by MortInfinite (https://oculuswrap.codeplex.com/)


2) Projects that work with Ab3d.DXEngine and Ab3d.PowerToys libraries:

- Ab3d.DXEngine.OculusWrap
  Defines OculusWrapVirtualRealityProvider that can be used to enable rendering for Oculus Rift.
	
- Ab3d.DXEngine.OculusWrap.Sample
  Sample that shows how easy is turn on rendering for Oculus Rift and how easy is to create 3D content with Ab3d.DXEngine and Ab3d.PowerToys libraries.


Ab3d.DXEngine is a super fast 3D rendering engine that uses DirectX 11 and SharpDX and can be used in .Net Desktop applications.
Ab3d.DXEngine is very easy to use. You can use WPF 3D objects to define the 3D scene, cameras and lights.
To make working with 3D graphics even easier, the Ab3d.DXEngine also fully support the Ab3d.PowerToys library (see below).
More information about the Ab3d.DXEngine can be read on www.ab4d.com/DXEngine.aspx

Ab3d.PowerToys library is the ultimate helper library for WPF 3D graphics.
The library contains many helper classes that greatly simply work with 3D graphics.
It also comes with many samples that can be used to jump-start development.
More information about the Ab3d.PowerToys library can be read on www.ab4d.com/PowerToys.aspx

To compile the Ab3d.DXEngine.OculusWrap and Ab3d.DXEngine.OculusWrap.Sample projects,
the Ab3d.DXEngine and Ab3d.PowerToys libraries needs to be installed.
A 60-day evaluation version can be downloaded from www.ab4d.com/Downloads.aspx