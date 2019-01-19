using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BadmintonLt.Integration.Players.Crawler.Persistence.Entities
{
    public class PlayerEntity: TableEntity
    {
        public PlayerEntity(string firstName, string lastName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        public int Gender { get; set; }

        public string ProfileUrl { get; set; }
    }
}
