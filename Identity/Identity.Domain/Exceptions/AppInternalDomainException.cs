namespace Identity.Domain.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
public class AppInternalDomainException : AppBaseDomainException
{
    public AppInternalDomainException(string message = null, Exception innerException = null) : base(message,
        innerException)
    {
    }
}

