using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;

namespace BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Integration
{
    public interface IPlayersIntegration
    {
        Task SchedulePlayerProfilesUpdateAsync(IEnumerable<Player> players);
    }
}
