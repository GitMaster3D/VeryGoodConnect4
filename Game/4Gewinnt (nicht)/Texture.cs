using _4Gewinnt__nicht_;
using OpenTK.Graphics.OpenGL4;
using Image = SixLabors.ImageSharp.Image;

namespace Shaders
{
    public class Texture
    {
        public int handle;
        public string texturePath; // use this if this texture needs to be imported as an Image

        private int textureUnit;

        private string path;
        public Texture(string name)
        {
            this.path = AppDomain.CurrentDomain.BaseDirectory + $"Images/{name}";

            handle = GL.GenTexture();
            try
            {
                Load();
            }
            catch (Exception ex)
            {
                Logging.LogError("Unable to load Texture, attempting to load error texture: " + ex);
                path = AppDomain.CurrentDomain.BaseDirectory + $"Images/error.jpg";
            }

            texturePath = path;
        }

        public Texture(string name, bool nameIsPath)
        {
            if (!nameIsPath)
            {
                this.path = AppDomain.CurrentDomain.BaseDirectory + $"Images/{name}";
            }
            else
            {
                this.path = name;   
            }

            handle = GL.GenTexture();

            try
            {
                Load();
            }
            catch (Exception ex)
            {
                Logging.LogError("Unable to load Texture, attempting to load error texture: " + ex);
                path = AppDomain.CurrentDomain.BaseDirectory + $"Images/error.jpg";
            }

            texturePath = path;
        }

        void Load()
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            // Load Image
            Image<Rgba32> image = Image.Load<Rgba32>(path);

            //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
            //This will correct that, making the texture display properly.
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            // Copy Image to array
            var pixels = new byte[4 * image.Width * image.Height];
            image.CopyPixelDataTo(pixels);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            image.Dispose();
        }

        public void Use()
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }

        public static Image LoadImage(string name)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + $"Images/{name}";
            return Image.Load<Rgba32>(path);
        }
    }
}