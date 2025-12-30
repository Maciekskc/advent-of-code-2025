var path = "input.txt";
try
{
    if (!File.Exists(path)) throw new ArgumentException($"File {path} does not exist.");

    var safe = new Safe();
    using var sr = new StreamReader(path);
    var answer = safe.Decode(sr, true);

    Console.WriteLine($"The answer for given input is {answer}");
}
catch (Exception e)
{
    Console.WriteLine($"Could not decode safe. Exception details: {e}");
}

internal class Safe
{
    private const int DialRange = 100;
    private bool _isProtocol0X434C49434BEnabled;
    private int _state = 50;
    private int _zeroCounter;

    private void ResetState()
    {
        _state = 50;
        _zeroCounter = 0;
    }

    private void AdjustState()
    {
        _state = (_state + DialRange) % DialRange;
    }

    private void Turn(string? request)
    {
        ArgumentException.ThrowIfNullOrEmpty(request);
        var side = request[0];
        var requestedClicks = int.Parse(request.Substring(1));

        var turnInfo = GetTurnInfo(side switch
        {
            'L' => -requestedClicks,
            'R' => requestedClicks,
            _ => throw new ArgumentOutOfRangeException("Rotation side")
        });
        CalculateTurn(turnInfo);
        _state += turnInfo.clicksFixed;
        AdjustState();
    }

    private void CalculateTurn((int fullRotations, int clicksFixed) turnInfo)
    {
        if (_isProtocol0X434C49434BEnabled)
            CalculateFor0X434C49434B(turnInfo);

        CalculateDefault(turnInfo.clicksFixed);
    }

    private void CalculateFor0X434C49434B((int fullRotations, int clicksFixed) turnInfo)
    {
        _zeroCounter += turnInfo.fullRotations;
        var nextPosition = _state + turnInfo.clicksFixed;

        var crossZeroCondition = nextPosition switch
        {
            > DialRange when _state < DialRange => true,
            < 0 when _state > 0 => true,
            _ => false
        };

        if (crossZeroCondition) _zeroCounter++;
    }

    private void CalculateDefault(int clicks)
    {
        if (_state + clicks is 0 or DialRange) _zeroCounter++;
    }

    private static (int fullRotations, int clicksFixed) GetTurnInfo(int totalClicks)
    {
        return (int.Abs(totalClicks / DialRange), totalClicks % DialRange);
    }

    public int Decode(StreamReader reader, bool isProtocol0X434C49434BEnabled)
    {
        _isProtocol0X434C49434BEnabled = isProtocol0X434C49434BEnabled;
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            Turn(line);
        }

        return _zeroCounter;
    }
}