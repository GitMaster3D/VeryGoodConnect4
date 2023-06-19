using Network;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

public class GameState
{

    public int[,] board; // Represend board as 2d array
                         // Each Players are saved in form of their textureID's, int.MinValue is an empty spot
    private int width, height;  

    public GameState(int width = 7, int height = 6)
    {
        this.width = width;
        this.height = height;

        Init(width, height);
    }

    /// <summary>
    /// Initializes the board
    /// </summary>
    private void Init(int width, int height)
    {
        board = new int[width, height];

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                board[i, j] = int.MinValue;
            }
        }
    }
    
    /// <summary>
    /// Resets the board
    /// </summary>
    public void Reset()
    {
        Init(width, height);
    }


    /// <summary>
    /// Applies the given move as the given player Player,
    /// bot is the player he plays against
    /// </summary>
    /// <returns>
    /// if the move was applyed
    /// </returns>
    public bool ApplyMove(int move, int playerID)
    {
        int y = GetLowestFreePosition(move);
        if (y < 0 || y >= board.GetLength(1))
        {
            Logging.LogWarning("Illegal Move!");
            return false;
        }

        board[move, y] = playerID;
        return true;
    }


    public int GetLowestFreePosition(int index)
    {
        if (board[index, board.GetLength(1) - 1] != int.MinValue) return -1;


        for (int i = board.GetLength(1) - 1; i >= 0; i--)
        {
            if (board[index, i] != int.MinValue)
            {
                return i + 1;
            }
        }

        return 0;
    }





    /// <summary>
    /// Checks if the given coin won the game
    /// </summary>
    /// <param name="connectAmount"> How many connections for a win are needed </param>
    /// <param name="move"> the highest coin of wich row to check </param>
    /// <param name="player"> textureID of the player who should be checked </param>
    /// <param name="winner"> textureID of the winning player </param>
    public bool WinCheck(int move, int player, int connectAmount = 4)
    {
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
                int lowestFree = GetLowestFreePosition(move);
                Vector2 pos = directions[i] * j + new Vector2(move, (lowestFree - 1) > 0 ? lowestFree - 1 : 0);

                if (pos.X > board.GetLength(0) - 1 || pos.X < 0) continue;
                if (pos.Y > board.GetLength(1) - 1 || pos.Y < 0) continue;

                if (board[pos.X, pos.Y] == player) connections++;
                else connections = 0;

                if (connections >= connectAmount)
                {
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
            if (board[i, board.GetLength(1) - 1] == int.MinValue)
            {
                return false;
            }
        }

        return true;
    }

    internal class Vector2
    {
        public int X;
        public int Y;

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator * (Vector2 a, int b)
        => new Vector2(a.X * b, a.Y * b);

        public static Vector2 operator + (Vector2 a, Vector2 b)
        => new Vector2(a.X + b.X, a.Y + b.Y);
    }
}