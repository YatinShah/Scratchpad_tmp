namespace DigitsWorker.Models
{
    public class NewNr
    { //Yatin: Using new type with same name, on purpose to show namespace and assembly does not matter when serializing data
        public string Nr { get; set; }
        public string CountryCode { get; set; }
    }
}
