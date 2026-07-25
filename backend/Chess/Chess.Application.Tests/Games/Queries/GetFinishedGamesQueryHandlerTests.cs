using Chess.Application.Contracts.Responses.GameHistory;
using Chess.Application.Features.Games.Queries.GetFinishedGames;
using Chess.Core.Entities;
using Chess.Core.Repositories;

namespace Chess.Application.Tests.Games.Queries;

public sealed class GetFinishedGamesQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenPlayerHasFinishedGames_ReturnsThemMostRecentFirstAndSkipsOthers()
    {
        Guid playerId = Guid.NewGuid();

        GameInfo olderGame = new()
        {
            Fens = ["fen-older-1", "fen-older-2"],
            Moves = ["e4", "e5"],
            IsActiveGame = false,
            FirstPlayerId = playerId,
            Result = "DRAW",
            FinishedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        GameInfo newerGame = new()
        {
            Fens = ["fen-newer"],
            Moves = ["d4"],
            IsActiveGame = false,
            FirstPlayerId = playerId,
            Result = "Computer",
            FinishedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        GameInfo activeGame = new()
        {
            Fens = ["fen-active"],
            Moves = [],
            IsActiveGame = true,
            FirstPlayerId = playerId
        };

        GameInfo otherPlayersGame = new()
        {
            Fens = ["fen-other"],
            Moves = [],
            IsActiveGame = false,
            FirstPlayerId = Guid.NewGuid(),
            Result = "DRAW",
            FinishedAt = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        FakeGameRepository repository = new([olderGame, newerGame, activeGame, otherPlayersGame]);
        GetFinishedGamesQueryHandler handler = new(repository);

        GetFinishedGamesResponse response = await handler.Handle(
            new GetFinishedGamesQuery(playerId), CancellationToken.None);

        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Games.Count);
        Assert.Equal("fen-newer", response.Games[0].FinalFen);
        Assert.Equal("fen-older-2", response.Games[1].FinalFen);
    }

    private sealed class FakeGameRepository(List<GameInfo> games) : IGameRepository
    {
        private readonly List<GameInfo> _games = games;

        public Task<GameInfo?> GetActiveByFirstPlayerIdAsync(Guid firstPlayerId, CancellationToken token)
            => throw new NotSupportedException();

        public Task<GameInfo?> GetByFirstPlayerIdAsync(Guid firstPlayerId, CancellationToken token)
            => throw new NotSupportedException();

        public Task<GameInfo?> GetActiveByPlayerIdAsync(Guid playerId, CancellationToken token)
            => throw new NotSupportedException();

        public Task<List<GameInfo>> GetFinishedByPlayerIdAsync(Guid playerId, CancellationToken token)
        {
            List<GameInfo> result = [.. _games
                .Where(game => (game.FirstPlayerId == playerId || game.SecondPlayerId == playerId)
                    && !game.IsActiveGame)
                .OrderByDescending(game => game.FinishedAt)];

            return Task.FromResult(result);
        }

        public Task AddAsync(GameInfo game, CancellationToken token)
            => throw new NotSupportedException();
    }
}
