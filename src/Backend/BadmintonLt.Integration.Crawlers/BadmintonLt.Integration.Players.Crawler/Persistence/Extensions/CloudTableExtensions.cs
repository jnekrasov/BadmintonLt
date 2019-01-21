using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Build.Utilities;
using Microsoft.WindowsAzure.Storage.Table;

namespace BadmintonLt.Integration.Players.Crawler.Persistence.Extensions
{
    public static class CloudTableExtensions
    {
        public static async Task<IEnumerable<T>> ExecuteQueryAsync<T>(
            this CloudTable dataSource, 
            TableQuery<T> query)
            where T: ITableEntity, new()
        {
            TableContinuationToken token = null;
            var entities = new List<T>();

            do
            {
                var queryResult = await dataSource.ExecuteQuerySegmentedAsync(
                    query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;
        }
    }
}