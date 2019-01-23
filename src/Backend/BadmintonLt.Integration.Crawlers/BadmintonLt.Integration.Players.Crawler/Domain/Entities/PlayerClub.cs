using System;

namespace BadmintonLt.Integration.Players.Crawler.Domain.Entities
{
    public class PlayerClub
    {
        public string ExternalId { get; }

        public string Name { get; }

        public string LogoUrl { get; }

        public PlayerClub(
            string externalId,
            string name, 
            string logoUrl)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                throw new ArgumentNullException(nameof(externalId));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(logoUrl))
            {
                throw new ArgumentNullException(nameof(logoUrl));
            }

            ExternalId = externalId;
            Name = name;
            LogoUrl = logoUrl;
        }
    }
}