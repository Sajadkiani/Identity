namespace Identity.Domain.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
public class OrderingDomainException : AppBaseDomainException
{
    public OrderingDomainException(string message = null, Exception innerException = null) : base(message, innerException)
    {}
}

