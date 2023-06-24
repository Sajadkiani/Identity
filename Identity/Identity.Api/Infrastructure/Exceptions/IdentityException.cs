using System;

namespace Identity.Api.Infrastructure.Exceptions;

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
    public static AppMessage NotFound = new AppMessage("notFound");
}

public class ApplicationException
{
    
    public class BaseException : Exception
    {
        public AppMessage AppMessage { get; }

        public BaseException(
            AppMessage appMessage
        ) : base(appMessage.message)
        {
            AppMessage = appMessage;
        }
    }

    public class Internal : BaseException
    {
        public Internal(AppMessage message) : base(message)
        {}
    }

    public class NotFound : BaseException
    {
        public NotFound(AppMessage message) : base(message)
        {}
    }
    
    public class Unauthorized : BaseException
    {
        public Unauthorized() : base(AppMessages.Unauthenticated)
        {}
    }
}