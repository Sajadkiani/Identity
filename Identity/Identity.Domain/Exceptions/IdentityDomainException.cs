namespace Identity.Domain.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
public class IdentityDomainException : AppBaseDomainException
{
    public IdentityDomainException(string message = null, Exception innerException = null) : base(message, innerException)
    {}
}

