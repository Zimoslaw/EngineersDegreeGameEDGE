using System.Collections.Generic;

/// <summary>
/// Represents cell in Randomized depth-first search algorythm implementation.
/// </summary>
public class Cell
{
    public bool visited;

    public List<byte> walls; // 0 = rear, 1 = rigth, 2 = forward, 3 = left

    public Cell()
    {
        visited = false;
        walls = new List<byte> { 0, 1, 2, 3 }; // rear, rigth, forward, left
    }
}
