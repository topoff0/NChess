namespace Account.Application.Features.Auth.DTOs.Requests;

public record SendEmailRequest(string ToEmail,
                               string Subject,
                               string Body);
