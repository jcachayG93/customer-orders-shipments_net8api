namespace Domain.Exceptions;

public class InvalidEntityStateException : DomainException
{
    public InvalidEntityStateException(
        string errorMessage) : base(errorMessage)
    {
    }
}