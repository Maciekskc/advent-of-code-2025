using Helpers;

var path = "input.txt";
var lines = FileHelper.GetFileStream(path).ReadAllLines();

var solver = new CafeteriaSolver(lines, true);
Console.WriteLine($"The answer for given input is {solver.CountFreshProducts()}");

internal class CafeteriaSolver
{
    private readonly bool _isDebugEnabled;
    private readonly List<(long from,long to)> _ranges = [];
    private readonly List<long> _products;

    public CafeteriaSolver(List<string> database, bool isDebugEnabled = false)
    {
        _isDebugEnabled = isDebugEnabled;
        _products = new List<long>();

        foreach (var line in database)
        {
            if(line.Contains('-'))
            {
                var split = line.Split('-');
                _ranges.Add((from: long.Parse(split[0]),to: long.Parse(split[1])));
                continue;
            }
            
            if(long.TryParse(line, out long value))
                _products.Add(value);
        }
        
        if(isDebugEnabled)
            Console.WriteLine($"Decoded database. Found {_ranges.Count} ranges elements and {_products.Count} products.");
    }

    public int CountFreshProducts()
    {
        var freshProductsCount = _products.Count(product =>
            _ranges.Any(range => product >= range.from && product <= range.to));
            
        return freshProductsCount;
    }
}