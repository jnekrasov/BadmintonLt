using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Integration.Messages;

namespace BadmintonLt.Integration.Players.Crawler.Integration
{
    public static class PlayerIntegrationExtensions
    {
        public static PlayerProfileUpdateMessage ToContract(this Player player)
        {
            return new PlayerProfileUpdateMessage()
            {
                FirstName = player.FirstName,
                Id = player.InternalId,
                LastName = player.LastName,
                ProfileUrl = player.ProfileUrl,
                ClubLogoUrl = player.ClubInformation.LogoUrl,
                ClubName = player.ClubInformation.Name
            };
        }
    }
}