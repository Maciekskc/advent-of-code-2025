using Helpers;

var path = "input-test.txt";
var matrix = FileHelper.GetFileStream(path).GetStringMatrixFromFile();

var solver = new TrashCompactorSolver(matrix);
solver.PrintMagazine();

internal class TrashCompactorSolver(string[][] worksheet)
{
    public void PrintMagazine()
    {
        Console.WriteLine("Worksheet:");
        foreach (var line in worksheet)
            Console.WriteLine(string.Join(" ", line));
    }
}