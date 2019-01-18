using System.Collections.Generic;
using System.Linq;
using BadmintonLt.Integration.Players.Crawler.Entities;
using Microsoft.Build.Utilities;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace BadmintonLt.Integration.Players.Crawler.Services
{
    public class ParsingService
    {
        private readonly IBrowsingContext _parsingContext;

        public ParsingService()
        {
            _parsingContext = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        }

        public async Task<IEnumerable<Player>> GetPlayersFromAsync(string playersPageUrl)
        {
            var document = await _parsingContext.OpenAsync(playersPageUrl);
            var playersTable = document.QuerySelectorAll("table")[9];
            var playerRows = playersTable.QuerySelectorAll("tr").Skip(2);

            foreach (var playerRow in playerRows)
            {
                var playerData = playerRow.QuerySelectorAll("td");

                var player = playerData[1].QuerySelector("a").TextContent;
            }

            var selected = document.QuerySelectorAll<IElement>("body > table > table");
            return null;
        }
    }
}