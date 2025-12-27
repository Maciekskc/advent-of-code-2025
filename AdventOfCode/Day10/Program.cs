using Helpers;

var path = "input-test.txt";
var lines = FileHelper.GetFileStream(path).GetStringListFromFile();

foreach (var line in lines)
    Console.WriteLine(line);

var solver = new FactorySolver(lines);
// Console.WriteLine($"The answer for given input is {solver.}");

internal class FactorySolver
{
    private List<Machine> _machines = [];
    public FactorySolver(string[] lines)
    {
        foreach (var line in lines)
        {
            _machines.Add(new Machine(line));
        }
    }

    private class Machine
    {
        private int _range = -1;
        private readonly string _indicatorTargetSequence;
        private string _currentSequence;
        private int[][] indicatorTriggeringLights;

        public Machine(string machineDescription)
        {
            string[] inputs = machineDescription.Split(' ');
            _indicatorTargetSequence = inputs[0][1..^1];
            _currentSequence = new string('.', _indicatorTargetSequence.Length);

            indicatorTriggeringLights = new int[_indicatorTargetSequence.Length][];
            int iterator = 0;
            foreach (var input in inputs[1..^1])
            {
                var digits = input[1..^1].Split(',');
                indicatorTriggeringLights[iterator] = new int[digits.Length];
                for (var index = 0; index < digits.Length; index++)
                {
                    indicatorTriggeringLights[iterator][index] = int.Parse(digits[index]);
                }

                iterator++;
            }
        }
    }
}