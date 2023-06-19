#define AUTOSHUTDOWN

using Network;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Timers;

public class Program
{
    public static Dictionary<Player, Queue<ValueItem>> valueBuffers = new Dictionary<Player, Queue<ValueItem>>();
    public static SynchronizationContext synchronizationContext;

    public static List<Player> clients; // Clients
    public static List<ValueItem> playerData; // Players and their owners

    public static int maxPlayerCount = 4;
    public static int colorAmount = 8;

    public static int currentPlayer = 0;
    public static Player currentPlayerClient; // Client who owns the current player


    public static NetworkManager networkManager;

    public static bool started = false; // If a connection to this server happened once
    public static float serverCloseTimer; // How long until the server closes
    public static int serverCloseCheckTime = 100; // how fast (milliseconds) the delay between connection checks is
    public static float serverCloseDelay = 1; // how long (seconds) it takes for the server to close after all clients disconnected
    private static ServerCloseMode serverCloseMode = ServerCloseMode.Restart;
    public static float serverStartTimeoutDelay = 10; // how long (seconds) it takes for the server to restart itself when not able to start the Tcp listener

    public static int currentPlayerTextureID
    {
        get
        {
            lock (playerData)
            {
                return ((PlayerData)Program.playerData[(Program.currentPlayer) % Program.playerData.Count].value).textureID;
            }
        }
    }

    public static int nextPlayerTextureID
    {
        get
        {
            lock (playerData)
            {
                return ((PlayerData)Program.playerData[((Program.currentPlayer + 1) % Program.playerData.Count)].value).textureID;
            }
        }
    }

    enum ServerCloseMode
    {
        Close = 0,
        Restart = 1
    }

    public static GameState gameState;

    public static void Main()
    {
        synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();

        RpcMethods.Init();


        clients = new List<Player>();
        playerData = new List<ValueItem>();
        gameState = new GameState();


        Console.Title = "Connect4 Server";

#if AUTOSHUTDOWN
        // Server Timeout Timer
        // Restarts the server after the specified delay
        System.Timers.Timer timer = new System.Timers.Timer();
        timer.Interval = (int)(serverStartTimeoutDelay * 1000);
        timer.Elapsed += (object sender, ElapsedEventArgs e) =>
        {
            RestartServer("Restarting Server due to timeout!");
        };
        timer.Start();
#endif

        // Host Server
        StartHost();

#if AUTOSHUTDOWN
        // Stop the ServerTimeout timer from restarting
        // The server since the host was succsessful
        timer.Stop();
        timer.Dispose();
#endif


        ResetServerCloseTimer();

        while (true)
        {
#if AUTOSHUTDOWN
            if (clients.Count <= 0 && started)
            {
                serverCloseTimer -= 1;
            }

            if (serverCloseTimer <= 0 && started)
            {
                if (serverCloseMode == ServerCloseMode.Restart)
                {
                    RestartServer("Restarted server due to no players being connected!");
                }
                else
                {
                    Logging.LogWarning("Closing server due to no connected players");
                    Environment.Exit(0);
                }
            }
#endif

            Thread.Sleep(serverCloseCheckTime);
        }
    }

    public static void RestartServer(string msg)
    {
        Console.Clear();
        Logging.LogWarning(msg);

        // Restart app
        Process.Start(AppDomain.CurrentDomain.FriendlyName);
        Environment.Exit(0);
    }

    public static void ResetServerCloseTimer()
    {
        serverCloseTimer = (serverCloseDelay * 1000) / (serverCloseCheckTime);
    }

    public static void StartHost()
    {


        networkManager = new NetworkManager();
        string ip = networkManager.LocalIPAddress.ToString() ?? "127.0.0.1";
        int port = 1234;


        networkManager.MultiHostAsync(port, OnConnect);


        // Log IP and Port
        Console.WriteLine(ip);
        Console.WriteLine(port);
    }

    public static void SendRPC(Stream stream, string methodName)
    {
        stream.WriteByte((byte)CallType.rpc); // Tell server that a RPC is called
        stream.Write(GetHash(methodName), 0, sizeof(int)); // Send the methodname as a hash
    }

    public static void SendValue(Stream stream, int value)
    {
        stream.WriteByte((byte)CallType.value); // Tell client that a value follows 
        stream.WriteByte((byte)DataType.int_); // Tell the client that that value is an int

        stream.Write(BitConverter.GetBytes(value), 0, sizeof(int)); // Send the Value
    }


