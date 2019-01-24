namespace BadmintonLt.Integration.Players.Crawler.Integration.Messages
{
    public class PlayerProfileUpdateMessage
    {
        public string Id { get; set; }

        public string ProfileUrl { get; set; }

        public string ClubName { get; set; }

        public string ClubLogoUrl { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}