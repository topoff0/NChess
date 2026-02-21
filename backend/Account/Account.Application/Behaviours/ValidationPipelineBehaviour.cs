using Account.Application.Common.Errors;
using Account.Application.Exceptions;
using FluentValidation;
using MediatR;

namespace Account.Application.Behaviours;
public sealed class ValidationPipelineBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request,
                                              RequestHandlerDelegate<TResponse> next,
                                              CancellationToken token)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, token)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        var errorsDictionary = failures
            .GroupBy(f => f.PropertyName.ToLower())
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray());

        var error = Error.Validation(errorsDictionary);

        throw new CustomValidationException(error);
    }
}
