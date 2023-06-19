using Network;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

public class RpcMethods
{
    public static Dictionary<int, Action<Player>> rpcDictionary;

    public static bool gameRunning_ = false;
    public static bool gameRunning
    {
        get
        {
            return gameRunning_;
        }

        set
        {
            gameRunning_ = value;

            // Prevent connections in running games
            if (gameRunning_)
                Program.networkManager.StopAsyncHost();
            else
                Program.StartHost();
        }
    }

    /// <summary>
    /// Finds all Methods ending with "RPC" in this class
    /// and adds them to the RPC dictionary
    /// </summary>
    public static void Init()
    {
        rpcDictionary = new Dictionary<int, Action<Player>>();

        Type type = typeof(RpcMethods);
        MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        var rpcMethods = methods.Where(method => method.Name.EndsWith("RPC"));
        
        foreach (var method in rpcMethods)
        {
            Action<Player> action = (Action<Player>)Delegate.CreateDelegate(typeof(Action<Player>), null, method);

            Program.AddRpc(method.Name, action);
        }
    }

    /// <summary>
    /// Request for the toggling of a bot
    /// Sent by client
    /// </summary>
    private static void BotCheckRPC(Player sender)
    {
        bool newValue = (bool)Program.valueBuffers[sender].Dequeue().value;
        int playerDataID = (int)Program.valueBuffers[sender].Dequeue().value;

        ((PlayerData)Program.playerData.FirstOrDefault(c => ((PlayerData)c.value).textureID == playerDataID).value).mode = newValue ? Player.Mode.Bot : Player.Mode.Player;

        lock (Program.clients) lock (Program.playerData)
        {
            foreach (var client in Program.clients)
            {
                Program.SendValue(client.stream, newValue);
                Program.SendValue(client.stream, playerDataID);
                Program.SendRPC(client.stream, "BotCheckRPC");
            }
        }
    }

    /// <summary>
    /// Request for a namechange, sent by client
    /// </summary>
    private static void NameChangeRPC(Player sender)
    {
        string newName = (string)Program.valueBuffers[sender].Dequeue().value;
        int playerDataID = (int)Program.valueBuffers[sender].Dequeue().value;


        lock (Program.playerData)
        {
            for (int i = 0; i < Program.playerData.Count; i++)
            {
                if (((PlayerData)Program.playerData[i].value).textureID == playerDataID)
                {
                    if (((PlayerData)Program.playerData[i].value).name == newName) return; // Prevent infinite name update Loops
                    ((PlayerData)Program.playerData[i].value).name = newName;
                }
            }
        }

        lock (Program.clients) lock (Program.playerData)
        {
            foreach (var client in Program.clients)
            {
                Program.SendValue(client.stream, newName);
                Program.SendValue(client.stream, playerDataID);
                Program.SendRPC(client.stream, "NameChangeRPC");
            }
        }
    }


    /// <summary>
    /// Starts the game, calles by clients
    /// </summary>
    private static void StartGameRPC(Player sender)
    {
        // Update Player on server
        Program.currentPlayerClient = Program.playerData[0].sender;
        Program.currentPlayerClient.sentMove = false;
        Program.currentPlayer = 0;


        // Send starting player, currently always 0
        int starter = 0;

        lock (Program.clients) lock (Program.playerData)
        {
            foreach (var c in Program.clients)
            {
                Program.SendValue(c.stream, starter);
                Program.SendRPC(c.stream, "StartGameRPC");
            }
        }

        gameRunning = true;

        Thread.Sleep(300);
        RequestBotTurn();
    }

