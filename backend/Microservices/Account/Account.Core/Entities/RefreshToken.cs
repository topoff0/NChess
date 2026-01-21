namespace Account.Core.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool IsActive => DateTime.UtcNow <= Expires;

    }
}
