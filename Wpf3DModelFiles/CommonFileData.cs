using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Linq;

namespace Wpf3DModelFiles
{
    public class CommonFileData
    {
        public int ID { get; set; }
        public Point3DCollection Positions { get; set; }
        public Vector3DCollection Normals { get; set; }
        public Int32Collection TriangleIndices { get; set; }
        public PointCollection TextureCoordinates { get; set; }
        public Matrix3D Transform { get; set; } = Matrix3D.Identity;
        public Color MaterialColor { get; set; }
        public string MaterialFilePath { get; set; } = string.Empty;

        private AutoCounter _indexCounter = null;
        private  Dictionary<Point3D, int> _positionDictionary = null;

        public CommonFileData()
        {
            ID = -1;
            Positions = new Point3DCollection();
            Normals = new Vector3DCollection();
            TriangleIndices = new Int32Collection();
            TextureCoordinates = new PointCollection();
            Transform = Matrix3D.Identity;
            MaterialColor = Colors.DarkSlateGray;
            _indexCounter = new AutoCounter();
            _positionDictionary = new Dictionary<Point3D, int>();
        }

        public void AddTriangle3DCollection(IEnumerable<Triangle3D> triangles)
        {
            foreach (Triangle3D triangle in triangles)
            {
                AddTriangle3D(triangle);
            }
        }

        public void AddTriangle3D(Triangle3D triangle)
        {
            int indexA = -1;
            int indexB = -1;
            int indexC = -1;

            if (!_positionDictionary.ContainsKey(triangle.A.Location))
            {
                indexA = _indexCounter.GetNext();
                _positionDictionary[triangle.A.Location] = indexA;
                Positions.Add(triangle.A.Location);
                triangle.A.Index = indexA;
            }
            else
            {
                indexA = _positionDictionary[triangle.A.Location];
            }

            if (!_positionDictionary.ContainsKey(triangle.B.Location))
            {
                indexB = _indexCounter.GetNext();
                _positionDictionary[triangle.B.Location] = indexB;
                Positions.Add(triangle.B.Location);
                triangle.B.Index = indexB;
            }
            else
            {
                indexB = _positionDictionary[triangle.B.Location];
            }

            if (!_positionDictionary.ContainsKey(triangle.C.Location))
            {
                indexC = _indexCounter.GetNext();
                _positionDictionary[triangle.C.Location] = indexC;
                Positions.Add(triangle.C.Location);
                triangle.C.Index = indexC;
            }
            else
            {
                indexC = _positionDictionary[triangle.C.Location];
            }

            TriangleIndices.Add(indexA);
            TriangleIndices.Add(indexB);
            TriangleIndices.Add(indexC);

            if (triangle.Normal != default(Vector3D))
            {
                Normals.Add(triangle.Normal);
            }
        }

        public static CommonFileData FromTriangle3DCollection(IEnumerable<Triangle3D> triangles)
        {
            CommonFileData result = new CommonFileData();
            result.AddTriangle3DCollection(triangles);
            return result;
        }

        public static CommonFileData FromMeshGeometry3D(MeshGeometry3D meshGeometry3D)
        {
            CommonFileData result = new CommonFileData();

            if (meshGeometry3D.Positions != null && meshGeometry3D.Positions.Any())
            {
                result.Positions = new Point3DCollection(meshGeometry3D.Positions.Select(p => new Point3D(p.X, p.Y, p.Z)));
            }

            if (meshGeometry3D.TriangleIndices != null && meshGeometry3D.TriangleIndices.Any())
            {
                result.TriangleIndices = new Int32Collection(meshGeometry3D.TriangleIndices.ToList());
            }

            if (meshGeometry3D.TextureCoordinates != null && meshGeometry3D.TextureCoordinates.Any())
            {
                result.TextureCoordinates = new PointCollection(meshGeometry3D.TextureCoordinates.Select(p => new System.Windows.Point(p.X, p.Y)));
            }

            if (meshGeometry3D.Normals != null && meshGeometry3D.Normals.Any())
            {
                result.Normals = new Vector3DCollection(meshGeometry3D.Normals.Select(v => new Vector3D(v.X, v.Y, v.Z)));
            }

            return result;
        }

