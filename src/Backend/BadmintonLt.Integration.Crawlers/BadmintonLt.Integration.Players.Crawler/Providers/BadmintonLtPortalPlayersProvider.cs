using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Providers;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Providers.Exceptions;

namespace BadmintonLt.Integration.Players.Crawler.Providers
{
    public class BadmintonLtPortalPlayersProvider : IPlayersProvider
    {
        private const string PlayerExternalIdParameterName = "zid";
        private const string ClubExternalIdParameterName = "kid";

        private readonly IBrowsingContext _parsingContext;

        public BadmintonLtPortalPlayersProvider()
        {
            _parsingContext = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        }

        public async Task<IEnumerable<Player>> GetPlayersFromAsync(string clubPlayersPageUrl)
        {
            var result = new List<Player>();

            var document = await _parsingContext.OpenAsync(clubPlayersPageUrl);

            var playersTable = GetPlayersTableFrom(document)
                               ?? throw new ParsingSourceFormatException(
                                   clubPlayersPageUrl,
                                   nameof(BadmintonLtPortalPlayersProvider),
                                   "Cannot get players table");

            var playerEntries = GetPlayerEntriesFrom(playersTable);

            foreach (var playerEntry in playerEntries)
            {
                result.AddRange(GetPlayersFrom(playerEntry));
            }

            return result;
        }

        private IEnumerable<IElement> GetPlayerEntriesFrom(IElement playersTable)
            => playersTable.QuerySelectorAll("tr").Skip(1);

        private IElement GetPlayersTableFrom(IDocument document)
        {
            var tables = document.QuerySelectorAll("table");
            return tables.ElementAtOrDefault(10);
        }

        private IEnumerable<Player> GetPlayersFrom(
            IElement playersData)
        {
            var genderImageEntry =
                playersData.QuerySelector<IHtmlImageElement>("td:nth-child(2) > img");
            var firstNameEntry =
                playersData.QuerySelector<IHtmlAnchorElement>("td:nth-child(4) > a")
                ?? playersData.QuerySelector<IHtmlAnchorElement>("td:nth-child(4) > b > a");
            var lastNameEntry =
                playersData.QuerySelector<IHtmlAnchorElement>("td:nth-child(5) > a")
                ?? playersData.QuerySelector<IHtmlAnchorElement>("td:nth-child(5) > b > a");
            var clubImageEntry =
                playersData.QuerySelector<IHtmlImageElement>("td:nth-child(3) > img");

            yield return new Player(
                GetExternalIdFrom(firstNameEntry, PlayerExternalIdParameterName),
                GetGenderFrom(genderImageEntry),
                firstNameEntry.TextContent,
                lastNameEntry.TextContent,
                firstNameEntry.Href,
                new PlayerClub(
                    GetExternalIdFrom(firstNameEntry, ClubExternalIdParameterName),
                    clubImageEntry.Title,
                    clubImageEntry.Source));
        }

        private string GetExternalIdFrom(IHtmlAnchorElement anchor, string externalIdParameterName)
        {
            var uriBuilder = new UriBuilder(anchor.Href);
            var queryParameters = HttpUtility.ParseQueryString(uriBuilder.Query);
            return queryParameters[externalIdParameterName];
        }

        private Gender GetGenderFrom(IHtmlImageElement image)
            => image.Source.Contains("V", StringComparison.InvariantCultureIgnoreCase)
                ? Gender.Male
                : Gender.Female;
    }
}