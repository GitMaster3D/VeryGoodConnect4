//#define USESHADOWS
// Calculates a depth Texture from the light source
// For use with shadowmapping.

using _4Gewinnt__nicht_;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.WinForms;
using Shaders;
using System.Collections.Generic;

namespace Rendering
{
    public class Renderer
    {
        public Dictionary<string, Shader> shaders;

        public int vertexBufferObject;
        public int uvBufferObject;
        public int normalBufferObject;

        public int frameBufferObject, shadowFrameBufferObject;
        public int colorTextureID;
        public int depthTextureID, shadowDepthTextureID;


        public Vector3 sunPosition = new Vector3(0, 20, 0);
        public Vector3 sunColor = new Vector3(2, 2, 2);
        public Vector3 sunRotation = new Vector3(-40, -120, 0);

        public Dictionary<Shader, List<RenderableObject>> objects;

        private Camera _camera;
        public static Camera mainCam;
        private Texture errorTex;

        private RenderableObject outputQuad;
        private Shader postProcessShader;


        public Color4 backgroundColor
        {
            get { return backgroundColor_; }
            set
            {
                backgroundColor_ = value;
                GL.ClearColor(backgroundColor_);
            }
        }

        private Color4 backgroundColor_;

        // Renders a Frame
        public void Render(GLControl glControl)
        {
            // Can sometimes fail on certain gpu's
            try
            {
                glControl.MakeCurrent();
            }
            catch (Exception ex)
            {
                Logging.LogError(ex);
            }




#if USESHADOWS
            ///////////////////////////// LIGHTING /////////////////////////////////

            GL.ColorMask(false, false, false, false);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadowFrameBufferObject);


            GL.Clear(ClearBufferMask.DepthBufferBit);

            
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            
            Camera mainLight = new Camera(mainCam.position + new Vector3(0, 1, 0), mainCam.aspectRatio);
            mainLight.pitch = mainCam.pitch - 30;
            mainLight.yaw = mainCam.yaw;
            mainLight.farPlane = 100;
            mainLight.nearPlane = 0.1f;
            mainLight.orthographicWidth = 10;
            mainLight.orthographicHeight = 5;

            DrawScene(mainLight, false, false, true);
#endif




            /////////////////////////////// DRAW SCENE //////////////////////////////////////////

            GL.ColorMask(true, true, true, true);

            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferObject);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

#if USESHADOWS
            DrawScene(mainCam, true, false, false);
#elif !USESHADOWS
            DrawScene(mainCam, false, false, false);
#endif


            /////////////////////////////// POST PROCESSING //////////////////////////////////////////

            // Clear Screen
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Save Camera position and rotation:
            Vector3 camPos = mainCam.position;
            float camYaw = mainCam.yaw;
            float camPitch = mainCam.pitch;

            // Set Camera to fixed position and rotation for easiyer Post Processing:
            mainCam.position = Vector3.Zero;
            mainCam.pitch = 0;
            mainCam.yaw = -90;



            if (postProcessShader == null) GeneratePostProcessShader();
            if (outputQuad == null) GenerateOutputQuad();

            GL.UseProgram(postProcessShader.handle);

            outputQuad.EnableBufferData();
            
