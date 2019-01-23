namespace BadmintonLt.Integration.Players.Crawler.Providers.Models
{
    public class PlayerViewModel: ParseableViewModel
    {
        public int Gender { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfileUrl { get; set; }
    }
}