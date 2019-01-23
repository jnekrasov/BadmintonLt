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
using BadmintonLt.Integration.Players.Crawler.Persistence.Extensions;

namespace BadmintonLt.Integration.Players.Crawler.Persistence
{
    public class PlayersTableStorageRepository : IPlayersRepository
    {
        private const string PlayersTableName = "Players";
        private const int TableServiceBatchMaximumOperations = 100;
        private readonly LazyAsync<CloudTable> _dataSource;

        public PlayersTableStorageRepository(string connectionString)
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
            _dataSource = new LazyAsync<CloudTable>(
                async () =>
                {
                    var dataSource = tableClient.GetTableReference(PlayersTableName);
                    await dataSource.CreateIfNotExistsAsync();

                    return dataSource;
                });
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            var dataSource = await _dataSource;
            var query = new TableQuery<PlayerEntity>();
            var existed = await dataSource.ExecuteQueryAsync(query);

            return existed.Select(p => p.ToDomain());
        }

        public async Task InsertOrMergeAsync(IEnumerable<Player> players)
        {
            var batchOperations = new List<TableBatchOperation>();
            var grouped = players.GroupBy(p => p.ClubInformation.ExternalId);

            foreach (var group in grouped)
            {
                var currentOperationIndex = 0;
                var batchOperation = new TableBatchOperation();

                foreach (var player in group)
                {
                    batchOperation.InsertOrMerge(player.ToPersisted());
                    currentOperationIndex++;

                    if (currentOperationIndex >= TableServiceBatchMaximumOperations)
                    {
                        currentOperationIndex = 0;
                        batchOperations.Add(batchOperation);
                        batchOperation = new TableBatchOperation();
                    }
                }

                batchOperations.Add(batchOperation);
            }

            var dataSource = await _dataSource;
            await dataSource.ExecuteBatchAsync(batchOperations);
        }
    }
}