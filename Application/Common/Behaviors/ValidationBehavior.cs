using Application.Abstractions.Validators;
using Domain.Exceptions;
using MediatR;

namespace Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        foreach (var validator in validators)
        {
            if (!validator.Validate(request))
                errors.AddRange(validator.GetErrors);
        }

        if (errors.Count > 0)
            throw new AppValidationException(errors);

        return await next(cancellationToken);
    }
}
