var path = "input.txt";
try
{
    if (!File.Exists(path)) throw new ArgumentException($"File {path} does not exist.");

    var safe = new EscalatorPowerResolver(false, 12);
    using var sr = new StreamReader(path);
    var answer = safe.GetTotalVoltagePower(sr);

    Console.WriteLine($"The answer for given input is {answer}");
}
catch (Exception e)
{
    Console.WriteLine($"Could calculate joltage. Exception details: {e}");
}

public class EscalatorPowerResolver(bool isDebugEnabled, int allowedDigits)
{
    private long _totalVoltagePower;

    public long GetTotalVoltagePower(StreamReader sr)
    {
        _totalVoltagePower = 0;
        while (!sr.EndOfStream)
        {
            var bankInput = sr.ReadLine();
            ArgumentException.ThrowIfNullOrEmpty(bankInput);
            var partialSum = CalculateHighestJoltageOfTheBank(bankInput);
            _totalVoltagePower += partialSum;
            Console.WriteLine($"Search in {bankInput} joltage {partialSum}");
        }

        return _totalVoltagePower;
    }

    private long CalculateHighestJoltageOfTheBank(string line)
    {
        var cursor = 0;
        long sum = 0;
        if (isDebugEnabled)
            Console.WriteLine($"Finding max joltage ({allowedDigits} digits) in {line}");

        for (var i = allowedDigits; i > 0; i--)
        {
            var partLength = line.Length - i - cursor + 1;
            var nextMax = line
                .Substring(cursor, partLength)
                .Max();

            if (isDebugEnabled)
                StringExtensions.JoltageSearchDebugging(line, cursor, partLength, nextMax.position + cursor);

            sum += nextMax.value;
            cursor += nextMax.position + 1;

            sum *= 10;
        }

        return sum / 10;
    }
}

internal static class StringExtensions
{
    internal static (long value, int position) Max(this string input, int cursorBaseOffset = 0)
    {
        var max = 0;
        var position = -1;
        for (var cursor = 0; cursor < input.Length; cursor++)
        {
            var digit = int.Parse(input[cursor].ToString());

            if (digit <= max) continue;

            max = digit;
            position = cursor;
        }

        return (max, position + cursorBaseOffset);
    }

    /// <summary>
    ///     This is an internal metod for debugging joltage. It is printing line with highlighting range of the search and max
    ///     value found.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cursor"></param>
    /// <param name="length"></param>
    /// <param name="maxPosition"></param>
    internal static void JoltageSearchDebugging(string message, int cursor, int length, int maxPosition = -1)
    {
        if (string.IsNullOrEmpty(message))
        {
            Console.WriteLine();
            return;
        }

        if (cursor < 0) cursor = 0;
        if (length <= 0)
        {
            Console.WriteLine(message);
            return;
        }

        var original = Console.ForegroundColor;
        var end = Math.Min(message.Length, cursor + length);

        for (var i = 0; i < message.Length; i++)
        {
            Console.ForegroundColor = i switch
            {
                _ when i == maxPosition => ConsoleColor.Red,
                _ when i >= cursor && i < end => ConsoleColor.Blue,
                _ => original
            };


            Console.Write(message[i]);
        }

        Console.ForegroundColor = original;
        Console.WriteLine();
    }
}