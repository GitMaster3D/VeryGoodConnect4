using Network;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.WinForms;
using Rendering;
using Shaders;
using System.Runtime.InteropServices;

namespace _4Gewinnt__nicht_
{
    public partial class ChatWindow : Form
    {
        public ChatWindow()
        {
            InitializeComponent();
            this.Text = "Chat";
        }

        private void SendChatMessage()
        {
            if (NetworkHandler.server == null) return; // Don't send message if no server is connected

            string message = chatTextBox.Text;
            RpcMethods.SendChatMessage(NetworkHandler.server, message);

            chatTextBox.Text = ""; // Clear chat textbox
        }

        private void chatTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                SendChatMessage();
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            SendChatMessage();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Game.instance.chatWindow.Visible = false;
        }
    }
}