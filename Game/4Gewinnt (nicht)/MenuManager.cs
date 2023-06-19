using Network;
using System.Reflection;

namespace _4Gewinnt__nicht_
{
    public class MenuManager
    {
        public static Dictionary<string, Form> menus;
        public static Dictionary<string, Action> onLoadEvents;
        public static Panel mainFormPanel;

        public static void Init(Panel mainFormPanel)
        {
            menus = new Dictionary<string, Form>();
            onLoadEvents = new Dictionary<string, Action>();

            MenuManager.mainFormPanel = mainFormPanel;

            // Add all forms to the menu manager, except the "Mainform"
            foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
            {
                if (typeof(Form).IsAssignableFrom(type) && type != typeof(MainForm))
                {
                    AddMenu(type.Name, (Form)Activator.CreateInstance(type));
                }
            }
        }

        public static void AddMenu(string name, Form menu, Action loadAction = null)
        {
            menus[name] = menu;
            onLoadEvents[name] = loadAction;
        }

        public static Form ChangeMenu(string menu)
        {
            // Check if menu is available
            if (!menus.ContainsKey(menu))
            {
                Logging.LogError($"Menu {menu} does not exist!");
                return null;
            }
            // Remove old menu
            mainFormPanel.Controls.Clear();

            // Show new Menu
            menus[menu].FormBorderStyle = FormBorderStyle.None;
            menus[menu].TopLevel = false;
            menus[menu].Dock = DockStyle.Fill;
            menus[menu].Show();

            mainFormPanel.Controls.Add(menus[menu]);


            // Menu load event
            onLoadEvents[menu]?.Invoke();


            return menus[menu];
        }


        public static void ChangeMenuThreadSafe(string menu, Action onChanged)
        {
            mainFormPanel.BeginInvoke(new Action(() =>
            {
                lock (menus) lock (mainFormPanel.Controls)
                {
                    ChangeMenu(menu);
                    onChanged?.Invoke();
                }
            }));
        }

        /// <summary>
        /// Returns to main menu and closes
        /// ongoing connections and kills the
        /// server if it is hosted on this machine
        /// </summary>
        public static void ExitToMain()
        {
            ChangeMenu("MainMenu");

            if (NetworkHandler.server != null)
            {
                RpcMethods.SendCloseRequest(NetworkHandler.server);
                NetworkHandler.server.Close(); // End Server Connection
                NetworkHandler.server.Dispose();
                NetworkHandler.server = null;
            }

            if (NetworkHandler.serverHost != null)
            {
                NetworkHandler.serverHost.Kill();
                NetworkHandler.serverHost.Dispose();
                NetworkHandler.serverHost = null;
            }

            // Cleanup
            ChatHandler.Init(PreGame.instance.chatOutputTextBox);
        }
    }
}