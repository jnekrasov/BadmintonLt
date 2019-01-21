using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Integration.Messages;

namespace BadmintonLt.Integration.Players.Crawler.Integration
{
    public static class PlayerIntegrationExtensions
    {
        public static PlayerCreatedMessage ToCreatedMessageFor(this Player player, string identity)
        {
            return new PlayerCreatedMessage()
            {
                Id = identity,
                FirstName = player.FirstName,
                LastName = player.LastName,
                ProfileUrl = player.ProfileUrl
            };
        }
        public static PlayerUpdatedMessage ToUpdatedMessageFor(this Player player, string identity)
        {
            return new PlayerUpdatedMessage()
            {
                Id = identity,
                ProfileUrl = player.ProfileUrl
            };
        }
    }
}