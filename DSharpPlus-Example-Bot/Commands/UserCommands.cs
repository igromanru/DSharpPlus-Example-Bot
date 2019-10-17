using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DSharpPlus_Example_Bot.Commands
{
    /// <summary>
    /// Commands that can be used by @everyone. 
    /// </summary>
    public class UserCommands : BaseCommandModule
    {

        // Holding the Random instance to improve multiple usages
        private readonly Random _random;

        public UserCommands()
        {
            _random = new Random();
        }

        [Command("dice"), Description("Roll a dice")]
        public async Task DiceAsync(CommandContext commandContext, [Description("Lower limit")] int lowerLimit = 1, [Description("Upper limit")] int upperLimit = 6)
        {
            var randomNumber = 0;
            // calling Next 3 times to improve the randomization 
            for (var i = 0; i < 3; i++)
            {
                randomNumber = _random.Next(lowerLimit, upperLimit + 1);
            }
            await commandContext.RespondAsync("Rolling the dice: " + randomNumber);
        }
    }
}
