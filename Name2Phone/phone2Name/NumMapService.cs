namespace phone2Name
{
    public enum NumSystemEnum
    {
        English=0,Greek=1
    }
    public interface INumMapService
    {
        IEnumerable<char> Map(char num); 
    }
    public class NumMapService : INumMapService
    {
        private readonly Dictionary<char, List<char>> map = new()
        {
            {'1',new List<char>{} },
            {'2',new List<char>{'a','b','c'} },
            {'3',new List<char>{'d','e','f'} },
            {'4',new List<char>{'g','h','i'} },
            {'5',new List<char>{'j','k','l'} },
            {'6',new List<char>{'m','n','o'} },
            {'7',new List<char>{'p','q','r','s'} },
            {'8',new List<char>{'t','u','v'} },
            {'9',new List<char>{'w','x','y','z'} },
            {'0',new List<char>{'+'} }
        };
        public IEnumerable<char> Map(char num)
        {
            var returnList = map[num];
            foreach (var item in returnList) yield return item;
        }
    }

    public class GreekNumMapService : INumMapService
    {
        private readonly Dictionary<char, List<char>> map = new()
        {
            {'1',new List<char>{} },
            {'2',new List<char>{'\u0391','\u0392','\u0393'} },
            {'3',new List<char>{'\u0394','\u0395','\u0396'} },
            {'4',new List<char>{'\u0397','\u0398','\u0399'} },
            {'5',new List<char>{'\u039A','\u039B','\u039C'} },
            {'6',new List<char>{'\u039D','\u039E','\u039F'} },
            {'7',new List<char>{'\u03A0','\u03A1','\u03A2','\u03A3'} },
            {'8',new List<char>{'\u03A4','\u03A5','\u03A6','\u03A7'} },
            {'9',new List<char>{'\u03A8','\u03A8','\u03A9','\u03AA'} },
            {'0',new List<char>{'+'} }
        };
        public IEnumerable<char> Map(char num)
        {
            var returnList = map[num];
            foreach (var item in returnList) yield return item;
        }
    }
}
