using System.ComponentModel.DataAnnotations;

var path = "input.txt";
try
{
    if (!File.Exists(path)) throw new ArgumentException($"File {path} does not exist.");

    var safe = new EscalatorPowerResolver(false);
    using var sr = new StreamReader(path);
    var answer = safe.GetTotalVoltagePower(sr);

    Console.WriteLine($"The answer for given input is {answer}");
}
catch (Exception e)
{
    Console.WriteLine($"Could calculate joltage. Exception details: {e}");
}

public class EscalatorPowerResolver(bool isDebugEnabled)
{
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
        var firstMax = line.Max();
        var rightPart = line[(firstMax.position + 1)..];
        
        var secondMax = rightPart.Length > 0 ? rightPart.Max(firstMax.position) : line.Remove(firstMax.position, 1).Max();

        if (firstMax.position <= secondMax.position)
            secondMax.position++;
        
        if(isDebugEnabled)
            Console.WriteLine($"Bank line: {line}, First max: {firstMax.value} at {firstMax.position}, Second max: {secondMax.value} at {secondMax.position}");
        
        if (firstMax.position >= secondMax.position)
            return secondMax.value * 10 + firstMax.value;
        
        return firstMax.value * 10 + secondMax.value;
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
}