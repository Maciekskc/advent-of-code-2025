using Helpers;

var path = "input.txt";
var lines = FileHelper.GetFileStream(path).ReadAllLines();

var solver = new CafeteriaSolver(lines, true);
Console.WriteLine($"The answer for given input is {solver.SolveRiddle(true)}");

internal class CafeteriaSolver
{
    private readonly bool _isDebugEnabled;
    private readonly List<(long from, long to)> _ranges = [];
    private readonly List<long> _products;

    public CafeteriaSolver(List<string> database, bool isDebugEnabled = false)
    {
        _isDebugEnabled = isDebugEnabled;
        _products = new List<long>();

        foreach (var line in database)
        {
            if (line.Contains('-'))
            {
                var split = line.Split('-');
                _ranges.Add((from: long.Parse(split[0]), to: long.Parse(split[1])));
                continue;
            }

            if (long.TryParse(line, out long value))
                _products.Add(value);
        }

        if (_isDebugEnabled)
            Console.WriteLine(
                $"Decoded database. Found {_ranges.Count} ranges elements and {_products.Count} products.");
    }

    private long CountFreshProducts()
    {
        var freshProductsCount = _products.Count(product =>
            _ranges.Any(range => product >= range.from && product <= range.to));

        return freshProductsCount;
    }

    private long CountProductsConsideredFresh()
    {
        var joinedRanges = JoinFreshnessRanges(_ranges);
        int previousCount = 0;
        int rangesCount = 0;

        if (_isDebugEnabled)
            Console.WriteLine(
                $"Initial iteration reduced ranges from {_ranges.Count} to {joinedRanges.Count}");
        
        do
        {
            previousCount = joinedRanges.Count;
            joinedRanges = JoinFreshnessRanges(joinedRanges);

            rangesCount = joinedRanges.Count;
            if (_isDebugEnabled && rangesCount != previousCount)
                Console.WriteLine(
                    $"After next iteration, we merged overlying ranges, reducing ranges count from {previousCount} to {rangesCount}");
        } while (rangesCount != previousCount);

        if (_isDebugEnabled)
        {
            Console.WriteLine(
                $"Stopped loop. The final count of not overlying ranges is {rangesCount}.");
            PrintRanges(joinedRanges);
        }
        return joinedRanges.Sum(x => x.to - x.from + 1);
    }

    private void PrintRanges(List<(long from, long to)> joinedRanges)
        => Console.WriteLine($"FINAL RANGES: [{string.Join(", ", joinedRanges.Select(r => $"[{r.from},{r.to}]"))}]");

    /// <summary>
    /// Algorithm
    /// We will be iterating over each range and add build result list based on the conditions. We have several cases to consider:
    /// ....|_______|...... - lets consider this as a base
    /// we have several cases for separate handling
    /// ......|___|........ - CASE 1: fully contained
    /// ..|___________|.... - CASE 2: fully contained reversed
    /// .........|_______|. - CASE 3: left overlap
    /// .|_______|......... - CASE 4: right overlap
    /// OTHERS: everything else is just not overlapping
    ///
    /// For case 1-4 we modify base record with new from/to values
    /// For others we just add it to output list
    /// This is not handling overlaps of more than 2 ranges, but each run reduce number of multiple overlaps by at least one. Recursive call will handle the rest.
    ///</summary>
    /// <returns></returns>
    private List<(long from, long to)> JoinFreshnessRanges(List<(long from, long to)> ranges, bool isFullDebugEnabled = false)
    {
        var calculatedRanges = new List<(long from, long to)>();

        foreach (var range in ranges)
        {
            //CASE 1: fully contained
            // BASE: ....|_______|......
            // NEW:  ......|___|........
            if (calculatedRanges.Any(calculatedRange =>
                    range.from >= calculatedRange.from && range.to <= calculatedRange.to))
            {
                if (_isDebugEnabled && isFullDebugEnabled)
                    Console.WriteLine("Full inclusion detected, skipping range.");
                continue;
            }

            //CASE 2: fully contained reversed
            // BASE: ....|_______|......
            // NEW:  ..|___________|....
            if (calculatedRanges.FindIndex(calculatedRange =>
                    range.from <= calculatedRange.from && range.to >= calculatedRange.to) is var indexOverlap and not -1)
            {
                var overlap = calculatedRanges[indexOverlap];
                if (_isDebugEnabled && isFullDebugEnabled)
                    Console.WriteLine(
                        $"Full overlap detected between existing:[{overlap.from},{overlap.to}] and new [{range.from},{range.to}]. Unifying to [{range.from},{range.to}]");
                calculatedRanges[indexOverlap] = (range.from, range.to);
                continue;
            }

            // CASE 3: overlap left
            // BASE: ....|_______|......
            // NEW:  .........|_______|.
            if (calculatedRanges.FindIndex(calculatedRange =>
                    range.from > calculatedRange.from && range.from <= calculatedRange.to &&
                    range.to > calculatedRange.to) is var indexLeftOverlap and not -1)
            {
                var overlapLeft = calculatedRanges[indexLeftOverlap];
                if (_isDebugEnabled && isFullDebugEnabled)
                    Console.WriteLine(
                        $"Left overlap detected between existing:[{overlapLeft.from},{overlapLeft.to}] and new [{range.from},{range.to}]. Unifying to [{overlapLeft.from},{range.to}]");
                calculatedRanges[indexLeftOverlap] = (overlapLeft.from, range.to);
                continue;
            }

            // CASE 4: overlap right
            // BASE: ....|_______|......
            // NEW:  .|_______|.........
            if (calculatedRanges.FindIndex(calculatedRange =>
                    range.from < calculatedRange.from && range.to >= calculatedRange.from &&
                    range.to < calculatedRange.to) is var indexRightOverlap and not -1)
            {
                var overlapRight = calculatedRanges[indexRightOverlap];
                if (_isDebugEnabled && isFullDebugEnabled)
                    Console.WriteLine(
                        $"Right overlap detected between existing:[{overlapRight.from},{overlapRight.to}] and new [{range.from},{range.to}]. Unifying to [{range.from},{overlapRight.to}]");
                overlapRight.from = range.from;
                calculatedRanges[indexRightOverlap] = (range.from, overlapRight.to);
                continue;
            }

            //OTHERS: no overlap
            if (_isDebugEnabled && isFullDebugEnabled)
                Console.WriteLine(
                    $"New range. Adding [{range.from},{range.to}]");
            calculatedRanges.Add(range);
        }

        return calculatedRanges;
    }

    public long SolveRiddle(bool riddlePartTwo = false)
        => riddlePartTwo ? CountProductsConsideredFresh() : CountFreshProducts();
}