namespace NotificationService.Application.Contracts.Notifications.Operations;

public static class SendNotification
{
    public readonly record struct Request(string UserEmail);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success() : Result;

        public sealed record MissingUser() : Result;

        public sealed record NothingToSend() : Result;

        public sealed record NotificationAlreadySent() : Result;
    }
}