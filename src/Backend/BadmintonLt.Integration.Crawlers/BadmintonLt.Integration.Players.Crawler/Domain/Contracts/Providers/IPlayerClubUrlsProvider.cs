using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;

namespace BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Providers
{
    public interface IPlayerClubUrlsProvider
    {
        Task<IEnumerable<string>> GetPlayerClubUrlsAsyncFrom(string clubsPageUrl);
    }
}