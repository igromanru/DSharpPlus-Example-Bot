using System.IO;
using Newtonsoft.Json;

namespace DSharpPlus_Example_Bot.Configurations
{
    /// <summary>
    /// Singleton class that loads settings from a file on the first usage.
    /// Settings are stored in the Cfg-Property.
    /// </summary>
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
        
        /// <summary>
        /// Settings object
        /// </summary>
        public Settings Cfg { get; private set; }

        private SettingsService()
        {
            if (File.Exists(DefaultConfigLocation))
            {
                LoadFromFile();
            }

            if (Cfg == null)
            {
                // If not exists, creates a new Settings object and saves it to the file to use as template. 
                Cfg = new Settings();
                SaveToFile();
            }
        }

        /// <summary>
        /// Serializes <c>this.Cfg</c> to JSON and writes it to the default file.
        /// </summary>
        public void SaveToFile()
        {
            using (var file = File.CreateText(DefaultConfigLocation))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, Cfg);
            }
        }

        /// <summary>
        /// Reads default config file and deserializes JSON to <c>this.Cfg</c>.
        /// </summary>
        public void LoadFromFile()
        {
            Cfg = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(DefaultConfigLocation));
        }
    }
}
