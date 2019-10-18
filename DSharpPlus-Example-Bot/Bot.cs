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
                TokenType = TokenType.Bot
            });

            // Activating Interactivity module for the DiscordClient
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
            // Registering command classes
            _commands.RegisterCommands<UserCommands>();
            _commands.RegisterCommands<AdminCommands>();
            _commands.RegisterCommands<OwnerCommands>();
            _commands.RegisterCommands<DemonstrationCommands>();
            
            // Registering OnCommandError method for the CommandErrored event
            _commands.CommandErrored += OnCommandError;
        }

        private void RegisterEvents()
        {
            Discord.Ready += OnReady;

            Discord.DebugLogger.LogMessageReceived += OnLogMessageReceived;
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

        /// <summary>
        /// Logs DSharpPlus internal errors with NLog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">DebugLogMessageEventArgs object</param>
        private void OnLogMessageReceived(object sender, DebugLogMessageEventArgs e)
        {
            var message = $"{e.Application}: {e.Message}";
            switch (e.Level)
            {
                case LogLevel.Debug:
                    Logger.Debug(e.Exception, message);
                    break;
                case LogLevel.Info:
                    Logger.Info(e.Exception, message);
                    break;
                case LogLevel.Warning:
                    Logger.Warn(e.Exception, message);
                    break;
                case LogLevel.Error:
                    Logger.Error(e.Exception, message);
                    break;
                case LogLevel.Critical:
                    Logger.Fatal(e.Exception, message);
                    break;
            }
        }

        private Task OnCommandError(CommandErrorEventArgs e)
        {
            // Send command error message as response.
            e.Context.RespondAsync(e.Exception.Message);
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