            // Bind Rendered Texture to Output quad
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, colorTextureID);



            postProcessShader.Use();

            // Draw output Quad
            GL.BindVertexArray(outputQuad.vao);
            GL.DrawElements(PrimitiveType.Triangles, outputQuad.indiceCount, DrawElementsType.UnsignedInt, 0);

            // Restore Original Cam Position and rotation
            mainCam.position = camPos;
            mainCam.yaw = camYaw;
            mainCam.pitch = camPitch;


            /////////////////////////////// OUTPUT //////////////////////////////////////////

            GL.Disable(EnableCap.Blend);

            try
            {
                glControl.SwapBuffers();
            }
            catch (Exception ex)
            {
                Logging.LogError("Unable to swap buffers! " + ex);
            }
        }


        /// <summary>
        /// Draws the scene using the specifyed camera
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="lightMode"> If the scene should be rendered using lighting </param>
        public void DrawScene(Camera cam, bool lightMode = false, bool orthographic = false, bool renderShadows = false)
        {
            foreach (var shader in shaders)
            {
                if (!objects.ContainsKey(shader.Value)) continue;

                // Tell openGl wich shader to use
                GL.UseProgram(shader.Value.handle);


                shader.Value.SetMatrix4(cam.GetViewMatrix(), Shader.viewAttribName);

                if (!orthographic)
                    shader.Value.SetMatrix4(cam.GetProjectionMatrix(), Shader.projectionAttribName);
                else
                    shader.Value.SetMatrix4(cam.GetProjectionMatrixOrtho(), Shader.projectionAttribName);


                // Setup light data:

                // Diffuse
                shader.Value.SetVec3(sunPosition, Shader.lightPositionAttribName);
                shader.Value.SetVec3(sunColor, Shader.lightColorAttribName);

                // Specular
                shader.Value.SetVec3(cam.position, Shader.viewPositionAttribName);

                lock (objects)
                {
                    foreach (var obj in objects[shader.Value])
                    {

                        if (!obj.enabled) continue;
                        if (obj == null) continue;
                        if (!obj.isValid) continue;
                        if (renderShadows && !obj.isShadowCaster) continue;

                        obj.UpdateTransform();
                        obj.EnableBufferData(); // Enables the buffers (verts etc.) of this object


                        // Load Texture
                        if (obj.tex == null) errorTex.Use();
                        obj.tex?.Use();

#if USESHADOWS
                        // Pass shadow depth buffer
                        if (lightMode)
                        {
                            // Set the shadow map uniform in the shader
                            int shadowMapUniform = GL.GetUniformLocation(shader.Value.handle, Shader.shadowMapAttribName);

                            GL.ActiveTexture(TextureUnit.Texture1);
                            GL.BindTexture(TextureTarget.Texture2D, shadowDepthTextureID);

                            GL.Uniform1(shadowMapUniform, 1); // 1 corresponds to TextureUnit.Texture1

                            shader.Value.SetMatrix4(cam.CalculateLightSpaceMatrix(), "lightSpaceMatrix");
                        }
#endif



                        shader.Value.SetMatrix4(obj.transform, Shader.transformAttribName); // Set Transform in shader
                        shader.Value.SetMaterial(obj.material, Shader.materialAttribName); // Set material in shader

                        shader.Value.SetFloat(obj.alpha, Shader.alphaAttribName);

                        // Shader Use event
                        shader.Value.Use();

                        // Draw 
                        GL.BindVertexArray(obj.vao);
                        GL.DrawElements(PrimitiveType.Triangles, obj.indiceCount, DrawElementsType.UnsignedInt, 0);
                    }
                }
            }
        }


        /////////////////////////////// LOAD //////////////////////////////////////////
        public void Load()
        {

            // Create Vertex Buffer
            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            // Create UV Buffer
            uvBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, uvBufferObject);

            // Create Normal Buffer
            normalBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, normalBufferObject);



            // Framebuffer
            frameBufferObject = GenerateFrameBuffer(frameBufferObject);
            GenerateFrameBufferTextures(FrameBufferMode.color | FrameBufferMode.depth, ref colorTextureID, ref depthTextureID, frameBufferObject);

            
            // Shadow Framebuffer
            shadowFrameBufferObject = GenerateFrameBuffer(shadowFrameBufferObject);
            int i = 0;
            GenerateFrameBufferTextures(FrameBufferMode.depth, ref i, ref shadowDepthTextureID, shadowFrameBufferObject);
            

            objects = new Dictionary<Shader, List<RenderableObject>>();
            shaders = new Dictionary<string, Shader>();


            GL.Enable(EnableCap.DepthTest);

            // Default Shader
            shaders.Add("Default", new Shader("Default.vert", "Default.frag"));

            // Error Texture
            errorTex = new Texture("error.jpg");

            // Setup Camera
            _camera = new Camera(new Vector3(0, 0, 5), 90);
            _camera.aspectRatio = 2;
            mainCam = _camera;
        }

        /// <summary>
        /// Set value of outputQuad to a new Quad
        /// that can be used to display the rendered scene
        /// </summary>
        void GenerateOutputQuad()
        {
            outputQuad = new RenderableObject(
            this,
            "Default",
            Primitives.quadVertices,
            Primitives.quadUvs,
            Primitives.quadIndices,
            Primitives.quadNormals,
            true
            );
            outputQuad.enabled = false;

            outputQuad.scale = new Vector3(1, 1, 1);
            outputQuad.position = new Vector3(0, 0, -1);

            outputQuad.material = new Material
            {
                shininess = 32f,
                specular = new Vector3(0f, 0f, 0f),
                ambient = new Vector3(1f, 1f, 1f),
                diffuse = new Vector3(0f, 0f, 0f),
            };
        }

        public void DeleteFrameBufferAndTextures()
        {
            GL.DeleteTexture(colorTextureID);
            GL.DeleteTexture(depthTextureID);
            GL.DeleteFramebuffer(frameBufferObject);

            GL.DeleteFramebuffer(shadowFrameBufferObject);
            GL.DeleteTexture(shadowDepthTextureID);
        }

        
        public void GenerateFrameBuffer()
        {
            // Create Frame Buffer
            frameBufferObject = GenerateFrameBuffer(frameBufferObject);
        }


        public int GenerateFrameBuffer(int fbo)
        {
            // Create Frame Buffer
            fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            return fbo;
        }


        public void GenerateFrameBufferTextures(FrameBufferMode mode, ref int colorTextureID, ref int depthTextureID, int framebuffer)
        {
            // Render Texture:
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

            if (mode.HasFlag(FrameBufferMode.color))
            {
                // Color Texture
                GL.GenTextures(1, out colorTextureID);
                GL.BindTexture(TextureTarget.Texture2D, colorTextureID);

                // Color Texture Parameters
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, GameWindow.instance.glControl_.Width, GameWindow.instance.glControl_.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, colorTextureID, 0);
            }

            if (mode.HasFlag(FrameBufferMode.depth))
            {
                // Depth Texture
                GL.GenTextures(1, out depthTextureID);
                GL.BindTexture(TextureTarget.Texture2D, depthTextureID);

                // Depth Texture Parameters
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent16, GameWindow.instance.glControl_.Width, GameWindow.instance.glControl_.Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTextureID, 0);
            }



            // Check for framebuffer error
            FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                Logging.LogError(status.ToString());
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // Unbind the framebuffer
            GL.BindTexture(TextureTarget.Texture2D, 0); // Unbind the textures
        }

        public enum FrameBufferMode
        {
            color = 0,
            depth = 1
        }

        void GeneratePostProcessShader()
        {
            postProcessShader = new Shader("Postprocess.vert", "Postprocess.frag");
        }



        /////////////////////////////// CLEANUP //////////////////////////////////////////
        
        // Cleanup shaders to prevent GPU Resource Leaks
        public void UnLoad(object sender, EventArgs e)
        {
            foreach (var shader in shaders)
            {
                shader.Value.Dispose();
            }
        }
    }
}