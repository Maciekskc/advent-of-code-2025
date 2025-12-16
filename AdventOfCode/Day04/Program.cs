using Helpers;

var path = "input.txt";

using var filestream = FileHelper.GetFileStream(path);
var magazine = FileHelper.GetCharMatrixFromFile(filestream);
filestream.Close();

var answer = new PrintingDepartmentSolver(magazine).FindAccessibleRolls();
Console.WriteLine($"The answer for given input is {answer}");

internal class PrintingDepartmentSolver(char[][] magazine, bool isDebugEnabled = false)
{
    const char RollChar = '@';
    const char AccesibleRollChar = 'x';
    const char EmptySlot = '.';
    const int MaxRollNeighbours = 4;

    internal int FindAccessibleRolls()
    {
        for (var i = 0; i < magazine.Length; i++)
        for (var j = 0; j < magazine[i].Length; j++)
        {
            if (magazine[i][j] == EmptySlot)
                continue;

            if (IfAccessible(i, j))
                magazine[i][j] = AccesibleRollChar;
        }

        PrintMagazine();
        return CountAccessibleRolls();
    }

    private int CountAccessibleRolls() => magazine.SelectMany(row => row).Count(column => column == AccesibleRollChar);

    private bool IfAccessible(int x, int y)
    {
        var counter = 0;
        if (isDebugEnabled)
            Console.WriteLine($"Check for Magazine[{x}][{y}] is a roll . Incrementing.");

        for (var i = x - 1; i <= x + 1; i++)
        {
            if (i < 0 || i >= magazine.Length)
                continue;
            for (var j = y - 1; j <= y + 1; j++)
            {
                if (i == x && j == y)
                    continue;

                if (j < 0 || j >= magazine[i].Length)
                    continue;
                if (magazine[i][j] == RollChar || magazine[i][j] == AccesibleRollChar)
                {
                    if (isDebugEnabled)
                        Console.WriteLine($"Magazine[{i}][{j}] is a roll . Incrementing.");
                    counter++;
                }
            }
        }

        if (isDebugEnabled)
            Console.WriteLine($"Magazine[{x}][{y}] have {counter} roll neighbours.");

        return counter < MaxRollNeighbours;
    }

    private void PrintMagazine()
    {
        foreach (var line in magazine)
        {
            foreach (var character in line)
            {
                Console.Write(character);
            }

            Console.WriteLine();
        }
    }
}