using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BadmintonLt.Integration.Players.Crawler.Persistence.Entities
{
    public class PlayerEntity: TableEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Gender { get; set; }

        public string ProfileUrl { get; set; }
    }
}
