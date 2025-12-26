using Helpers;

var path = "input.txt";
var lines = FileHelper.GetFileStream(path).GetStringListFromFile();

var solver = new MovieTheaterSolver(lines);
Console.WriteLine($"The answer for given input is {solver.RectangleMax(true)}");


internal class MovieTheaterSolver
{
    private readonly Point[] _points;
    private readonly Line[] _lines;

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
    }

    public (int IndexOfFirst, int IndexOfSecond, long Area) RectangleMax(bool withinLines = false)
    {
        (int, int, long) biggestRectangle = (-1, -1, -1);
        for (var i = 0; i < _points.Length; i++)
        for (var j = i + 1; j < _points.Length; j++)
        {
            //Algorithm for part 2 is not enough, because we check only corners, but apparently the lines can define some whole in the full rectangle. It have to be revisited
            if (withinLines)
            {
                var thirdCorner = new Point(_points[i].X, _points[j].Y);
                var fourthCorner = new Point(_points[j].X, _points[i].Y);

                // Console.WriteLine($"Checking if all within the lines [1]({_points[i]}) [2]({_points[j]}) [3]({thirdCorner}) [4]({fourthCorner})");

                if (!IsWithinPolygon(thirdCorner))
                {
                    // Console.WriteLine($"Third {thirdCorner} outside of the polygon");
                    continue;
                }


                if (!IsWithinPolygon(fourthCorner))
                {
                    // Console.WriteLine($"Fourth {thirdCorner} outside of the polygon");
                    continue;
                }
            }

            var a = Math.Abs(_points[i].X - _points[j].X) + 1;
            var b = Math.Abs(_points[i].Y - _points[j].Y) + 1;
            var area = a * b;
            if (biggestRectangle.Item3 < area) biggestRectangle = (i, j, area);
            // Console.WriteLine($"Bigger rectangle [({_points[i]})({_points[j]})]; a:{a} x b:{b} = {area}");
        }

        return biggestRectangle;
    }

    private bool IsWithinPolygon(Point investigated) =>
        _lines.Any(x => x.IfCrossLine(investigated, Direction.South)) &&
        _lines.Any(x => x.IfCrossLine(investigated, Direction.North)) &&
        _lines.Any(x => x.IfCrossLine(investigated, Direction.East)) &&
        _lines.Any(x => x.IfCrossLine(investigated, Direction.West));

    private record Point(long X, long Y);

    private record Line
    {
        public Point A { get; }
        public Point B { get; }

        private LineOrientation _orientation;

        public Line(Point A, Point B)
        {
            this.A = A;
            this.B = B;
            if (A.X == B.X)
                _orientation = LineOrientation.Vertical;
        }

        public bool IfCrossLine(Point investigated, Direction direction)
        {
            if (_orientation == LineOrientation.Horizontal)
            {
                if (investigated.X < Math.Min(A.X, B.X) || investigated.X > Math.Max(A.X, B.X))
                    return false;

                switch (direction)
                {
                    case Direction.North when investigated.Y >= A.Y:
                    case Direction.South when investigated.Y <= A.Y:
                        return true;
                    case Direction.East:
                    case Direction.West:
                    default:
                        return false;
                }
            }

            if (_orientation == LineOrientation.Vertical)
            {
                if (investigated.Y < Math.Min(A.Y, B.Y) || investigated.Y > Math.Max(A.Y, B.Y))
                    return false;

                switch (direction)
                {
                    case Direction.West when investigated.X >= A.X:
                    case Direction.East when investigated.X <= A.X:
                        return true;
                    case Direction.North:
                    case Direction.South:
                    default:
                        return false;
                }
            }

            throw new ArgumentOutOfRangeException(
                $"Invalid check of {investigated} for object {this} with direction {direction.ToString()}");
        }

        internal enum LineOrientation
        {
            Horizontal,
            Vertical
        }
    }

    internal enum Direction
    {
        North,
        East,
        South,
        West
    }
}