using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ContestProject
{
#if Level2
    public class Code : CodeBase
    {
        private static readonly char[] newLineSeparator = ['\r', '\n'];

        public override string Execute(string input)
        {
            var lines = input.Split(newLineSeparator, StringSplitOptions.RemoveEmptyEntries);

            int countPaths = int.Parse(lines[0]);

            var matrixes = lines[1..].Select(calc);

            var results = matrixes.Select(matrix => $"{Math.Abs(matrix.x)} {Math.Abs(matrix.y)}");

            return string.Join(Environment.NewLine, results);

            static (int x, int y) calc(string line)
            {
                (int x, int y) directions = (0, 0);
                (int x, int y) maxDirections = (0, 0);
                (int x, int y) minDirections = (0, 0);

                foreach (var direction in line)
                {
                    switch (direction)
                    {
                        case 'W':
                            directions.y++;
                            break;
                        case 'D':
                            directions.x++;
                            break;
                        case 'S':
                            directions.y--;
                            break;
                        case 'A':
                            directions.x--;
                            break;
                    }

                    if (directions.x > maxDirections.x)
                    {
                        maxDirections.x = directions.x;
                    }
                    if (directions.y > maxDirections.y)
                    {
                        maxDirections.y = directions.y;
                    }
                    if (directions.x < minDirections.x)
                    {
                        minDirections.x = directions.x;
                    }
                    if (directions.y < minDirections.y)
                    {
                        minDirections.y = directions.y;
                    }
                }

                return (Math.Abs(maxDirections.x) + Math.Abs(minDirections.x) + 1, Math.Abs(maxDirections.y) + Math.Abs(minDirections.y) + 1);
            }
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