    public static void SendValue(Stream stream, string value)
    {
        stream.WriteByte((byte)CallType.value); // Tell client that a value follows
        stream.WriteByte((byte)DataType.string_); // Tell client that that value is a string


        byte[] bytes = Encoding.UTF8.GetBytes(value);

        stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int)); // Tell the client how many bytes that string contains

        stream.Write(bytes, 0, bytes.Length);
    }

    public static void SendValue(Stream stream, PlayerData value)
    {
        stream.WriteByte((byte)CallType.value); // Tell client that a value follows
        stream.WriteByte((byte)DataType.playerData_); // Tell client that that value is a string

        byte[] bytes = value.GetFormatedBytes(); // Get Playerdata as bytes

        stream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int)); // Tell client the amount of bytes this takes
        stream.Write(bytes, 0, bytes.Length); // Send the data
    }


    public static void SendValue(Stream stream, PlayerData[] value)
    {
        foreach (var data in value)
        {
            SendValue(stream, data);
        }
    }

    /// <summary>
    /// Sends the values of <paramref name="value"/> to the given client
    /// TREATED AS PLAYERDATA!!!
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="value"></param>
    public static void SendValue(Stream stream, ValueItem[] value)
    {
        foreach (var data in value)
        {
            SendValue(stream, (PlayerData)data.value);
        }
    }


    /// <summary>
    /// Sends bool
    /// </summary>
    public static void SendValue(Stream stream, bool value)
    {
        stream.WriteByte((byte)CallType.value);
        stream.WriteByte((byte)DataType.bool_);
        stream.WriteByte(BitConverter.GetBytes(value)[0]);
    }

    /// <summary>
    /// reada <paramref name="stream"/> and gets the next
    /// byte and parses it as Bool 
    /// </summary>
    /// <param name="stream"></param>
    public static bool ParseBool(Stream stream)
    {
        return BitConverter.ToBoolean(new byte[] { (byte)stream.ReadByte() }, 0);
    }

    /// <summary>
    /// Reads <paramref name="bufferLength"/> bytes and turns
    /// Them into <see cref="PlayerData"/>
    /// </summary>
    public static PlayerData ParsePlayerData(Stream stream, int bufferLength)
    {
        byte[] buffer = new byte[bufferLength];
        stream.Read(buffer, 0, bufferLength);
        return PlayerData.GetPlayerData(buffer);
    }


    /// <summary>
    /// Reads The next <paramref name="length"/> bytes on the stream and
    /// turns them into an int
    /// </summary>
    /// <returns>
    /// The resulting int
    /// </returns>
    public static string ParseString(Stream stream, int length, bool removeModeCall = false)
    {
        if (removeModeCall)
        {
            stream.ReadByte();
            stream.ReadByte();
        }

        byte[] bytes = new byte[length];
        stream.Read(bytes, 0, bytes.Length);

        return System.Text.Encoding.UTF8.GetString(bytes);
    }


    /// <summary>
    /// Reads The next 4 bytes on the stream and
    /// turns them into an int
    /// </summary>
    /// <returns>
    /// The resulting int
    /// </returns>
    public static int ParseInt(Stream stream, bool removeFormatting = false)
    {
        if (removeFormatting)
        {
            stream.ReadByte();
        }

        byte[] bytes = new byte[4];
        stream.Read(bytes, 0, bytes.Length);

        return BitConverter.ToInt32(bytes, 0);
    }


    enum DataType : byte
    {
        int_ = 0,
        string_ = 1,
        playerData_ = 2,
        bool_ = 3
    }

    enum CallType : byte
    {
        value = 1,
        rpc = 2
    }

    
    /// <summary>
    /// Checks if color of <paramref name="player"/> is
    /// in use, if yes, change it to the next possible
    /// unused color
    /// </summary>
    /// <returns>
    /// The new ColorID, -1 when this operation failed
    /// </returns>
    public static int ValidatePlayerColor(PlayerData player)
    {
        if (playerData.Count > colorAmount) return -1; // Prevent stackoverflow

        for (int i = 0; i < playerData.Count; i++)
        {
            if (playerData[i].value == player) continue;

            if (player.textureID == ((PlayerData)playerData[i].value).textureID)
            {
                player.textureID = (player.textureID + 1) % colorAmount;
                ValidatePlayerColor(player);
            }
        }

        return player.textureID;
    }


    public static async void OnConnect(TcpClient client)
    {
        started = true; // Enable auto shutdown
        ResetServerCloseTimer(); // Reset auto shutdown timer

        await Task.Run(() =>
        {
            Stream stream = client.GetStream();

            // Skip calltype and valuetype bytes
            stream.ReadByte();
            stream.ReadByte();
            

            int nameLength = ParseInt(stream);
            string name = ParseString(stream, nameLength);


            // Log chosen name
            Logging.Log(name + " Connected!");

            SendRPC(stream, "ConnectionConfirmRPC");
            Player player = new Player(stream, name);

            lock (clients)
            {
                // Save Player
                clients.Add(player);

                valueBuffers.Add(player, new Queue<ValueItem>());
            }

            while (true)
            {
                try
                {
                    if (!player.stream.CanWrite || !player.stream.CanRead || player.stream == null) throw new Exception("Player is null!");

                    ParseInput(player);
                }
                catch (Exception ex)
                {
                    Logging.LogError(ex + "\n Closing connection");

                    valueBuffers.Remove(player);
                    RpcMethods.CloseRequestRPC(player);


                    stream.Close();
                    stream.Dispose();
                    stream = null;


                    return;
                }
                Task.Delay(10);
            }
        });
    }

    public static void ParseInput(Player player)
    {
        int command = player.stream.ReadByte();

        switch (command)
        {
            // Ignore -1 Case as it means there is no command
            case -1:
                break;


            case 1: // Value transmition
                ParseValue(player);
                break;


            case 2: // RPC call

                byte[] rpcHashBytes = new byte[4];

                try
                {
                    player.stream.Read(rpcHashBytes, 0, rpcHashBytes.Length);
                }
                catch (Exception ex)
                {
                    Logging.LogError("Unable to parse RPC call: " + ex);
                }


                int rpcHash = BitConverter.ToInt32(rpcHashBytes, 0);
                RpcMethods.rpcDictionary[rpcHash]?.Invoke(player);

                break;


            default:
                Logging.LogError("Unkown command: " + command);
                return;
        }
    }


    public static void ParseValue(Player player)
    {
        DataType mode = (DataType)player.stream.ReadByte();

        switch (mode)
        {
            case DataType.int_:
                valueBuffers[player].Enqueue(new ValueItem(ParseInt(player.stream), player));

                Logging.Log("Reciving int");
                break;


            case DataType.string_:
                valueBuffers[player].Enqueue(new ValueItem(ParseString(player.stream, ParseInt(player.stream)), player));

                Logging.Log("Reciving string");
                break;


            case DataType.playerData_:
                valueBuffers[player].Enqueue(new ValueItem(ParsePlayerData(player.stream, ParseInt(player.stream)), player));

                Logging.Log("Reciving PlayerData");
                break;


            case DataType.bool_:
                valueBuffers[player].Enqueue(new ValueItem(ParseBool(player.stream), player));

                Logging.Log("Reciving bool");
                break;


            default:
                Logging.LogError("Unkown mode: " + (int)mode);
                return;
        }
    }



    public static void AddRpc(string name, Action<Player> action)
    {
        int hash = BitConverter.ToInt32(GetHash(name), 0);

        RpcMethods.rpcDictionary.Add(hash, action);
    }

    public static byte[] GetHash(string inputString)
    {
        using (HashAlgorithm algorithm = SHA256.Create())
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }
}

