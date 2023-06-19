using _4Gewinnt__nicht_;
using Network;
using objLoader;
using OpenTK.Graphics.OpenGL4;
using Rendering;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;


namespace Shaders
{
    public class Game
    {
        public RenderableObject board, table, placeHint;
        public Texture tableTex;

        public Renderer renderer;
        public GameWindow form;
        public ChatWindow chatWindow;

        public GamestateHandler gamestateHandler;

        public const float gridXscale = 0.28f;
        public const float gridYscale = 0.27f;

        public static readonly Vector3 gridOffset = new Vector3(-0.8f, 0.23f, 0.35f);
        public static Game instance;


        public Action OnUpdate { get; set; }
        public Action OnReloadGame { get; set; }


        ///////////////////////////////////////////////// LOAD /////////////////////////////////////////////////////

        public Game(GameWindow form, Renderer renderer)
        {
            instance = this;
            this.renderer = renderer;
            this.form = form;


            Grid.form = this.form;
            Grid.renderer = renderer;

            gamestateHandler = new GamestateHandler(this);

            gamestateHandler.OnWinPersistent += (Player w) =>
            {
                Logging.Log(w.name + " Won the game!");
            };

            gamestateHandler.OnDrawPersistent += () =>
            {
                Logging.Log("Draw!!!");
            };

            // create chat window
            chatWindow = new ChatWindow();
            chatWindow.Size = GameWindow.instance.Size / 2;
            chatWindow.ControlBox = false;
        }

        public void InitGame(PlayerData[] players)
        {
            GameWindow.enableExitButton = true;

            foreach (var p in players)
            {
                Player player = p.ToPlayer();
                player.textureID = p.textureID;

                if (player.mode == Player.Mode.Bot)
                {
                    player.bot = new MinMaxBot(gamestateHandler, player, new Player(Player.Mode.Player, null, "", 0), 4);
                }

                gamestateHandler.players.Add(player);
            }
        }

        /// <summary>
        /// Shows or hides chat window
        /// </summary>
        public void ToggleChatWindow()
        {
            if (chatWindow.Visible)
            {
                chatWindow.Hide();
            }
            else
            {
                chatWindow.Show();
            }
        }

        public void InitGamestateHandler()
        {
            // Prepare Game: 

            gamestateHandler.GenerateCoins(gridOffset);
        }

        public void StartGame()
        {
            GameWindow.enableExitButton = true;
            gamestateHandler.StartGame();
        }

        /// <summary>
        /// Loads and compiles shaders that arent "Default" or "Postprocess"
        /// </summary>
        public void LoadShaders()
        {
            Shader backGround = new Shader("Default.vert", "Default.frag");

            backGround.onUse += () =>
            {
                int location = GL.GetUniformLocation(backGround.handle, "time");
                GL.Uniform1(location, Time.timeSinceStartup);
            };

            renderer.shaders.Add("Background", backGround);
        }

