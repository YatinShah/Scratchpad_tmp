namespace Digitizing.Model
{
    public class PhoneDigits
    { //Yatin: Using new type with same name, on purpose to show namespace and assembly does not matter when serializing data
        public bool? HandleSpecial { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
    }
}