public class ValueItem
{
    public object value;
    public Player sender;

    public ValueItem(object value, Player sender)
    {
        this.value = value;
        this.sender = sender;
    }
}

public class Player
{
    public string name;
    public Stream stream;
    public bool sentMove;


    public Player(Stream stream, string name)
    {
        this.stream = stream;
        this.name = name;
    }

    public enum Mode : byte
    {
        Player = 0,
        Bot = 1
    }


}


/// <summary>
/// Can be used to store playerdata without
/// an exisiting GLcontrol.
/// 
/// Also, can be used to store playerInformation on the form
/// for easyer use of the add and check events
/// </summary>
public class PlayerData
{
    public string name;
    public string texture;

    public int textureID; // used for cycling colors
    public Player.Mode mode;

    /// <summary>
    /// Generates Bytearray from the variables in this
    /// </summary>
    /// <returns>
    /// A Formatted Bytearray
    /// </returns>
    public byte[] GetFormatedBytes()
    {
        List<byte> bytes = new List<byte>();

        // Get Bytes of values
        byte[] nameBytes = Encoding.UTF8.GetBytes(name);
        byte[] textureBytes = Encoding.UTF8.GetBytes(texture);
        byte[] textureIDBytes = BitConverter.GetBytes(textureID);
        byte mode = (byte)this.mode;

        // Save bytes in a formatted way
        bytes.Add((byte)nameBytes.Length); // Name Length
        bytes.AddRange(nameBytes);

        bytes.Add((byte)textureBytes.Length); // textureLength
        bytes.AddRange(textureBytes);

        bytes.AddRange(textureIDBytes); // textureID bytelength is fixed
        bytes.Add(mode); // Mode is only one byte


        return bytes.ToArray();
    }

    /// <summary>
    /// Converts the data from the GetFormattedBytes Method
    /// Back to a PlayerData object
    /// </summary>
    public static PlayerData GetPlayerData(byte[] data)
    {
        PlayerData playerData = new PlayerData();

        playerData.name = Encoding.UTF8.GetString(data.SubArray<byte>(1, data[0])); // Get Name

        playerData.texture = Encoding.UTF8.GetString(data.SubArray<byte>(data[0] + 2, data[data[0] + 1])); // Get Texture

        playerData.textureID = BitConverter.ToInt32(data.SubArray<byte>(data.Length - 5, 4)); // Get TextureID
        playerData.mode = (Player.Mode)data[data.Length - 1]; // Get Mode

        return playerData;
    }
}