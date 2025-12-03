var path = "input.txt";
try
{
    if (!File.Exists(path)) throw new ArgumentException($"File {path} does not exist.");

    var giftShopDatabase = new GiftShopDatabase();
    using var sr = new StreamReader(path);
    var invalidIds = giftShopDatabase.FindInvalidIds(sr);

    var answer = invalidIds.Sum();
    Console.WriteLine($"The answer for given input is {answer}");
}
catch (Exception e)
{
    Console.WriteLine($"Could not find invalid ids. Exception details: {e}");
}

public class GiftShopDatabase
{
    private string[] _items = [];

    public List<long> FindInvalidIds(StreamReader sr)
    {
        var input = sr.ReadLine();
        ArgumentException.ThrowIfNullOrEmpty(input);
        
        List<long> result = [];
        var idRanges = input.Split(',');

        foreach (var idRange in idRanges)
        {
            var idsParsed = ParseIdRange(idRange);
            var invalidIds = GetInvalidIds(idsParsed);
            
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
        
        if(idLength % 2 != 0)
            return true;

        var firstHalf = idAsString[..(idLength / 2)];
        var secondHalf = idAsString[(idLength / 2)..];
        
        return firstHalf != secondHalf;
    }
}