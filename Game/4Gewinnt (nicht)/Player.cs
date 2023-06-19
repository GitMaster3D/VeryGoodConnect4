using Shaders;

namespace _4Gewinnt__nicht_
{
    public class Player
    {
        public Mode mode;
        public Texture texture;
        public int textureID;
        public string name;

        public bool owned = true; // If this Player is owned by this client

        public MinMaxBot bot = null;

        public Player(Mode mode, Texture texture, string name, int textureID)
        {
            this.mode = mode;
            this.texture = texture;
            this.name = name;
            this.textureID = textureID;
        }

        public enum Mode
        {
            Player = 0,
            Bot = 1
        }

    }
}