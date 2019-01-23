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
    public class BadmintonLtClubsPageParser 
        : PageParserBase, IPageParser<PlayerClubViewModel>
    {
        private const string PlayersPageQueryIdParameterName = "m";
        private const string PlayersPageQueryIdParameterValue = "3";
        private const string ClubIdQueryParameterName = "kid";

        public async Task<IEnumerable<PlayerClubViewModel>> ParseAsync(string clubsPageUrl)
        {
            var document = await ParsingContext.OpenAsync(clubsPageUrl);
            var clubsTable = GetClubsTableFrom(document)
                             ?? throw new ParsingSourceFormatException(
                                 clubsPageUrl,
                                 nameof(BadmintonLtClubsPageParser),
                                 "Cannot get clubs table");

            var clubsEntries = GetClubsEntriesFrom(clubsTable);

            var result = new List<PlayerClubViewModel>();

            foreach (var clubEntry in clubsEntries)
            {
                if (TryGetClubFrom(clubEntry, out var club))
                {
                    result.Add(club);
                }
            }

            return result;
        }

        private IEnumerable<IElement> GetClubsEntriesFrom(IElement clubsTable)
            => clubsTable.QuerySelectorAll("tr").Skip(1);

        private IElement GetClubsTableFrom(IDocument document)
        {
            var tables = document.QuerySelectorAll("table");
            return tables.ElementAtOrDefault(9);
        }

        private bool TryGetClubFrom(
            IElement clubEntry,
            out PlayerClubViewModel playerClubViewModel)
        {
            var clubPageUrlEntry =
                clubEntry.QuerySelector<IHtmlAnchorElement>("td:nth-child(2) > a");
            var clubLogoEntry =
                clubEntry.QuerySelector<IHtmlImageElement>("td:nth-child(1) > img");

            if (clubPageUrlEntry == null
                || clubLogoEntry == null)
            {
                playerClubViewModel = null;
                return false;
            }

            playerClubViewModel = new PlayerClubViewModel()
            {
                Id = GetClubIdFrom(clubPageUrlEntry),
                Name = clubPageUrlEntry.TextContent,
                LogoUrl = clubLogoEntry.Source,
                PlayersPageUrl = GetPlayersPageUrlFrom(clubPageUrlEntry)
            };
            return true;
        }


        private static string GetPlayersPageUrlFrom(IHtmlAnchorElement clubPageUrlEntry)
        {
            var uriBuilder = new UriBuilder(clubPageUrlEntry.Href);
            var queryParameters = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryParameters[PlayersPageQueryIdParameterName] = PlayersPageQueryIdParameterValue;
            uriBuilder.Query = queryParameters.ToString();
            return uriBuilder.Uri.ToString();
        }

        private static string GetClubIdFrom(IHtmlAnchorElement clubPageUrlEntry)
        {
            var uriBuilder = new UriBuilder(clubPageUrlEntry.Href);
            var queryParameters = HttpUtility.ParseQueryString(uriBuilder.Query);
            return queryParameters[ClubIdQueryParameterName];
        }
    }
}