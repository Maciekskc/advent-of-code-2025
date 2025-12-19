internal partial class LaboratoriesSolver
{
    private void RecursiveQuantumLevelCalculation(string[] lines, string[] previousLines, int beamPosition,
        int currentLevel, int maxLevel)
    {
        currentLevel++;
        if (currentLevel == maxLevel)
        {
            _timeLineCounter++;
            // PrintBeam(previousLines.ToList());
            return;
        }

        var currentLine = lines[currentLevel].ToCharArray();
        if (currentLine[beamPosition] == '^')
        {
            // Split left
            if (beamPosition - 1 >= 0 && currentLine[beamPosition - 1] == '.')
            {
                var currentLineLeftCopy = (char[])currentLine.Clone();
                currentLineLeftCopy[beamPosition - 1] = '|';
                RecursiveQuantumLevelCalculation(lines, previousLines.Append(new string(currentLineLeftCopy)).ToArray(),
                    beamPosition - 1, currentLevel, maxLevel);
            }

            // Split right
            if (beamPosition + 1 < currentLine.Length && currentLine[beamPosition + 1] == '.')
            {
                var currentLineRightCopy = (char[])currentLine.Clone();
                currentLineRightCopy[beamPosition + 1] = '|';
                RecursiveQuantumLevelCalculation(lines,
                    previousLines.Append(new string(currentLineRightCopy)).ToArray(), beamPosition + 1, currentLevel,
                    maxLevel);
            }
        }
        else
        {
            currentLine[beamPosition] = '|';
            RecursiveQuantumLevelCalculation(lines, previousLines.Append(new string(currentLine)).ToArray(),
                beamPosition, currentLevel, maxLevel);
        }
    }
}