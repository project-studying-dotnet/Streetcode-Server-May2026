using FluentValidation;
using FluentResults;
using MediatR;

namespace Streetcode.Auth.MediatR.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Count != 0)
        {
            var result = new TResponse();

            foreach (var failure in failures)
            {
                result.Reasons.Add(
                    new Error(failure.ErrorMessage)
                        .WithMetadata("PropertyName", failure.PropertyName)
                        .WithMetadata("ErrorCode", failure.ErrorCode));
            }

            return result;
        }

        return await next();
    }
}