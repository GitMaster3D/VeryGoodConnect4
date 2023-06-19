using Network;
using System.Runtime.InteropServices;

namespace _4Gewinnt__nicht_
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
#if DEBUG 
            AllocConsole();
#endif

            Application.ApplicationExit += OnClose;

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());


        }

        /// <summary>
        /// Cleanup after Progeam ends
        /// </summary>
        static void OnClose(object sender, EventArgs e)
        {
            // Tell server to close Connection
            if (NetworkHandler.server != null)
                RpcMethods.SendCloseRequest(NetworkHandler.server);

            // Close ongoing connections
            if (NetworkHandler.server != null)
            {
                NetworkHandler.server.Close();
                NetworkHandler.server.Dispose();
            }

            // End hosting server if one is hosted
            if (NetworkHandler.serverHost != null)
            {
                NetworkHandler.serverHost.Kill();
                NetworkHandler.serverHost.Dispose();
            }

        }

#if DEBUG
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
#endif
    }
}