        public static CommonFileData FromGeometryModel3D(GeometryModel3D geometryModel3D)
        {
            MeshGeometry3D meshGeometry3D = geometryModel3D.Geometry as MeshGeometry3D;
            if (meshGeometry3D == null)
            {
                throw new ArgumentException("GeometryModel3D's Geometry Dependency Property must be of the type MeshGeometry3D for this to work.");
            }

            CommonFileData result = FromMeshGeometry3D(meshGeometry3D);

            if (geometryModel3D.Transform.Value != Matrix3D.Identity)
            {
                result.Transform = geometryModel3D.Transform.Value;
            }

            if (geometryModel3D.Material != null)
            {
                Color? color = ExtractMaterialColor(geometryModel3D.Material);

                if (color.HasValue)
                {
                    result.MaterialColor = color.Value;
                }
            }

            return result;
        }

        private static Color? ExtractMaterialColor(Material material)
        {
            if (material != null)
            {
                DiffuseMaterial diffuseMaterial = material as DiffuseMaterial;
                if (diffuseMaterial != null)
                {
                    return diffuseMaterial.Color;
                }

                EmissiveMaterial emissiveMaterial = material as EmissiveMaterial;
                if (emissiveMaterial != null)
                {
                    return emissiveMaterial.Color;
                }

                SpecularMaterial specularMaterial = material as SpecularMaterial;
                if (specularMaterial != null)
                {
                    return specularMaterial.Color;
                }

                MaterialGroup materialGroup = material as MaterialGroup;
                if (materialGroup != null)
                {
                    foreach (Material mat in materialGroup.Children)
                    {
                        Color? color = ExtractMaterialColor(mat);
                        if (color.HasValue)
                        {
                            return color;
                        }
                    }
                }
            }

            return null;
        }

        public string ToXamlString()
        {
            GeometryModel3D model = ToGeometryModel3D();

            ModelUIElement3D uIElement3D = new ModelUIElement3D();
            uIElement3D.Model = model;

            Viewport3D viewport3D = new Viewport3D();
            viewport3D.Children.Add(uIElement3D);

            Canvas canvas = new Canvas();
            canvas.Children.Add(viewport3D);

            Viewbox viewbox = new Viewbox();
            viewbox.Child = canvas;

            Grid grid = new Grid();
            grid.Children.Add(viewbox);

            Window window = new Window();
            window.Content = grid;

            StringBuilder sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb, CultureInfo.InvariantCulture))
            using (XmlTextWriter xmlWriter = new XmlTextWriter(writer))
            {
                XamlDesignerSerializationManager serializationManager = new XamlDesignerSerializationManager(xmlWriter);
                serializationManager.XamlWriterMode = XamlWriterMode.Value;

                XamlWriter.Save(window, serializationManager);
            }

            return sb.ToString();
        }

        public MeshGeometry3D ToMeshGeometry3D()
        {
            var result = new MeshGeometry3D()
            {
                Positions = Positions,
                TriangleIndices = TriangleIndices
            };

            if (TextureCoordinates.Any())
            {
                result.TextureCoordinates = TextureCoordinates;
            }

            if (Normals.Any())
            {
                result.Normals = Normals;
            }

            return result;
        }

        public GeometryModel3D ToGeometryModel3D()
        {
            return new GeometryModel3D()
            {
                Geometry = ToMeshGeometry3D(),
                Transform = new MatrixTransform3D(Transform)
            };
        }

        public List<Triangle3D> ToTriangle3DCollection()
        {
            Vertex3D[] vertices = Positions.Select(pnt3d => new Vertex3D(pnt3d)).ToArray();

            var triangleIndexTriples = TriangleIndices.Chunk(3).ToList();
            var results = triangleIndexTriples
                    .Select(chk => new Triangle3D(vertices[chk[0]], vertices[chk[1]], vertices[chk[2]]))
                    .ToList();

            if (results.Count == Normals.Count)
            {
                int index = 0;
                int max = results.Count;
                while (index < max)
                {
                    results[index].Normal = Normals[index];
                    index++;
                }
            }

            return results;
        }

    }
}
