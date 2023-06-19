using Network;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.WinForms;
using Rendering;
using Shaders;
using System.Numerics;
using System.Runtime.InteropServices;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace _4Gewinnt__nicht_
{
    public partial class GameWindow : Form
    {
        private System.Windows.Forms.Timer _timer = null!;
        public static bool enableExitButton = true;

        private string imagesPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + "/Images/";
            }
        }

        public GLControl glControl_;

        public Game game;
        public Action loaded; // Gets invoked after the window has finished loading

        public static GameWindow instance;
        private Renderer renderer;

        public GameWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            glControl_ = glControl;
            instance = this;

            instance.tableLayoutPanel1.BackgroundImageLayout = ImageLayout.Stretch;
            instance.winnerLabel.Text = "";
            instance.winnerLabel.Hide();
        }

        private void ChangePlayerDisplay(Player player)
        {
            currentPlayerLabel.Text = $"Player: {player.name}";

            instance.tableLayoutPanel1.BackgroundImage = System.Drawing.Image.FromFile(player.texture.texturePath);
        }


        private void ShowWinner(Player player)
        {
            instance.winnerLabel.Text = $"{player.name} won the game!";
            instance.winnerLabel.Show();
            instance.tableLayoutPanel1.BackgroundImage = System.Drawing.Image.FromFile(imagesPath + "Winner.jpg");

            // Prevent message box from showing in online games, since
            // Those have their own messagebox
            if (NetworkHandler.server != null)
                MessageBox.Show($"{player.name} won the game!", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void ShowDraw()
        {
            MessageBox.Show($"The game ended in a draw", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void ResetUI()
        {
            winnerLabel.Invoke(() =>
            {
                instance.winnerLabel.Text = "";
                instance.winnerLabel.Hide();
            });
        }


        public void glControl_Load(object sender, EventArgs e)
        {

            // Prepare Renderer
            renderer = new Renderer();
            renderer.backgroundColor = Color4.LightSlateGray;
            renderer.Load();

            // Load Game
            this.game = new Game(this, renderer);
            game.Load();

            game.gamestateHandler.OnPassTurnPersistent += ChangePlayerDisplay;
            game.gamestateHandler.OnStartGamePersistent += ChangePlayerDisplay;
            game.gamestateHandler.OnWinPersistent += ChangePlayerDisplay; 
            game.gamestateHandler.OnWinPersistent += ShowWinner;
            game.gamestateHandler.OnDrawPersistent += ShowDraw;
            game.OnReloadGame += ResetUI;

            // Make sure that when the GLControl is resized or needs to be painted,
            // we update our projection matrix or re-render its contents, respectively.
            glControl.Resize += glControl_Resize;
            glControl.Resize += (object sender, EventArgs e) =>
            {
                renderer.DeleteFrameBufferAndTextures();


                // Shadow Framebuffer
                renderer.shadowFrameBufferObject = renderer.GenerateFrameBuffer(renderer.shadowFrameBufferObject);
                int i = 0;
                renderer.GenerateFrameBufferTextures(Renderer.FrameBufferMode.depth, ref i, ref renderer.shadowDepthTextureID, renderer.shadowFrameBufferObject);
              

                
                // Framebuffer
                renderer.frameBufferObject = renderer.GenerateFrameBuffer(renderer.frameBufferObject);
                renderer.GenerateFrameBufferTextures(Renderer.FrameBufferMode.color | Renderer.FrameBufferMode.depth, ref renderer.colorTextureID, ref renderer.depthTextureID, renderer.frameBufferObject);
            };
            FormClosed += renderer.UnLoad;


            // Ensure that the viewport and projection matrix are set correctly initially.
            glControl_Resize(glControl, EventArgs.Empty);


            Time.currentTimestamp = DateTime.Now;
            Time.lastTimestamp = DateTime.Now;



            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1;


            // Update Loop
            _timer.Tick += (object sender, EventArgs e) =>
            {
                Time.currentTimestamp = DateTime.Now;


                game.Update();
                renderer.Render(glControl);

                Time.deltaTime = (Time.currentTimestamp.Ticks - Time.lastTimestamp.Ticks) / 10000000f;


                Time.timeSinceStartup += Time.deltaTime;
                Time.lastTimestamp = Time.currentTimestamp;
            };
            // Start Game Loop
            _timer.Start();



            // Toggle wether chat button is available or not
            MenuManager.onLoadEvents["GameWindow"] += () =>
            {
                if (NetworkHandler.server == null)
                {
                    chatButton.Hide();
                }
                else
                {
                    chatButton.Show();
                }
            };

            loaded?.Invoke();
        }


        /// <summary>
        /// Sets the text of given label to the given string
        /// This Method work thread safe
        /// </summary>
        public static void UpdateLabelSafe(string text, Label label)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(() =>
                {
                    UpdateLabelSafe(text, label);
                });
            }
            else
            {
                label.Text = text;
            }
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            game.MouseHover(new Vector2(e.Location.X, e.Location.Y));
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            game.MouseClick(new Vector2(e.Location.X, e.Location.Y));
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            glControl.MakeCurrent();

            if (glControl.ClientSize.Height == 0)
                glControl.ClientSize = new System.Drawing.Size(glControl.ClientSize.Width, 1);

            GL.Viewport(0, 0, glControl.ClientSize.Width, glControl.ClientSize.Height);

            float aspect_ratio = Math.Max(glControl.ClientSize.Width, 1) / (float)Math.Max(glControl.ClientSize.Height, 1);
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }

        public void ResetGame()
        {
            game.Reset();
            enableExitButton = true;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            if (!enableExitButton) return;

            ResetGame();
            MenuManager.ExitToMain();
        }

        private void chatButton_Click(object sender, EventArgs e)
        {
            game.ToggleChatWindow();
        }
    }
}

public class Time
{
    public static float deltaTime { get; internal set; }
    public static float timeSinceStartup;

    internal static DateTime lastTimestamp;
    internal static DateTime currentTimestamp;
}