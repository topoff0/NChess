namespace Account.Application.DTOs.Requests.EmailSender;

public record SendEmailDto(string Recipient, string Subject, string Body);
