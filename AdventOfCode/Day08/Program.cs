using Helpers;

var path = "input.txt";
using var reader = FileHelper.GetFileStream(path);

var solver = new PlaygroundSolver(reader);
Console.WriteLine($"The answer for given input is {solver.ConnectAll()}");

public class PlaygroundSolver
{
    public record JunctionBox(int X, int Y, int Z)
    {
        public double CalculateDistance(JunctionBox target) => Math.Sqrt(Math.Pow((target.X - this.X), 2) +
                                                                         Math.Pow((target.Y - this.Y), 2) +
                                                                         Math.Pow((target.Z - this.Z), 2));
    }

    private List<JunctionBox> JunctionBoxes { get; } = [];

    public PlaygroundSolver(StreamReader reader)
    {
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var positions = line!.Split(',');
            JunctionBoxes.Add(
                new JunctionBox(int.Parse(positions[0]), int.Parse(positions[1]), int.Parse(positions[2])));
        }
    }

    private double[][] CalculateDistanceMatrix()
    {
        var result = new double[JunctionBoxes.Count][];
        for (var i = 0; i < JunctionBoxes.Count; i++)
        {
            result[i] = new double[JunctionBoxes.Count];
            for (var j = 0; j < JunctionBoxes.Count; j++)
            {
                if (j <= i)
                {
                    result[i][j] = -1;
                }
                else
                {
                    result[i][j] = JunctionBoxes[i].CalculateDistance(JunctionBoxes[j]);
                }
            }
        }

        return result;
    }

    public void PrintDistanceMatrix(double[][] matrix)
    {
        for (var i = 0; i < JunctionBoxes.Count; i++)
        {
            for (var j = 0; j < JunctionBoxes.Count; j++)
            {
                Console.Write($"{matrix[i][j],8:F3} ");
            }

            Console.WriteLine();
        }
    }

    private (int FirstIterator, int SecondIterator, double distance) FindMinimalDistance(double[][] matrix)
    {
        var result = (0, 1, double.MaxValue);
        for (var i = 0; i < JunctionBoxes.Count; i++)
        for (var j = i + 1; j < JunctionBoxes.Count; j++)
            if (matrix[i][j] < result.Item3 && matrix[i][j] != -1)
            {
                // Console.WriteLine($"[{result.Item1}][{result.Item2}]{result.Item3} <  [{i}][{j}]{matrix[i][j]}");                
                result = (i, j, matrix[i][j]);
            }

        return result;
    }

    public int FindCircuits(int iterations)
    {
        List<List<int>> circuits = [];
        var matrix = CalculateDistanceMatrix();
        for (int i = 0; i < iterations; i++)
        {
            var shorterDistance = FindMinimalDistance(matrix);
            // Console.WriteLine(
            //     $"Next minimal distance: [{shorterDistance.FirstIterator}][{shorterDistance.SecondIterator}]{shorterDistance.distance}");
            AdjustCircuits(circuits, shorterDistance);
            matrix = GetFixedMatrix(matrix, shorterDistance.FirstIterator, shorterDistance.SecondIterator);
            // Console.WriteLine("[" + string.Join(",",
            //     circuits.Select(inner => "[" + string.Join(",", inner) + "]")
            // ) + "]");
            // PrintDistanceMatrix(matrix);
        }
        
        return circuits.Select(circuit => circuit.Count).OrderByDescending(x => x).Take(3).Aggregate((a, b) => a * b);
    }
    
    public int ConnectAll()
    {
        List<List<int>> circuits = [];
        var matrix = CalculateDistanceMatrix();
        var iterationCounter = 0;
        (int FirstIterator, int SecondIterator, double distance) shorterDistance = (-1,-1,-1);
        while(!(circuits.Count == 1 && circuits.First().Count == JunctionBoxes.Count))
        {
            iterationCounter++;
            shorterDistance = FindMinimalDistance(matrix);
            AdjustCircuits(circuits, shorterDistance);
            matrix = GetFixedMatrix(matrix, shorterDistance.FirstIterator, shorterDistance.SecondIterator);
        }
        Console.WriteLine($"Job is finished after {iterationCounter} iterations. Last Elements are {shorterDistance.FirstIterator}({JunctionBoxes[shorterDistance.FirstIterator]}) and {shorterDistance.SecondIterator}({JunctionBoxes[shorterDistance.SecondIterator]})");
        return JunctionBoxes[shorterDistance.FirstIterator].X * JunctionBoxes[shorterDistance.SecondIterator].X;
    }

    private void AdjustCircuits(List<List<int>> circuits,
        (int FirstIterator, int SecondIterator, double distance) minimalDistance)
    {
        var circuitOfTheFirstElement = circuits.Select((value, index) => new { index, value })
            .FirstOrDefault(c => c.value.Contains(minimalDistance.FirstIterator));
        var circuitOfTheSecondElement = circuits.Select((value, index) => new { index, value })
            .FirstOrDefault(c => c.value.Contains(minimalDistance.SecondIterator));
        
        //CASE 1 - They are not in any circuit
        if (circuitOfTheFirstElement is null && circuitOfTheSecondElement is null)
            circuits.Add([
                minimalDistance.FirstIterator,
                minimalDistance.SecondIterator
            ]);

        //CASE 2 - one of them is in circuit
        if (circuitOfTheFirstElement is not null && circuitOfTheSecondElement is null)
        {
            circuits[circuitOfTheFirstElement.index].Add(minimalDistance.SecondIterator);
            circuits[circuitOfTheFirstElement.index].Sort();
        }

        if (circuitOfTheFirstElement is null && circuitOfTheSecondElement is not null)
        {
            circuits[circuitOfTheSecondElement.index].Add(minimalDistance.FirstIterator);
            circuits[circuitOfTheSecondElement.index].Sort();
        }

        //CASE 3 - both are
        if (circuitOfTheFirstElement is not null && circuitOfTheSecondElement is not null && circuitOfTheFirstElement.index !=  circuitOfTheSecondElement.index)
        {
            circuits[circuitOfTheFirstElement.index].AddRange(circuits[circuitOfTheSecondElement.index]);
            circuits[circuitOfTheFirstElement.index].Sort();
            circuits.RemoveAt(circuitOfTheSecondElement.index);
        }
    }

    private static double[][] GetFixedMatrix(double[][] matrix, int firstIterator, int secondIterator)
    {
        matrix[int.Min(firstIterator,secondIterator)][int.Max(firstIterator,secondIterator)] = -1;
        return matrix;
    }
}