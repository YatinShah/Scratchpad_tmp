using System.Text;
using DigitsWorker.Models;

namespace DigitsWorker;
public interface IDigitService
{
    Task<string> ProcessDigitsAsync(PhoneDigits input);

}
public class DigitService : IDigitService
{
    public async Task<string> ProcessDigitsAsync(PhoneDigits input)
    {
        var result = await Task.Run(() =>
        {
            var includeSpecial = input.HandleSpecial ?? false;
            char[] inputChars = input.Name.ToUpper().ToCharArray();
            byte[] digits = new byte[inputChars.Length];
            for (int i = 0; i < inputChars.Length; i++)
            {
                char c = inputChars[i];
                digits[i] = (byte)Map(c, includeSpecial);
            }
            return Encoding.ASCII.GetString(digits);
        });
        return result;
    }
    private static char Map(char input, bool includeSpecial)
    {
        return input switch
        {
            'A' or 'B' or 'C' => '2',
            'D' or 'E' or 'F' => '3',
            'G' or 'H' or 'I' => '4',
            'J' or 'K' or 'L' => '5',
            'M' or 'N' or 'O' => '6',
            'P' or 'Q' or 'R' or 'S' => '7',
            'T' or 'U' or 'V' => '8',
            'W' or 'X' or 'Y' or 'Z' => '9',
            '*' => includeSpecial ? '1' : '0',
            _ => '0',
        };
    }
}
