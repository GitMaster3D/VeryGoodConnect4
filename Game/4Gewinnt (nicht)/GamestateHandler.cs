using objLoader;
using OpenTK.Mathematics;
using Rendering;
using Shaders;


namespace _4Gewinnt__nicht_
{
    public class GamestateHandler
    {
        public Coin[,] board { get; private set; }
        private Game game;

        public int currentPlayer_;
        public int currentPlayer
        {
            get
            {
                return currentPlayer_;
            }

            set
            {
                currentPlayer_ = value;
            }
        }

        public int connectAmount = 4; // How many coins have to be in one row to win

        public List<Player> players { get; set; }
        public List<RenderableObject> coinObjects;
 

        public bool started { get; private set; }
        public bool ended { get; private set; }


        public Action<Player> OnPassTurnPersistent { get; set; }
        public Action<Player> OnWinPersistent { get; set; }
        public Action<Player> OnStartGamePersistent { get; set; }
        public Action<Player> OnPassTurn { get; set; }
        public Action OnDrawPersistent { get; set; }


        public GamestateHandler(Game game)
        {
            this.game = game;
            board = new Coin[7, 6];

            this.players = new List<Player>();
            this.coinObjects = new List<RenderableObject>();
        }


        /// <summary>
        /// Calculates the lowest free Position of 
        /// the given row
        /// 
        /// yIndex = the vertical row of the position
        /// </summary>
        /// <returns>
        /// The lowest free Position of the given row
        /// </returns>
        public Vector3 GetLowestFreePosition(int index, Coin[,] board, out int yIndex)
        {
            yIndex = -1;

            if (!started) return Vector3.NegativeInfinity;
            if (ended) return Vector3.NegativeInfinity;

            if (board[index, board.GetLength(1) - 1].placed) return Vector3.NegativeInfinity;


            for (int i = board.GetLength(1) - 1; i >= 0; i--)
            {
                if (board[index, i].placed)
                {
                    yIndex = i + 1;
                    return board[index, i + 1].pos;
                }
            }

            yIndex = 0;
            return board[index, 0].pos;
        }

        public Vector3 GetLowestFreePosition(int index, out int yIndex)
        {
            return GetLowestFreePosition(index, board, out yIndex);
        }

        /// <summary>
        /// Checks if the given row is full or nor
        /// </summary>
        public bool CanAddCoin(int index)
        {
            return CanAddCoin(board, index);
        }


        /// <summary>
        /// Checks if the given row is full or nor
        /// </summary>
        public bool CanAddCoin(Coin[,] board, int index)
        {
            return !board[index, board.GetLength(1) - 1].placed;
        }


        /// <summary>
        /// Gets the lowest Coin in the given row
        /// </summary>
        /// <returns>
        /// The lowest coin of the given row, null when there is no
        /// coin in the given row
        /// </returns>
        public Coin GetLowestCoin(int index, Coin[,] board)
        {
            GetLowestFreePosition(index, board, out int yIndex);
            yIndex--;

            if (yIndex >= 0 && yIndex < board.GetLength(1))
                return board[index, yIndex];

            return null;
        }


        /// <summary>
        /// Gets the lowest coin on the board
        /// 
        /// </summary>
        /// <returns>
        /// The lowest coin on the board,
        /// null when there are none
        /// 
        /// </returns>
        public Coin GetLowestCoin(Coin[,] board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                GetLowestFreePosition(i, board, out int yIndex);
                yIndex--;

                if (yIndex >= 0 && yIndex < board.GetLength(1))
                {
                    return board[i, yIndex];
                }
            }

            return null;
        }


