namespace Digitizing.Services;

public interface ICountryService
{
    Task<string> Identify(string countryCode);
    Task<string> Format(string phoneNumber);
}
public class CountryService : ICountryService
{
    public Task<string> Format(string phoneNumber)
    {
        throw new NotImplementedException();
    }

    public Task<string> Identify(string countryCode)
    {
        throw new NotImplementedException();
    }

}
