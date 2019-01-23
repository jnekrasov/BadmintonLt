using BadmintonLt.Integration.Players.Crawler.Domain.Entities;

namespace BadmintonLt.Integration.Players.Crawler.Providers.Models
{
    public static class PlayerMapper
    {
        public static Player ToDomain(
            this PlayerViewModel playerViewModel,
            PlayerClubViewModel playerClubViewModel)
        {
            return new Player(
                playerViewModel.Id,
                Gender.CreateFrom(playerViewModel.Gender), 
                playerViewModel.FirstName,
                playerViewModel.LastName,
                playerViewModel.ProfileUrl,
                new PlayerClub(
                    playerClubViewModel.Id, 
                    playerClubViewModel.Name, 
                    playerClubViewModel.LogoUrl) 
                );
        }
    }
}