using System.Text.RegularExpressions;

public interface ITokenizer{
    IEnumerable<string> Tokenize(string input);
}
public partial class Tokenizer : ITokenizer
{
    public IEnumerable<string> Tokenize(string input)
    {
        var regEx = MyRegex();
        var grps = regEx.Match(input).Groups;
        return grps.Values.Skip(1).Select(x=>x.Value)
               ;
    }

    [GeneratedRegex("(?<country>\\d{3,3})(?<Nr>[a-zA-Z]{10,10})",RegexOptions.Singleline|RegexOptions.Compiled|RegexOptions.IgnoreCase)]
    private static partial Regex MyRegex();
}