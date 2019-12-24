
using DSharpPlus_Example_Bot.Configurations;

namespace DSharpPlus_Example_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var settingsService = new SettingsService();
            using (var bot = new Bot(settingsService.LoadFromFile()))
            {   
                bot.RunAsync().GetAwaiter().GetResult();
            }
        }
    }
}
