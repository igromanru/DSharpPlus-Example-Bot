using System;

namespace DSharpPlus_Example_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bot = new Bot())
            {   
                bot.RunAsync().Wait();
            }
        }
    }
}
