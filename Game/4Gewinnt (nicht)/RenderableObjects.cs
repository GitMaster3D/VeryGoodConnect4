using _4Gewinnt__nicht_;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Shaders;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Rendering
{
    public class RenderableObject
    {
        public Vector3 position { get; set; }
        public Vector3 scale { get; set; }
        public Vector3 rotation;

        public Matrix4 transform;
        public Texture tex;
        public Vector3 offset;

        public bool isShadowCaster = true;
        public string shader = "Default";
        private bool isStatic;

        public int indiceCount
        {
            get { return indices.Length; }
            private set { Logging.LogError("Tryed to write to indiceCount"); } // Make indicecount impossible to override
        }

        public bool enabled = true;


        private float[] vertices;
        private float[] uv;
        private float[] normals;
        public uint[] indices;

        public float alpha = 1f;

        public int vao;
        public int vertexBufferObject;
        public int uvBufferObject;
        public int normalBufferObject;
        private int elementBufferObject;

        public Material material;

        public bool isValid = true; // if this object can be safely rendered

        private Renderer renderer;



        public RenderableObject(Renderer renderer, string shader, float[] vertices, float[] uv, uint[] indices, float[] normals, bool isStatic)
        {
            InitDefaults();

            this.isStatic = isStatic;
            this.shader = shader;
            this.renderer = renderer;

            vertexBufferObject = renderer.vertexBufferObject;
            uvBufferObject = renderer.uvBufferObject;
            normalBufferObject = renderer.normalBufferObject;

            this.vertices = vertices;
            this.uv = uv;
            this.indices = indices;
            this.normals = normals;

            // Check if buffers are valid
            if (this.vertices.Length <= 0) { Logging.LogError($"vertices is empty! buffer: {vertexBufferObject}"); isValid = false; }
            if (this.uv.Length <= 0) { Logging.LogError($"uv is empty! buffer: {uvBufferObject}"); isValid = false; }
            if (this.indices.Length <= 0) { Logging.LogError($"indices is empty! buffer: {elementBufferObject}"); isValid = false; }
            if (this.normals.Length <= 0) { Logging.LogError($"normals is empty! buffer: {normalBufferObject}"); isValid = false; }

            Build();
            AddToRenderer();
        }

        void InitDefaults()
        {
            transform = Matrix4.Identity;
            rotation = Vector3.Zero;

            material = new Material();
            material.shininess = 64f;
            material.ambient = new Vector3(0.1f, 0.1f, 0.1f);
            material.diffuse = new Vector3(0.5f, 0.5f, 0.5f);
            material.specular = new Vector3(1f, 1f, 1f);
        }

        /// <summary>
        /// Calculates Transformation Matrix from
        /// Position, Scale and Rotation
        /// </summary>
        public void UpdateTransform()
        {
            transform = Matrix4.CreateScale(scale) *
                        Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X)) *
                        Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y)) *
                        Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z)) *
                        Matrix4.CreateTranslation(position.X + offset.X, position.Y + offset.Y, position.Z + offset.Z);
        }

        /// <summary>
        /// Turns this into a Object that
        /// can be used by the Renderer
        /// </summary>
        public void Build()
        {
            // Create Vertex Array Object
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            CreateEBO();

            EnableBufferData();
        }



        public void EnableBufferData()
        {

            // Bind vertex buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.DynamicDraw);

            // vertex Position format
            GL.VertexAttribPointer(Shader.vertexPositionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); // Vertex is a vec3
            GL.EnableVertexAttribArray(Shader.vertexPositionLocation);



            // Bind uv's
            GL.BindBuffer(BufferTarget.ArrayBuffer, uvBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, uv.Length * sizeof(float), uv, isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.DynamicDraw);

            // Specify uv format
            GL.VertexAttribPointer(Shader.texCoordLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0); // UV is a vec2
            GL.EnableVertexAttribArray(Shader.texCoordLocation);



            // Bind normals
            GL.BindBuffer(BufferTarget.ArrayBuffer, normalBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.DynamicDraw);

            // Secify normal format
            GL.VertexAttribPointer(Shader.normalLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); // Normal is a vec3
            GL.EnableVertexAttribArray(Shader.normalLocation);
        }


        internal void CreateEBO()
        {
            elementBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);

            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.DynamicDraw);
        }


        /// <summary>
        /// Makes The Renderer render this object
        /// </summary>
        void AddToRenderer()
        {
            if (renderer.objects.ContainsKey(renderer.shaders[shader]))
            {
                renderer.objects[renderer.shaders[shader]].Add(this);
            }
            else
            {
                renderer.objects.Add(renderer.shaders[shader], new List<RenderableObject>());
                AddToRenderer();
            }
        }

        /// <summary>
        /// Makes this object impossible
        /// to render
        /// </summary>
        public void Destroy()
        {
            renderer.objects[renderer.shaders[shader]].Remove(this);
        }
    }
}
