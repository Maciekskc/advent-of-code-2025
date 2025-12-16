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

    public static StreamReader GetFileStream(string path)
    {
        if (!File.Exists(path)) throw new ArgumentException($"File {path} does not exist.");
        return new StreamReader(path);
    }
}