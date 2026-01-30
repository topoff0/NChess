using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Chess.DTO.Responses.GameProcess
{
    public class GameResponse(bool isSuccess, string message,string fen,
    Dictionary<int, List<int>> legalMoves, List<string> moveNotations, bool isGameEnded, string? winner)

            : BaseResponse(isSuccess, message)
    {
        [JsonPropertyName("fen")]
        public string Fen { get; set; } = fen;
        [JsonPropertyName("legalMoves")]
        public Dictionary<int, List<int>> LegalMoves { get; set; } = legalMoves;
        [JsonProperty("moveNotations")]
        public List<string> MoveNotations { get; set; } = moveNotations;
        [JsonPropertyName("isGameEnded")]
        public bool IsGameEnded { get; set; } = isGameEnded;
        [JsonPropertyName("winner")]
        public string? Winner { get; set; } = winner;
    }
}