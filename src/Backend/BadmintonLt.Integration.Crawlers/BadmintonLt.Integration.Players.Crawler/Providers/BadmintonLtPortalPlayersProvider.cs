using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Providers;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Providers.Exceptions;

namespace BadmintonLt.Integration.Players.Crawler.Providers
{
    public class BadmintonLtPortalPlayersProvider: IPlayersProvider
    {
        private readonly IBrowsingContext _parsingContext;

        public BadmintonLtPortalPlayersProvider()
        {
            _parsingContext = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        }

        public async Task<IEnumerable<Player>> GetPlayersFromAsync(string sourceUrl)
        { 
            var result = new List<Player>();
            var document = await _parsingContext.OpenAsync(sourceUrl);

            var playersTable = GetPlayersTableFrom(document)
                ?? throw new ParsingSourceFormatException(
                    sourceUrl,
                    nameof(BadmintonLtPortalPlayersProvider),
                    "Cannot get players tabe");

            var playersEntries = GetPlayersEntriesFrom(playersTable);

            foreach (var playersEntry in playersEntries)
            {
                var playersData = playersEntry.QuerySelectorAll("td");

                result.AddRange(GetPlayersFrom(playersData));
            }

            return result;
        }

        private IEnumerable<IElement> GetPlayersEntriesFrom(IElement playersTable)
            => playersTable.QuerySelectorAll("tr").Skip(2);

        private IElement GetPlayersTableFrom(IDocument document)
        {
            var tables = document.QuerySelectorAll("table");
            return tables.ElementAtOrDefault(9);
        }

        private IEnumerable<Player> GetPlayersFrom(IHtmlCollection<IElement> playersData)
        {
            foreach (var index in new[]{ 1, 7 })
            {
                var playerProfile = GetProfileFrom(playersData, index);
                if (!playerProfile.HasValue)
                {
                    continue;
                }

                yield return new Player(
                    GetGenderFrom(index),
                    playerProfile.Value.name,
                    playerProfile.Value.profileUrl);
            }
        }

        private Gender GetGenderFrom(int index)
            => index == 1 ? Gender.Male : Gender.Female;

        private (string name, string profileUrl)? GetProfileFrom(
            IHtmlCollection<IElement> playersData, 
            int index)
        {
            var nameElement = playersData.ElementAtOrDefault(index);
            if (nameElement == null)
            {
                return null;
            }

            if (!(nameElement.QuerySelector("a") is IHtmlAnchorElement nameEntry))
            {
                return null;
            }

            return (nameEntry.TextContent, nameEntry.Href);
        }
    }
}
