using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Tests.Integ.Common;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;

namespace Telegram.Bot.Tests.Integ.SendingMessages
{
    [Collection(Constants.TestCollections.TextMessage)]
    [TestCaseOrderer(Constants.TestCaseOrderer, Constants.AssemblyName)]
    public class TextMessageTests : IClassFixture<TextMessageTestsFixture>
    {
        public ITelegramBotClient BotClient => _fixture.BotClient;

        private readonly TestsFixture _fixture;

        private readonly TextMessageTestsFixture _classFixture;

        public TextMessageTests(TextMessageTestsFixture classFixture)
        {
            _classFixture = classFixture;
            _fixture = classFixture.TestsFixture;
        }

        #region 1. Sending text message

        [Fact(DisplayName = FactTitles.ShouldSendTextMessage)]
        [Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
        [ExecutionOrder(1.0)]
        public async Task Should_Send_Text_Message()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldSendTextMessage);

            const string text = "Hello world!";

            Message message = await BotClient.SendTextMessageAsync(
                chatId: _fixture.SuperGroupChatId,
                text: text
            );

            Assert.Equal(text, message.Text);
            Assert.Equal(MessageType.TextMessage, message.Type);
            Assert.Equal(_fixture.SuperGroupChatId.ToString(), message.Chat.Id.ToString());
        }

        [Fact(DisplayName = FactTitles.ShouldSendTextMessageToChannel)]
        [Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
        [ExecutionOrder(1.1)]
        public async Task Should_Send_Text_Message_To_Channel()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldSendTextMessageToChannel);

            string text = $"Hello members of channel {_classFixture.ChannelChatId}";

            Message message = await BotClient.SendTextMessageAsync(
                chatId: _classFixture.ChannelChatId,
                text: text
            );

            Assert.Equal(text, message.Text);
            Assert.Equal(MessageType.TextMessage, message.Type);
            Assert.Equal(_classFixture.ChannelChat.Id, message.Chat.Id);
            Assert.Equal(_classFixture.ChannelChat.Username, message.Chat.Username);
        }

        #endregion

        #region 2. Parsing text message entities

        [Fact(DisplayName = FactTitles.ShouldParseMessageEntities)]
        [Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
        [ExecutionOrder(2.1)]
        public async Task Should_Parse_Message_Entities()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldParseMessageEntities);

            MessageEntityType[] types = {
                MessageEntityType.Bold,
                MessageEntityType.Italic,
                MessageEntityType.Code,
                MessageEntityType.Pre,
            };

            Message message = await BotClient.SendTextMessageAsync(_fixture.SuperGroupChatId,
                string.Join("\n", $"*{types[0]}*", $"_{types[1]}_", $"`{types[2]}`", $"```{types[3]}```"),
                ParseMode.Markdown);

            Assert.Equal(types, message.Entities.Select(e => e.Type));
        }

        [Fact(DisplayName = FactTitles.ShouldPaseMessageEntitiesIntoValues)]
        [Trait(Constants.MethodTraitName, Constants.TelegramBotApiMethods.SendMessage)]
        [ExecutionOrder(2.2)]
        public async Task Should_Pase_Message_Entities_Into_Values()
        {
            await _fixture.SendTestCaseNotificationAsync(FactTitles.ShouldPaseMessageEntitiesIntoValues);

            string[] values =
            {
                "#TelegramBots",
                "@BotFather",
                "http://github.com/TelegramBots",
                "email@example.org",
                "/test",
                // "/test@" + _fixture.Bot.Username // ToDo
                // "text_link" // ToDo What is text_link type?
            };

            Message message = await BotClient.SendTextMessageAsync(_fixture.SuperGroupChatId,
                string.Join("\n", values));

            Assert.Equal(values, message.EntityValues);
        }

        #endregion

        private static class FactTitles
        {
            public const string ShouldSendTextMessage = "Should send text message";

            public const string ShouldSendTextMessageToChannel = "Should send text message to channel";

            public const string ShouldParseMessageEntities = "Should send markdown formatted text message and parse its entities";

            public const string ShouldPaseMessageEntitiesIntoValues = "Should send text message and parse its entity values";
        }
    }
}
