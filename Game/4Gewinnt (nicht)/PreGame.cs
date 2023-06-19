using Network;
using Shaders;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Image = System.Drawing.Image;

namespace _4Gewinnt__nicht_
{
    public partial class PreGame : Form
    {
        int nextColorID = 0;
        public static PreGame instance;

        public bool started = false;
        public int maxPlayercount = 4;

        internal static string serverFolderPath
        {
            get
            {
                return (AppDomain.CurrentDomain.BaseDirectory + "/Server/");
            }
        }


        internal static string colorFolderPath
        {
            get
            {
                return (AppDomain.CurrentDomain.BaseDirectory + "/Images/PlayerColors/");
            }
        }

        internal static string imagesFolderPath
        {
            get
            {
                return (AppDomain.CurrentDomain.BaseDirectory + "/Images/");
            }
        }

        public List<PlayerData> players;
        
        public List<PlayerColorData> playerTextures;
        bool texturesLoaded; // Textures need to be loaded after the GLcontrol exists

        public PreGame()
        {
            InitializeComponent();

            instance = this;

            players = new List<PlayerData>();
            playerTextures = new List<PlayerColorData>();

            // Start with 2 Players
            for (int i = 0; i < 2; i++)
                AddPlayerButton_Click(null, null);

            // Make Chat work
            ChatHandler.Init(chatOutputTextBox);
        }

        /// <summary>
        /// Loads Chosen DefaultName from the
        /// According file and writes it into 
        /// the playerName textbox
        /// </summary>
        public void LoadDefaultName()
        {
            // Load last used name from name file
            string path = AppDomain.CurrentDomain.BaseDirectory + "Saves/DefaultName.save";
            if (File.Exists(path))
                playerNameTextBox.Text = File.ReadAllText(path);
        }

        /// <summary>
        /// Sets the Default Name in the
        /// According file, Creates one if it isn't Present
        /// </summary>
        public void SetDefaultName()
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory + "Saves";
            string path = folderPath + "/DefaultName.save";

            // Create new saves folder if none is present
            if (!Directory.Exists(folderPath))
            {
                var info = Directory.CreateDirectory(folderPath);
            }

            // Create or override name Save file with new name
            using (FileStream stream = File.Create(path))
            {
                stream.Write(Encoding.UTF8.GetBytes(playerNameTextBox.Text));
            }
        }

        /// <summary>
        /// Loads all files in the playercolor folder 
        /// and stors them in "playerTextures"
        /// </summary>
        void LoadTextures()
        {
            if (texturesLoaded) return;

            // Load all colors in playercolor folder
            foreach (var file in Directory.GetFiles(colorFolderPath))
            {
                string extension = Path.GetExtension(file);

                if (extension != ".png" && extension != ".jpg")
                {
                    Logging.LogError($"Invalid file extension at {file}, only .png and .jpg are supported!");
                    continue;
                }

                playerTextures.Add(new PlayerColorData(Path.GetFileName(file), false));
            }


            texturesLoaded = true;
        }


        /// <summary>
        /// Adds Player
        /// </summary>
        /// <returns>
        /// The Created Panel 
        /// </returns>
        public Panel AddPlayer(PlayerData player)
        {
            // Container
            Panel playerPanel = new Panel();
            playerPanel.Dock = DockStyle.Top;


            // Name Label
            TextBox nameTextbox = new TextBox();
            nameTextbox.Dock = DockStyle.Left;
            nameTextbox.Text = player.name + " " + PlayerPanel.Controls.Count;
            nameTextbox.TextChanged += NameChange; // Update Name in Playerdata
            nameTextbox.Name = "NameTextbox";
            nameTextbox.BackColor = SystemColors.ControlDark;


            // Bot checkbox
            CheckBox botCheckbox = new CheckBox();
            botCheckbox.Dock = DockStyle.Left;
            botCheckbox.Text = "Bot";
            botCheckbox.Name = "BotCheckbox";
            botCheckbox.BackColor = SystemColors.ControlDarkDark;

            botCheckbox.Checked = player.mode == Player.Mode.Bot;
            botCheckbox.CheckedChanged += BotCheck;

            // Remove Button
            Button removeBtn = new Button();
            removeBtn.Dock = DockStyle.Left;
            removeBtn.Text = "Remove";
            removeBtn.Click += RemovePlayer;
            removeBtn.BackColor = SystemColors.ControlDark;

            // Color Show
            PictureBox colorShow = new PictureBox();

            if (player.texture != String.Empty)
            {
                colorShow.Image = Image.FromFile(colorFolderPath + player.texture);
            }
            else
            {
                colorShow.Image = Image.FromFile(imagesFolderPath + "error.jpg");
            }

            colorShow.Dock = DockStyle.Left;
            colorShow.SizeMode = PictureBoxSizeMode.StretchImage;
            colorShow.MouseDown += ChangeColor;

            // Color text
            Label lbl = new Label();
            lbl.Text = "Color: ";
            colorShow.Controls.Add(lbl);

            // Owned Label
            Label ownedlbl = new Label();
            ownedlbl.Text = player.owned ? "Owned" : "";
            ownedlbl.Dock = DockStyle.Left;

            playerPanel.Controls.Add(ownedlbl);
            playerPanel.Controls.Add(colorShow);
            playerPanel.Controls.Add(removeBtn);
            playerPanel.Controls.Add(botCheckbox);
            playerPanel.Controls.Add(nameTextbox);
            playerPanel.Controls.Add(player);


            this.PlayerPanel.Controls.Add(playerPanel);


            NameChange(nameTextbox, null); // Update name Of Player to the automatically assigned one

            return playerPanel;
        }

