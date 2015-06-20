using System.Collections.Generic;

namespace GameAlgorithms.Lands
{
    public interface IGrid
    {
        bool IsWalkableAt(int x, int y);
        IEnumerable<Path> GetNeighbors(Path path);
        Cell this[int x, int y] { get; set; }
    }
}
