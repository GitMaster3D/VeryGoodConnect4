using _4Gewinnt__nicht_;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Shaders;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Network
{
    public class NetworkHandler
    {
        public static Queue<object> valueBuffer = new Queue<object>();
        public static Stream server { get; set; }
        public static Process serverHost;
        
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


            Logging.Log("Sending PlayerData");
        }


        public static void SendValue(Stream stream, PlayerData[] value)
        {
            foreach (var data in value)
            {
                SendValue(stream, data);
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
        public static string ParseString(Stream stream, int length)
        {
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
        public static int ParseInt(Stream stream)
        {
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


        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }


        /// <summary>
        /// Handle Connection
        /// </summary>
        /// <param name="client"> Wich Client to connect </param>
        /// <param name="name"> Username of the client </param>
        public static async void OnConnect(TcpClient client, string name, PlayerData[] players)
        {
            await Task.Run(() =>
            {
                Stream stream = client.GetStream();

                SendValue(stream, name); // Tell server the chosen Username

                
                SendValue(stream, players.Length); // Tell server how many players should join
                SendValue(stream, players); // Tell server wich players should join
                SendRPC(stream, "AddPlayersRPC");
                
                
                SendRPC(stream, "GetCurrentPlayersRPC"); // Request Current Players form server
                                                         // Loading is handled in an RPC Call


                SendRPC(stream, "ConnectionConfirmRPC"); // Confirm succsessful connection


                while (true)
                {
                    try
                    {
                        ParseInput(stream);
                    }
                    catch (Exception ex)
                    {
                        Logging.LogError(ex + "\n Closing connection");


                        // Try telling the server that an error happened.
                        // Before closing the connection
                        try
                        {
                            RpcMethods.SendCloseRequest(stream);
                        }
                        catch (Exception e)
                        {
                            Logging.LogWarning("Failed to send close request \n" + e);
                        }


                        client.Close();
                        server = null;

                        // Make it clearer that the connection ended
                        MessageBox.Show("Disconnected from the Server!");


                        Game.instance.Reset();

                        // Exit to main menu
                        MenuManager.menus["MainMenu"].Invoke(() =>
                        {
                            MenuManager.ExitToMain();
                        });


                        return;
                    }
                    Task.Delay(10);
                }
            });
        }




        public static void ParseInput(Stream stream)
        {
            if (stream == null) return;

            int command = stream.ReadByte();

            switch ((CallType)command)
            {
                case CallType.value: // Value transmition
                    ParseValue(stream);
                    break;


                case CallType.rpc: // RPC call

                    byte[] rpcHashBytes = new byte[4];

                    try
                    {
                        for (int i = 0; i < rpcHashBytes.Length; i++)
                        {
                            rpcHashBytes[i] = (byte)stream.ReadByte();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.LogError("Unable to parse RPC call: " + ex);
                    }


                    int rpcHash = BitConverter.ToInt32(rpcHashBytes, 0);
                    RpcMethods.rpcDictionary[rpcHash]?.Invoke();

                    break;


                    // Ignore default case
                default:
                    return;
            }
        }


        /// <summary>
        /// Prints the first value from the valueBuffer
        /// </summary>
        public static void PrintValue()
        {
            int value = (int)valueBuffer.Dequeue();
            Logging.Log(value);
        }


        public static void ParseValue(Stream stream)
        {
            DataType mode = (DataType)stream.ReadByte();

            switch (mode)
            {
                case DataType.int_:
                    valueBuffer.Enqueue(ParseInt(stream));
                    break;


                case DataType.string_:
                    valueBuffer.Enqueue(ParseString(stream, ParseInt(stream)));
                    break;

                case DataType.playerData_:
                    valueBuffer.Enqueue(ParsePlayerData(stream, ParseInt(stream)));
                    break;

                case DataType.bool_:
                    valueBuffer.Enqueue(ParseBool(stream));
                    break;

                    // Ignore default case
                default:
                    return;
            }
        }
    }
}