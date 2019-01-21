using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BadmintonLt.Integration.Players.Crawler.Persistence.Entities
{
    public class PlayerEntity: TableEntity
    {
        public int Gender { get; set; }

        public string ProfileUrl { get; set; }

        public string CorrelationId { get; set; }
    }
}
