using System;
using System.Threading.Tasks;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Persistence;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Persistence.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using BadmintonLt.Integration.Players.Crawler.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace BadmintonLt.Integration.Players.Crawler.Persistence
{
    public class PlayersRepository : IPlayersRepository
    {
        private const string PlayersTableName = "Players";
        private LazyAsync<Dictionary<(string, string), PlayerEntity>> _players;

        public PlayersRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (!CloudStorageAccount.TryParse(connectionString, out var storageAccount))
            {
                throw new ArgumentException(
                    $"ConnectionString cannot be parsed: [{connectionString}]",
                    nameof(connectionString));
            }

            var tableClient = storageAccount.CreateCloudTableClient();
            _players = new LazyAsync<Dictionary<(string, string), PlayerEntity>>(
                async () => await PreloadPlayersAsync(tableClient));
        }

        public async Task<bool> ExistsAsync(Player player)
        {
            var dataSource = await GetDataSource();
            var retrieveOperation = TableOperation.Retrieve<PlayerEntity>(player.LastName, player.FirstName);

            var existed = await dataSource.ExecuteAsync(retrieveOperation);
            return existed != null;
        }

        private async Task<Dictionary<(string, string), PlayerEntity>> PreloadPlayersAsync(
            CloudTableClient tableClient)
        {
            var dataSource = tableClient.GetTableReference(PlayersTableName);
            await dataSource.CreateIfNotExistsAsync();

            TableContinuationToken token = null;
            var entities = new List<PlayerEntity>();
            do
            {
                var queryResult = await dataSource.ExecuteQuerySegmentedAsync(
                    new TableQuery<PlayerEntity>(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities.ToDictionary(p => (p.FirstName, p.LastName), p => p);
        }
    }
}
