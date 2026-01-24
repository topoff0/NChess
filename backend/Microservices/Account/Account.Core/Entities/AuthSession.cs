namespace Account.Core.Entities;

public class AuthSession
{
    public string Email { get; set; } = string.Empty;
    public RegistrationMethod Method { get; set; }
    public RegistrationStatus Status { get; set; }
}


public enum RegistrationMethod
{
    Email = 1,
    Google = 2
}

public enum RegistrationStatus
{
    Started = 1,
    EmailSent = 2,
    EmailVerified = 3,
    GoogleVerified = 4,
    AccountCreated = 5,
    Completed = 6
}
