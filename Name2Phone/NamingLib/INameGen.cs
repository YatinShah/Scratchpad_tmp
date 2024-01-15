using Dapr.Actors;

namespace NamingLib
{
    public interface INameGen: IActor
    {
        Task<IEnumerable<string>> GenerateNamesAsync(string numbers);
    }
}
