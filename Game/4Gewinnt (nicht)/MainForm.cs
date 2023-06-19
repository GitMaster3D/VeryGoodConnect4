using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.WinForms;
using Rendering;
using Shaders;
using System.Runtime.InteropServices;

namespace _4Gewinnt__nicht_
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(920, 600);

            MenuManager.Init(this.MainMenuPanel);
            MenuManager.ExitToMain();
        }
    }
}