using System.Collections.Generic;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Providers;

namespace BadmintonLt.Integration.Players.Crawler.Services
{
    public class ParsingService
    {
        private readonly IPlayersProvider _playersProvider;

        public ParsingService(IPlayersProvider playersProvider)
        {
            _playersProvider = playersProvider;
        }

        public async Task<IEnumerable<Player>> GetPlayersFromAsync(string playersPageUrl)
        {
            var players = await _playersProvider.GetPlayersFromAsync(playersPageUrl);


            return null;
        }
    }
}