using Chess.Application.Contracts.Responses.GameProcess;

namespace Chess.Application.Features.Games.Common;

public sealed record GameCommandResult(GameResponse? Response, bool IsGameFound)
{
    public static GameCommandResult Success(GameResponse response)
    {
        return new GameCommandResult(response, IsGameFound: true);
    }

    public static GameCommandResult GameNotFound()
    {
        return new GameCommandResult(Response: null, IsGameFound: false);
    }
}
