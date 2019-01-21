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
    public class PlayersTableStorageRepository : IPlayersRepository, IDisposable
    {
        private const string PlayersTableName = "Players";
        private readonly LazyAsync<CloudTable> _dataSource;
        private readonly LazyAsync<Dictionary<(string, string), PlayerEntity>> _playersCache;

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
                async () => await SetupDataSourceFor(tableClient));
            _playersCache = new LazyAsync<Dictionary<(string, string), PlayerEntity>>(
                async () => await LoadPlayersAsync());
        }


        public async Task<bool> ExistsAsync(Player player)
        {
            var players = await _playersCache;
            return players.ContainsKey((player.LastName, player.FirstName));
        }

        public async Task AddForAsync(string identity, Player player)
        {
            if (await ExistsAsync(player))
            {
                return;
            }

            var players = await _playersCache;
            var entity = player.ToPersistedWith(identity);
            players.Add((player.LastName, player.FirstName), entity);
        }

        public async Task<string> GetCorrelationIdentityForAsync(Player player)
        {
            if (!await ExistsAsync(player))
            {
                return null;
            }

            var players = await _playersCache;
            var name = (player.LastName, player.FirstName);
            return players[name].CorrelationId;
        }

        public async Task UpdateAsync(Player player)
        {
            if (!await ExistsAsync(player))
            {
                return;
            }

            var players = await _playersCache;
            var name = (player.LastName, player.FirstName);
            players[name].ProfileUrl = player.ProfileUrl;
        }

        private async Task<CloudTable> SetupDataSourceFor(CloudTableClient tableClient)
        {
            var dataSource = tableClient.GetTableReference(PlayersTableName);
            await dataSource.CreateIfNotExistsAsync();

            return dataSource;
        }

        private async Task<Dictionary<(string, string), PlayerEntity>> LoadPlayersAsync()
        {
            var dataSource = await _dataSource;
            var query = new TableQuery<PlayerEntity>();
            var players = await dataSource.ExecuteQueryAsync(query);

            return players.ToDictionary(
                p => (p.PartitionKey, p.RowKey),
                p => p);
        }

        public void Dispose()
        {

        }
    }
}