namespace ExpressYourself.Domain.Entities;

/// <summary>
/// Represents a country in the system.
/// </summary>
public sealed class Country
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string TwoLetterCode { get; private set; }
    public string ThreeLetterCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<IpAddress> IpAddresses => _ipAddresses.AsReadOnly();
    private readonly List<IpAddress> _ipAddresses = new();

    /// <summary>
    /// Required by EF Core.
    /// </summary>
    private Country()
    {
        Name = string.Empty;
        TwoLetterCode = string.Empty;
        ThreeLetterCode = string.Empty;
    }

    /// <summary>
    /// Creates a new country instance.
    /// </summary>
    /// <param name="name">Country name.</param>
    /// <param name="twoLetterCode">ISO alpha-2 code.</param>
    /// <param name="threeLetterCode">ISO alpha-3 code.</param>
    public Country(string name, string twoLetterCode, string threeLetterCode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Country name cannot be empty.");

        if (string.IsNullOrWhiteSpace(twoLetterCode) || twoLetterCode.Length != 2)
            throw new ArgumentException("TwoLetterCode must have exactly 2 characters.");

        if (string.IsNullOrWhiteSpace(threeLetterCode) || threeLetterCode.Length != 3)
            throw new ArgumentException("ThreeLetterCode must have exactly 3 characters.");

        Name = name.Trim();
        TwoLetterCode = twoLetterCode.Trim().ToUpperInvariant();
        ThreeLetterCode = threeLetterCode.Trim().ToUpperInvariant();
        CreatedAt = DateTime.UtcNow;
    }
}