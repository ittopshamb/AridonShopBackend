namespace OnlineStore.Domain.Exceptions;

public class AccountNotFoundException : Exception
{
    public AccountNotFoundException(string message) : base(message)
    {
    }
}