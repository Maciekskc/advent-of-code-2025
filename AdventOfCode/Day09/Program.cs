using Helpers;

var path = "input.txt";
var lines = FileHelper.GetFileStream(path).GetStringListFromFile();

var solver = new MovieTheaterSolver(lines);
Console.WriteLine($"The answer for given input is {solver.RectangleMax().Area}");


internal class MovieTheaterSolver
{
    private readonly Point[] _points;

    public MovieTheaterSolver(string[] input)
    {
        _points = new Point[input.Length];
        for (var index = 0; index < input.Length; index++)
        {
            var coordinates = input[index].Split(',');
            _points[index] = new Point(long.Parse(coordinates[0]), long.Parse(coordinates[1]));
        }
    }

    public (int IndexOfFirst, int IndexOfSecond, long Area) RectangleMax()
    {
        (int, int, long) biggestRectangle = (-1, -1, -1);
        for (var i = 0; i < _points.Length; i++)
        for (var j = i + 1; j < _points.Length; j++)
        {
            var a = Math.Abs(_points[i].X - _points[j].X) + 1;
            var b = Math.Abs(_points[i].Y - _points[j].Y) + 1;
            var area = a * b;
            if (biggestRectangle.Item3 < area) biggestRectangle = (i, j, area);
            // Console.WriteLine($"Bigger rectangle [({_points[i]})({_points[j]})]; a:{a} x b:{b} = {area}");
        }

        return biggestRectangle;
    }

    private record Point(long X, long Y);
}