namespace Shaders
{
    public class Primitives
    {
        public static readonly uint[] quadIndices =
        {
            0, 1, 2,
            2, 3, 0,
        };

        public static readonly float[] quadUvs =
        {
            0.0f, 0.0f, // Bottom left
            1.0f, 0.0f, // Bottom right
            1.0f, 1.0f, // Top right
            0.0f, 1.0f, // Top left
        };

        public static readonly float[] quadVertices =
        {
            -1f, -1f,  0.5f, // Bottom left
            1f, -1f,  0.5f, // Bottom right
            1f,  1f,  0.5f, // Top right
            -1f,  1f,  0.5f, // Top left
        };

        public static readonly float[] quadNormals =
        {
            0, 0, 1,
            0, 0, 1,
            0, 0, 1,
            0, 0, 1,
        };


        public static readonly uint[] cubeIndicies = 
        {
            // Front face
            0, 1, 2,
            2, 3, 0,

            // Back face
            4, 5, 6,
            6, 7, 4,

            // Top face
            8, 9, 10,
            10, 11, 8,

            // Bottom face
            12, 13, 14,
            14, 15, 12,

            // Left face
            16, 17, 18,
            18, 19, 16,

            // Right face
            20, 21, 22,
            22, 23, 20
        };

        public static readonly float[] cubeUVs =
        {
            // Front face
            0.0f, 0.0f, // Bottom left
            1.0f, 0.0f, // Bottom right
            1.0f, 1.0f, // Top right
            0.0f, 1.0f, // Top left

            // Back face
            1.0f, 0.0f, // Bottom left
            0.0f, 0.0f, // Bottom right
            0.0f, 1.0f, // Top right
            1.0f, 1.0f, // Top left

            // Top face
            0.0f, 1.0f, // Front left
            1.0f, 1.0f, // Front right
            1.0f, 0.0f, // Back right
            0.0f, 0.0f, // Back left

            // Bottom face
            0.0f, 0.0f, // Front left
            1.0f, 0.0f, // Front right
            1.0f, 1.0f, // Back right
            0.0f, 1.0f, // Back left

            // Left face
            1.0f, 0.0f, // Front bottom
            1.0f, 1.0f, // Front top
            0.0f, 1.0f, // Back top
            0.0f, 0.0f, // Back bottom

            // Right face
            0.0f, 0.0f, // Front bottom
            1.0f, 0.0f, // Front top
            1.0f, 1.0f, // Back top
            0.0f, 1.0f  // Back bottom
        };

        public static readonly float[] cubeVerts =
        {
            // Front face
            -0.5f, -0.5f,  0.5f, // Bottom left
            0.5f, -0.5f,  0.5f, // Bottom right
            0.5f,  0.5f,  0.5f, // Top right
            -0.5f,  0.5f,  0.5f, // Top left

            // Back face
            -0.5f, -0.5f, -0.5f, // Bottom left
            0.5f, -0.5f, -0.5f, // Bottom right
            0.5f,  0.5f, -0.5f, // Top right
            -0.5f,  0.5f, -0.5f, // Top left

            // Top face
            -0.5f,  0.5f,  0.5f, // Front left
            0.5f,  0.5f,  0.5f, // Front right
            0.5f,  0.5f, -0.5f, // Back right
            -0.5f,  0.5f, -0.5f, // Back left

            // Bottom face
            -0.5f, -0.5f,  0.5f, // Front left
             0.5f, -0.5f,  0.5f, // Front right
             0.5f, -0.5f, -0.5f, // Back right
            -0.5f, -0.5f, -0.5f, // Back left

            // Left face
            -0.5f, -0.5f,  0.5f, // Front bottom
            -0.5f,  0.5f,  0.5f, // Front top
            -0.5f,  0.5f, -0.5f, // Back top
            -0.5f, -0.5f, -0.5f, // Back bottom

            // Right face
            0.5f, -0.5f,  0.5f, // Front bottom
            0.5f,  0.5f,  0.5f, // Front top
            0.5f,  0.5f, -0.5f, // Back top
            0.5f, -0.5f, -0.5f  // Back bottom
       };

        public static readonly float[] cubeNormals =
        {
            0, 0, 1,
            0, 0, 1,
            0, 0, 1,
            0, 0, 1,

            0, 0, -1,
            0, 0, -1,
            0, 0, -1,
            0, 0, -1,

            0, 1, 0,
            0, 1, 0,
            0, 1, 0,
            0, 1, 0,

            0, -1, 0,
            0, -1, 0,
            0, -1, 0,
            0, -1, 0,

            -1, 0, 0,
            -1, 0, 0,
            -1, 0, 0,
            -1, 0, 0,

            1, 0, 0,
            1, 0, 0,
            1, 0, 0,
            1, 0, 0,
        };
    }

}