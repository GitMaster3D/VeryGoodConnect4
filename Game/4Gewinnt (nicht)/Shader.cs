using _4Gewinnt__nicht_;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Shaders
{
    public class Shader : IDisposable
    {

        public int handle;

        int vertexShader;
        int fragmentShader;

        private bool disposed;

        public delegate void OnUseEvent();
        public event OnUseEvent onUse;

        public const string texCoordAttribName = "aTexCoord";
        public const string vertexAttribName = "aPosition";
        public const string normalAttribName = "aNormal";
        public const string viewAttribName = "view";
        public const string projectionAttribName = "projection";
        public const string tintAttribName = "tint";
        public const string transformAttribName = "transform";
        public const string lightColorAttribName = "lightColor";
        public const string ambientStrengthAttribName = "ambientStrength";
        public const string lightPositionAttribName = "lightPos";
        public const string viewPositionAttribName = "viewPos";
        public const string specularStrengthAttribName = "specularStrength";
        public const string materialAttribName = "material";
        public const string alphaAttribName = "alpha";
        public const string shadowMapAttribName = "shadowMap";

        public const int vertexPositionLocation = 0;
        public const int texCoordLocation = 1;
        public const int normalLocation = 2;

        ///////////////////////////////////////////////// COMPILATION /////////////////////////////////////////////////////

        public Shader(string s_vertexShader, string s_fragmentShader)
        {
            string vertexPath = AppDomain.CurrentDomain.BaseDirectory + $"Shaders/{s_vertexShader}";
            string fragmentPath = AppDomain.CurrentDomain.BaseDirectory + $"Shaders/{s_fragmentShader}";



            // Compile Vertex Shader:

            string vertexShaderSource = File.ReadAllText(vertexPath);

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);


            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                Logging.LogError(GL.GetShaderInfoLog(vertexShader));
            }



            // Compile Fragment Shader:

            string fragmentShaderSource = File.ReadAllText(fragmentPath);
            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                Logging.LogError(GL.GetShaderInfoLog(fragmentShader));
            }

            // Turn Shader into GPU Program
            Create();

            // Cleanup
            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
        }

        /// <summary>
        /// Creates Shader Program from compiled Shaders
        /// </summary>
        void Create()
        {
            handle = GL.CreateProgram();

            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);

            GL.LinkProgram(handle);

            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                Logging.LogError(GL.GetProgramInfoLog(handle));
            }
        }



        ///////////////////////////////////////////////// CLEANUP /////////////////////////////////////////////////////
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                GL.DeleteProgram(handle);
                disposed = true;
            }
        }


        ~Shader()
        {
            if (!disposed)
            {
                Logging.LogError("GPU Resource leak! did you forget to call Dispose()?");
            }
        }

        /// <summary>
        /// Cleanup to prevent 
        /// GPU Resource Leak
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        ///////////////////////////////////////////////// USE /////////////////////////////////////////////////////

        /// <summary>
        /// Activates this Shader
        /// Call before rendering object
        /// </summary>
        public void Use()
        {
            onUse?.Invoke();
        }

        /// <summary>
        /// Sets a unitform mat4 from this shader
        /// </summary>
        public void SetMatrix4(Matrix4 matrix, string name)
        {
            int location = GL.GetUniformLocation(handle, name);

            if (location == -1) Logging.LogError("location was -1 at shader " + handle);

            GL.UniformMatrix4(location, true, ref matrix);
        }

        public void SetVec4(Vector4 vec, string name)
        {
            int location = GL.GetUniformLocation(handle, name);

            if (location == -1) Logging.LogError("location was -1 at shader " + handle);

            GL.Uniform4(location, vec);
        }


        public void SetVec3(Vector3 vec, string name)
        {
            int location = GL.GetUniformLocation(handle, name);

            if (location == -1) Logging.LogError("location was -1 at shader " + handle);

            GL.Uniform3(location, vec);
        }

        public void SetFloat(float value, string name)
        {
            int location = GL.GetUniformLocation(handle, name);

            if (location == -1) Logging.LogError("location was -1 at shader " + handle);

            GL.Uniform1(location, value);
        }

        public void SetMaterial(Material value, string name)
        {
            SetVec3(value.ambient, name + ".ambient");
            SetVec3(value.diffuse, name + ".diffuse");
            SetVec3(value.specular, name + ".specular");
            SetFloat(value.shininess, name + ".shininess");
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(handle, attribName);
        }
    }

    public struct Material
    {
        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;

        public float shininess;
    }
}