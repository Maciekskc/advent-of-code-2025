var path = "input.txt";
try
{
    if (!File.Exists(path))
    {
        throw new ArgumentException($"File {path} does not exist.");
    }
    
    var safe = new Safe();
    using StreamReader sr = new StreamReader(path);
    var answer = safe.Decode(sr);
    
    Console.WriteLine($"The answer for given input is {answer}");
}
catch (Exception e)
{
    Console.WriteLine($"Could not decode safe. Exception details: {e}");
}

class Safe
{
    private int _state = 50;
    private int _zeroCounter;
    private const int DialRange = 100;

    private void ResetState()
    {
        _state = 50;
        _zeroCounter = 0;
    }
    
    private void AdjustState() => _state = (_state + DialRange) % DialRange;

    private void Turn(string? request)
    {
        ArgumentException.ThrowIfNullOrEmpty(request);
        var side = request[0];
        var requestedClicks = int.Parse(request.Substring(1));

        _state += side switch
        {
            'L' => -requestedClicks,
            'R' => requestedClicks,
            _ => throw new ArgumentOutOfRangeException("Rotation side")
        };
        AdjustState();
        if(_state == 0) _zeroCounter++;
    }

    public int Decode(StreamReader reader)
    {
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            Turn(line);
        }
        return _zeroCounter;
    }
}