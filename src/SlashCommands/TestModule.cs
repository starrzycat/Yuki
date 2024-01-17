using Discord.Interactions;

namespace Yuki.SlashCommands {
    public class TestModule : InteractionModuleBase {
        [SlashCommand("test", "test")]
        public async Task TestCommandAsync() {
            await RespondAsync("test response");
        }
    }
}