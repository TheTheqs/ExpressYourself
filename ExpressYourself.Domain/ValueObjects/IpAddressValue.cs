using System.Net;
using ExpressYourself.Domain.Enums;
using ExpressYourself.Domain.Exceptions;

namespace ExpressYourself.Domain.ValueObjects;

/// <summary>
/// Represents an IP address as a value object.
/// Supports IPv4 and IPv6 validation and classification.
/// </summary>
public sealed class IpAddressValue : IEquatable<IpAddressValue>
{
    /// <summary>
    /// Gets the normalized IP address value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the detected IP version.
    /// </summary>
    public IpAddressVersion Version { get; }

    private IpAddressValue(string value, IpAddressVersion version)
    {
        Value = value;
        Version = version;
    }

    /// <summary>
    /// Creates a new <see cref="IpAddressValue"/> from the provided string.
    /// </summary>
    /// <param name="value">The raw IP address string.</param>
    /// <returns>A validated and normalized IP value object.</returns>
    /// <exception cref="DomainException">Thrown when the IP is invalid.</exception>
    public static IpAddressValue Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("IP address cannot be null, empty or whitespace.");
        }

        var trimmedValue = value.Trim();

        if (!IPAddress.TryParse(trimmedValue, out var parsedIp))
        {
            throw new DomainException("Invalid IP address format.");
        }

        var version = parsedIp.AddressFamily switch
        {
            System.Net.Sockets.AddressFamily.InterNetwork => IpAddressVersion.IPv4,
            System.Net.Sockets.AddressFamily.InterNetworkV6 => IpAddressVersion.IPv6,
            _ => throw new DomainException("Unsupported IP address version.")
        };

        var normalizedValue = parsedIp.ToString();

        return new IpAddressValue(normalizedValue, version);
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
        => obj is IpAddressValue other && Equals(other);

    public bool Equals(IpAddressValue? other)
        => other is not null &&
           Value == other.Value &&
           Version == other.Version;

    public override int GetHashCode()
        => HashCode.Combine(Value, Version);

    public static bool operator ==(IpAddressValue? left, IpAddressValue? right)
        => EqualityComparer<IpAddressValue>.Default.Equals(left, right);

    public static bool operator !=(IpAddressValue? left, IpAddressValue? right)
        => !(left == right);
}