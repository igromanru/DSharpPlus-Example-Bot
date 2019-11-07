using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace DSharpPlus_Example_Bot.Extensions
{
    /// <summary>
    /// New methods for CommandContext
    /// </summary>
    public static class CommandContextExtensions
    {
        /// <summary>
        /// Always respond to a user with a DM.
        /// </summary>
        /// <param name="commandContext"></param>
        /// <param name="content"></param>
        /// <param name="isTTS"></param>
        /// <param name="embed"></param>
        /// <returns></returns>
        public static async Task<DiscordMessage> RespondWithDmAsync(this CommandContext commandContext, string content = null, bool isTTS = false, DiscordEmbed embed = null)
        {
            if (commandContext == null)
                throw new InvalidOperationException("CommandContext can't be null");
            return await (commandContext.Member != null ? commandContext.Member.SendMessageAsync(content, isTTS, embed) : commandContext.RespondAsync(content, isTTS, embed));
        }
    }
}
