namespace ExpressYourself.entities;
//Country partial class, made for the constructor. This is needed to avoid editing the EF generated class.
public partial class Ipaddress
{
    //Empty Constructor. Necessary for EF to work.
    public Ipaddress () {}
    //Basic constructor
    public Ipaddress (String ip, Country country)
    {
        this.Ip = ip;
        this.Country = country;
        this.CountryId = country.Id;
    }
    public Ipaddress(Ipaddress clone) //This clone constructor is necessary in the save IP process (DB)
    {
        this.Ip = clone.Ip;
        this.CountryId = clone.CountryId;
    }
}