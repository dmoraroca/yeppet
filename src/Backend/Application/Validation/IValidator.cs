namespace Zuppeto.Application.Validation;

public interface IValidator<in T>
{
    ValidationResult Validate(T request);
}
