using System;

namespace IdentityService.Exceptions;

public class AppMessage
{
    public readonly string message;

    public AppMessage(string message)
    {
        this.message = message;
    }
}

public static class AppMessages
{
    public static AppMessage UserNotFound = new AppMessage("کاربر یافت نشد");
}

public class IdentityException
{
    
    public class BaseIdentityInternalException : Exception
    {
        public AppMessage AppMessage { get; }

        public BaseIdentityInternalException(
            AppMessage appMessage
        ) : base(appMessage.message)
        {
            AppMessage = appMessage;
        }
    }

    public class IdentityInternalException : BaseIdentityInternalException
    {
        public IdentityInternalException(AppMessage message) : base(message)
        {}
    }

    public class IdentityNotFoundException : BaseIdentityInternalException
    {
        public IdentityNotFoundException(AppMessage message) : base(message)
        {}
    }
    
    public class IdentityUnauthorizedException : BaseIdentityInternalException
    {
        public IdentityUnauthorizedException(AppMessage message) : base(message)
        {}
    }
}