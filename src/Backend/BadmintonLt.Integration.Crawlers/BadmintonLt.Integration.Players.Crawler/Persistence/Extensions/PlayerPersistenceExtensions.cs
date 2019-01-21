using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Persistence.Entities;

namespace BadmintonLt.Integration.Players.Crawler.Persistence.Extensions
{
    public static class PlayerPersistenceExtensions
    {
        public static PlayerEntity ToPersistedWith(this Player player, string identity)
        {
            return new PlayerEntity()
            {
                PartitionKey = player.LastName,
                RowKey = player.FirstName,
                Gender = player.Gender.NumericEquivalent,
                ProfileUrl = player.ProfileUrl,
                CorrelationId = identity
            };
        }

        public static Player ToDomain(this PlayerEntity entity)
        {
            return new Player(
                Gender.CreateFrom(entity.Gender), 
                entity.PartitionKey, 
                entity.RowKey,
                entity.ProfileUrl);
        }
    }
}