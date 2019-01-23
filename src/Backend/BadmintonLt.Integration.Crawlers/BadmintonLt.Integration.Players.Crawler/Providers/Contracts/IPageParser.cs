using System.Collections.Generic;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Providers.Models;

namespace BadmintonLt.Integration.Players.Crawler.Providers.Contracts
{
    public interface IPageParser<T>
        where T : ParseableViewModel
    {
        Task<IEnumerable<T>> ParseAsync(string sourceUrl);
    }
}