using System.Diagnostics;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using NAudio.Wave;

namespace DSharpPlus_Example_Bot.Commands
{
    [RequireGuild]
    public sealed class VoiceCommands : BaseCommandModule
    {
        [Command("join"), Description("Connect the bot to a voice channel")]
        public async Task JoinAsync(CommandContext commandContext, DiscordChannel channel)
        {
            var voiceNext = commandContext.Client.GetVoiceNext();
            await voiceNext.ConnectAsync(channel);
        }

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

        [Command("playwav"), Description("Voice chat test command: plays a hardcoded wav file")]
        public async Task PlayAsync(CommandContext commandContext)
        {
            var voiceNext = commandContext.Client.GetVoiceNext();
            var voiceNextConnection = voiceNext.GetConnection(commandContext.Guild);
            if (voiceNextConnection != null)
            {
                voiceNextConnection.SendSpeaking();
                using (var transmitStream = voiceNextConnection.GetTransmitStream())
                using (var reader = new WaveFileReader("Flurry.wav"))
                {
                    await reader.CopyToAsync(transmitStream);
                }
                voiceNextConnection.SendSpeaking(false);
            }
            else
            {
                await commandContext.RespondAsync("The bot is not connected to any voice channel");
            }
        }

        [Command("playmp3"), Description("Voice chat test command: plays a hardcoded mp3 file")]
        public async Task PlayMp3Async(CommandContext commandContext)
        {
            const string mp3File = "Track A - Stephen Bennett.mp3";
            var voiceNext = commandContext.Client.GetVoiceNext();
            var voiceNextConnection = voiceNext.GetConnection(commandContext.Guild);
            if (voiceNextConnection != null)
            {
                voiceNextConnection.SendSpeaking();
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
                voiceNextConnection.SendSpeaking(false);
            }
            else
            {
                await commandContext.RespondAsync("The bot is not connected to any voice channel");
            }
        }
    }
}
