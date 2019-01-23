using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using BadmintonLt.Integration.Players.Crawler.Providers.Contracts;
using BadmintonLt.Integration.Players.Crawler.Providers.Exceptions;
using BadmintonLt.Integration.Players.Crawler.Providers.Models;

namespace BadmintonLt.Integration.Players.Crawler.Providers.Parsers
{
    public class BadmintonLtPlayersPageParser 
        : PageParserBase, IPageParser<PlayerViewModel>
    {
        private const string PlayerIdQueryParameterName = "zid";

        public async Task<IEnumerable<PlayerViewModel>> ParseAsync(string playersPageUrl)
        {
            var document = await ParsingContext.OpenAsync(playersPageUrl);

            var playersTable = GetPlayersTableFrom(document)
                               ?? throw new ParsingSourceFormatException(
                                   playersPageUrl,
                                   nameof(BadmintonLtPlayersProvider),
                                   "Cannot get players table");

            var playerEntries = GetPlayerEntriesFrom(playersTable);

            var result = new List<PlayerViewModel>();

            foreach (var playerEntry in playerEntries)
            {
                result.Add(GetPlayerFrom(playerEntry));
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

        private PlayerViewModel GetPlayerFrom(IElement playersData)
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

            return new PlayerViewModel()
            {
                Id = GetPlayerIdFrom(firstNameEntry),
                FirstName = firstNameEntry.TextContent,
                LastName = lastNameEntry.TextContent,
                Gender = GetGenderFrom(genderImageEntry),
                ProfileUrl = firstNameEntry.Href
            };
        }

        private string GetPlayerIdFrom(IHtmlAnchorElement anchor)
        {
            var uriBuilder = new UriBuilder(anchor.Href);
            var queryParameters = HttpUtility.ParseQueryString(uriBuilder.Query);
            return queryParameters[PlayerIdQueryParameterName];
        }

        private int GetGenderFrom(IHtmlImageElement image)
            => image.Source.Contains("V") ? 1 : 2;
    }
}