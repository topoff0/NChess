namespace Account.Application.Common.DTOs;

public record SendEmailDto(string Recipient, string Subject, string Body);
