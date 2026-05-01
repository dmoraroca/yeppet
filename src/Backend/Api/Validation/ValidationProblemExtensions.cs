using Microsoft.AspNetCore.Http.HttpResults;
using Zuppeto.Application.Validation;

namespace Zuppeto.Api.Validation;

internal static class ValidationProblemExtensions
{
    public static ValidationProblem ToValidationProblem(this ValidationResult result)
    {
        var errors = result.Errors
            .GroupBy(error => error.Field)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Message).ToArray(),
                StringComparer.Ordinal);

        return TypedResults.ValidationProblem(errors);
    }
}
