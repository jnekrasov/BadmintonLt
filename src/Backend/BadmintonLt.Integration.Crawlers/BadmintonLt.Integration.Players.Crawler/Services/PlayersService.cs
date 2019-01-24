using System.Collections.Generic;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Providers;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Persistence;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Integration;
using System;
using System.Linq;

namespace BadmintonLt.Integration.Players.Crawler.Services
{
    public class PlayersService
    {
        private readonly IPlayersProvider _playersProvider;
        private readonly IPlayersRepository _playersRepository;
        private readonly IPlayersIntegration _playersIntegration;

        public PlayersService(
            IPlayersProvider playersProvider,
            IPlayersRepository playersRepository,
            IPlayersIntegration playersIntegration)
        {
            _playersRepository = playersRepository;
            _playersProvider = playersProvider;
            _playersIntegration = playersIntegration;
        }

        public async Task ImportFrom(string sourcePageUrl)
        {
            var importedPlayers = (await _playersProvider.GetPlayersAsync(sourcePageUrl))
                .ToList();

            var existedPlayers = (await _playersRepository.GetAllAsync())
                .ToDictionary(p => p.MergedExternalId, p => p);

            foreach (var importedPlayer in importedPlayers)
            {
                if (existedPlayers.ContainsKey(importedPlayer.MergedExternalId))
                {
                    var existed = existedPlayers[importedPlayer.MergedExternalId];
                    importedPlayer.UpdateFrom(existed);
                }
            }

            await _playersRepository.InsertOrMergeAsync(importedPlayers);
            await _playersIntegration.SchedulePlayerProfilesUpdateAsync(importedPlayers);
        }
    }
}