using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Integration;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Integration.Messages;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace BadmintonLt.Integration.Players.Crawler.Integration
{
    public class PlayersServiceBusIntegration : IPlayersIntegration
    {
        private readonly TopicClient _topicClient;

        public PlayersServiceBusIntegration(string connectionString, string topic)
        {
            _topicClient = new TopicClient(connectionString, topic);
        }

        public async Task SchedulePlayerProfilesUpdateAsync(IEnumerable<Player> players)
        {
            foreach (var message in GetMessagesFor(players))
            {
                await _topicClient.SendAsync(message);
            }
        }

        private static IEnumerable<Message> GetMessagesFor(IEnumerable<Player> players)
        {
            var groupedContracts = players
                .Select(p => p.ToContract())
                .GroupBy(c => c.ClubName);

            foreach (var contracts in groupedContracts)
            {
                yield return new Message(
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(contracts)))
                {
                    ContentType = "application/json",
                    CorrelationId = Guid.NewGuid().ToString("D"),
                    UserProperties = {{"messageType", nameof(PlayerProfileUpdateMessage)}}
                };
            }
        }
    }
}