        /// <summary>
        /// Prepare the game, gets called
        /// before the first Update
        /// </summary>
        public void Load()
        {
            // Shaders and textures
            LoadShaders();
            tableTex = new Texture("container.jpg");

            // Models:

            table = new RenderableObject(
                renderer,
                "Default",
                Primitives.cubeVerts,
                Primitives.cubeUVs,
                Primitives.cubeIndicies,
                Primitives.cubeNormals,
                false
                );

            table.tex = tableTex;
            table.position = new Vector3(0, -1, 0);
            table.scale = new Vector3(10f, 1f, 10f);
            table.material = new Material
            {
                shininess = 16f,
                specular = new Vector3(0.3f, 0.3f, 0.3f),
                ambient = new Vector3(0.3f, 0.2f, 0.2f),
                diffuse = new Vector3(0.5f, 0.5f, 0.5f),
            };
            table.isShadowCaster = false;




            float[] vertices;
            uint[] indices;
            float[] uv;
            float[] normals;


            ObjImporter.ParseOBJ("board.obj", out vertices, out indices, out normals, out uv);
            uv = Primitives.cubeUVs;


            board = new RenderableObject(renderer, "Default", vertices, uv, indices, normals, false);

            board.tex = tableTex;
            board.position = new Vector3(-1.03f, 0.07f, 0);
            board.scale = new Vector3(0.01f, 0.01f, 0.01f);
            board.material = new Material
            {
                shininess = 32f,
                specular = new Vector3(0.1f, 0.1f, 0.1f),
                ambient = new Vector3(0.1f, 0.1f, 0.1f),
                diffuse = new Vector3(0.5f, 0.5f, 0.5f),
            };



            
            ObjImporter.ParseOBJ("Toaster.obj", out vertices, out indices, out normals, out uv);
            RenderableObject toaster = new RenderableObject(renderer, "Default", vertices, uv, indices, normals, false);

            toaster.tex = new Texture("ToasterTexture.png");
            toaster.position = new Vector3(-2f, 0.07f, 0);
            toaster.rotation = new Vector3(0, 45, 0);
            toaster.scale = new Vector3(0.08f, 0.08f, 0.08f);
            toaster.material = new Material
            {
                shininess = 16f,
                specular = new Vector3(0.1f, 0.1f, 0.1f),
                ambient = new Vector3(0.1f, 0.1f, 0.1f),
                diffuse = new Vector3(0.5f, 0.5f, 0.5f),
            };

            ObjImporter.ParseOBJ("rat.obj", out vertices, out indices, out normals, out uv);
            RenderableObject rat = new RenderableObject(renderer, "Default", vertices, uv, indices, normals, false);

            rat.tex = new Texture("rattex.jpg");
            rat.position = new Vector3(3.5f, 0f, -1);
            rat.rotation = new Vector3(0, 135, 0);
            rat.scale = new Vector3(0.005f, 0.005f, 0.005f);
            rat.material = new Material
            {
                shininess = 16f,
                specular = new Vector3(0.1f, 0.1f, 0.1f),
                ambient = new Vector3(0.1f, 0.1f, 0.1f),
                diffuse = new Vector3(0.5f, 0.5f, 0.5f),
            };

            // Make rat spin (Very important)
            OnUpdate += () =>
            {
                rat.rotation = new Vector3(0, rat.rotation.Y + 50 * Time.deltaTime, 0);
            };


            vertices = Primitives.cubeVerts;
            indices = Primitives.cubeIndicies;
            normals = Primitives.cubeNormals;
            uv = Primitives.cubeUVs;


            ObjImporter.ParseOBJ("coin.obj", out vertices, out indices, out normals, out uv);
            uv = new float[] { 0f, 0f, 0f, 1f, 1f, 0f, 1f, 1f };

            placeHint = new RenderableObject(
                renderer,
                "Default",
                vertices,
                uv,
                indices,
                normals,
                false
                );


            placeHint.position = new Vector3(0, 0, -3);
            placeHint.scale = new Vector3(0.01f, 0.01f, 0.01f);
            placeHint.offset = new Vector3(-0.12f, 0.07f, 0);
            placeHint.rotation = new Vector3(90, 0, 0);
            placeHint.enabled = false;
            placeHint.alpha = 0.5f;

            placeHint.material = new Material
            {
                shininess = 32f,
                specular = new Vector3(1f, 1f, 1f),
                ambient = new Vector3(0.1f, 0.1f, 0.1f),
                diffuse = new Vector3(0.5f, 0.5f, 0.5f),
            };


            // Camera and lighting:

            Renderer.mainCam.position = new Vector3(0.15f, 1.3f, 1.6f);
            Renderer.mainCam.pitch = -8;


            renderer.sunPosition = new Vector3(1.5f, 2.6f, 1);

            InitGamestateHandler();
        }

        ///////////////////////////////////////////////// UI /////////////////////////////////////////////////////

        /// <summary>
        /// Gets called every frame
        /// </summary>
        public void Update()
        {
            OnUpdate?.Invoke();
        }

        ///////////////////////////////////////////////// GAMEPLAY /////////////////////////////////////////////////////

        public void MouseHover(Vector2 position)
        {
            // Get Mouse pos in world space
            Vector3 pos = Grid.MouseToWorldCoordinates((int)position.X, (int)position.Y, board.position.Z);

            // Calculate Position in Grid
            Grid.GetGridPosX(pos, gridXscale, out int p);


            // Show current grid Position
            placeHint.enabled = false;
            if (Grid.IsInsideGrid(pos))
            {
                placeHint.position = gamestateHandler.GetLowestFreePosition(p, out int yIndex);

                placeHint.tex = gamestateHandler.players[gamestateHandler.currentPlayer].texture;

                placeHint.enabled = true;
            }
        }


        public void MouseClick(Vector2 position)
        {
            Vector3 pos = Grid.MouseToWorldCoordinates((int)position.X, (int)position.Y, board.position.Z);

            // Calculate Position in Grids
            Grid.GetGridPosX(pos, gridXscale, out int p);


            if (!Grid.IsInsideGrid(pos)) return;

            if (NetworkHandler.server != null)
            {
                RpcMethods.SendMove(NetworkHandler.server, p);
            }
            else
            {
                PlaceCoin(p);
            }
        }

        /// <summary>
        /// Place coin in the row "row" on the grid.
        /// Logs if somebody won the game
        /// </summary>
        void PlaceCoin(int row)
        {
            if (gamestateHandler.players[gamestateHandler.currentPlayer].mode != Player.Mode.Player) return;

            if (gamestateHandler.CanAddCoin(row))
            {
                gamestateHandler.AddCoin(row);
                gamestateHandler.PassTurn();
            }

        }

        ///////////////////////////////////////////////// RESET /////////////////////////////////////////////////////
        
        public void Reset()
        {
            gamestateHandler.Cleanup();

            
            GamestateHandler newHandler = new GamestateHandler(this);

            newHandler.OnStartGamePersistent = gamestateHandler.OnStartGamePersistent;
            newHandler.OnPassTurnPersistent = gamestateHandler.OnPassTurnPersistent;
            newHandler.OnWinPersistent = gamestateHandler.OnWinPersistent;
            newHandler.OnDrawPersistent = gamestateHandler.OnDrawPersistent;
            
            
            gamestateHandler = newHandler;
       

            InitGamestateHandler();

            OnReloadGame?.Invoke();
        }

    }
}