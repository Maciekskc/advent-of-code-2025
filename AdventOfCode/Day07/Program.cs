using System.Diagnostics;
using Helpers;

var path = "input.txt";
var lines = FileHelper.GetFileStream(path).GetStringListFromFile();

var solver = new LaboratoriesSolver(lines);
Console.WriteLine($"The answer for given input is {solver.AnalyseQuantumBeamSplit()}");

internal partial class LaboratoriesSolver(string[] lines)
{
    private readonly Dictionary<(int, int), long> _nodes = new();
    private int _splitCounter;
    private long _timeLineCounter;

    public long AnalyseQuantumBeamSplit()
    {
        _timeLineCounter = 0;

        var sw = Stopwatch.StartNew();
        Console.WriteLine(
            "Quantum calculation need to consider whole multiverse. Please give it some time to finish...");

        _timeLineCounter = AnalyseEachMultiverse();

        sw.Stop();
        Console.WriteLine($"Job done. It took {sw.Elapsed.Seconds} seconds.");


        return _timeLineCounter;
    }

    private long AnalyseEachMultiverse()
    {
        var startIndex = lines[0].IndexOf('S');
        _nodes.Add((startIndex, 0), CalculateSubgraphs(startIndex, 0));

        return _nodes.GetValueOrDefault((startIndex, 0));
    }

    private long CalculateSubgraphs(int x, int y)
    {
        if (_nodes.TryGetValue((x, y), out var node)) return node;

        var i = y + 1;
        while (i < lines.Length - 1 && lines[i][x] != '^')
            i++;

        if (i == lines.Length - 1)
        {
            _nodes.TryAdd((x, i), 1);
            return 1;
        }

        long total = 0;
        if (lines[i][x] == '^')
        {
            var leftChildSubgraphs = CalculateSubgraphs(x - 1, i);
            _nodes.TryAdd((x - 1, i), leftChildSubgraphs);
            total += leftChildSubgraphs;

            var rightChildSubgraphs = CalculateSubgraphs(x + 1, i);
            total += rightChildSubgraphs;
            _nodes.TryAdd((x + 1, i), rightChildSubgraphs);
        }

        return total;
    }

    private static void PrintBeam(List<string> lines)
    {
        Console.WriteLine("BEAMS:");
        foreach (var line in lines)
            Console.WriteLine(line);
    }
}