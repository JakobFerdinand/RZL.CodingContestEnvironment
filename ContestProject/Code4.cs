using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace ContestProject
{
#if Level4
    public class Code : CodeBase
    {
        private static readonly char[] newLineSeparator = ['\r', '\n'];

        public override string Execute(string input)
        {
            var lines = input.Split(newLineSeparator, StringSplitOptions.RemoveEmptyEntries);

            int countLawns = int.Parse(lines[0]);
            lines = lines.Skip(1).ToArray();

            List<string> results = [];

            for (int count = 0; count < countLawns; count++)
            {
                var firstLine = lines[0].Split(' ').Select(int.Parse).ToArray();
                (int x, int y) lawnSize = (firstLine[0], firstLine[1]);
                (int x, int y) treeLocation = getTreeLocation(lines[1..(lawnSize.y + 1)]);

                var path = inventPath(lawnSize, treeLocation);
                results.Add(path);

                if (isValid(lawnSize, treeLocation, path) is false)
                    throw new Exception();

                lines = lines.Skip(lawnSize.y + 2).ToArray();
            }

            return string.Join(Environment.NewLine, results);

            static string inventPath((int x, int y) lawnSize, (int x, int y) treeLocation)
            {
                Action[,] field = new Action[lawnSize.x, lawnSize.y];
                field[treeLocation.x, treeLocation.y] = Action.Tree;

                (int x, int y) currentPosition = (0, 0);

                StringBuilder path = new();

                // move vom left to right and from right to left
                while (true)
                {
                    field[currentPosition.x, currentPosition.y] = Action.Touched;

                    if (currentPosition.x < lawnSize.x - 1 && field[currentPosition.x + 1, currentPosition.y] is Action.Untouched)
                    {
                        path.Append('D');
                        currentPosition.x++;
                        continue;
                    }

                    if (currentPosition.x > 0 
                        && field[currentPosition.x - 1, currentPosition.y] is Action.Untouched
                        && (field[currentPosition.x-1, currentPosition.y-1] is Action.Touched || currentPosition.y + 1 == lawnSize.y))
                    {
                        path.Append('A');
                        currentPosition.x--;
                        continue;
                    }

                    if (currentPosition.y < lawnSize.y - 1 && field[currentPosition.x, currentPosition.y + 1] is Action.Untouched)
                    {
                        path.Append('W');
                        currentPosition.y++;
                        continue;
                    }

                    if (currentPosition.y > 0 && field[currentPosition.x, currentPosition.y - 1] is Action.Untouched)
                    {
                        path.Append('S');
                        currentPosition.y--;
                        continue;
                    }

                    if (canMove(currentPosition, field) is false)
                        break;
                }

                return path.ToString();
            }

            static bool canMove((int x, int y) currentPosition, Action[,] field)
            {
                // check if any field around the current position is untouched
                if (currentPosition.x < field.GetLength(0) - 1 && field[currentPosition.x + 1, currentPosition.y] is Action.Untouched)
                    return true;
                if (currentPosition.x > 0 && field[currentPosition.x - 1, currentPosition.y] is Action.Untouched)
                    return true;
                if (currentPosition.y < field.GetLength(1) - 1 && field[currentPosition.x, currentPosition.y + 1] is Action.Untouched)
                    return true;
                if (currentPosition.y > 0 && field[currentPosition.x, currentPosition.y - 1] is Action.Untouched)
                    return true;
                return false;
            }



            static bool isValid((int x, int y) lawnSize, (int x, int y) treeLocation, string path)
            {
                (int x, int y) currentPosition = (0, 0);
                (int x, int y) maxDirections = (0, 0);
                (int x, int y) minDirections = (0, 0);
                (int x, int y) startingPosition = currentPosition;

                List<(int x, int y)> usedFiedls = [];
                usedFiedls.Add(currentPosition);

                foreach (var direction in path)
                {
                    switch (direction)
                    {
                        case 'W':
                            currentPosition.y++;
                            break;
                        case 'D':
                            currentPosition.x++;
                            break;
                        case 'S':
                            currentPosition.y--;
                            break;
                        case 'A':
                            currentPosition.x--;
                            break;
                    }

                    if (usedFiedls.Contains(currentPosition))
                        return false;

                    usedFiedls.Add(currentPosition);


                    if (currentPosition.x > maxDirections.x)
                        maxDirections.x = currentPosition.x;
                    if (currentPosition.y > maxDirections.y)
                        maxDirections.y = currentPosition.y;
                    if (currentPosition.x < minDirections.x)
                        minDirections.x = currentPosition.x;
                    if (currentPosition.y < minDirections.y)
                        minDirections.y = currentPosition.y;
                }

                (int x, int y) calculatedLawn = (Math.Abs(maxDirections.x) + Math.Abs(minDirections.x) + 1, Math.Abs(maxDirections.y) + Math.Abs(minDirections.y) + 1);
                if (calculatedLawn != lawnSize)
                    return false;

                var offset = calculateOffset(usedFiedls.ToArray(), lawnSize.x, lawnSize.y);
                var offsetUsedFields = usedFiedls.Select(f => (f.x + offset.x, f.y + offset.y)).ToArray();

                if (offsetUsedFields.Any(t => t == treeLocation))
                    return false;

                return allWhereHit(lawnSize, offsetUsedFields, treeLocation);
            }
            static (int x, int y) calculateOffset((int x, int y)[] points, int gridWidth, int gridHeight)
            {
                int minX = int.MaxValue;
                int minY = int.MaxValue;
                int maxX = int.MinValue;
                int maxY = int.MinValue;

                // Find min and max points
                foreach (var point in points)
                {
                    minX = Math.Min(minX, point.x);
                    minY = Math.Min(minY, point.y);
                    maxX = Math.Max(maxX, point.x);
                    maxY = Math.Max(maxY, point.y);
                }

                // Calculate necessary offsets
                int offsetX = 0;
                int offsetY = 0;
                if (minX < 0) offsetX = -minX;
                if (minY < 0) offsetY = -minY;
                if (maxX + offsetX >= gridWidth) offsetX -= maxX + offsetX - gridWidth + 1;
                if (maxY + offsetY >= gridHeight) offsetY -= maxY + offsetY - gridHeight + 1;

                return (offsetX, offsetY);
            }
            static (int x, int y) getTreeLocation(string[] lawn)
            {
                lawn = lawn.Reverse().ToArray();

                for (int x = 0; x < lawn[0].Length; x++)
                {
                    for (int y = 0; y < lawn.Length; y++)
                    {
                        if (lawn[y][x] == 'X')
                        {
                            return (x, y);
                        }
                    }
                }
                return (-1, -1);
            }
            static bool allWhereHit((int x, int y) lawnSize, (int x, int y)[] usedFields, (int x, int y) tree)
            {
                HashSet<(int, int)> allPoints = [];

                // Create a set of all points in the grid except the exception
                for (int x = 0; x < lawnSize.x; x++)
                    for (int y = 0; y < lawnSize.y; y++)
                        if (x != tree.x || y != tree.y)
                            allPoints.Add((x, y));

                foreach (var hit in usedFields)
                    allPoints.Remove(hit);

                return allPoints.Count is 0;
            }

        }

        private enum Action
        {
            Untouched = 0,
            Tree,
            Touched
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
