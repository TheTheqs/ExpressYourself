using System;
using System.Collections.Generic;

namespace ExpressYourself.entities;
//Country partial class, made for the constructor. This is needed to avoid editing the EF generated class.
public partial class Ipaddress
{
    //Constructor
    public Ipaddress (String ip, Country country)
    {
        this.Ip = ip;
        this.Country = country;
        this.CountryId = country.Id;
        this.CreatedAt = DateTime.UtcNow;
        this.UpdatedAt = DateTime.UtcNow;
    }
}