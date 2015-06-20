using System;
using System.Collections.Generic;
using GameAlgorithms.Lands;
using System.Linq;


namespace GameAlgorithms.Pathfinders
{
    //TODO використовувати бібліотеку PowerCollections orderedBag - як чергу з пріорітетами
    //http://users.cecs.anu.edu.au/~dharabor/data/papers/harabor-grastien-aaai11.pdf
    //http://habrahabr.ru/post/162915/*/
    //TODO за рекомендаціями авторів можлива комбінація з Swamps (2010 рік)
    public class JumpPointSearch
    {
        private readonly IGrid _grid;

        public JumpPointSearch(IGrid land)
        {
            _grid = land;
        }

        /// <summary>Функція відсікання сусідів</summary>
        /// <param name="cell">Поточна позиція</param>
        /// <returns>Переліченння відсічених сусідів</returns>
        private IEnumerable<Path> PruneNeighbors(Path cell)
        {
            //для початкової позиції повертаємо всіх сусідів клітинки
            if (cell.Previous == null)
            {
                foreach (var neighbor in _grid.GetNeighbors(cell))
                    yield return neighbor;
            }
            else
            {
                var direction = Path.GetDirection(cell.Previous, cell);
                //рух подіагоналі
                if (Path.IsNotZero(direction))
                {
                    //три природні сусіди
                    if (_grid.IsWalkableAt(cell.X, cell.Y + direction.Y))
                        yield return new Path(cell.X, cell.Y + direction.Y);
                    if (_grid.IsWalkableAt(cell.X + direction.X, cell.Y))
                        yield return new Path(cell.X + direction.X, cell.Y);
                    //шлях не може проходити по діагоналі, якщо по обидві сторни непрохідні позиції (навіть якщо перед діагоналлю є прохідна позиція)
                    if ((_grid.IsWalkableAt(cell.X, cell.Y + direction.Y) ||
                         _grid.IsWalkableAt(cell.X + direction.X, cell.Y)) &&
                        _grid.IsWalkableAt(cell.X + direction.X, cell.Y + direction.Y))
                        yield return new Path(cell.X + direction.X, cell.Y + direction.Y);

                    //дві можливі вимушені позиції
                    foreach (var forcedNeighbor in DiagonalForcedNeighbors(cell, direction))
                        yield return forcedNeighbor;
                }
                else //тут рух по горизонталі і по вертикалі аналогічні
                {
                    //рух по вертикалі
                    if (direction.X == 0)
                    {
                        //якщо є природній сусід (а при такому русі він один), то шукаємо вимушених
                        //інакше - нема ніяких потрібних сусідів
                        if (_grid.IsWalkableAt(cell.X, cell.Y + direction.Y))
                        {
                            //природній сусід
                            yield return new Path(cell.X, cell.Y + direction.Y);
                            //можливі два вимушені сусіди
                            foreach (var forcedNeighbor in VerticalForcedNeighbors(cell, direction))
                                yield return forcedNeighbor;
                        }
                    }
                    else
                    {
                        //рух по горизонталі
                        if (_grid.IsWalkableAt(cell.X + direction.X, cell.Y))
                        {
                            //природній сусід
                            yield return new Path(cell.X + direction.X, cell.Y);
                            //а також перевіряємо двох можливих вимушених сусідів по горизонталі
                            foreach (var forcedNeighbor in HorizontalForcedNeighbors(cell, direction))
                                yield return forcedNeighbor;
                        }
                    }
                }
            }
        }

        private IEnumerable<Path> HorizontalForcedNeighbors(Path cell, Path direction)
        {
            if (!_grid.IsWalkableAt(cell.X, cell.Y + 1) && (_grid.IsWalkableAt(cell.X + direction.X, cell.Y + 1)))
                yield return new Path(cell.X + direction.X, cell.Y + 1);
            if (!_grid.IsWalkableAt(cell.X, cell.Y - 1) && (_grid.IsWalkableAt(cell.X + direction.X, cell.Y - 1)))
                yield return new Path(cell.X + direction.X, cell.Y - 1);
        }

        private IEnumerable<Path> VerticalForcedNeighbors(Path cell, Path direction)
        {
            if (!_grid.IsWalkableAt(cell.X + 1, cell.Y) && _grid.IsWalkableAt(cell.X + 1, cell.Y + direction.Y))
                yield return new Path(cell.X + 1, cell.Y + direction.Y);
            if (!_grid.IsWalkableAt(cell.X - 1, cell.Y) && _grid.IsWalkableAt(cell.X - 1, cell.Y + direction.Y))
                yield return new Path(cell.X - 1, cell.Y + direction.Y);
        }

