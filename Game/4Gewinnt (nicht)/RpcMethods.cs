using _4Gewinnt__nicht_;
using Shaders;
using System.Reflection;

namespace Network
{
    public class RpcMethods
    {
        public static Dictionary<int, Action> rpcDictionary;
        private static bool initialized = false;

        /// <summary>
        /// Finds all Methods ending with "RPC" in this class
        /// and adds them to the RPC dictionary
        /// </summary>
        public static void Init()
        {
            if (initialized) return;

            rpcDictionary = new Dictionary<int, Action>();

            Type type = typeof(RpcMethods);
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            var rpcMethods = methods.Where(method => method.Name.EndsWith("RPC"));

            foreach (var method in rpcMethods)
            {
                Action action = (Action)Delegate.CreateDelegate(typeof(Action), null, method);

                AddRpc(method.Name, action);
            }

            initialized = true;
        }

        public static void AddRpc(string name, Action action)
        {
            int hash = BitConverter.ToInt32(NetworkHandler.GetHash(name), 0);

            RpcMethods.rpcDictionary.Add(hash, action);
        }

        /// <summary>
        /// Updates if Player is bot or not.
        /// Called by server
        /// </summary>
        private static void BotCheckRPC()
        {
            bool value = (bool)NetworkHandler.valueBuffer.Dequeue();
            int playerID = (int)NetworkHandler.valueBuffer.Dequeue();


            var checkBox = ((Panel)PreGame.instance.players.FirstOrDefault(c => c.textureID == playerID).Parent).Controls.OfType<CheckBox>
                ().FirstOrDefault(d => d.Name == "BotCheckbox");


            checkBox.Invoke(() =>
            {
                // Prevent infinite loops
                if (checkBox.Checked != value)
                {
                    checkBox.Checked = value;
                }
            });
        }

        /// <summary>
        /// Gets called by server whane the game starts
        /// </summary>
        private static void StartGameRPC()
        {
            int starter = (int)NetworkHandler.valueBuffer.Dequeue();
            
            PreGame.instance.Invoke(() =>
            {
                lock (PreGame.instance)
                {
                    PreGame.instance.StartGame();
                    Game.instance.gamestateHandler.currentPlayer = starter;
                }
            });            
        }


        public static void PrintNextValueRPC()
        {
            Logging.Log(NetworkHandler.valueBuffer.Dequeue());
        }


