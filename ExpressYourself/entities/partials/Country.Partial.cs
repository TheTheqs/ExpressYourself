namespace ExpressYourself.entities;
//Country partial class, made for the constructor and equals function creation. This is needed to avoid editing the EF generated class.
public partial class Country 
{
    //Empty Constructor. Necessary for EF to work.
    public Country () {}

    //Constructor
    public Country(String name, String twoLetterCode, String threeLetterCode)
    {
        this.Name = name;
        this.TwoLetterCode = twoLetterCode;
        this.ThreeLetterCode = threeLetterCode;
    }
};