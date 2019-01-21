using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Integration;
using BadmintonLt.Integration.Players.Crawler.Persistence;
using BadmintonLt.Integration.Players.Crawler.Providers;
using BadmintonLt.Integration.Players.Crawler.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BadmintonLt.Integration.Players.Crawler
{
    public static class PlayersCrawler
    {
        [FunctionName("PlayersCrawler")]
        public static async Task Run(
            [TimerTrigger("0/5 * * * * *")]TimerInfo myTimer,
            ExecutionContext executionContext,
            ILogger log)
        {
            log.LogInformation("Started {integrationName} integration at {integrationDate}", 
                nameof(PlayersCrawler), 
                DateTime.UtcNow.ToString("g"));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(executionContext.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var competitionTypes =
                JsonConvert.DeserializeObject<IEnumerable<string>>(
                    configuration["CompetitionTypes"]);

            var playerPageUrlFormat = configuration["BadmintonLtPlayersPageUrlFormat"];
            var playersStorageConnectionString =
                configuration.GetConnectionString("StorageConnectionString");
            var messageBusConnectionString =
                configuration.GetConnectionString("MessageBusConnectionString");
            var playersIntegrationTopicName =
                configuration["PlayersIntegrationTopicName"];


            var playersService = new PlayersService(
                new BadmintonLtPortalPlayersProvider(),
                new PlayersTableStorageRepository(playersStorageConnectionString),
                new PlayersServiceBusTopicsIntegration(messageBusConnectionString, playersIntegrationTopicName));

            var players = new List<Player>();

            foreach (var competitionType in competitionTypes)
            {
                var formattedPageUrl = string.Format(playerPageUrlFormat, competitionType);
                players.AddRange(await playersService.GetPlayersFromAsync(formattedPageUrl));
            }

            var distincted = players.Distinct();
            foreach (var player in distincted)
            {
                await playersService.Import(player);
            }
        }
    }
}
