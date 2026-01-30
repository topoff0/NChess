using System.Text.Json.Serialization;

namespace Chess.DTO.Responses
{
    public class BaseResponse
    {
        public BaseResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            ResponseMessage = message;
        }
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonPropertyName("message")]
        public string ResponseMessage { get; set; }
    }
}