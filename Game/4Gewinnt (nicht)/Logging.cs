namespace _4Gewinnt__nicht_
{
    static class Logging
    {
        public static void LogError(string log)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + log);
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }

        public static void LogError(object log)
        {
#if DEBUG 
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + log.ToString());
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }

        public static void LogWarning(string log)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warning: " + log);
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }

        public static void LogWarning(object log)
        {
#if DEBUG 
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warning: " + log.ToString());
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }


        public static void Log(string log)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Log: " + log);
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }

        public static void Log(object log)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Log: " + log.ToString());
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }
    }

}