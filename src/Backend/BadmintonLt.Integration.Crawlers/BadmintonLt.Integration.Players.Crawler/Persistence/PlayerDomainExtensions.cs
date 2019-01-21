using System;
using BadmintonLt.Integration.Players.Crawler.Domain.Entities;
using BadmintonLt.Integration.Players.Crawler.Persistence.Entities;

namespace BadmintonLt.Integration.Players.Crawler.Persistence
{
    public static class PlayerDomainExtensions
    {
        public static PlayerEntity ToPersistedWith(this Player player, string identity)
        {
            return null;
        }
    }
}
