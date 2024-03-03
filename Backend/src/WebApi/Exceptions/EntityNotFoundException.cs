namespace WebApi.Exceptions;

public class EntityNotFoundException 
: UserFacingException
{
    public EntityNotFoundException(string errorMessage) 
        : base(errorMessage)
    {
    }
}