namespace ExpressYourself.Domain.Exceptions;

/// <summary>
/// Represents domain validation errors.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}