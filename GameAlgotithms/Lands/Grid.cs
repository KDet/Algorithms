using System.Collections.Generic;

namespace GameAlgorithms.Lands
{
    public class Grid : IGrid
    {
        private readonly Cell[,] _field;

        public Grid(Cell[,] field)
        {
            _field = field;
        }

        public virtual bool IsWalkableAt(int x, int y)
        {
            if (x > _field.GetLowerBound(0) || x < 0 || y >= _field.GetLowerBound(1) || y < 0) return false;
            return _field[x, y].IsWalkable();
        }

        public bool IsWalkableAt(Path path)
        {
            return IsWalkableAt(path.X, path.Y);
        }

        public virtual IEnumerable<Path> GetNeighbors(Path path)
        {
            if (!IsWalkableAt(path.X, path.Y)) yield break;
            if (IsWalkableAt(path.X - 1, path.Y - 1)) yield return new Path(path.X - 1, path.Y - 1);
            if (IsWalkableAt(path.X, path.Y - 1)) yield return new Path(path.X, path.Y - 1);
            if (IsWalkableAt(path.X + 1, path.Y - 1)) yield return new Path(path.X + 1, path.Y - 1);
            if (IsWalkableAt(path.X - 1, path.Y)) yield return new Path(path.X - 1, path.Y);
            if (IsWalkableAt(path.X + 1, path.Y)) yield return new Path(path.X + 1, path.Y);
            if (IsWalkableAt(path.X - 1, path.Y + 1)) yield return new Path(path.X - 1, path.Y + 1);
            if (IsWalkableAt(path.X, path.Y + 1)) yield return new Path(path.X, path.Y + 1);
            if (IsWalkableAt(path.X + 1, path.Y + 1)) yield return new Path(path.X + 1, path.Y + 1);
        }

        public Cell this[int x, int y]
        {
            get { return _field[x, y]; }
            set { _field[x, y] = value; }
        }

        public Cell this[Path p]
        {
            get { return _field[p.X, p.Y]; }
        }
    }
}
