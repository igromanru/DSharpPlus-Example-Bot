using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus_Example_Bot.Configurations;

namespace DSharpPlus_Example_Bot
{
    public class Bot : IDisposable
    {
        private bool _disposed;
        private bool _run;

        private CommandsNextExtension _commands;

        private DiscordClient Discord { get; }

        public Bot()
        {
            Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = SettingsService.Instance.Cfg.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            Discord.UseInteractivity(new InteractivityConfiguration());
        }

        ~Bot()
        {
            Dispose(false);
        }
        
        public async Task RunAsync()
        {
            _run = true;
            await Discord.ConnectAsync();
            while (_run)
            {
                await Task.Delay(200);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Discord.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
