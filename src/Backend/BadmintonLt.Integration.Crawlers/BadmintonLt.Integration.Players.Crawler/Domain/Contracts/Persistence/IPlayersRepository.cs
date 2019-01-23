using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;

namespace BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Persistence
{
    public interface IPlayersRepository
    {
        Task<IEnumerable<Player>> GetAllAsync();

        Task InsertOrMergeAsync(IEnumerable<Player> players);
    }
}
