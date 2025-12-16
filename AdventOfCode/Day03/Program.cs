using System.ComponentModel.DataAnnotations;

var path = "input.txt";
try
{
    if (!File.Exists(path)) throw new ArgumentException($"File {path} does not exist.");

    var safe = new EscalatorPowerResolver(true, 12);
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
    private int _allowedDigits = allowedDigits;
    private long _totalVoltagePower;
    public long GetTotalVoltagePower(StreamReader sr)
    {
        _totalVoltagePower = 0;
        while (!sr.EndOfStream)
        {
            var bankInput = sr.ReadLine();
            ArgumentException.ThrowIfNullOrEmpty(bankInput);
            _totalVoltagePower += CalculateHighestJoltageOfTheBank(bankInput);
        }

        return _totalVoltagePower;
    }

    private int CalculateHighestJoltageOfTheBank(string line)
    {
        var cursor = 0; // cursor for substringing
        var sum = 0; // current sum
        if(isDebugEnabled)
            Console.WriteLine($"Finding max joltage ({_allowedDigits} digits) in {line}");
        
        for (var i = _allowedDigits ; i > 0; i--) //  we iterate for allowed joltage digits
        {

            var nextMax = line //we take line
                .Substring(cursor, line.Length - i) // substringg from cursor to max size
                .Max(); // find max value in this sub array
            if (isDebugEnabled)
                Console.WriteLine($"MAX in {line.Substring(cursor, line.Length - i)} = {nextMax.value}[pos{nextMax.position}]");
            sum += nextMax.value; // we add max value to the sum
            cursor += nextMax.position + 1; // we move cursor to the new possition

            sum *= 10;
        }

        return sum / 10;
    }
}

internal static class StringExtensions{

    internal static (int value, int position) Max(this string input, int cursorBaseOffset = 0)
    {
        var max = 0;
        var position = -1;
        for(var cursor = 0; cursor < input.Length; cursor++)
        {
            var digit = int.Parse(input[cursor].ToString());

            if (digit <= max) continue;
            
            max = digit;
            position = cursor;
        }
        
        return (max, position + cursorBaseOffset);
    }

    internal static void WriteLineHighlight(string message, int cursor, int length, ConsoleColor color)
    {
        for (var i = 0; i < message.Length; i++)
        {
            if(i <= cursor && i >=(cursor + length) )
            {
                Console.ForegroundColor = color;
            }
            else
            {
                Console.ForegroundColor = default;
            }
            Console.Write(message[i]);
        }
        Console.WriteLine();
    }
}
