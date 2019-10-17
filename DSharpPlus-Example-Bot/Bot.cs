using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus_Example_Bot.Commands;
using DSharpPlus_Example_Bot.Configurations;
using NLog;
using LogLevel = DSharpPlus.LogLevel;

namespace DSharpPlus_Example_Bot
{
    public class Bot : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                UseInternalLogHandler = false,
                LogLevel = LogLevel.Debug
            });

            Discord.UseInteractivity(new InteractivityConfiguration());

            RegisterCommands();
            RegisterEvents();
        }

        ~Bot()
        {
            Dispose(false);
        }

        private void RegisterCommands()
        {
            var commandsNextConfiguration = new CommandsNextConfiguration
            {
                StringPrefixes = SettingsService.Instance.Cfg.Prefixes,
            };
            _commands = Discord.UseCommandsNext(commandsNextConfiguration);

            _commands.RegisterCommands<UserCommands>();
            _commands.RegisterCommands<AdminCommands>();
            _commands.RegisterCommands<OwnerCommands>();
        }

        private void RegisterEvents()
        {
            Discord.Ready += OnReady;
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

        private Task OnReady(ReadyEventArgs e)
        {
            Logger.Info("The bot is online");
            return Task.CompletedTask;
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
