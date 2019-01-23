using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Persistence.Entities;

namespace BadmintonLt.Integration.Players.Crawler.Persistence.Extensions
{
    public static class PlayerPersistenceExtensions
    {
        public static PlayerEntity ToPersisted(this Player player)
        {
            return new PlayerEntity()
            {
                PartitionKey = player.ClubInformation.ExternalId,
                RowKey = player.ExternalId,
                Gender = player.Gender.NumericEquivalent,
                ProfileUrl = player.ProfileUrl,
                InternalId = player.InternalId,
                ClubName = player.ClubInformation.Name,
                ClubLogoUrl = player.ClubInformation.LogoUrl,
                FistName = player.FirstName,
                LastName = player.LastName
            };
        }

        public static Player ToDomain(this PlayerEntity entity)
        {
            return new Player(
                entity.RowKey,
                Gender.CreateFrom(entity.Gender),
                entity.FistName,
                entity.LastName,
                entity.ProfileUrl,
                new PlayerClub(
                    entity.PartitionKey,
                    entity.ClubName,
                    entity.ClubLogoUrl),
                entity.InternalId);
        }
    }
}