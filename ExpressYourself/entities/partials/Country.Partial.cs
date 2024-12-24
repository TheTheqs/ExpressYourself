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

    //Comparable function (Equals Override)
       public override bool Equals(object? obj)
    {
        if (obj is not Country other)
        return false;

        return (this.Name == other.Name && 
                this.TwoLetterCode == other.TwoLetterCode && 
                this.ThreeLetterCode == other.ThreeLetterCode);
    }
    //HashCode override, needed due to warning when you override Equals but do not override HashCode
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, TwoLetterCode, ThreeLetterCode);
    }
};