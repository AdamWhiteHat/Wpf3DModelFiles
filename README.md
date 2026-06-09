# Wpf3DModelFiles

A C# .NET Core library that parses common 3D model file formats into data types used by Windows Presentation Foundation (WPF).

The key trick here is the class `CommonFileData`, a common intermediate representation of a 3D model. `CommonFileData` can freely convert to and from a:
- .3MF file
- .STL file
- MeshGeometry3D
- GeometryModel3D
- List<Triangle3D>

or (output only) to a .XAML markup file.

Here is a diagram of that:




