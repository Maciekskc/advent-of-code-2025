var path = "input.txt";
try
{
    if (!File.Exists(path)) throw new ArgumentException($"File {path} does not exist.");

    var giftShopDatabase = new GiftShopDatabaseSolver(false);
    using var sr = new StreamReader(path);
    var invalidIds = giftShopDatabase.FindInvalidIds(sr);

    var answer = invalidIds.Sum();
    Console.WriteLine($"The answer for given input is {answer}");
}
catch (Exception e)
{
    Console.WriteLine($"Could not find invalid ids. Exception details: {e}");
}

public class GiftShopDatabaseSolver(bool isDebugEnabled)
{
    public List<long> FindInvalidIds(StreamReader sr)
    {
        var input = sr.ReadLine();
        ArgumentException.ThrowIfNullOrEmpty(input);
        
        List<long> result = [];
        var idRanges = input.Split(',');

        foreach (var idRange in idRanges)
        {
            var idsParsed = ParseIdRange(idRange);
            if(isDebugEnabled)
                Console.WriteLine($"Range: {idsParsed.min}-{idsParsed.max}");
            
            var invalidIds = GetInvalidIds(idsParsed);
            
            if(isDebugEnabled)
                invalidIds.ForEach(Console.WriteLine);

            result.AddRange(invalidIds);
        }
        
        return result;
    }

    private static (long min, long max) ParseIdRange(string idRange)
    {
       var ids = idRange.Split('-');
       if (ids.Length != 2)
           throw new ArgumentException($"Id range {idRange} is not valid.");
       
       if(!long.TryParse(ids[0], out var min) || !long.TryParse(ids[1], out var max))
           throw new ArgumentException($"Id range {idRange} contains non-longeger values.");
       
       return (min, max);
    }

    private static List<long> GetInvalidIds((long min, long max) idRange)
    {
        List<long> invalidIds = [];
        for (long id = idRange.min; id <= idRange.max; id++)
        {
            if (!IsValidId(id))
            {
                invalidIds.Add(id);
            }
        }
        return invalidIds;
    }

    private static bool IsValidId(long id)
    {
        var idAsString = id.ToString();
        var idLength = idAsString.Length;

        for (var chunkSize = 1; chunkSize <= idLength / 2; chunkSize++)
        {
            if(idLength % chunkSize != 0)
                continue;
            
            var chunks = idAsString.Split(chunkSize);
            if(chunks.All(chunk => chunk == chunks[0]))
                return false;
        }

        return true;
    }
}
internal  static class StringExtensions
{
    internal static string[] Split(this string str, int chunkSize) =>
        Enumerable.Range(0, str.Length / chunkSize)
            .Select(i => str.Substring(i * chunkSize, chunkSize)).ToArray();
}