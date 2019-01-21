using System;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;

namespace BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Integration
{
    public interface IPlayersIntegration
    {
        Task CreatedForAsync(string identity, Player player);

        Task UpdatedForAsync(string identity, Player player);
    }
}
