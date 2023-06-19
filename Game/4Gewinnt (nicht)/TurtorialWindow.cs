using Network;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.WinForms;
using Rendering;
using Shaders;
using System.Runtime.InteropServices;

namespace _4Gewinnt__nicht_
{
    public partial class TurtorialWindow : Form
    {
        string[] images;
        int currentImage = int.MaxValue / 2; // Start in the middle of the number to prevent hitting minimum or maximum when in- or decrementing

        string turtorialImagesPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + "/Images/Turtorial/";
            }
        }

        public TurtorialWindow()
        {
            InitializeComponent();


            images = Directory.GetFiles(turtorialImagesPath);
            turtorialPicturebox.ImageLocation = images[0];

            // Start CurrentImage with 0
            while (currentImage % images.Length != 0)
            {
                currentImage++;
            }
        }

        private void backButton_Click_1(object sender, EventArgs e)
        {
            MenuManager.ChangeMenu("MainMenu");
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            currentImage++;
            turtorialPicturebox.ImageLocation = images[currentImage % images.Length];
        }
        
        private void previousButton_Click(object sender, EventArgs e)
        {
            --currentImage;
            turtorialPicturebox.ImageLocation = images[currentImage % images.Length];
        }
    }
}