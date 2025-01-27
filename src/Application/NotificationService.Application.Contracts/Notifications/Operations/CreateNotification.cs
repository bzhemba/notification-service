namespace NotificationService.Application.Contracts.Notifications.Operations;

public static class CreateNotification
{
    public readonly record struct Request(
        long ApplicationId,
        string UserEmail,
        DateTime CreatedAt);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success() : Result;

        public sealed record Failed() : Result;
    }
}