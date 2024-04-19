using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContestProject
{
#if Level1
    public class Code : CodeBase
    {
        private static readonly char[] newLineSeparator = ['\r', '\n'];

        public override string Execute(string input)
        {
            var lines = input.Split(newLineSeparator, StringSplitOptions.RemoveEmptyEntries);

            int countPaths = int.Parse(lines[0]);

            var results = lines[1..]
                .Select(line =>
                {
                    var groupcount = line.GroupBy(d => d).ToDictionary(g => g.Key, g => g.Count());
                    groupcount = ensureAllDirections(groupcount);
                    groupcount = groupcount.OrderBy(kvp => kvp.Key, new DirectionsComparer()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    return string.Join(' ', groupcount.Select(g => g.Value));
                });

            return string.Join(Environment.NewLine, results);
        }

        private Dictionary<char, int> ensureAllDirections(Dictionary<char, int> groupcount)
        {
            foreach (var direction in new[] { 'W', 'D', 'S', 'A' })
            {
                if (!groupcount.ContainsKey(direction))
                {
                    groupcount[direction] = 0;
                }
            }

            return groupcount;
        }
    }

    public class DirectionsComparer : IComparer<char>
    {
        private readonly Dictionary<char, int> order = new Dictionary<char, int>
        {
            {'W', 0},
            {'D', 1},
            {'S', 2},
            {'A', 3} 
        };

        public int Compare(char x, char y)
        {
            // Check if both characters are in the custom sort order
            if (order.ContainsKey(x) && order.ContainsKey(y))
            {
                return order[x].CompareTo(order[y]);
            }

            // If one of the characters is not in the order, sort it alphabetically
            if (order.ContainsKey(x))
            {
                return -1;  // x is in order, y is not, x should come first
            }
            if (order.ContainsKey(y))
            {
                return 1;   // y is in order, x is not, y should come first
            }

            // If neither character is in the order, sort them alphabetically
            return x.CompareTo(y);
        }
    }
#endif
}
