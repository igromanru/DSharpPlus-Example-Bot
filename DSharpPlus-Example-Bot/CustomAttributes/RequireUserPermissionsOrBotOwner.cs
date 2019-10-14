using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DSharpPlus_Example_Bot.CustomAttributes
{
    /// <summary>
    /// Defines that usage of this command is restricted to the owner of the bot or a user with special permissions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public sealed class RequireUserPermissionsOrBotOwner : CheckBaseAttribute
    {
        /// <summary>Gets the permissions required by this attribute.</summary>
        public Permissions Permissions { get; }

        /// <summary>
        /// Defines that usage of this command is restricted to members with specified permissions.
        /// </summary>
        /// <param name="permissions">Permissions required to execute this command.</param>
        public RequireUserPermissionsOrBotOwner(Permissions permissions)
        {
            Permissions = permissions;
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            var member = ctx.Member;
            if (member != null)
            {
                var currentApplication = ctx.Client.CurrentApplication;
                var permissions = ctx.Channel.PermissionsFor(member);
                if ((currentApplication != null && currentApplication.Owners.Any(x => x.Id == ctx.User.Id) || ctx.User.Id == ctx.Client.CurrentUser.Id)
                    || (member.Id == ctx.Guild.Owner.Id)
                    || ((permissions & Permissions.Administrator) != Permissions.None)
                    || ((permissions & this.Permissions) == Permissions))
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }
    }
}
