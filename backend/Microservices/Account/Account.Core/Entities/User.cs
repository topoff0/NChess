namespace Account.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public bool IsEmailConfirmed { get; set; }
        public List<RefreshToken> RefreshTokens{ get; set; } = [];
        // TODO: Make cleaning mechanism for old tokens

        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