        /// <summary>
        /// Generates Positions of where coins should be
        /// </summary>
        public void GenerateCoins(Vector3 offset)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {

                    float x = i * Game.gridXscale;
                    float y = j * Game.gridYscale;

                    board[i, j] = new Coin(x + offset.X, y + offset.Y, game.board.position.Z + offset.Z, false);
                }
            }
        }



        /// <summary>
        /// Remove all nonvisual coins from given row
        /// </summary>
        public void RemoveAllInvisibleCoins(int index)
        {
            for (int i = 0; i < board.GetLength(1); i++)
            {
                if (board[index, i].noVisuals)
                {
                    board[index, i] = new Coin(board[index, i].pos.X, board[index, i].pos.Y, board[index, i].pos.Z, false);
                }
            }
        }

        /// <summary>
        /// Remove the first noVisual coin in given row
        /// </summary>
        public void RemoveInvisibleCoin(int index)
        {
            for (int i = 0; i < board.GetLength(1); i++)
            {
                if (board[index, i].noVisuals)
                {
                    board[index, i] = new Coin(board[index, i].pos.X, board[index, i].pos.Y, board[index, i].pos.Z, false);
                    break;
                }
            }
        }

        /// <summary>
        /// Adds a Coin in the given row.
        /// Automatically checks for winner.
        /// </summary>
        public Coin AddCoin(int index, bool animate = true, float animationSpeed = 3)
        {
            if (!started) return null;
            if (ended) return null;

            RenderableObject obj = CreateCoin();

            obj.position = GetLowestFreePosition(index, out int yIndex);


            if (obj.position == Vector3.NegativeInfinity)
            {
                Logging.LogError($"Invalid Placement, {index} was full!");
                return null; // Invalid Position
            }

            obj.tex = players[currentPlayer].texture;
            
            if (yIndex >= 0)
            {

                board[index, yIndex].placed = true;
                board[index, yIndex].index = new Vector2i(index, yIndex);
                board[index, yIndex].owner = players[currentPlayer];
                board[index, yIndex].obj = obj;

                if (animate)
                {
                    // Animate Coin
                    Task tas = Task.Run(() =>
                    {
                        float t = 0;
                        float startY = 3;
                        float targetY = obj.position.Y;

                        while (t < 1)
                        {
                            lock (obj)
                            {
                                obj.position = new Vector3(obj.position.X, Mathmatics.Lerp(startY, targetY, t), obj.position.Z);
                            }

                            t += Time.deltaTime * animationSpeed;
                            Thread.Sleep((int)(Time.deltaTime * 1000));
                        }

                        obj.position = new Vector3(obj.position.X, targetY, obj.position.Z);
                    });
                }

                Coin[] coins = new Coin[0];
                // Win check
                if (WinCheck(board[index, yIndex], out Player winner, out coins) && winner != null)
                {
                    ended = true;
                    OnWinPersistent?.Invoke(winner);

                    // Highlight winning coins
                    foreach (var c in coins)
                    {
                        c.obj.alpha = 0.5f;
                        c.obj.material.ambient *= 10;
                    }
                }

                if (DrawCheck())
                {
                    ended = true;
                    OnDrawPersistent?.Invoke();
                }

                return board[index, yIndex];
            }



            return null;
        }

        /// <summary>
        /// Create one coin for the grid
        /// </summary>
        public RenderableObject CreateCoin()
        {
            if (!started) return null;
            if (ended) return null;

            ObjImporter.ParseOBJ("coin.obj", out float[] vertices, out uint[] indices, out float[] normals, out float[] uv);
            uv = new float[] { 0f, 0f, 0f, 1f, 1f, 0f, 1f, 1f };

            RenderableObject obj = new RenderableObject(
            game.renderer,
            "Default",
            vertices,
            uv,
            indices,
            normals,
            false
            );

            obj.scale = new Vector3(0.01f, 0.01f, 0.01f);
            obj.offset = new Vector3(-0.12f, 0.07f, 0);
            obj.rotation = new Vector3(90, 0, 0);

            obj.position = new Vector3(0, 0, game.board.position.Z);

            obj.material = new Material
            {
                shininess = 32f,
                specular = new Vector3(1f, 1f, 1f),
                ambient = new Vector3(0.1f, 0.1f, 0.1f),
                diffuse = new Vector3(0.5f, 0.5f, 0.5f),
            };

            coinObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// Checks if the given coin won the game
        /// </summary>
        public bool WinCheck(Coin coin, out Player winner, out Coin[] winningCoins)
        {
            winner = null;
            winningCoins = new Coin[connectAmount];

            if (!started) return false;
            if (ended) return false;
            if (coin == null) return false;

            // How many connections in the current row
            int connections = 0;

            // Directions to look in when checking connections
            Vector2[] directions = new Vector2[]
            {
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, -1),
            };


            // Check in each diredction "connectAmount" times
            for (int i = 0; i < directions.Length; i++)
            {
                for (int j = -connectAmount + 1; j < connectAmount; j++)
                {
                    Vector2 pos = directions[i] * j + coin.index;

                    if (pos.X > board.GetLength(0) - 1 || pos.X < 0) continue;
                    if (pos.Y > board.GetLength(1) - 1 || pos.Y < 0) continue;

                    if (board[(int)MathF.Round(pos.X), (int)MathF.Round(pos.Y)].owner == coin.owner)
                    {
                        winningCoins[connections++] = board[(int)MathF.Round(pos.X), (int)MathF.Round(pos.Y)];
                    }
                    else
                    {
                        connections = 0;
                    }

                    winner = players[currentPlayer];
                    if (connections >= connectAmount)
                    {
                        ended = true;
                        return true;
                    }
                }

                connections = 0;
            }

            return false;
        }

        /// <summary>
        /// Checks if the game is a draw
        /// </summary>
        public bool DrawCheck()
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                if (!board[i, board.GetLength(1) - 1].placed)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Pass Turn
        /// </summary>
        /// <returns>
        /// The current player after passing the turn
        /// </returns>
        public int PassTurn()
        {
            return PassTurn(currentPlayer + 1);
        }

        /// <summary>
        /// Pass Turn to <paramref name="nextPlayer"/>
        /// </summary>
        /// <returns>
        /// The current player after passing the turn
        /// </returns>
        public int PassTurn(int nextPlayer, bool suppressEvents = false)
        {
            if (!started) return -1;
            if (ended) return -1;

            currentPlayer = nextPlayer;
            currentPlayer %= players.Count;

            if (!suppressEvents)
            {
                OnPassTurnPersistent?.Invoke(players[currentPlayer]);
                OnPassTurn?.Invoke(players[currentPlayer]);
            }

            return currentPlayer;

        }

        public void StartGame()
        {
            started = true;

            OnStartGamePersistent?.Invoke(players[0]);

            // When the fist player is a bot, make it take a turn, since
            // it would otherwise stop the game from starting
            if (players[0].mode == Player.Mode.Bot) players[0].bot.NextTurn();
        }

        /// <summary>
        /// Resets the game, destroys all visible coins
        /// </summary>
        public void Cleanup()
        {
            ended = true;
            players.Clear();

            foreach (var coin in coinObjects)
            {
                coin.Destroy();
            }

            coinObjects.Clear();
        }
    }


    public class Coin
    {
        public Coin(float x, float y, float z)
        {
            pos = new Vector3(x, y, z);
        }

        public Coin(float x, float y, float z, bool placed)
        {
            pos = new Vector3(x, y, z);
            this.placed = placed;
        }

        public Vector3 pos;
        public bool placed;
        public Player owner = null;
        public Vector2i index;
        public bool noVisuals = false;
        public RenderableObject obj;
    }
}