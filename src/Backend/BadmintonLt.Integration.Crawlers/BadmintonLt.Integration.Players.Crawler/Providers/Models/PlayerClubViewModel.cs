namespace BadmintonLt.Integration.Players.Crawler.Providers.Models
{
    public class PlayerClubViewModel: ParseableViewModel
    {
        public string Name { get; set; }

        public string PlayersPageUrl { get; set; }

        public string LogoUrl { get; set; }
    }
}