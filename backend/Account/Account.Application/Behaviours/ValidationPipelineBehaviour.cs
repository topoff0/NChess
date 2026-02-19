using Account.Application.Common.Errors;
using Account.Application.Common.Results;
using FluentValidation;
using MediatR;

namespace Account.Application.Behaviours;

public sealed class ValidationPipelineBehaviour<TRequest, TValue>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, ResultT<TValue>>
    where TRequest : IRequest<ResultT<TValue>>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<ResultT<TValue>> Handle(TRequest request,
                                              RequestHandlerDelegate<ResultT<TValue>> next,
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
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray());

        var error = Error.Validation(ErrorCodes.GeneralValidation, errorsDictionary);

        return error;
    }
}
