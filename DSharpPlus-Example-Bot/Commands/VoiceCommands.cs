using System.Diagnostics;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using NAudio.Wave;

namespace DSharpPlus_Example_Bot.Commands
{
    /// <summary>
    /// Commands to control the bot in voice channels. 
    /// </summary>
    [RequireGuild]
    public sealed class VoiceCommands : BaseCommandModule
    {
        /// <summary>
        /// Connects the bot to a voice channel.
        /// Since voice channels can't be mentioned with #, you have to use the ID of a voice channel.
        /// <see cref="https://support.discordapp.com/hc/en-us/articles/206346498-Where-can-I-find-my-server-ID-number-"/>
        /// </summary>
        /// <param name="commandContext">CommandContext</param>
        /// <param name="channel">Voice channel ID</param>
        /// <returns></returns>
        [Command("join"), Description("Connect the bot to a voice channel")]
        public async Task JoinAsync(CommandContext commandContext, [Description("Target voice channel ID")] DiscordChannel channel)
        {
            if (channel.Type == ChannelType.Voice)
            {
                var voiceNext = commandContext.Client.GetVoiceNext();
                await voiceNext.ConnectAsync(channel);
            }
            else
            {
                await commandContext.RespondAsync($"{channel.Mention} is not a voice channel!");
            }
        }

        /// <summary>
        /// Disconnects the bot from any voice channel in the current Guild 
        /// </summary>
        /// <param name="commandContext">CommandContext</param>
        /// <returns></returns>
        [Command("leave"), Description("Disconnect the bot from any voice channel")]
        public async Task LeaveAsync(CommandContext commandContext)
        {
            var voiceNext = commandContext.Client.GetVoiceNext();
            var voiceNextConnection = voiceNext.GetConnection(commandContext.Guild);
            if (voiceNextConnection == null)
            {
                await commandContext.RespondAsync("The bot is not connected to any voice channel");
            }
            else
            {
                voiceNextConnection?.Disconnect();
            }
        }

        /// <summary>
        /// Plays Flurry.wav located in bots root directory.
        /// </summary>
        /// <param name="commandContext"></param>
        /// <returns></returns>
        [Command("playwav"), Description("Voice channel test command: plays a hardcoded wav file")]
        public async Task PlayAsync(CommandContext commandContext)
        {
            var voiceNext = commandContext.Client.GetVoiceNext();
            var voiceNextConnection = voiceNext.GetConnection(commandContext.Guild);
            if (voiceNextConnection != null)
            {
                await voiceNextConnection.SendSpeakingAsync();
                var transmitStream = voiceNextConnection.GetTransmitStream();
                using (var reader = new WaveFileReader("Flurry.wav"))
                {
                    await reader.CopyToAsync(transmitStream);
                }
                await voiceNextConnection.SendSpeakingAsync(false);
            }
            else
            {
                await commandContext.RespondAsync("The bot is not connected to any voice channel");
            }
        }

        /// <summary>
        /// Plays Track A - Stephen Bennett.mp3 located in bots root directory.
        /// You need a ffmpeg.exe on Windows or installed ffmpeg package on a linux system.
        /// </summary>
        /// <param name="commandContext"></param>
        /// <returns></returns>
        [Command("playmp3"), Description("Voice channel test command: plays a hardcoded mp3 file")]
        public async Task PlayMp3Async(CommandContext commandContext)
        {
            const string mp3File = "Track A - Stephen Bennett.mp3";
            var voiceNext = commandContext.Client.GetVoiceNext();
            var voiceNextConnection = voiceNext.GetConnection(commandContext.Guild);
            if (voiceNextConnection != null)
            {
                await voiceNextConnection.SendSpeakingAsync();
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $@"-i ""{mp3File}"" -ac 2 -f s16le -ar 48000 pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                using (var transmitStream = voiceNextConnection.GetTransmitStream())
                using (var ffmpeg = Process.Start(processStartInfo))
                {
                    var ffmpegOutputStream = ffmpeg.StandardOutput.BaseStream;
                    await ffmpegOutputStream.CopyToAsync(transmitStream);
                }
                await voiceNextConnection.SendSpeakingAsync(false);
            }
            else
            {
                await commandContext.RespondAsync("The bot is not connected to any voice channel");
            }
        }
    }
}
