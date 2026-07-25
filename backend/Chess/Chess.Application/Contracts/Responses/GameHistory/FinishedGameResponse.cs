using System.Text.Json.Serialization;

namespace Chess.Application.Contracts.Responses.GameHistory;

public sealed class FinishedGameResponse(int id, string finalFen, List<string> moves, string result, DateTime finishedAt)
{
    [JsonPropertyName("id")]
    public int Id { get; set; } = id;
    [JsonPropertyName("finalFen")]
    public string FinalFen { get; set; } = finalFen;
    [JsonPropertyName("moves")]
    public List<string> Moves { get; set; } = moves;
    [JsonPropertyName("result")]
    public string Result { get; set; } = result;
    [JsonPropertyName("finishedAt")]
    public DateTime FinishedAt { get; set; } = finishedAt;
}
