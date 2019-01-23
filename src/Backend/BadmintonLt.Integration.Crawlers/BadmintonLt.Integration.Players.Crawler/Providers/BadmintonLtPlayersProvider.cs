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
using BadmintonLt.Integration.Players.Crawler.Providers.Contracts;
using BadmintonLt.Integration.Players.Crawler.Providers.Exceptions;
using BadmintonLt.Integration.Players.Crawler.Providers.Models;
using BadmintonLt.Integration.Players.Crawler.Providers.Parsers;

namespace BadmintonLt.Integration.Players.Crawler.Providers
{
    public class BadmintonLtPlayersProvider : IPlayersProvider
    {
        private readonly IPageParser<PlayerClubViewModel> _playerClubsPageParser;
        private readonly IPageParser<PlayerViewModel> _playersPageParser;

        public BadmintonLtPlayersProvider(
            IPageParser<PlayerClubViewModel> playerClubsPageParser = null,
            IPageParser<PlayerViewModel> playersPageParser = null)
        {
            _playersPageParser = playersPageParser 
                                 ?? new BadmintonLtPlayersPageParser();
            _playerClubsPageParser = playerClubsPageParser 
                                     ?? new BadmintonLtClubsPageParser();
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync(string clubsPageUrl)
        {
            var result = new List<Player>();
            var playerClubs = await _playerClubsPageParser.ParseAsync(clubsPageUrl);

            foreach (var playerClub in playerClubs)
            {
                var players = await _playersPageParser.ParseAsync(playerClub.PlayersPageUrl);
                result.AddRange(players.Select(p => p.ToDomain(playerClub)));
            }

            return result;
        }
    }
}