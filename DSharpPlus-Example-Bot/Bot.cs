using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using DSharpPlus_Example_Bot.Commands;
using DSharpPlus_Example_Bot.Configurations;
using NLog;
using LogLevel = DSharpPlus.LogLevel;

namespace DSharpPlus_Example_Bot
{
    public class Bot : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private DiscordClient Discord { get; }
        private CommandsNextExtension CommandsNext { get; set; }
        private bool IsRunning { get; set; }
        private bool IsDisposed { get; set; }
        private Settings Settings { get; }

        public Bot(Settings settings)
        {
            Settings = settings;

            Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Settings.Token,
                TokenType = TokenType.Bot
            });

            // Activating Interactivity module for the DiscordClient
            Discord.UseInteractivity(new InteractivityConfiguration());
            // Activating VoiceNext module
            Discord.UseVoiceNext(new VoiceNextConfiguration());

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
                StringPrefixes = Settings.Prefixes,
            };
            CommandsNext = Discord.UseCommandsNext(commandsNextConfiguration);
            // Registering command classes
            CommandsNext.RegisterCommands<UserCommands>();
            CommandsNext.RegisterCommands<AdminCommands>();
            CommandsNext.RegisterCommands<OwnerCommands>();
            CommandsNext.RegisterCommands<DemonstrationCommands>();
            CommandsNext.RegisterCommands<VoiceCommands>();

            // Registering OnCommandError method for the CommandErrored event
            CommandsNext.CommandErrored += OnCommandError;
        }

        private void RegisterEvents()
        {
            Discord.Ready += OnReady;

            Discord.DebugLogger.LogMessageReceived += OnLogMessageReceived;
        }

        public async Task RunAsync()
        {
            if (IsRunning)
            {
                throw new MethodAccessException("The bot is already running");
            }

            await Discord.ConnectAsync();
            IsRunning = true;
            while (IsRunning)
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
            if (IsDisposed) return;

            if (disposing)
            {
                Discord.Dispose();
            }
            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
