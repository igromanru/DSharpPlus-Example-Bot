using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DSharpPlus_Example_Bot.Commands
{
    /// <summary>
    /// Commands that can be used by @everyone. 
    /// </summary>
    public sealed class UserCommands : BaseCommandModule
    {

        // Holding the Random instance to improve multiple usages
        private readonly Random _random;

        public UserCommands()
        {
            _random = new Random();
        }

        /// <summary>
        /// A simple function that prints out a random number between lower and upper limits.
        /// By default it acts like a dice, the value is between 1 and 6.
        /// </summary>
        /// <param name="commandContext"></param>
        /// <param name="lowerLimit">Lowest possible value (default = 1)</param>
        /// <param name="upperLimit">Highest possible value (default = 6)</param>
        /// <returns>Random number between the lowerLimit and upperLimit.</returns>
        [Command("dice"), Description("Roll a dice")]
        public async Task DiceAsync(CommandContext commandContext, [Description("Lower limit")] int lowerLimit = 1, [Description("Upper limit")] int upperLimit = 6)
        {
            // Error check
            if (lowerLimit > upperLimit)
            {
                await commandContext.RespondAsync($"Error: the lower limit ({lowerLimit}) can't be bigger as the upper limit ({upperLimit})");
            }
            else
            {
                var randomNumber = 0;
                // calling Next 3 times to improve the randomization 
                for (var i = 0; i < 3; i++)
                {
                    randomNumber = _random.Next(lowerLimit, upperLimit + 1);
                }
                await commandContext.RespondAsync($"Rolling the dice: {randomNumber}");
            }
        }
    }
}
