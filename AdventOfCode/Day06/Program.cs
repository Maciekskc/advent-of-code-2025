using System.Text;
using Helpers;

var path = "input.txt";
var matrix = FileHelper.GetFileStream(path).GetStringListFromFile();

var solver = new TrashCompactorSolver(matrix);
Console.WriteLine($"The answer for given input is {solver.SolveEquations()}");

internal class TrashCompactorSolver
{
    private readonly int[][] _problemsNumbers;
    private readonly string[] _problemsOperations;
    private readonly string[][] _worksheet;

    public TrashCompactorSolver(string[] worksheetLines)
    {
        _worksheet = SplitWorkbookRespectingNumbers(worksheetLines);
        PrintWorkbook();

        _problemsOperations = worksheetLines[^1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        _problemsNumbers = ReadNumbersRightToLeft(_worksheet);

        // PrintEquations();
    }

    private static int[][] ReadNumbersRightToLeft(string[][] worksheet)
    {
        var numberMatrixDimensions = (rows: worksheet.Length - 1, columns: worksheet[0].Length);
        var numbers = new int[numberMatrixDimensions.columns][];

        for (var column = 0; column < numberMatrixDimensions.columns; column++)
        {
            numbers[column] = new int[worksheet[0][column].Length];

            for (var digit = 0; digit < worksheet[0][column].Length; digit++)
            {
                var builder = new StringBuilder(worksheet[0][column].Length);
                for (var row = 0; row < numberMatrixDimensions.rows; row++)
                    builder.Append(worksheet[row][column][digit]);

                numbers[column][digit] = int.Parse(builder.ToString());
            }
        }

        return numbers;
    }

    private static string[][] SplitWorkbookRespectingNumbers(string[] worksheetLines)
    {
        var fixedWorksheet = new string[worksheetLines.Length][];
        string[] worksheetLinesFixed = new string[worksheetLines.Length];

        for (var i = 0; i < worksheetLines.Length; i++)
        {
            if (i != worksheetLines.Length - 1)
            {
                var builder = new StringBuilder(worksheetLines[i].Length);
                for (var j = 0; j < worksheetLines[i].Length; j++)
                    if (worksheetLines[i][j] == ' ' && IsSeparatorPosition(worksheetLines, j))
                        builder.Append(':');
                    else
                        builder.Append(worksheetLines[i][j]);

                worksheetLinesFixed[i] = builder.ToString();
            }
            else
            {
                worksheetLinesFixed[i] = worksheetLines[i];
            }

            fixedWorksheet[i] = worksheetLinesFixed[i].Split(':', StringSplitOptions.RemoveEmptyEntries);
        }

        return fixedWorksheet;
    }

    private static bool IsSeparatorPosition(string[] worksheetLines, int positionInLine)
    {
        var numberOfRealCharacters = 0;
        for (var i = 0; i < worksheetLines.Length - 1; i++)
            if (worksheetLines[i][positionInLine] != ' ')
                numberOfRealCharacters++;

        return numberOfRealCharacters == 0;
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
        for (var i = 0; i < _worksheet[0].Length; i++)
        {
            Console.Write(_problemsOperations[i]);
            foreach (var line in _problemsNumbers[i])
                Console.WriteLine(string.Join(' ', line));
        }
    }

    public long SolveEquations()
    {
        var partialResults = new long[_problemsNumbers.Length];
        for (var index = 0; index < _problemsNumbers.Length; index++)
        {
            long partialSum = _problemsOperations[index] == "*" ? 1 : 0;
            foreach (var element in _problemsNumbers[index])
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

            partialResults[index] = partialSum;
            Console.WriteLine($"{string.Join(_problemsOperations[index], _problemsNumbers[index])} = {partialSum}");
        }

        var sum = partialResults.Sum();
        Console.WriteLine($"{string.Join("+", partialResults)} = {sum}");

        return sum;
    }
}