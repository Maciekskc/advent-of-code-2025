using Helpers;

var path = "input-test.txt";
using var reader = FileHelper.GetFileStream(path);

var solver = new PlaygroundSolver(reader);
// Console.WriteLine($"The answer for given input is {solver.AnalyseBeamSplit()}");

public class PlaygroundSolver
{
    private record JunctionBox(int X, int Y, int Z);

    private List<JunctionBox> JunctionBoxes { get; } = [];
    
    public PlaygroundSolver(StreamReader reader)
    {
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var positions = line!.Split(',');
            JunctionBoxes.Add(new JunctionBox(int.Parse(positions[0]), int.Parse(positions[1]), int.Parse(positions[2])));
        }

        foreach (var box in JunctionBoxes)
        {
            Console.WriteLine(box);
        }
    }
}

