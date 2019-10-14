using System.IO;
using Newtonsoft.Json;

namespace DSharpPlus_Example_Bot.Configurations
{
    public sealed class SettingsService
    {
        private const string DefaultConfigLocation = "bot.cfg";

        private static readonly object SyncObj = new object();
        private static SettingsService _instance;
        public static SettingsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncObj)
                    {
                        _instance = new SettingsService();
                    }
                }
                return _instance;
            }
        }
        
        public Settings Cfg { get; private set; }

        private SettingsService()
        {
            if (File.Exists(DefaultConfigLocation))
            {
                LoadFromFile();
            }

            if (Cfg == null)
            {
                Cfg = new Settings();
                SaveToFile();
            }
        }

        public void SaveToFile()
        {
            using (var file = File.CreateText(DefaultConfigLocation))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, Cfg);
            }
        }

        public void LoadFromFile()
        {
            using (var file = File.OpenText(DefaultConfigLocation))
            {
                var serializer = new JsonSerializer();
                Cfg = (Settings) serializer.Deserialize(file, typeof(Settings));
            }
        }
    }
}
