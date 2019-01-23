using System;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Watts.Shared.Functions.DependencyInjection.Composition;

namespace BadmintonLt.Integration.Players.Crawler
{
    public static class PlayersCrawler
    {
        [FunctionName("PlayersCrawler")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            ExecutionContext executionContext,
            [Import] PlayersService playersService,
            [Import] IConfiguration configuration,
            ILogger log)
        {
            log.LogInformation("Started {integrationName} integration at {integrationDate}", 
                nameof(PlayersCrawler), 
                DateTime.UtcNow.ToString("g"));

            var playersClubsPageUrl = configuration["BadmintonLtPlayersClubsPageUrl"];
            await playersService.ImportFrom(playersClubsPageUrl);
        }
    }
}
