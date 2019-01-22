using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BadmintonLt.Integration.Players.Crawler.Persistence.Entities
{
    public class PlayerEntity: TableEntity
    {
        public string FistName { get; set; }

        public string LastName { get; set; }

        public int Gender { get; set; }

        public string ProfileUrl { get; set; }

        public string CorrelationId { get; set; }

        public string ClubName { get; set; }

        public string ClubLogoUrl { get; set; }
    }
}
