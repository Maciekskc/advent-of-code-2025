using Helpers;

var path = "input.txt";
var matrix = FileHelper.GetFileStream(path).GetStringMatrixFromFile();

var solver = new TrashCompactorSolver(matrix);
Console.WriteLine($"The answer for given input is {solver.SolveEquations(true)}");

internal class TrashCompactorSolver
{
    private readonly string[] _problemsOperations;
    private readonly string[][] _worksheet;
    private readonly int[][] _problemsSorted;

    public TrashCompactorSolver(string[][] worksheet)
    {
        _worksheet = worksheet;
        PrintWorkbook();

        var dimensions = (rows: worksheet.Length, columns: worksheet[0].Length);
        _problemsSorted = new int[dimensions.columns][];
        _problemsOperations = new string[dimensions.columns];
        
        for (var i = 0; i < dimensions.columns; i++)
        {
            _problemsSorted[i] = new int[dimensions.rows -1];
            for (var j = 0; j < dimensions.rows; j++)
            {
                if (j != dimensions.rows - 1)
                {
                    _problemsSorted[i][j] = int.Parse(worksheet[j][i]);
                }
                else
                {
                    _problemsOperations[i] = worksheet[j][i];
                }
            }
        }
    }

    private void PrintWorkbook()
    {
        Console.WriteLine("Worksheet:");
        foreach (var line in _worksheet)
            Console.WriteLine(string.Join(" ", line));
    }
    
    private void PrintEquations()
    {
        Console.WriteLine("Equations:");
        for (int i = 0; i < _worksheet[0].Length; i++)
        {
            Console.Write(_problemsOperations[i]);
            foreach (var line in _problemsSorted[i])
                Console.WriteLine(string.Join(" ", line));
        }
        
    }

    public long SolveEquations(bool b)
    {
        var partialResults = new long[_problemsSorted.Length];
        for (var index = 0; index < _problemsSorted.Length; index++)
        {
            long partialSum = _problemsOperations[index] == "*" ? 1 : 0;
            foreach (var element in _problemsSorted[index])
            {
                switch (_problemsOperations[index]) 
                {
                    case "+":
                        partialSum += element;
                        break;
                    case "*":
                        partialSum *= element;
                        break;
                    
                    default:
                        throw new ArgumentException($"Unknown operation: {_problemsOperations[index]}");
                }
            }
            partialResults[index] = partialSum;
            Console.WriteLine($"{string.Join(_problemsOperations[index] , _problemsSorted[index])} = {partialSum}");
        }
        var sum = partialResults.Sum();
        Console.WriteLine($"{string.Join("+", partialResults)} = {sum}");

        return sum;
    }
}