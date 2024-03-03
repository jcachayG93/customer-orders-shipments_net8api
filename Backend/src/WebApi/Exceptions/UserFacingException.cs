namespace WebApi.Exceptions;

public abstract class UserFacingException : Exception
{
    protected UserFacingException(string errorMessage) 
        : base(errorMessage)
    {
        
    }
}