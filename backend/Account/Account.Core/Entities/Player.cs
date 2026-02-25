
namespace Account.Core.Entities;

public sealed class Player
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public int Elo { get; private set; }

    public Guid[] GamesIds { get; private set; } = [];
    public Guid[] TournamentsIds { get; private set; } = [];
    public Guid[] FriendsIds { get; private set; } = [];

    private Player() { }


    public static Player Create(Guid userId)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Elo = 1000
        };

    public void Win()
    {
        Elo += 30;
    }

    public void Lose()
    {
        Elo -= 30;
    }
}
