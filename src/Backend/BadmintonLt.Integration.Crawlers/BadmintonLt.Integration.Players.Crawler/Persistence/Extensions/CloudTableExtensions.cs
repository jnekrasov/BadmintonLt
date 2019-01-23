using System.Collections.Generic;
using System.Threading.Tasks;
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

        public static async Task<IEnumerable<TableResult>> ExecuteBatchAsync(
            this CloudTable dataSource, 
            IEnumerable<TableBatchOperation> batchOperations)
        {
            var result = new List<TableResult>();

            foreach (var batchOperation in batchOperations)
            {
                result.AddRange(await dataSource.ExecuteBatchAsync(batchOperation));
            }

            return result;
        }
    }
}