using Google.OrTools.LinearSolver;
using Helpers;

var path = "input.txt";
var lines = FileHelper.GetFileStream(path).GetStringListFromFile();

var solver = new FactorySolver(lines);
Console.WriteLine($"The answer for given input is {solver.SolvePuzzles(true)}");

internal class FactorySolver
{
    private List<Machine> _machines = [];

    public FactorySolver(string[] lines)
    {
        foreach (var line in lines)
            _machines.Add(new Machine(line));
    }

    public long SolvePuzzles(bool joltageMode = false)
    {
        long sum = 0;
        foreach (var machine in _machines)
        {
            Console.WriteLine(machine);
            var requiredClicks = joltageMode
                ? CalculateClicksForJoltage(machine)
                : CalculateClicksForLightsIndicator(machine);
            Console.WriteLine($"To get to the initiation state you need {requiredClicks} clicks");
            sum += requiredClicks;
        }

        return sum;
    }

    private int CalculateClicksForLightsIndicator(Machine machine)
    {
        var combinationLength = 1;
        do
        {
            var combinations = CombinationsWithRepetition(machine.Buttons.Length, combinationLength);
            foreach (var combination in combinations)
            {
                var currentSequence = machine.GetIndicatorLightStateFromButtonsSequenceClick(combination);
                if (currentSequence == machine.IndicatorTargetSequence)
                    return combinationLength;
            }

            combinationLength++;

        } while (true); //it has to find at some point
    }

    private int CalculateClicksForJoltage(Machine machine)
    {
        var solver = Solver.CreateSolver("SCIP");
        var maxClicks = machine.JoltageSequence.Sum();
        var variables = new List<Variable>();
        
        for (var i = 0; i < machine.Buttons.Length; i++) variables.Add(solver.MakeIntVar(0,maxClicks, $"button_{i}"));
        
        for (var i = 0; i < machine.JoltageSequence.Length; i++)
        {
            var constraint = solver.MakeConstraint(machine.JoltageSequence[i], machine.JoltageSequence[i], $"joltage_position_{i}");
            for (var buttonIndex = 0; buttonIndex < machine.Buttons.Length; buttonIndex++)
                if ( machine.Buttons[buttonIndex].Any(p => p == i))
                    constraint.SetCoefficient(variables[buttonIndex], 1);
        }
        var objective = solver.Objective();
        foreach (var variable in variables) objective.SetCoefficient(variable, 1);
        objective.SetMinimization();
        solver.Solve();
        
        return (int)variables.Select(v=> v.SolutionValue()).Sum();
    }

    static IEnumerable<int[]> CombinationsWithRepetition(int n, int k)
    {
        if (k <= 0)
            yield break;

        var maxValue = n - 1;

        var combo = new int[k];
        while (true)
        {
            yield return (int[])combo.Clone();

            var i = k - 1;

            while (i >= 0 && combo[i] == maxValue)
                i--;

            if (i < 0)
                yield break;

            combo[i]++;

            for (var j = i + 1; j < k; j++)
                combo[j] = combo[i];
        }
    }

    private class Machine
    {
        public readonly string IndicatorTargetSequence;
        public readonly int[] JoltageSequence;
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

            var joltagaArray = inputs[^1][1..^1].Split(',');
            JoltageSequence = new int[joltagaArray.Length];
            for (var index = 0; index < joltagaArray.Length; index++)
                JoltageSequence[index] = int.Parse(joltagaArray[index]);
        }

        public override string ToString() =>
            $"Machine with initiating sequence [{IndicatorTargetSequence}]. Required Joltage [{string.Join(',',JoltageSequence)}]. Buttons [{string.Join(",", Buttons.Select(button => "(" + string.Join(",", button) + ")"))}]";

        public string GetIndicatorLightStateFromButtonsSequenceClick(int[] buttonsSequence)
        {
            var lights = new char[IndicatorTargetSequence.Length];
            Array.Fill(lights, '.');

            foreach (var buttonIndex in buttonsSequence)
            {
                PressButton(buttonIndex, lights);
            }

            return new string(lights);
        }
        
        public int[] GetJoltageStateFromButtonsSequenceClick(int[] buttonsSequence)
        {
            var joltageState = new int[JoltageSequence.Length];

            foreach (var buttonIndex in buttonsSequence)
            {
                var positions = Buttons[buttonIndex];
                foreach (var position in positions)
                {
                    joltageState[position]++;
                }
            }

            return joltageState;
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