using Helpers;

var path = "input.txt";
using var filestream = FileHelper.GetFileStream(path);
var magazine = FileHelper.GetCharMatrixFromFile(filestream);
filestream.Close();

var solver = new PrintingDepartmentSolver(magazine, true);
Console.WriteLine($"The answer for given input is {solver.CalculateAccessibleRolls(true)}");

internal class PrintingDepartmentSolver(char[][] magazine, bool isDebugEnabled = false)
{
    private const char RollChar = '@';
    private const char AccessibleRollChar = 'x';
    private const char EmptySlot = '.';
    private const int MaxRollNeighbours = 4;

    public int CalculateAccessibleRolls(bool recursiveCalculation = false)
    {
        var accessibleRollsCount = 0;
        int newAccessibleRolls;

        do
        {
            DetectAccessibleRolls();
            newAccessibleRolls = CountAccessibleRolls();
            accessibleRollsCount += newAccessibleRolls;
            if (isDebugEnabled)
                Console.WriteLine(
                    $"Found {newAccessibleRolls} new accessible rolls. Total accessible rolls in this iteration.");
            if (false) // keep for special debugging needs - it prints too much so tweak the condition if needed
                PrintMagazine();
            RemoveAccessibleRolls();
        } while (recursiveCalculation && newAccessibleRolls > 0);

        return accessibleRollsCount;
    }

    private void DetectAccessibleRolls()
    {
        for (var i = 0; i < magazine.Length; i++)
        for (var j = 0; j < magazine[i].Length; j++)
        {
            if (magazine[i][j] == EmptySlot)
                continue;

            if (IfAccessible(i, j))
                magazine[i][j] = AccessibleRollChar;
        }
    }

    private int CountAccessibleRolls()
    {
        return magazine.SelectMany(row => row).Count(column => column == AccessibleRollChar);
    }

    private void RemoveAccessibleRolls()
    {
        foreach (var t in magazine)
            for (var j = 0; j < t.Length; j++)
                if (t[j] == AccessibleRollChar)
                    t[j] = EmptySlot;
    }

    private bool IfAccessible(int x, int y)
    {
        var counter = 0;

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
                if (magazine[i][j] != RollChar && magazine[i][j] != AccessibleRollChar) continue;

                counter++;

                if (counter < MaxRollNeighbours) continue;
                break;
            }
        }

        return counter < MaxRollNeighbours;
    }

    private void PrintMagazine()
    {
        Console.WriteLine("Current magazine state:");
        foreach (var line in magazine)
        {
            foreach (var character in line) Console.Write(character);

            Console.WriteLine();
        }
    }
}