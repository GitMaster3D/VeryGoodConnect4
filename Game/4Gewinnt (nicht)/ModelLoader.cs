using OpenTK.Mathematics;
using System.Globalization;

namespace objLoader
{
    public class ObjImporter
    {
        public static void ParseOBJ(string filePath, out float[] vertices, out uint[] indices, out float[] normals, out float[] texcoords)
        {
            filePath = AppDomain.CurrentDomain.BaseDirectory + $"Models/{filePath}";

            List<float> vertexList = new List<float>();
            List<uint> indexList = new List<uint>();
            List<float> normalList = new List<float>();
            List<float> texcoordList = new List<float>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(' ');

                    switch (parts[0])
                    {
                        case "v":
                            vertexList.Add(float.Parse(parts[1], CultureInfo.InvariantCulture));
                            vertexList.Add(float.Parse(parts[2], CultureInfo.InvariantCulture));
                            vertexList.Add(float.Parse(parts[3], CultureInfo.InvariantCulture));
                            break;

                        case "vt":
                            texcoordList.Add(float.Parse(parts[1], CultureInfo.InvariantCulture));
                            texcoordList.Add(float.Parse(parts[2], CultureInfo.InvariantCulture));
                            break;

                        case "vn":
                            normalList.Add(float.Parse(parts[1], CultureInfo.InvariantCulture));
                            normalList.Add(float.Parse(parts[2], CultureInfo.InvariantCulture));
                            normalList.Add(float.Parse(parts[3], CultureInfo.InvariantCulture));
                            break;

                        case "f":
                            int numVertices = parts.Length - 1;
                            uint[] faceIndices = new uint[numVertices];
                            for (int i = 0; i < numVertices; i++)
                            {
                                string[] subParts = parts[i + 1].Split('/');
                                uint index;
                                if (uint.TryParse(subParts[0], out index))
                                {
                                    faceIndices[i] = index - 1;
                                }
                                else
                                {

                                    continue;
                                }

                                if (subParts.Length >= 2 && !string.IsNullOrEmpty(subParts[1]))
                                {
                                    int texcoordIndex;
                                    if (int.TryParse(subParts[1], out texcoordIndex))
                                    {
                                        texcoordIndex -= 1;
                                        if (texcoordIndex * 2 + 1 < texcoordList.Count)
                                        {
                                            texcoordList.CopyTo(texcoordIndex * 2, vertexList.ToArray(), vertexList.Count - 2, 2);
                                        }
                                    }
                                }

                                if (subParts.Length >= 3 && !string.IsNullOrEmpty(subParts[2]))
                                {
                                    int normalIndex;
                                    if (int.TryParse(subParts[2], out normalIndex))
                                    {
                                        normalIndex -= 1;
                                        if (normalIndex * 3 + 2 < normalList.Count)
                                        {
                                            normalList.CopyTo(normalIndex * 3, normalList.ToArray(), normalList.Count - 3, 3);
                                        }
                                    }
                                }
                            }

                            for (int i = 0; i < numVertices - 2; i++)
                            {
                                indexList.Add(faceIndices[0]);
                                indexList.Add(faceIndices[i + 1]);
                                indexList.Add(faceIndices[i + 2]);
                            }
                            break;
                    }
                }
            }

            vertices = vertexList.ToArray();
            indices = indexList.ToArray();
            normals = CalculateNormals(vertices, CalculateTriangles(vertices, indices));
            texcoords = texcoordList.ToArray();
        }



        public static float[] CalculateNormals(float[] vertices, int[] triangles)
        {
            if (vertices.Length % 3 != 0)
            {
                throw new ArgumentException("Vertices array length must be divisible by 3", nameof(vertices));
            }

            if (triangles.Length % 3 != 0)
            {
                throw new ArgumentException("Triangles array length must be divisible by 3", nameof(triangles));
            }

            var normals = new Vector3[vertices.Length / 3];

            for (var i = 0; i < triangles.Length; i += 3)
            {
                var i1 = triangles[i] * 3;
                var i2 = triangles[i + 1] * 3;
                var i3 = triangles[i + 2] * 3;

                var v1 = new Vector3(vertices[i1], vertices[i1 + 1], vertices[i1 + 2]);
                var v2 = new Vector3(vertices[i2], vertices[i2 + 1], vertices[i2 + 2]);
                var v3 = new Vector3(vertices[i3], vertices[i3 + 1], vertices[i3 + 2]);

                var normal = Vector3.Cross(v2 - v1, v3 - v1);

                normals[triangles[i]] += normal;
                normals[triangles[i + 1]] += normal;
                normals[triangles[i + 2]] += normal;
            }

            var result = new float[normals.Length * 3];

            for (var i = 0; i < normals.Length; i++)
            {
                normals[i] = Vector3.Normalize(normals[i]);
                result[i * 3] = normals[i].X;
                result[i * 3 + 1] = normals[i].Y;
                result[i * 3 + 2] = normals[i].Z;
            }

            return result;
        }

        public static int[] CalculateTriangles(float[] vertices, uint[] indices)
        {
            if (vertices.Length % 3 != 0)
            {
                throw new ArgumentException("Vertices array length must be divisible by 3", nameof(vertices));
            }

            if (indices.Length % 3 != 0)
            {
                throw new ArgumentException("Indices array length must be divisible by 3", nameof(indices));
            }

            var triangles = new int[indices.Length];

            for (var i = 0; i < indices.Length; i++)
            {
                triangles[i] = (int)indices[i];
            }

            return triangles;
        }

    }
}