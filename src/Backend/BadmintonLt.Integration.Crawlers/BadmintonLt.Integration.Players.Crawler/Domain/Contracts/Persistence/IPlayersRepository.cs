using System;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;

namespace BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Persistence
{
    public interface IPlayersRepository
    {
        Task<bool> ExistsAsync(Player player);

        Task AddForAsync(string identity, Player player);

        Task<string> GetCorrelationIdentityForAsync(Player player);

        Task UpdateAsync(Player player);

    }
}
