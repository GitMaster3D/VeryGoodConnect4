using _4Gewinnt__nicht_;

namespace Network
{
    public static class ChatHandler
    {
        public static TextBox chatOutput;
        private static string chat;

        public static void Init(TextBox output)
        {
            chatOutput = output;

            // Reset Chat
            output.Text = "";
            chat = "";
        }

        /// <summary>
        /// Adds Message to chat
        /// </summary>
        /// <param name="message"> The message added </param>
        public static void UpdateChat(string message, string sender)
        {
            chat += $"{Environment.NewLine}{sender}: {message}"; // Add Message to Chat
            UpdateChatLabelSafe(chat); // Show updated chat
        }

        /// <summary>
        /// Chagnes where the chat is displayed
        /// </summary>
        /// <param name="output"> The label where the chat is shown </param>
        public static void ChangeChatOutput(TextBox output)
        {
            chatOutput = output;
            UpdateChatLabelSafe(chat);
        }

        /// <summary>
        /// A Threadsafe way of updating
        /// the Chat Label
        /// </summary>
        private static void UpdateChatLabelSafe(string message)
        {
            // Update the text Threadsafely
            if (chatOutput.InvokeRequired)
            {
                chatOutput.BeginInvoke(() =>
                {
                    chatOutput.Text = message;

                    // Scroll to bottom
                    chatOutput.Select(chatOutput.TextLength + 1, 0);
                    chatOutput.ScrollToCaret();
                });
            }
            else
            {
                chatOutput.Text = message;

                // scroll to bottom
                chatOutput.Select(chatOutput.TextLength + 1, 0);
                chatOutput.ScrollToCaret();
            }
        }
    }
}