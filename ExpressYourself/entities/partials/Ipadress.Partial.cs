namespace ExpressYourself.entities;
//Country partial class, made for the constructor. This is needed to avoid editing the EF generated class.
public partial class Ipaddress
{
    //Empty Constructor. Necessary for EF to work.
    public Ipaddress () {}
    //Basic constructor. Used only in the class itself.
    private Ipaddress (String ip, Country country)
    {
        this.Ip = ip;
        this.Country = country;
        this.CountryId = country.Id;
        this.CreatedAt = DateTime.UtcNow;
        this.UpdatedAt = DateTime.UtcNow;
    }

    //Official constructor, used in the project. Needed to make sure Country.id is always valid
    //To create a new IP entity is a delegation only for the EA class
    public static async Task<Ipaddress> NewIpAsync(String ip, Country country)
    {
        if(country.Id == 0)
        {
            country.Id = await DB.GetCountryId(country);
        };

        return new Ipaddress(ip, country);
    }
}