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
    public static AppMessage Unauthenticated = new AppMessage("unauthenticated");
    public static AppMessage InternalError = new AppMessage("internalError");
    public static AppMessage Forbidden = new AppMessage("forbidden");
    public static AppMessage NotFound = new AppMessage("NotFound");
}

public class IdentityException
{
    
    public class BaseIdentityException : Exception
    {
        public AppMessage AppMessage { get; }

        public BaseIdentityException(
            AppMessage appMessage
        ) : base(appMessage.message)
        {
            AppMessage = appMessage;
        }
    }

    public class IdentityInternalException : BaseIdentityException
    {
        public IdentityInternalException(AppMessage message) : base(message)
        {}
    }

    public class IdentityNotFoundException : BaseIdentityException
    {
        public IdentityNotFoundException(AppMessage message) : base(message)
        {}
    }
    
    public class IdentityUnauthorizedException : BaseIdentityException
    {
        public IdentityUnauthorizedException() : base(AppMessages.Unauthenticated)
        {}
    }
}