using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Providers;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Providers.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace BadmintonLt.Integration.Players.Crawler.Providers
{
    public class BadmintonLtPortalClubsProvider : IPlayerClubUrlsProvider
    {
        private const string PlayersPageQueryIdParameterName = "m";
        private const string PlayersPageQueryIdParameterValue = "3";

        private readonly IBrowsingContext _parsingContext;

        public BadmintonLtPortalClubsProvider()
        {
            _parsingContext = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        }

        public async Task<IEnumerable<string>> GetPlayerClubUrlsAsyncFrom(string clubsPageUrl)
        {
            var document = await _parsingContext.OpenAsync(clubsPageUrl);
            var clubsTable = GetClubsTableFrom(document)
                             ?? throw new ParsingSourceFormatException(
                                 clubsPageUrl,
                                 nameof(BadmintonLtPortalClubsProvider),
                                 "Cannot get clubs table");

            var clubsEntries = GetClubsEntriesFrom(clubsTable);

            var result = new List<string>();

            foreach (var clubEntry in clubsEntries)
            {
                if (TryGetClubUrlFrom(clubEntry, out var club))
                {
                    result.Add(club);
                }
            }

            return result;
        }

        private bool TryGetClubUrlFrom(IElement clubEntry, out string clubPlayersSourceUrl)
        {
            var clubPageEntry =
                clubEntry.QuerySelector<IHtmlAnchorElement>("td:nth-child(2) > a");
            var clubLogoEntry =
                clubEntry.QuerySelector<IHtmlImageElement>("td:nth-child(1) > img");

            if (clubPageEntry == null
                || clubLogoEntry == null)
            {
                clubPlayersSourceUrl = null;
                return false;
            }

            clubPlayersSourceUrl = GetPlayersPageUrl(clubPageEntry);
            return true;
        }

        private static string GetPlayersPageUrl(IHtmlAnchorElement clubPageEntry)
        {
            var uriBuilder = new UriBuilder(clubPageEntry.Href);
            var queryParameters = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryParameters[PlayersPageQueryIdParameterName] = PlayersPageQueryIdParameterValue;
            uriBuilder.Query = queryParameters.ToString();
            return uriBuilder.Uri.ToString();
        }


        private IEnumerable<IElement> GetClubsEntriesFrom(IElement clubsTable)
            => clubsTable.QuerySelectorAll("tr").Skip(1);

        private IElement GetClubsTableFrom(IDocument document)
        {
            var tables = document.QuerySelectorAll("table");
            return tables.ElementAtOrDefault(9);
        }
    }
}