        /// <summary>
        /// Change Name Of Player corrosponding
        /// to the PlayerLabel the textbox is on
        /// </summary>
        private void NameChange(object sender, EventArgs e)
        {
            PlayerData data = ((TextBox)sender).Parent.Controls.OfType<PlayerData>().ToList()[0];

            if (NetworkHandler.server != null)
            {
                NetworkHandler.SendValue(NetworkHandler.server, ((TextBox)sender).Text); // send new name
                NetworkHandler.SendValue(NetworkHandler.server, data.textureID); // Send TextureID as an identifyer of the player
                NetworkHandler.SendRPC(NetworkHandler.server, "NameChangeRPC"); // Request namechange
            }
            else
            {
                data.name = ((TextBox)sender).Text;
            }
        }

        /// <summary>
        /// Changes name of Player to <paramref name="newName"/>
        /// This is a thread safe operation
        /// </summary>
        public void ChangeName(PlayerData player, string newName)
        {
            Panel panel = (Panel)player.Parent;

            lock (player)
            {
                panel.BeginInvoke(() =>
                {
                    lock (player)
                    {
                        // Changing the text will call an event that handles the rest
                        TextBox textBox = panel.Controls.OfType<TextBox>().SingleOrDefault(c => c.Name == "NameTextbox");

                    
                        // Disable name changing to prevent infinite updating
                        // Loops between server and clients
                        textBox.TextChanged -= NameChange;

                        // Update Name
                        textBox.Text = newName;
                        player.name = newName;


                        // Enable name changing again
                        textBox.TextChanged += NameChange;
                    }
                });
            }
        }


        /// <summary>
        /// Toggle if a player is a bot or not
        /// </summary>
        public void BotCheck(object sender, EventArgs e)
        {
            ((CheckBox)sender).Parent.Controls.OfType<PlayerData>().ToList()[0].mode = ((CheckBox)sender).Checked ? Player.Mode.Bot : Player.Mode.Player;

            if (NetworkHandler.server != null)
            {
                NetworkHandler.SendValue(NetworkHandler.server, ((CheckBox)sender).Checked); // Tell server what is the new Value
                NetworkHandler.SendValue(NetworkHandler.server, ((CheckBox)sender).Parent.Controls.OfType<PlayerData>().ToList()[0].textureID); // Tell server of wich player
                NetworkHandler.SendRPC(NetworkHandler.server, "BotCheckRPC"); // Tell the server that a botmode was checked
            }
        }

        /// <summary>
        /// Toggle if Player is a bot or not
        /// This is a thread safe operation
        /// </summary>
        public void ChangeBotChecked(PlayerData player, bool isBot)
        {
            Panel panel = (Panel)player.Parent;

            panel.BeginInvoke(() =>
            {
                // Changing the Checkbox will call an event that handles the rest
                panel.Controls.OfType<CheckBox>().SingleOrDefault(c => c.Name == "BotCheckbox").Checked = isBot;
            });
        }

        /// <summary>
        /// Finds the next Unused color in the list
        /// </summary>
        private string FindNextUnusedColor(out int textureID)
        {
            int iterations = 0;

            string texture = "";
            textureID = -1;

            do
            {
                nextColorID++;

                texture = playerTextures[nextColorID % playerTextures.Count].color;
                textureID = nextColorID % playerTextures.Count;

                if (iterations++ > playerTextures.Count * 2)
                {
                    Logging.LogError("No available color found!");

                    textureID = -1;
                    return "";
                }
            } while (playerTextures[nextColorID % playerTextures.Count].used);

            return texture;
        }


