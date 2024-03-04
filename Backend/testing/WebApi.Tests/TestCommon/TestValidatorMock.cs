using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace WebApi.Tests.TestCommon;

public class TestValidatorMock<T>
{
    private readonly Mock<IValidator<T>> _moq;
    private readonly List<ValidationFailure> _errors;
    
    public TestValidatorMock()
    {
        _moq = new();
        _errors = new();

        ValidationResult validationResult = new ValidationResult(_errors);
        
        _moq.Setup(x =>
                x.Validate(It.IsAny<T>()))
            .Returns(validationResult);
    }

    public void VerifyValidate(T modelToValidate)
    {
        _moq.Verify(x=>x.Validate(modelToValidate));
    }

    public void SetupIsValid(bool isValid)
    {
        _moq.Setup(x => x.Validate(It.IsAny<T>()).IsValid)
            .Returns(isValid);
    }

    public void AddError(string propertyName, string message)
    {
        _errors.Add(new(propertyName, message));
        SetupIsValid(false);
    }

    public IValidator<T> Object => _moq.Object;
}