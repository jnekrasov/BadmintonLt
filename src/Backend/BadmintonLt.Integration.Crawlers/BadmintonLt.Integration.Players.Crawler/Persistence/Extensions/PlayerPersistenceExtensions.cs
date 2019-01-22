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
                PartitionKey = player.Club.ExternalId,
                RowKey = player.ExternalId,
                Gender = player.Gender.NumericEquivalent,
                ProfileUrl = player.ProfileUrl,
                CorrelationId = identity,
                ClubName = player.Club.Name,
                ClubLogoUrl = player.Club.LogoUrl,
                FistName = player.FirstName,
                LastName = player.LastName
            };
        }

        public static Player ToDomain(this PlayerEntity entity)
        {
            return new Player(
                entity.RowKey,
                Gender.CreateFrom(entity.Gender),
                entity.PartitionKey,
                entity.RowKey,
                entity.ProfileUrl,
                new PlayerClub(
                    entity.PartitionKey,
                    entity.ClubName,
                    entity.ClubLogoUrl));
        }
    }
}