        private void AddPlayerButton_Click(object sender, EventArgs e)
        {
            if (players.Count >= maxPlayercount) return;

            LoadTextures();

            // Find next unused color
            string texture = FindNextUnusedColor(out int textureID);

            if (textureID >= 0)
                playerTextures[textureID % playerTextures.Count].used = true;

            PlayerData p = new PlayerData
            {
                name = "Player ",
                texture = texture,
                textureID = textureID,
                mode = Player.Mode.Player
            };

            if (NetworkHandler.server != null)
            {
                NetworkHandler.SendValue(NetworkHandler.server, 1); // Tell server that 1 player was added
                NetworkHandler.SendValue(NetworkHandler.server, p); // Tell server wich player as added
                NetworkHandler.SendRPC(NetworkHandler.server, "AddPlayersRPC"); // Tell server that a player was added

                return;
            }


            // "Normal" Add player
            players.Add(p);
            AddPlayer(p);
        }

        /// <summary>
        /// Removes the player corrosponding to the
        /// clicked Container
        /// </summary>
        private void RemovePlayer(object sender, EventArgs e)
        {
            PlayerData data = ((Button)sender).Parent.Controls.OfType<PlayerData>().ToList()[0];

            // Netork Remove Player
            if (NetworkHandler.server != null)
            {
                NetworkHandler.SendValue(NetworkHandler.server, data.textureID);
                NetworkHandler.SendRPC(NetworkHandler.server, "RemovePlayerRPC");

                return;
            }

            // "Normal" Remove Player

            if (data.textureID >= 0)
                playerTextures[data.textureID % playerTextures.Count].used = false; // Make the selected color usable again

            players.Remove(((Button)sender).Parent.Controls.OfType<PlayerData>().ToList()[0]);

            PlayerPanel.Controls.Remove(((Button)sender).Parent);
        }

        /// <summary>
        /// Removes the player
        /// </summary>
        public void RemovePlayer(PlayerData playerData)
        {
            Panel parent = (Panel)playerData.Parent;
            players.Remove(playerData);

            if (playerData.textureID >= 0)
                playerTextures[playerData.textureID % playerTextures.Count].used = false; // Make the selected color usable again

            PlayerPanel.Controls.Remove(parent);
        }

        public void ClearPlayers()
        {
            players.Clear();
            PlayerPanel.Controls.Clear();

            foreach (var c in playerTextures)
            {
                c.used = false;
            }
        }

        /// <summary>
        /// Changes Color of player
        /// </summary>
        public void ChangeColor(object sender, EventArgs e)
        {
            PlayerData data = ((PictureBox)sender).Parent.Controls.OfType<PlayerData>().ToList()[0];

            // Network Color change
            if (NetworkHandler.server != null)
            {
                NetworkHandler.SendValue(NetworkHandler.server, data.textureID);
                NetworkHandler.SendRPC(NetworkHandler.server, "ChangeColorRPC");

                return;
            }
            

            // "Normal" Color Change

            string newTexture = FindNextUnusedColor(out int textureID);

            // Update Texture
            if (newTexture != String.Empty)
            {
                data.texture = newTexture;

                playerTextures[data.textureID % playerTextures.Count].used = false; // Make the selected color usable again

                data.textureID = textureID;

                // Update Menu image
                ((PictureBox)sender).Image = Image.FromFile(colorFolderPath + data.texture);

                playerTextures[textureID % playerTextures.Count].used = true; // Make the selected color unusable for others again
            }
        }


        /// <summary>
        /// Changes Color of player to the
        /// color corrosponging to <paramref name="textureID"/>>
        /// </summary>
        public void ChangeColor(PlayerData data, int textureID)
        {
            PictureBox pictureBox = data.Parent.Controls.OfType<PictureBox>().FirstOrDefault();

            string newTexture = playerTextures[textureID].color;

            // Update Texture
            if (newTexture != String.Empty)
            {
                data.texture = newTexture;

                playerTextures[data.textureID % playerTextures.Count].used = false; // Make the selected color usable again

                data.textureID = textureID;

                // Update Menu image
                pictureBox.Image = Image.FromFile(colorFolderPath + data.texture);

                playerTextures[textureID % playerTextures.Count].used = true; // Make the selected color unusable for others
            }
        }


        /// <summary>
        /// Start Game
        /// </summary>
        private void StartButton_Click(object sender, EventArgs e)
        {
            if (players.Count < 2) return;

            if (NetworkHandler.server != null)
            {
                NetworkHandler.SendRPC(NetworkHandler.server, "StartGameRPC");
            }
            else
            {
                StartGame();
            }
        }


        public void StartGame()
        {
            GameWindow window = (GameWindow)MenuManager.ChangeMenu("GameWindow");
           

            window.game.InitGame(players.ToArray());
            window.game.StartGame();

            ChatHandler.ChangeChatOutput(Game.instance.chatWindow.chatOutputTextBox);
        }


