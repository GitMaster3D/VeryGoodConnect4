//#define LOG

using Network;
using OpenTK.Mathematics;

namespace _4Gewinnt__nicht_
{
    public class MinMaxBot
    {
        private GamestateHandler gamestateHandler;
        private Player bot, enemy;

        public int searchDeph = 100;


        public MinMaxBot(GamestateHandler gamestateHandler, Player player, Player enemy, int connectAmount)
        {
            this.gamestateHandler = gamestateHandler;
            this.bot = player;
            this.enemy = enemy;

            gamestateHandler.OnPassTurn += (Player player) =>
            {
                if (player != this.bot) return;
                NextTurn();
            };
        }


        public async void NextTurn()
        {

            if (gamestateHandler.ended) return;
            if (bot.mode != Player.Mode.Bot) return;
            if (NetworkHandler.server != null) return; // Prevent bot moves in online mode
#if LOG
            Console.Clear();
#endif

            int bestMove = await GetBestMove(300);

            Coin[] coins = new Coin[0];
            if (gamestateHandler.CanAddCoin(bestMove))
            {
                gamestateHandler.WinCheck(gamestateHandler.AddCoin(bestMove), out Player winner, out coins);
                gamestateHandler.DrawCheck();
                gamestateHandler.PassTurn();
            }
            else
            {
                Logging.LogError("bestMove was invalid! using Fallback!");

                for (int i = 0; i < gamestateHandler.board.GetLength(0); i++)
                {
                    if (gamestateHandler.CanAddCoin(i))
                    {
                        gamestateHandler.WinCheck(gamestateHandler.AddCoin(i), out Player winner, out coins);
                        gamestateHandler.PassTurn();
                        return;
                    }
                }
                Logging.LogError("no possible move was found!");
                return;
            }
        }


        public int GetBestMove()
        {

            int bestMove = 0;
            int bestScore = int.MinValue;


            SubGame sub = SubGame.BoardToSubgame(gamestateHandler.board, bot, 4);
            
            List<int> immediateThreats = sub.ImmediateThreatCheck(enemy, bot);

            if (immediateThreats.Count > 0)
            {
#if LOG
                Logging.LogWarning("Immediate Threat at " + immediateThreats[0]);
#endif


                // Block random immediate threat
                Random rnd = new Random();
                return immediateThreats[rnd.Next(0, immediateThreats.Count)];
            }
            else
            {
                for (int i = 0; i < gamestateHandler.board.GetLength(0); i++)
                {
                    SubGame sub_ = SubGame.BoardToSubgame(gamestateHandler.board, bot, 4);
                    if (!sub_.ApplyMove(i, bot, bot)) continue;


                    int score = Minimax(sub_, searchDeph, bot, int.MaxValue, int.MinValue);


                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = i;
                    }
                }
            }

            

            return bestMove;
        }

        public async Task<int> GetBestMove(int delay)
        {
            await Task.Delay(delay);
            return GetBestMove();
        }


        private int Minimax(SubGame game, int depth, Player player, int alpha, int beta)
        {
            // If game is over or depth = 0, evaluate the score from the perspective of the bot.
            int value = game.EvaluateGame(out int ownConnects, out int enemyConnects);
            if (depth == 0 || game.IsWinningGame(enemyConnects, ownConnects))
            {
#if LOG
                game.PrintState();
                Logging.Log(value);
#endif
                return value;
            }

            if (player == bot)
            {
                // The bot is the maximizing player.
                int maxEval = int.MinValue;

                foreach (var move in game.GetPossibleMoves())
                {
                    SubGame newGame = game.Clone();
                    newGame.ApplyMove(move, player, bot);
                    int eval = Minimax(newGame, depth - 1, enemy, alpha, beta);

                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);

                    // Alpha-Beta Pruning
                    if (beta <= alpha)
                        break;
                }

                return maxEval;
            }
            else
            {
                // The enemy is the minimizing player.
                int minEval = int.MaxValue;

                foreach (var move in game.GetPossibleMoves())
                {
                    SubGame newGame = game.Clone();

                    newGame.ApplyMove(move, player, bot);
                    int eval = Minimax(newGame, depth - 1, bot, alpha, beta);

                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);

                    // Alpha-Beta Pruning
                    if (beta <= alpha)
                        break;
                }

