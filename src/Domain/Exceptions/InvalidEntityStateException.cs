namespace Domain.Exceptions;

public class InvalidEntityStateException : Exception
{
    public InvalidEntityStateException(
        string errorMessage) : base(errorMessage)
    {
        
    }
}