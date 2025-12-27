internal partial class LaboratoriesSolver
{
    public (int splitCounter, string[] finallMatrix) AnalyseBeamSplit()
    {
        _splitCounter = 0;
        var linesWithBeamAdapted = new List<string>() { lines[0] };
        var beamPositions = new List<int> { lines[0].IndexOf('S') };

        foreach (var line in lines[1..])
        {
            var (propagatedLine, newBeamPositions) = PropagateBeamToTheNextRow(line, beamPositions);
            linesWithBeamAdapted.Add(propagatedLine);
            beamPositions = newBeamPositions;
        }

        PrintBeam(linesWithBeamAdapted);

        return (_splitCounter, linesWithBeamAdapted.ToArray());
    }

    private (string line, List<int> newBeamPositions) PropagateBeamToTheNextRow(string nextLine,
        List<int> beamPositions)
    {
        var lineChars = nextLine.ToCharArray();
        var newBeamPositions = new List<int>();
        foreach (var beamPosition in beamPositions)
        {
            var doesSplit = false;
            var characterAtBeamPosition = nextLine[beamPosition];
            if (characterAtBeamPosition == '^')
            {
                if (beamPosition - 1 >= 0 && lineChars[beamPosition - 1] == '.')
                {
                    doesSplit = true;
                    lineChars[beamPosition - 1] = '|';
                    newBeamPositions.Add(beamPosition - 1);
                }

                if (beamPosition + 1 <= lineChars.Length && lineChars[beamPosition + 1] == '.')
                {
                    doesSplit = true;
                    lineChars[beamPosition + 1] = '|';
                    newBeamPositions.Add(beamPosition + 1);
                }
            }
            else
            {
                lineChars[beamPosition] = '|';
                newBeamPositions.Add(beamPosition);
            }

            if (doesSplit)
                _splitCounter++;
        }

        // Console.WriteLine($"{new string(lineChars)} - Splits: {_splitCounter}");
        return (new string(lineChars), newBeamPositions);
    }
}