    /// <summary>
    /// Play move given by player if it is it's turn
    /// </summary>
    private static void SendMoveRPC(Player sender)
    {
        int move = (int)Program.valueBuffers[sender].Dequeue().value;

        if (sender != Program.currentPlayerClient) return; // Prevent moves from clients whose turn it isn't
        if (!gameRunning) return; // Dont accept moves if the game isnt running
        if (sender.sentMove) return;

        if (!Program.gameState.ApplyMove(move, Program.currentPlayerTextureID)) return; // Apply move locally and prevent illegal moves

        
        Logging.Log("Move by: " + Program.currentPlayerClient.name);


        
        lock (Program.clients) lock (Program.playerData)
        {
            sender.sentMove = true;

            foreach (var c in Program.clients)
            {

                Program.SendValue(c.stream, move);
                Program.SendValue(c.stream, Program.currentPlayerTextureID); // The player who made the move
                Program.SendValue(c.stream, Program.nextPlayerTextureID); // Tell player the next Player

                Program.SendRPC(c.stream, "SendMoveRPC");


            }
            // Winchecks
            if (Program.gameState.WinCheck(move, Program.currentPlayerTextureID))
            {
                Logging.LogWarning("Somebodey Won!");
                EndGame();
            }

            // Drawcheck
            if (Program.gameState.DrawCheck())
            {
                Logging.LogWarning("Draw!");
                EndGame();
            }
            ChangeTurn(sender);
        }
    }

    /// <summary>
    /// Tell each player the TurnChange
    /// </summary>
    public static void ChangeTurn(Player sender)
    {
        if (sender != Program.currentPlayerClient) return; // Prevent turn changing from other players

        // Update Player on server
        Program.currentPlayerClient = Program.playerData[++Program.currentPlayer % Program.playerData.Count].sender;
        Program.currentPlayerClient.sentMove = false;

        RequestBotTurn();
    }

    private static void RequestBotTurn()
    {
        lock (Program.clients) lock (Program.playerData)
        {
            // Request Bot turn if the next Player is a bot
            if (((PlayerData)Program.playerData[Program.currentPlayer % Program.playerData.Count].value).mode == Player.Mode.Bot)
            {
                Thread.Sleep(500); // Short delay to prevent instant turns

                Program.SendValue(Program.currentPlayerClient.stream, ((PlayerData)Program.playerData[Program.currentPlayer % Program.playerData.Count].value).textureID);
                Program.SendRPC(Program.currentPlayerClient.stream, "RequestBotMoveRPC");
            }
        }
    }

    public static void EndGame()
    {
        gameRunning = false;

        Program.gameState.Reset();


        foreach (var client in Program.clients)
        {
            Program.SendRPC(client.stream, "EndGameRPC");
        }
        
    }


    /// <summary>
    /// Log Value from valuebuffer, used for debugging
    /// </summary>
    public static void PrintNextValueRPC(Player sender)
    {
        Logging.Log(Program.valueBuffers[sender].Dequeue());
    }

    /// <summary>
    /// Confirm that a connection happened (Called by client)
    /// </summary>
    public static void ConnectionConfirmRPC(Player sender)
    {
        Logging.Log("Connection Confirmed by Client");
    }



    /// <summary>
    /// Close Connection to client
    /// </summary>
    public static void CloseRequestRPC(Player sender)
    {
        lock (Program.clients)
        {
            if (sender.stream != null)
            {
                sender.stream.Close();
                sender.stream.Dispose();
            }
            Program.clients.Remove(sender);
        }

        lock (Program.playerData)
        {
            List<int> toRemove = new List<int>();
            foreach (var player in Program.playerData)
            {
                if (player.sender == sender)
                {
                    toRemove.Add(((PlayerData)player.value).textureID);
                }
            }

            foreach (var p in toRemove)
            {
                RemovePlayer(p);
            }

            if (Program.clients.Count > 0 && Program.playerData.Count > 0)
            {
                Program.currentPlayerClient = Program.playerData[Program.currentPlayer % Program.playerData.Count].sender;
                Program.currentPlayerClient.sentMove = false;
                Logging.Log(Program.currentPlayerClient.name);
            }
        }

        Logging.Log("Disconnected " + sender.name);
    }


    /// <summary>
    /// Tell each Client the newly sent message, called by Client
    /// </summary>
    private static void SendChatMessageRPC(Player sender)
    {
        ValueItem data = Program.valueBuffers[sender].Dequeue();
        string message = (string)data.value;
        string datasender = data.sender.name;

    lock (Program.clients) lock (Program.playerData)
        {
            foreach (var p in Program.clients)
            {
                try
                {
                        Program.SendValue(p.stream, datasender);
                        Program.SendValue(p.stream, message);
                        Program.SendRPC(p.stream, "SendChatMessageRPC");
                


                }
                catch (Exception ex)
                {
                    Logging.LogError(ex);
                }
            }
        }
    }

