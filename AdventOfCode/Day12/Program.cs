using System.Text.RegularExpressions;
using Helpers;

var path = "input-test.txt";
var lines = FileHelper.GetFileStream(path).GetStringListFromFile();
var solver = new ChristmasTreeFarmSolver(lines);
Console.WriteLine($"The answer for given input is {solver.SolvePuzzles()}");

internal partial class ChristmasTreeFarmSolver
{
    private const int ShapeDimension = 3;

    private List<(int filled, int gaps, string[] shape)> _presentShapes;
    private List<(int x, int y, int[] requirements)> _treeRequirements;

    public ChristmasTreeFarmSolver(string[] lines)
    {
        _presentShapes = [];
        var iterator = 0;
        while (ShapeIndexPattern().Match(lines[iterator]).Success)
        {
            if (iterator + ShapeDimension >= lines.Length)
                break;
            var filled = 0;
            for (int i = 1; i <= ShapeDimension; i++)
                filled += lines[iterator + i].Count(c => c == '#');
            (int, int, string[]) shape = (filled, ShapeDimension * ShapeDimension - filled,
                [lines[iterator + 1], lines[iterator + 2], lines[iterator + 3]]);
            _presentShapes.Add(shape);
            iterator += ShapeDimension + 2;
        }

        _treeRequirements = [];
        foreach (var requirements in lines[iterator..])
        {
            var parts = requirements.Split([":", "x", " "], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3)
                continue;
            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);
            var reqs = parts[2..].Select(int.Parse).ToArray();
            _treeRequirements.Add((x, y, reqs));
        }
    }

    public int SolvePuzzles()
    {
        var countInvalidRegions = 0;
        foreach (var requirement in _treeRequirements)
        {
            var treeArea = requirement.x * requirement.y;

            var requiredFilledArea = 0;
            for(int i =0 ; i < requirement.requirements.Length; i++)
                requiredFilledArea += requirement.requirements[i] * _presentShapes[i].filled;

            countInvalidRegions += requiredFilledArea <= treeArea ? 1 : 0;
        }
        
        return countInvalidRegions;
    }

    [GeneratedRegex("^[0-9]:")]
    private static partial Regex ShapeIndexPattern();
}