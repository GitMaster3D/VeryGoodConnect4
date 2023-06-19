using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.WinForms;
using Rendering;
using Shaders;
using System.Runtime.InteropServices;
using Network;

namespace _4Gewinnt__nicht_
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            MenuManager.ChangeMenu("PreGame");
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void turtorialButton_Click(object sender, EventArgs e)
        {
            MenuManager.ChangeMenu("TurtorialWindow");
        }
    }
}