using System.Collections.Generic;
using System.Text;
using BadmintonLt.Integration.PlayerProfiles.Crawler.Integration.Contracts;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BadmintonLt.Integration.PlayerProfiles.Crawler
{
    public static class PlayerProfilesCrawler
    {
        [FunctionName("PlayerProfilesCrawler")]
        public static void Run(
        [ServiceBusTrigger(
            "%PlayersIntegrationTopic%",
            "%PlayerProfilesUpdateSubscription%",
            Connection = "MessageBusConnectionString")]
            Message playerProfilesUpdateMessage,
            ILogger log)
        {
            var playersProfilesUpdate =
                JsonConvert.DeserializeObject<IEnumerable<PlayerProfileUpdateMessage>>(
                    Encoding.UTF8.GetString(
                        playerProfilesUpdateMessage.Body));
        }
    }
}
