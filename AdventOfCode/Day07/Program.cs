using System.Diagnostics;
using Day07;
using Helpers;

var path = "input-test.txt";
var lines = FileHelper.GetFileStream(path).GetStringListFromFile();

var solver = new LaboratoriesSolver(lines);
Console.WriteLine($"The answer for given input is {solver.AnalyseQuantumBeamSplit()}");

internal partial class LaboratoriesSolver(string[] lines)
{
    private int _splitCounter = 0;
    private int _timeLineCounter = 0;
    
    public int AnalyseQuantumBeamSplit()
    {
        _timeLineCounter = 0;
        
        Stopwatch sw = Stopwatch.StartNew();
        Console.WriteLine($"Quantum calculation need to consider whole multiverse. Please give it some time to finish...");

        if (false)
        {
            //First try with DFS recursion. Inoptimal in the input case.
            var currentLines = lines[..0] ;
            var beamPosition = lines[0].IndexOf('S');
            var currentLevel = 1;
            var maxLevel = lines.Length;
            RecursiveQuantumLevelCalculation(lines, currentLines, beamPosition, currentLevel, maxLevel);
        }
        else
        {
            var fullDiagram = AnalyseBeamSplit().finallMatrix;
            var graph = CompileDiagramGraph(fullDiagram);
            PrintGraph(graph);
            _timeLineCounter = graph[0].NumberOfSubgraphs();
        }
        sw.Stop();
        Console.WriteLine($"Job done. It took {sw.Elapsed.Seconds} seconds.");

        
        return _timeLineCounter;
    }

    private List<GraphNode> CompileDiagramGraph(string[] lines)
    {
        var graphNodes = new List<GraphNode>();
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            // Console.WriteLine($"[{y:D2}]{line}");
            for (var x = 0; x < line.Length; x++)
            {
                var character = line[x];
                if (character == '^')
                {
                    var parentNodes = graphNodes
                        .Where(n => n.PosY < y && (n.PosX == x - 1 || n.PosX == x + 1))
                        .GroupBy(node => node.PosX)
                        .Select(g => g.OrderByDescending(n => n.PosY).First())
                        .ToList();
                    var currentNode = new GraphNode(x, y, parentNodes);
                    foreach (var parent in parentNodes)
                        parent.Childs.Add(currentNode);

                    graphNodes.Add(currentNode);
                    // Console.WriteL}ine($"[{y:D2}]{lines[y].Replace('^','.')}[({x},{y}) parents: {string.Join(",",parentNodes.Select(n => $"({n.PosX},{n.PosY})"))}]");
                    
                    
                }
                
                if (y == lines.Length -1)
                    if(line[x] == '|')
                    {
                        var parentNodes = graphNodes
                            .Where(n => n.PosY < y && n.PosX == x)
                            .GroupBy(node => node.PosX)
                            .Select(g => g.OrderByDescending(n => n.PosY).First())
                            .ToList();
                        var currentNode = new GraphNode(x, y, parentNodes);
                        foreach (var parent in parentNodes)
                            parent.Childs.Add(currentNode);

                        graphNodes.Add(currentNode);
                        Console.WriteLine($"[{y:D2}]{lines[y].Replace('|','.')}[({x},{y}) parents: {string.Join(",",parentNodes.Select(n => $"({n.PosX},{n.PosY})"))}]");
                    }
            }
        }
        return graphNodes;
    }


    private static void PrintBeam(List<string> lines)
    {
        Console.WriteLine("BEAMS:");
        foreach (var line in lines)
            Console.WriteLine(line);
    }

    private void PrintGraph(List<GraphNode> nodes){
        foreach (var node in nodes)
        {
            Console.WriteLine($"Node at ({node.PosX},{node.PosY}) with parents: {string.Join(",",node.Parents.Select(n => $"({n.PosX},{n.PosY})"))} and childs: {string.Join(",",node.Childs.Select(n => $"({n.PosX},{n.PosY})"))}");
        }
    }
}

