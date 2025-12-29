using Helpers;

var path = "input.txt";
var lines = FileHelper.GetFileStream(path).GetStringListFromFile();

var solver = new FactorySolver(lines);
Console.WriteLine($"The answer for given input is {solver.SolvePuzzles()}");

internal class FactorySolver
{
    private List<Machine> _machines = [];

    public FactorySolver(string[] lines)
    {
        foreach (var line in lines)
            _machines.Add(new Machine(line));
    }

    public long SolvePuzzles()
    {
        long sum = 0;
        foreach (var machine in _machines)
        {
            var requiredClicks = FindMinimalButtonClickCountToGetToTheLightsTargetSequence(machine);
            Console.WriteLine(machine);
            Console.WriteLine($"To get to the initiation state you need {requiredClicks} clicks");
            sum += requiredClicks;
        }

        return sum;
    }

    private int FindMinimalButtonClickCountToGetToTheLightsTargetSequence(Machine machine)
    {
        var combinationLength = 1;
        do
        {
            
            var combinations = Numerics.NET.Iterators.Combinations(machine.Buttons.Length, combinationLength);
            foreach (var combination in combinations)
            {
                var currentSequence = machine.SimulateButtonsSequenceClick(combination);
                if (currentSequence == machine.IndicatorTargetSequence)
                    return combinationLength;
            }

            combinationLength++;
            
            if(combinationLength == 20)
            {
                Console.WriteLine("We reach length 20. It is unexpectedly big. Terminating");
                return -1;
            }
        } while (true); //it has to find at some point
    }

    private class Machine
    {
        public readonly string IndicatorTargetSequence;
        public int[][] Buttons { get; }

        public Machine(string machineDescription)
        {
            var inputs = machineDescription.Split(' ');
            IndicatorTargetSequence = inputs[0][1..^1];

            Buttons = new int[inputs.Length - 2][];
            var iterator = 0;
            foreach (var input in inputs[1..^1])
            {
                var digits = input[1..^1].Split(',');
                Buttons[iterator] = new int[digits.Length];
                for (var index = 0; index < digits.Length; index++)
                {
                    Buttons[iterator][index] = int.Parse(digits[index]);
                }

                iterator++;
            }
        }

        public override string ToString() =>
            $"Machine with initiating sequence [{IndicatorTargetSequence}]. Buttons [{string.Join(",", Buttons.Select(button => "(" + string.Join(",", button) + ")"))}]";

        public string SimulateButtonsSequenceClick(int[] buttonsSequence)
        {
            var lights = new char[IndicatorTargetSequence.Length];
            Array.Fill(lights, '.');

            foreach (var buttonIndex in buttonsSequence)
            {
                PressButton(buttonIndex, lights);
            }

            return new string(lights);
        }

        private void PressButton(int buttonIndex, char[] lights)
        {
           foreach (var lightIndex in Buttons[buttonIndex])
           {
                if (lightIndex < 0 || lightIndex >= lights.Length)
                    continue;

                lights[lightIndex] = lights[lightIndex] == '.' ? '#' : '.';
           }
        }
    }
}