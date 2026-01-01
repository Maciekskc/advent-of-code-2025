using Helpers;

var path = "input.txt";
var lines = FileHelper.GetFileStream(path).GetStringListFromFile();
var solver = new ReactorSolver(lines);
Console.WriteLine($"The answer for given input is {solver.SolvePuzzlesPart2()}");

internal class ReactorSolver
{
    const string StartDevice = "you";
    const string ServerDevice = "svr";
    const string TargetDevice = "out";
    private readonly Dictionary<string,string[]> _deviceOutputs = [];
    private readonly Dictionary<(string name, bool seenDac, bool seenFft),long> graph = [];

    public ReactorSolver(string[] lines)
    {
        foreach (var line in lines)
        {
            var parts = line.Split(":", StringSplitOptions.TrimEntries);
            var key = parts[0].Trim();
            var outputs = parts[1].Split(" ", StringSplitOptions.TrimEntries);
            _deviceOutputs.TryAdd(key, outputs);
        }
        _deviceOutputs.TryAdd("out", []);
    }

    public int SolvePuzzlesPart1() => EnrollSequence([StartDevice]).Count;
    public long SolvePuzzlesPart2() => CountPaths(ServerDevice,false, false, []);

    private List<string[]> EnrollSequence(string[] sequence)
    {
        var nodeName = sequence[^1];
        if(!_deviceOutputs.TryGetValue(nodeName, out var nextElements))
           throw new ArgumentException("Cannot find next elements for the given node");
        List<string[]> result = [];
        foreach (var element in nextElements)
        {

            var newSequence = sequence.Append(element).ToArray();
            if(element == TargetDevice)
            {
                result.Add(newSequence);
                continue;
            }

            if (!sequence.Contains(element))
            {
                result.AddRange(EnrollSequence(newSequence));
            }
            
        }
        return result;
    }
    
    long CountPaths(
        string node,
        bool seenDac ,
        bool seenFft ,
        HashSet<string> visited )
    {
        if (node == "dac") seenDac = true;
        if (node == "fft") seenFft = true;

        if (node == "out")
            return (seenDac && seenFft) ? 1 : 0;

        var key = (node, seenDac, seenFft);
        if (graph.TryGetValue(key, out var cached))
            return cached;

        visited.Add(node);

        long total = 0;

        foreach (var next in _deviceOutputs[node])
        {
            if (!visited.Contains(next))
            {
                total += CountPaths(next, seenDac, seenFft, visited);
            }
        }

        visited.Remove(node);

        graph[key] = total;
        return total;
    }
}