    private static void GetCurrentPlayersRPC(Player sender)
    {
        lock (Program.clients) lock (Program.playerData)
        {
            Program.SendValue(sender.stream, Program.playerData.Count); // Send how many Players there are

            foreach (var player in Program.playerData)
            {
                Program.SendValue(sender.stream, (PlayerData)player.value); // Send Player
                Program.SendValue(sender.stream, player.sender == sender); // Send if clients owns the player or not
            }

            Program.SendRPC(sender.stream, "LoadPlayersRPC");
        }
    }

    private static void AddPlayersRPC(Player sender)
    {
        Logging.Log(sender.name + "Added the following Players: ");

        List<ValueItem> added = new List<ValueItem>();

        int amount = (int)Program.valueBuffers[sender].Dequeue().value; // How many players should be added
        

        lock (Program.clients) lock (Program.playerData)
        {
            for (int i = 0; i < amount; i++)
            {
                PlayerData data = (PlayerData)Program.valueBuffers[sender].Dequeue().value;

                // Block adding too many players
                // This check happens after dequing players to prevent Trash data on the valuebuffer
                if (Program.playerData.Count >= Program.maxPlayerCount)
                {
                    Logging.LogWarning($"Unable to add player, max count of {Program.maxPlayerCount} is reached!");
                    continue;
                }


                added.Add(new ValueItem(data, sender));

                Logging.Log(data.name);
                Program.ValidatePlayerColor(data); // Prevent double colors

                Program.playerData.Add(new ValueItem(data, sender));
            }

            foreach (var player in Program.clients)
            {
                Program.SendValue(player.stream, added.Count);

                foreach (var p in added)
                {
                    Program.SendValue(player.stream, (PlayerData)p.value);
                    Program.SendValue(player.stream, p.sender == player);
                }
                Program.SendRPC(player.stream, "AddPlayersRPC");
            }
        }
    }

    /// <summary>
    /// Removes the given Player,
    /// Called By Client
    /// </summary>
    private static void RemovePlayerRPC(Player sender)
    {
        int id = (int)Program.valueBuffers[sender].Dequeue().value;
        RemovePlayer(id);
    }

    /// <summary>
    /// Removes the Player corrosponding to <paramref name="id"/>
    /// </summary>
    /// <param name="id"> The colorID of the player that should be removed </param>
    private static void RemovePlayer(int id)
    {
        Logging.Log("Removing " + id);
        Logging.Log("PlayerCount: " + Program.playerData.Count);

        lock (Program.clients) lock(Program.playerData)
        {
            // Tell Clients to remove given Player
            foreach (var player in Program.clients)
            {
                Program.SendValue(player.stream, id);
                Program.SendRPC(player.stream, "RemovePlayerRPC");
            }


            // Remove Player on server
            for (int i = 0; i < Program.playerData.Count; i++)
            {
                if (((PlayerData)Program.playerData[i].value).textureID == id)
                {
                    Program.playerData.Remove(Program.playerData[i]);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Change Color of player,
    /// Caleld By Client
    /// </summary>
    private static void ChangeColorRPC(Player sender)
    {
        lock (Program.playerData)
        {
            int id = (int)Program.valueBuffers[sender].Dequeue().value;
            foreach (var playerData in Program.playerData)
            {
                if (((PlayerData)playerData.value).textureID == id)
                {
                    PlayerData data = (PlayerData)playerData.value;
                    int originalID = data.textureID;

                    data.textureID = (data.textureID + 1) % Program.colorAmount;

                    lock (Program.clients)
                    {
                        foreach (var player in Program.clients)
                        {
                            Program.SendValue(player.stream, originalID); // Original TextureID
                            Program.SendValue(player.stream, Program.ValidatePlayerColor(data)); // New TextureID
                            Program.SendRPC(player.stream, "ChangeColorRPC"); // Change Color
                        }
                    }


                    return;
                }
            }
        }
    }
}