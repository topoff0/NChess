
namespace Account.Core.Entities
{
    public class Player : User
    {
        public int Elo { get; set; }
        public List<int> GamesId { get; set; } = [];
        public List<int> TournamentsId { get; set; } = [];
        public List<Player> Friends { get; set; } = [];
    }
}
