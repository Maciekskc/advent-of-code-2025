using Helpers;

var path = "input.txt";
var lines = FileHelper.GetFileStream(path).GetStringListFromFile();

var solver = new MovieTheaterSolver(lines);
Console.WriteLine($"The answer for given input is {solver.RectangleMax(true).Area}");


internal class MovieTheaterSolver
{
    private readonly Point[] _points;
    private readonly Line[] _lines;
    private readonly List<(long x1, long y1, long x2, long y2, bool isFilled)> _regions = [];


    public MovieTheaterSolver(string[] input)
    {
        _points = new Point[input.Length];
        _lines = new Line[input.Length];
        for (var index = 0; index < input.Length; index++)
        {
            var coordinates = input[index].Split(',');
            _points[index] = new Point(long.Parse(coordinates[0]), long.Parse(coordinates[1]));
        }

        for (var index = 1; index < _points.Length; index++)
        {
            _lines[index] = new Line(_points[index - 1], _points[index]);
        }

        _lines[0] = new Line(_points[^1], _points[0]);

        CalculateRegions();
    }

    private void CalculateRegions()
    {
        var distinctX = _points.Select(p => p.X).Distinct().OrderBy(x => x).ToArray();
        var distinctY = _points.Select(p => p.Y).Distinct().OrderBy(y => y).ToArray();

        for (var xIndex = 1; xIndex < distinctX.Length; xIndex++)
        for (var yIndex = 1; yIndex < distinctY.Length; yIndex++)
        {
            var regionCenter = new Point((distinctX[xIndex - 1] + distinctX[xIndex]) / 2,
                (distinctY[yIndex - 1] + distinctY[yIndex]) / 2);
            var value = IsWithinPolygon(regionCenter);
            _regions.Add((distinctX[xIndex - 1], distinctY[yIndex - 1], distinctX[xIndex],
                distinctY[yIndex], value));
        }
    }

    public (int IndexOfFirst, int IndexOfSecond, long Area) RectangleMax(bool withinLines = false)
    {
        (int, int, long) biggestRectangle = (-1, -1, -1);
        for (var i = 0; i < _points.Length; i++)
        for (var j = i + 1; j < _points.Length; j++)
        {
            var xMin = Math.Min(_points[i].X, _points[j].X);
            var xMax = Math.Max(_points[i].X, _points[j].X);
            var yMin = Math.Min(_points[i].Y, _points[j].Y);
            var yMax = Math.Max(_points[i].Y, _points[j].Y);
            
            if (!_regions.Where(r => r.x1 >= xMin && r.y1 >= yMin && r.x2 <= xMax && r.y2 <= yMax).All(r => r.isFilled))
            {
                continue;
            }

            var a = Math.Abs(_points[i].X - _points[j].X) + 1;
            var b = Math.Abs(_points[i].Y - _points[j].Y) + 1;
            var area = a * b;
            if (biggestRectangle.Item3 < area)
            {
                biggestRectangle = (i, j, area);
            }
        }
    
        Console.WriteLine($"The biggest rectangle is between {_points[biggestRectangle.Item1]} and {_points[biggestRectangle.Item2]}");
        return biggestRectangle;
    }

    private bool IsWithinPolygon(Point investigated) =>
        investigated.RayCasting(_lines) % 2 == 1;

    private record Point(long X, long Y)
    {
        public int RayCasting(Line[] lines)
        {
            return lines.Count(line =>
                line.Orientation == Line.LineOrientation.Vertical && X < line.A.X &&
                Y >= Math.Min(line.A.Y, line.B.Y) && Y <= Math.Max(line.A.Y, line.B.Y));
        }
    };

    private record Line(Point A, Point B)
    {
        public readonly LineOrientation Orientation = A.X == B.X ? LineOrientation.Vertical : LineOrientation.Horizontal;

        internal enum LineOrientation
        {
            Horizontal,
            Vertical
        }
    }
}