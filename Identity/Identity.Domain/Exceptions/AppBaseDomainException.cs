namespace Identity.Domain.Exceptions;

public class AppBaseDomainException : Exception
{
    public AppBaseDomainException(string message = null, Exception innerException = null) : base(message, innerException)
    {}
}

public class AppDomainMessage
{
    public readonly string message;

    public AppDomainMessage(string message)
    {
        this.message = message;
    }
}

public static class AppDomainMessages
{
    public static string InvalidEmail = "ایمیل نامعتبر می باشد";
    public static string InvalidUserName = "نام کاربری تکراری وارد شد";
    public static string NotFound = "notFound";
}