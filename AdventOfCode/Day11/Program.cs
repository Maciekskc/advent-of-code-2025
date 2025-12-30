using Helpers;

var path = "input.txt";
var lines = FileHelper.GetFileStream(path).GetStringListFromFile();
var solver = new ReactorSolver(lines);
Console.WriteLine($"The answer for given input is {solver.SolvePuzzles()}");

internal class ReactorSolver
{
    const string StartDevice = "you";
    const string TargetDevice = "out";
    private readonly Dictionary<string,string[]> _deviceOutputs = [];
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

    public int SolvePuzzles() => EnrollSequence([StartDevice]).Count;

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
}