        private void Backbutton_Click(object sender, EventArgs e)
        {
            MenuManager.ExitToMain();
        }



        private async void JoinButton_Click(object sender, EventArgs e)
        {
            if (NetworkHandler.server != null) return; // Prevent double connections

            await JoinServer(players.ToArray());
        }

        public void SendButton_Click(object sender, EventArgs e)
        {
            if (NetworkHandler.server == null) return; // Don't send message if no server is connected

            string message = chatTextBox.Text;
            RpcMethods.SendChatMessage(NetworkHandler.server, message);

            chatTextBox.Text = ""; // Clear chat textbox
        }


        /// <summary>
        /// Joins the Server
        /// </summary>
        public async Task JoinServer(PlayerData[] players, string name, string ip, int port)
        {
            RpcMethods.Init();
            NetworkManager networkManager = new NetworkManager();


            if (!networkManager.Connect(ip, port))
            {
                Logging.LogError("Could not connect!");
                MessageBox.Show("Unable to connect!", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            NetworkHandler.server = networkManager.TcpClient.GetStream();

            Logging.Log("Joining");
            await Task.Run(() =>
            {
                NetworkHandler.OnConnect(networkManager.TcpClient, name, players);
            });
        }

        public async Task JoinServer(PlayerData[] players)
        {

            if (ipTextbox.Text == "IP") ipTextbox.Text = NetworkManager.LocalIPAddress.ToString();
            if (portTextbox.Text == "Port") portTextbox.Text = "1234";

            try
            {
                string name = playerNameTextBox.Text;
                await JoinServer(players, name, ipTextbox.Text, int.Parse(portTextbox.Text));
            }
            catch (Exception ex)
            {
                Logging.LogError(ex);
            }
        }

        private void ChatTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                SendButton_Click(sender, e);
            }
        }

        /// <summary>
        /// Hosts a server and connects to it
        /// </summary>
        private async void StartServerButton_Click(object sender, EventArgs e)
        {
            if (NetworkHandler.server != null) return; // Prevent double connections

            await Task.Run(() =>
            {
                string path = serverFolderPath + "Connect4Server.exe";

                NetworkHandler.serverHost = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
                };

                NetworkHandler.serverHost.Start();

                string ip = "";
                int port = -1;
                while (!NetworkHandler.serverHost.StandardOutput.EndOfStream)
                {
                    string line = NetworkHandler.serverHost.StandardOutput.ReadLine();
                    Logging.Log(line);


                    try
                    {
                        port = int.Parse(line);
                    }
                    catch
                    {
                        ip = line;
                    }

                    if (ip != "" && port != -1) break;
                }

                string name = playerNameTextBox.Text;
                JoinServer(players.ToArray(), name, ip, port);


                // Show where the server was hosted
                ipTextbox.BeginInvoke(() =>
                {
                    ipTextbox.Text = ip;
                    portTextbox.Text = port.ToString();
                    MessageBox.Show($"Server hosted with IP: {ip} and Port: {port}", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                });



                Logging.Log("Connected!");
            });
        }


        private void PreGame_Load(object sender, EventArgs e)
        {
            // Load Default name whenever PreGame is shown
            MenuManager.onLoadEvents["PreGame"] += LoadDefaultName;
            MenuManager.onLoadEvents["PreGame"] += () =>
            {
                // reset chat output
                ChatHandler.ChangeChatOutput(chatOutputTextBox);
            };

            // Update Default name when name was changed
            playerNameTextBox.TextChanged += (object sender, EventArgs e) =>
            {
                SetDefaultName();
            };
        }
    }

    /// <summary>
    /// Can be used to store playerdata without
    /// an exisiting GLcontrol.
    /// 
    /// Also, can be used to store playerInformation on the form
    /// for easyer use of the add and check events
    /// </summary>
    public class PlayerData : Label
    {
        public string name;
        public string texture;
        public bool owned = true; // IF this client own this

        public int textureID; // used for cycling colors
        public Player.Mode mode;

        public PlayerData()
        {
            this.Enabled = false;
            this.Visible = false;
            this.Text = "";
        }

        public Player ToPlayer()
        {
            Player p = new Player(mode, new Texture(PreGame.colorFolderPath + texture, true), name, this.textureID);
            p.owned = owned;
           

            return p;
        }

        /// <summary>
        /// Generates Bytearray from the variables in this object
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

    /// <summary>
    /// Save color name and if it's used,
    /// needed for cycling colors
    /// </summary>
    public class PlayerColorData
    {
        public string color;
        public bool used;

        public PlayerColorData(string color, bool used)
        {
            this.color = color;
            this.used = used;
        }
    }
}