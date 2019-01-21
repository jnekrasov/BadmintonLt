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
        private readonly IPlayersRepository _playersRepository;
        private readonly IPlayersIntegration _playersIntegration;
        private readonly IPlayerIdentityProvider _playerIdentityProvider;

        public PlayersService(
            IPlayersProvider playersProvider,
            IPlayersRepository playersRepository,
            IPlayersIntegration playersIntegration,
            IPlayerIdentityProvider playerIdentityProvider)
        {
            _playersProvider = playersProvider;
            _playersRepository = playersRepository;
            _playersIntegration = playersIntegration;
            _playerIdentityProvider = playerIdentityProvider;
        }

        public async Task<IEnumerable<Player>> GetPlayersFromAsync(string playersPageUrl)
        {
            return await _playersProvider.GetPlayersFromAsync(playersPageUrl);
        }

        public async Task Import(Player player)
        {
            if (!_playersRepository.Exists(player))
            {
                var identity = _playerIdentityProvider.CreateIdentity();
                _playersRepository.AddFor(identity, player);
                _playersIntegration.CreateFor(identity, player);
            }
            else
            {
                var identity = _playerIdentityProvider.GetFor(player);
                _playersRepository.UpdateFor(identity, player);
                _playersIntegration.UpdateFor(identity, player);
            }
        }
    }
}