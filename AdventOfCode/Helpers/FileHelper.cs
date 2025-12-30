namespace Helpers;

public static class FileHelper
{
    public static char[][] GetCharMatrixFromFile(StreamReader sr)
    {
        List<char[]> matrix = [];
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            ArgumentException.ThrowIfNullOrEmpty(line);
            matrix.Add(line.ToCharArray());
        }

        return matrix.ToArray();
    }

    public static string[][] GetStringMatrixFromFile(this StreamReader sr, char separator = ' ')
    {
        List<string[]> matrix = [];
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            ArgumentException.ThrowIfNullOrEmpty(line);
            matrix.Add(line.Split(separator, StringSplitOptions.RemoveEmptyEntries));
        }

        return matrix.ToArray();
    }

    public static string[] GetStringListFromFile(this StreamReader sr)
    {
        List<string> list = [];
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            ArgumentException.ThrowIfNullOrEmpty(line);
            list.Add(line);
        }

        return list.ToArray();
    }

    public static List<string> ReadAllLines(this StreamReader sr)
    {
        var result = new List<string>();
        while (!sr.EndOfStream)
            result.Add(sr.ReadLine()!);
        return result;
    }

    public static StreamReader GetFileStream(string path)
    {
        if (!File.Exists(path)) throw new ArgumentException($"File {path} does not exist.");
        return new StreamReader(path);
    }
}