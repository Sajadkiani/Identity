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
    public class IdentityInternalException : Exception
    {
        public AppMessage AppMessage { get; }

        public IdentityInternalException(
            AppMessage appMessage
            ) : base(appMessage.message)
        {
            AppMessage = appMessage;
        }
    }
}