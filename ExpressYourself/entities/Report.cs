namespace ExpressYourself.entities;
//Manualy created class to build Report objects. Using partial word just to follow the other classes, a partial will not be needed.
public partial class Report {    
    public required string CountryName { get; set; }

    public required int AddressesCount { get; set; }

    public required DateTime LastAddressUpdate { get; set; }

    //Constructor
    public Report(String countryName, int addressesCount, DateTime lastAddressUpdate)
    {
        this.CountryName = countryName;
        this.AddressesCount = addressesCount;
        this.LastAddressUpdate = lastAddressUpdate;
    }
}