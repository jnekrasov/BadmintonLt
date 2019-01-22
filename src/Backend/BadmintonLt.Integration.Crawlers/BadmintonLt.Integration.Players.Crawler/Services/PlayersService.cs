using System.Collections.Generic;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Providers;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Persistence;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Integration;
using System;

namespace BadmintonLt.Integration.Players.Crawler.Services
{
    public class PlayersService
    {
        private readonly IPlayersProvider _playersProvider;
        private readonly IPlayerClubUrlsProvider _playerClubUrlsProvider;
        private readonly IPlayersRepository _playersRepository;
        private readonly IPlayersIntegration _playersIntegration;

        public PlayersService(
            IPlayersProvider playersProvider,
            IPlayerClubUrlsProvider playerClubUrlsProvider,
            IPlayersRepository playersRepository,
            IPlayersIntegration playersIntegration)
        {
            _playersRepository = playersRepository;
            _playersProvider = playersProvider;
            _playersIntegration = playersIntegration;
            _playerClubUrlsProvider = playerClubUrlsProvider;
        }

        public async Task<IEnumerable<Player>> GetPlayersFromAsync(string clubsPageUrl)
        {
            var result = new List<Player>();
            var playerClubUrls = await _playerClubUrlsProvider.GetPlayerClubUrlsAsyncFrom(clubsPageUrl);

            foreach (var playerClubUrl in playerClubUrls)
            {
                result.AddRange(await _playersProvider.GetPlayersFromAsync(playerClubUrl));
            }

            return result;
        }

        public async Task Import(Player player)
        {
            if (!await _playersRepository.ExistsAsync(player))
            {
                var identity = CreateIdentity();
                await _playersRepository.AddForAsync(identity, player);
                //await _playersIntegration.CreatedForAsync(identity, player);
            }
            else
            {
                await _playersRepository.UpdateAsync(player);
                var identity = await _playersRepository.GetCorrelationIdentityForAsync(player);
                await _playersIntegration.UpdatedForAsync(identity, player);
            }
        }

        private static string CreateIdentity()
            => Guid.NewGuid().ToString("D");
    }
}