                return minEval;
            }
        }
    }


    /// <summary>
    /// A more efficient way of saving a connect4 game for
    /// use with Minmax functions, as it dosn't store values
    /// needed for visual output.
    /// 
    /// Contains useful Methods for use with bots
    /// </summary>
    public class SubGame
    {
        static readonly int[] ownConnectionWeights =
        {
            0, // 0 connections
            50, // 1 connections
            100, // 2 connections
            300, // 3 Connections
            1000 // 4 Connections
        };

        static readonly int[] enemyConnectionWeights =
        {
            0, // 0 connections
            50, // 1 connections
            100, // 2 connections
            300, // 3 Connections
            1000 // 4 Connections
        };

        // Directions to look at while evaluating game
        static readonly Vector2i[] directions = new Vector2i[]
        {
            new Vector2i(1, 0),
            new Vector2i(0, 1),
            new Vector2i(1, 1),
            new Vector2i(1, -1)
        };

        // how many points for each y level are removed
        static readonly int heightPenalty = 80;

        public enum BoardPosition : sbyte
        {
            undifined = -1,
            notOwned = 0,
            owned = 1,
            outOfBounds = 2
        }

        private BoardPosition[,] board;
        private int connectAmount;

        public SubGame(int width, int height, int connectAmount)
        {
            board = new BoardPosition[width, height];
            this.connectAmount = connectAmount;
        }

        /// <summary>
        /// Generates a list of all possible moves
        /// that can be played on this board
        /// </summary>
        public List<int> GetPossibleMoves()
        {
            List<int> moves = new List<int>();

            for (int i = 0; i < board.GetLength(0); i++)
            {
                int y = GetLowestFreePosition(i);
                if (y > -1)
                {
                    moves.Add(i);
                }
            }


            return moves;
        }


        /// <summary>
        /// Applies the given move as the given player Player,
        /// bot is the player he plays against
        /// </summary>
        /// <returns>
        /// if the move was applyed
        /// </returns>
        public bool ApplyMove(int move, Player player, Player bot)
        {
            int y = GetLowestFreePosition(move);
            if (y < 0 || y > board.GetLength(1) - 1)
            {
                Logging.LogWarning("Illegal Move!");
                return false;
            }

            board[move, y] = player == bot ? BoardPosition.owned : BoardPosition.notOwned;
            return true;
        }

        /// <summary>
        /// Undoes the last move in the given row
        /// </summary>
        public void UndoMove(int move)
        {
            int y = GetLowestFreePosition(move);
            board[move, (int)Mathmatics.Clamp(0, board.GetLength(1) - 1, y - 1)] = BoardPosition.undifined;
        }


        public int GetLowestFreePosition(int index)
        {
            if (board[index, board.GetLength(1) - 1] != BoardPosition.undifined) return -1;


            for (int i = board.GetLength(1) - 1; i >= 0; i--)
            {
                if (board[index, i] != BoardPosition.undifined)
                {
                    return i + 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Calculates the lowest placed coin in the given row
        /// </summary>
        /// <returns>
        /// the y index of the lowest coin in the given row, is
        /// 0 when no coin is in the given row
        /// </returns>
        public int GetLowestCoinIndex(int index, out BoardPosition pos)
        {
            int y = GetLowestFreePosition(index);
            y = (int)Mathmatics.Clamp(0, board.GetLength(1) - 1, y);

            pos = board[index, y];
            return y;
        }


        /// <summary>
        /// Logs the current board state to the console, with the following
        /// color codes:
        /// 
        /// Gray: Not placed
        /// Green: "Owned" (was placed as "bot")
        /// Red: "Not owened" (was placed but not as "bot")
        /// 
        /// </summary>
        public void PrintState()
        {
            for (int i = 0; i < board.GetLength(1); i++)
            {
                for (int j = 0; j < board.GetLength(0); j++)
                {
                    if (board[j, i] == BoardPosition.owned) Console.ForegroundColor = ConsoleColor.Green;
                    if (board[j, i] == BoardPosition.notOwned) Console.ForegroundColor = ConsoleColor.Red;
                    if (board[j, i] == BoardPosition.undifined) Console.ForegroundColor = ConsoleColor.DarkGray;

                    Console.Write(0);

                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Checks if somebodey in the current subgame
        /// has won, based on the given connection lengths
        /// </summary>
        public bool IsWinningGame(int enemyConnects, int ownConnects)
        {
            return ownConnects >= connectAmount || enemyConnects >= connectAmount;
        }


        /// <summary>
        /// Checks if somebodey in the current subgame
        /// has won
        /// </summary>
        public bool IsWinningGame()
        {
            EvaluateGame(out int ownConnects, out int enemyConnects);
            return ownConnects >= connectAmount || enemyConnects >= connectAmount;
        }


        /// <summary>
        /// Evaluates how favorable the current
        /// subgame is for the player "owning" this game
        /// (owning refers to who places the coins with the Addcoin function as "bot")
        /// </summary>
        /// <returns>
        /// A score of how favorable the current subgame is
        /// based on how connectionlength of each player
        /// </returns>
        public int EvaluateGame()
        {
            return EvaluateGame(out int a, out int b);
        }


        /// <summary>
        /// Evaluates how favorable the current
        /// subgame is for the player "owning" this game
        /// (owning refers to who places the coins with the Addcoin function as "bot")
        /// </summary>
        /// <returns>
        /// A score of how favorable the current subgame is
        /// based on how connectionlength of each player
        /// </returns>
        public int EvaluateGame(out int ownMaxConnections, out int enemyMaxConnections)
        {
            int value = 0;
            ownMaxConnections = 0;
            enemyMaxConnections = 0;


            for (int k = 0; k < board.GetLength(0); k++)
            {
                value = (int)MathF.Max(value, EvaluateRow(k, out int ownConnects, out int enemyConnects));


                ownMaxConnections = (int)MathF.Max(ownMaxConnections, ownConnects);
                enemyMaxConnections = (int)MathF.Max(enemyMaxConnections, enemyConnects);
            }


#if LOG
    Logging.Log("Own Connections: " + ownMaxConnections);
    Logging.Log("EnemyConnections: " + enemyMaxConnections);
#endif

            return value;
        }

        /// <summary>
        /// Evaluates how valuable the given row on
        /// this current subgame is. Also takes 
        /// height of the coin into account
        /// </summary>
        /// <returns>
        /// A score based on how much connections each player has
        /// </returns>
        public int EvaluateRow(int column, out int ownMaxConnections, out int enemyMaxConnections)
        {
            int y = GetLowestCoinIndex(column, out BoardPosition owner);
            Vector2i boardPos = new Vector2i(column, y);

            return EvaluateCoin(boardPos, out ownMaxConnections, out enemyMaxConnections) - y * heightPenalty * (board[column, y] == BoardPosition.owned ? 1 : -1);
        }

        /// <summary>
        /// Checks how many connections a given coin has
        /// </summary>
        public int EvaluateCoin(Vector2i boardPos, out int ownMaxConnections, out int enemyMaxConnections)
        {
            ownMaxConnections = 0;
            enemyMaxConnections = 0;

            for (int i = 0; i < directions.Length; i++)
            {
                int ownConnections = 0;
                int enemyConnections = 0;

                for (int j = -connectAmount + 1; j < connectAmount; j++)
                {
                    Vector2i pos = directions[i] * j + boardPos;



                    BoardPosition boardPosition;


                    // Prevent outofbounds exception
                    if (pos.X > board.GetLength(0) - 1 || pos.X < 0)
                    {
                        boardPosition = BoardPosition.outOfBounds;
                        goto endSafetyCheck;
                    }

                    // Prevent outofbounds exception
                    if (pos.Y > board.GetLength(1) - 1 || pos.Y < 0)
                    {
                        boardPosition = BoardPosition.outOfBounds;
                        goto endSafetyCheck;
                    }

                    // Check board state
                    boardPosition = board[pos.X, pos.Y];


                endSafetyCheck:;



                    switch (boardPosition)
                    {
                        case BoardPosition.owned:
                            ownConnections++;
                            enemyConnections = 0;
                            if (ownConnections > ownMaxConnections) ownMaxConnections = ownConnections;
                            break;

                        case BoardPosition.notOwned:
                            enemyConnections++;
                            ownConnections = 0;
                            if (enemyConnections > enemyMaxConnections) enemyMaxConnections = enemyConnections;
                            break;


                        case BoardPosition.outOfBounds:
                            ownConnections = 0;
                            enemyConnections = 0;
                            break;
                    }
                }
            }

            int value = 0;

            if (ownMaxConnections >= connectAmount) value += 10000;  // If bot is one move away from winning
            else value += ownConnectionWeights[ownMaxConnections];

            if (enemyMaxConnections >= connectAmount) value -= 10000;  // If enemy is one move away from winning
            else value -= enemyConnectionWeights[enemyMaxConnections];



            return value;
        }


        /// <summary>
        /// Check if a Player can win
        /// the next turn
        /// </summary>
        /// <returns>
        /// a list of all moves that can win or lose the next turn
        /// </returns>
        public List<int> ImmediateThreatCheck(Player player, Player bot)
        {
            List<int> columns = new List<int>();

            foreach (var move in GetPossibleMoves())
            {
                EvaluateRow(move, out int botMaxConnections, out int playerMaxConnections);

                if (botMaxConnections >= connectAmount - 1) columns.Add(move);
                if (playerMaxConnections >= connectAmount -1) columns.Add(move);
            }

            return columns;
        }

        /// <summary>
        /// Copys the current subgame
        /// </summary>
        /// <returns>
        /// a copy of this subgame
        /// </returns>
        public SubGame Clone()
        {
            SubGame clone = new SubGame(board.GetLength(0), board.GetLength(1), connectAmount);

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    clone.board[i, j] = board[i, j];
                }
            }

            return clone;
        }

        /// <summary>
        /// Converts a "normal" gamestate to a subgame
        /// </summary>
        /// <returns>
        /// A subgame with the same boardstate
        /// as the given "normal" game
        /// </returns>
        public static SubGame BoardToSubgame(Coin[,] board, Player bot, int connectAmount)
        {
            SubGame subGame = new SubGame(board.GetLength(0), board.GetLength(1), connectAmount);

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {

                    if (board[i, j].owner == bot)
                    {
                        subGame.board[i, j] = BoardPosition.owned;
                    }
                    else if (board[i, j].owner == null)
                    {
                        subGame.board[i, j] = BoardPosition.undifined;
                    }
                    else
                    {
                        subGame.board[i, j] = BoardPosition.notOwned;
                    }
                }
            }

            return subGame;
        }
    }
}