using System.Text.Json.Serialization;

namespace Chess.Application.Contracts.Responses.GameHistory
{
    public sealed class GetFinishedGamesResponse(bool isSuccess, string message, List<FinishedGameResponse> games)
        : BaseResponse(isSuccess, message)
    {
        [JsonPropertyName("games")]
        public List<FinishedGameResponse> Games { get; set; } = games;
    }
}