        private IEnumerable<Path> DiagonalForcedNeighbors(Path cell, Path direction)
        {
            if (!_grid.IsWalkableAt(cell.X - direction.X, cell.Y) &&
                _grid.IsWalkableAt(cell.X, cell.Y + direction.Y) &&
                _grid.IsWalkableAt(cell.X - direction.X, cell.Y + direction.Y))
                yield return new Path(cell.X - direction.X, cell.Y + direction.Y);

            if (!_grid.IsWalkableAt(cell.X, cell.Y - direction.Y) &&
                _grid.IsWalkableAt(cell.X + direction.X, cell.Y) &&
                _grid.IsWalkableAt(cell.X + direction.X, cell.Y - direction.Y))
                yield return new Path(cell.X + direction.X, cell.Y - direction.Y);
        }

        public List<Path> FindPath(Path from, Path to)
        {
            var sortedList = new List<Path> { from };
            while (sortedList.Count != 0)
            {
                var node = sortedList[0];
                node.Checked = true;
                sortedList.RemoveAt(0);

                if (node == to)
                {
                    var list = new List<Path>();
                    list.Insert(0, node);
                    while (node.Previous != null)
                    {
                        node = node.Previous;
                        list.Insert(0, node);
                    }
                    return list;
                }
                //TODO не повертає null в замкнутому колі
                sortedList.AddRange(IdentifySuccessors(node, to));
                sortedList.Sort((path, path1) => path.Weight > path1.Weight ? 1 : path.Weight < path1.Weight? -1 : 0);
            }
            return null;
        }

        private Path Jump(Path cell, Path end)
        {
            //якщо перешкода чи поза полем
            if (!_grid.IsWalkableAt(cell.X, cell.Y))
                return null;
            //якщо досягнуто кінцевої точки
            if (cell == end)
                return cell;
            //визначається напрямок руху
            var direction = Path.GetDirection(cell.Previous,cell);
            //Якщо серед сусідів є вимушені
            if (Path.IsNotZero(direction))
            {
                if (DiagonalForcedNeighbors(cell, direction).Any())
                    return cell;

                //якщо рухаємося по діагоналі додатково треба перевірити шлях по горизонталі і по вертикалі
                var jumpPosHorizontal = Jump(new Path(cell.X + direction.X, cell.Y) { Previous = cell },end);
                var jumpPosVertical = Jump(new Path(cell.X, cell.Y + direction.Y) { Previous = cell },end);
                if (jumpPosHorizontal != null /*Path.Indefinited*/|| jumpPosVertical != null /*Path.Indefinited*/)
                    return cell;
            }
            else
            {
                if (direction.X == 0)
                {
                    if (VerticalForcedNeighbors(cell, direction).Any())
                        return cell;
                }
                else
                {
                    if (HorizontalForcedNeighbors(cell, direction).Any())
                        return cell;
                }
            }

            //інакше рухаючись по діагоналі 
            //шлях не може проходити по діагоналі, якщо по обидві сторни непрохідні позиції (навіть якщо перед діагоналлю є прохідна позиція)
            if (_grid.IsWalkableAt(cell.X, cell.Y + direction.Y) ||
                _grid.IsWalkableAt(cell.X + direction.X, cell.Y))
                return Jump(new Path(cell.X + direction.X, cell.Y + direction.Y) { Previous = cell },end);
            return null /*Path.Indefinited*/;
        }

        private IEnumerable<Path> IdentifySuccessors(Path cell, Path end)
        {
            var pruned = PruneNeighbors(cell);
            foreach (var neighbor in pruned)
            {
                neighbor.Previous = cell;
                var jumpPoint = Jump(neighbor,end);
                if (jumpPoint == null) continue;
                var distance = GetEuclideanDistance(cell, jumpPoint) + cell.FromStart;
                if (!jumpPoint.Checked || distance < jumpPoint.FromStart)
                {
                    jumpPoint.FromStart = distance;
                    jumpPoint.ToEnd = GetEuclideanDistance(jumpPoint, end);
                    jumpPoint.Weight = jumpPoint.FromStart + jumpPoint.ToEnd;
                    yield return jumpPoint;
                }
            }
        }

        private static double GetEuclideanDistance(Path from, Path to)
        {
            return Math.Sqrt((from.X - to.X)*(from.X - to.X) + (from.Y - to.Y)*(from.Y - to.Y));
        }
    }
}
