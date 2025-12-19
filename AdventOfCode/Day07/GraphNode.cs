namespace Day07;

public class GraphNode(int x, int y, List<GraphNode> parents)
{
    public int PosX { get; init; } = x;
    public int PosY { get; init; } = y;
    public List<GraphNode> Parents { get; set; } = parents;
    public List<GraphNode> Childs { get; set; } = [];

    public int NumberOfSubgraphs()
    {
        if (Childs.Count == 0)
            return 1;
        int total = 0;
        foreach (var child in Childs)
        {
            total += child.NumberOfSubgraphs();
        }

        return total;
    }
}