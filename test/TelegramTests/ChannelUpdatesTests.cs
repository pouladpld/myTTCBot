using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Framework;
using TelegramTests.Shared;
using Xunit;

namespace TelegramTests
{
    [Collection("channel updates")]
    public class ChannelUpdatesTests : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture _fixture;

        public ChannelUpdatesTests(TestsFixture fixture)
        {
            _fixture = fixture;
        }

        [OrderedFact(DisplayName = "Should ignore a new channel post")]
        public async Task Should_Ignore_New_Post()
        {
            string update = @"{
                ""update_id"": 5952,
                ""channel_post"": {
                    ""message_id"": 760,
                    ""chat"": {
                        ""id"": -101,
                        ""title"": ""My Channel"",
                        ""type"": ""channel""
                    },
                    ""date"": 1540000000,
                    ""text"": ""Hola Mundo""
                }
            }";

            HttpResponseMessage response = await _fixture.HttpClient.PostWebhookUpdateAsync(update);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Empty(responseContent);

            _fixture.MockBotClient.VerifyAll();
            _fixture.MockBotClient.VerifyNoOtherCalls();
        }

        [OrderedFact(DisplayName = "Should ignore an edited channel post")]
        public async Task Should_Ignore_Edited_Post()
        {
            string update = @"{
                ""update_id"": 5953,
                ""edited_channel_post"": {
                    ""message_id"": 760,
                    ""chat"": {
                        ""id"": -101,
                        ""title"": ""My Channel"",
                        ""type"": ""channel""
                    },
                    ""date"": 1540000000,
                    ""edit_date"": 1540012000,
                    ""text"": ""Hello World""
                }
            }";

            HttpResponseMessage response = await _fixture.HttpClient.PostWebhookUpdateAsync(update);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Empty(responseContent);

            _fixture.MockBotClient.VerifyAll();
            _fixture.MockBotClient.VerifyNoOtherCalls();
        }
    }
}
