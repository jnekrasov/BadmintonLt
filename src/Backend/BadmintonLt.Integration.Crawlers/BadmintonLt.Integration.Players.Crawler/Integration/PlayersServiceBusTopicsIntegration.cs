using System.Text;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Integration;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace BadmintonLt.Integration.Players.Crawler.Integration
{
    public class PlayersServiceBusTopicsIntegration: IPlayersIntegration
    {
        private readonly TopicClient _topicClient;

        public PlayersServiceBusTopicsIntegration(string connectionString, string topic)
        {
            _topicClient = new TopicClient(connectionString, topic);
        }

        public async Task CreatedForAsync(string identity, Player player)
        {
            var created = player.ToCreatedMessageFor(identity);
            await _topicClient.SendAsync(GetMessageFor(identity, created));
        }

        public async Task UpdatedForAsync(string identity, Player player)
        {
            var updated = player.ToUpdatedMessageFor(identity);
            await _topicClient.SendAsync(GetMessageFor(identity, updated));
        }

        private static Message GetMessageFor<T>(string identity, T payload)
        {
            return new Message(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(payload)))
            {
                ContentType = "application/json",
                CorrelationId = identity,
                UserProperties = { { "eventType", typeof(T).Name } }
            };
        }
    }
}