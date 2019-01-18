using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

            var parsingService = new ParsingService();

            foreach (var competitionType in competitionTypes)
            {
                var formattedPageUrl = string.Format(playerPageUrlFormat, competitionType);
                var players = await parsingService.GetPlayersFromAsync(formattedPageUrl);
            }

            
            
        }
    }
}