        /// <summary>
        /// Confirmation that a connection happened (Called by server)
        /// </summary>
        public static void ConnectionConfirmRPC()
        {
            Logging.Log("Connected!");
            MessageBox.Show("Connected to server!", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }


        /// <summary>
        /// Make move, Sent by server
        /// </summary>
        private static void SendMoveRPC()
        {
            lock (Game.instance)
            { 
                int move = (int)NetworkHandler.valueBuffer.Dequeue();
                int player = PlayerIDtoIndex((int)NetworkHandler.valueBuffer.Dequeue());
                int nextPlayer = PlayerIDtoIndex((int)NetworkHandler.valueBuffer.Dequeue());


                Game.instance.form.BeginInvoke(() =>
                {
                    lock (Game.instance)
                    {
                        Game.instance.gamestateHandler.PassTurn(player, true);
                        Game.instance.gamestateHandler.AddCoin(move);
                        Game.instance.gamestateHandler.PassTurn(nextPlayer);
                    }
                });
            }
        }

        private static void RequestBotMoveRPC()
        {
            lock (Game.instance)
            {
                int player = PlayerIDtoIndex((int)NetworkHandler.valueBuffer.Dequeue());

                SendMove(NetworkHandler.server, Game.instance.gamestateHandler.players[player].bot.GetBestMove());
            }
        }

        /// <summary>
        /// Converts textureID from player to
        /// Index from the player Array 
        /// </summary>
        /// <returns>
        /// Index from the PlayerArray of <seealso cref="Game"/>
        /// </returns>
        private static int PlayerIDtoIndex(int id)
        {
            for (int i = 0; i < Game.instance.gamestateHandler.players.Count; i++)
            {

                if (Game.instance.gamestateHandler.players[i].textureID == id)
                {
                    return i;
                }
            }

            Logging.LogError("Failed to find id " + id);
            return -1;  
        }

        /// <summary>
        /// Change name of Player, called by Server
        /// </summary>
        private static void NameChangeRPC()
        {
            string newName = (string)NetworkHandler.valueBuffer.Dequeue();
            int playerDataID = (int)NetworkHandler.valueBuffer.Dequeue();


            // Find Player and change Name
            for (int i = 0; i < PreGame.instance.players.Count; i++)
            {
                if (PreGame.instance.players[i].textureID == playerDataID)
                {
                    PreGame.instance.ChangeName(PreGame.instance.players[i], newName);
                }
            }
        }
        
       
        private static void EndGameRPC()
        {
            Logging.Log("Ending Game!");

            GameWindow.enableExitButton = false;

            MessageBox.Show("Game ended, returning to lobby in 5 seconds!", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            Thread.Sleep(5000);

            MenuManager.ChangeMenuThreadSafe("PreGame", () =>
            {
                // Reset as soon as the menu was changed
                Game.instance.form.ResetGame();
                GameWindow.enableExitButton = true;
            });
        }


        /// <summary>
        /// Sends Move to Client
        /// </summary>
        public static void SendMove(Stream stream, int move)
        {
            NetworkHandler.SendValue(stream, move);
            NetworkHandler.SendRPC(stream, "SendMoveRPC");
        }

        /// <summary>
        /// Sends ChatMessage to the Server
        /// </summary>
        public static void SendChatMessage(Stream stream, string message)
        {
            if (message == "") return;

            NetworkHandler.SendValue(stream, message);
            NetworkHandler.SendRPC(stream, "SendChatMessageRPC");
        }

        /// <summary>
        /// Updates Chat, called by server
        /// </summary>
        private static void SendChatMessageRPC()
        {
            string sender = (string)NetworkHandler.valueBuffer.Dequeue();
            string message = (string)NetworkHandler.valueBuffer.Dequeue();

            ChatHandler.UpdateChat(message, sender);
        }



        public static void SendCloseRequest(Stream stream)
        {
            NetworkHandler.SendRPC(stream, "CloseRequestRPC");
        }

        public static void RemovePlayerRequest(PlayerData playerData)
        {
            NetworkHandler.SendValue(NetworkHandler.server, playerData.textureID); // Send TextureID of player that should be removed since it is Unique
            NetworkHandler.SendRPC(NetworkHandler.server, "RemovePlayerRPC"); // Tell server to remove the Player with the given TextureID
        }

        private static void RemovePlayerRPC()
        {

            int id = (int)NetworkHandler.valueBuffer.Dequeue();

            for (int i = 0; i < PreGame.instance.players.Count; i++)
            {
                if (PreGame.instance.players[i].textureID == id)
                {
                    PlayerData data = PreGame.instance.players[i];

                    if (data.textureID >= 0)
                        PreGame.instance.playerTextures[data.textureID % PreGame.instance.playerTextures.Count].used = false; // Make the selected color usable again

                    PreGame.instance.players.Remove(data);

                    Panel parent = (Panel)data.Parent;

                    PreGame.instance.Invoke(() =>
                    {
                        PreGame.instance.PlayerPanel.Controls.Remove(parent);
                    });

                    return;
                }
            }
        }


        /// <summary>
        /// Syncs Playerlist to the playerlist sent by server,
        /// Called by Server
        /// </summary>
        private static void LoadPlayersRPC()
        {
            PreGame.instance.Invoke(new Action(() =>
            {
                PreGame.instance.ClearPlayers();
            }));

            int playerCount = (int)NetworkHandler.valueBuffer.Dequeue();

            for (int i = 0; i < playerCount; i++)
            {
                PlayerData data = (PlayerData)NetworkHandler.valueBuffer.Dequeue();
                bool owned = (bool)NetworkHandler.valueBuffer.Dequeue();
                

                PreGame.instance.Invoke(() =>
                {
                    PreGame.instance.playerTextures[data.textureID].used = true; // Reserve color
                    data.texture = PreGame.instance.playerTextures[data.textureID].color; // Update Color
                    data.owned = owned;

                    PreGame.instance.AddPlayer(data);
                    PreGame.instance.players.Add(data);
                });
            }
        }

        /// <summary>
        /// Change color of player, called by server
        /// </summary>
        private static void ChangeColorRPC()
        {
            int originalID = (int)NetworkHandler.valueBuffer.Dequeue();
            int newID = (int)NetworkHandler.valueBuffer.Dequeue();

            if (newID == -1)
            {
                Logging.LogWarning("New TextureID was -1");
                return;
            }

            // find Player and change color
            for (int i = 0; i < PreGame.instance.players.Count; i++)
            {
                if (PreGame.instance.players[i].textureID == originalID)
                {
                    PlayerData data = PreGame.instance.players[i];

                    string newTexture = PreGame.instance.playerTextures[newID].color; // Get new Color
                    data.texture = newTexture;

                    PreGame.instance.playerTextures[data.textureID % PreGame.instance.playerTextures.Count].used = false; // Make the old color usable again


                    Panel parent = (Panel)data.Parent;
                    PreGame.instance.Invoke(() =>
                    {
                        parent.Controls.OfType<PictureBox>().FirstOrDefault().Image = System.Drawing.Image.FromFile(PreGame.colorFolderPath + data.texture);
                    });

                    data.textureID = newID;
                    PreGame.instance.playerTextures[data.textureID % PreGame.instance.playerTextures.Count].used = true; // Make the selected color unusable for others
                }
            }
        }


        /// <summary>
        /// Adds the Player to Player List, Called by server
        /// </summary>
        private static void AddPlayersRPC()
        {
            int amount = (int)NetworkHandler.valueBuffer.Dequeue();

            for (int i = 0; i < amount; i++)
            {
                PlayerData data = (PlayerData)NetworkHandler.valueBuffer.Dequeue();
                bool owned = (bool)NetworkHandler.valueBuffer.Dequeue();

                PreGame.instance.Invoke(new Action(() =>
                {
                    try
                    {
                        PreGame.instance.playerTextures[data.textureID].used = true; // Reserve Color
                        data.texture = PreGame.instance.playerTextures[data.textureID].color; // Update Color
                        data.owned = owned;

                        PreGame.instance.AddPlayer(data);
                        PreGame.instance.players.Add(data);
                    }
                    catch (Exception ex)
                    {
                        Logging.LogError(ex);
                    }
                }));
            }
        }
    }
}