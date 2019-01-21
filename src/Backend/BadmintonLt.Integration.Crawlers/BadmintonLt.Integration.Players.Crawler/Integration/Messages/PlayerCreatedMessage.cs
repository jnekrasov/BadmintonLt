namespace BadmintonLt.Integration.Players.Crawler.Integration.Messages
{
    public class PlayerCreatedMessage
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfileUrl { get; set; }
    }
}