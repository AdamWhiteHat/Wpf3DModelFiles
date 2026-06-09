# Wpf3DModelFiles

A C# .NET Core library that parses common 3D model file formats into data types used by Windows Presentation Foundation (WPF).

Here, this diagram should explain it:

![Wpf3DModelFiles Diagram Showing What Types Convert to What Types](https://raw.githubusercontent.com/AdamWhiteHat/Wpf3DModelFiles/refs/heads/master/Wpf3DModelFilesDiagram.png)

The key trick here is the class `CommonFileData`, a common intermediate representation of a 3D model. `CommonFileData` can freely convert to and from a:
- .3MF file
- .STL file
- MeshGeometry3D
- GeometryModel3D
- List&lt;Triangle3D&gt;
or (output only) to a .XAML markup file.

