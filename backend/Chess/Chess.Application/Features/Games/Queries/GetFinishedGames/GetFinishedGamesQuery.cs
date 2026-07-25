using Chess.Application.Contracts.Responses.GameHistory;
using Chess.Core.Entities;
using Chess.Core.Repositories;
using MediatR;

namespace Chess.Application.Features.Games.Queries.GetFinishedGames;

public record GetFinishedGamesQuery(Guid PlayerId) : IRequest<GetFinishedGamesResponse>;

public sealed class GetFinishedGamesQueryHandler(IGameRepository gameRepository)
    : IRequestHandler<GetFinishedGamesQuery, GetFinishedGamesResponse>
{
    private readonly IGameRepository _gameRepository = gameRepository;

    public async Task<GetFinishedGamesResponse> Handle(GetFinishedGamesQuery request, CancellationToken token)
    {
        List<GameInfo> games = await _gameRepository.GetFinishedByPlayerIdAsync(request.PlayerId, token);

        List<FinishedGameResponse> response = [.. games
            .Select(game => new FinishedGameResponse(
                id: game.Id,
                finalFen: game.Fens[^1],
                moves: game.Moves,
                result: game.Result ?? "UNKNOWN",
                finishedAt: game.FinishedAt ?? DateTime.MinValue))];

        return new GetFinishedGamesResponse(isSuccess: true, message: "Finished games retrieved", games: response);
    }
}
