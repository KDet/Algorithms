namespace GameAlgorithms.Lands
{
    public class Cell 
    {
        private  AreaType _area;

        public Cell(AreaType landType)
        {
            _area = landType;
        }

        public bool IsWalkable()
        {
            return _area == AreaType.Walkable;
        }

        public override string ToString()
        {
            return string.Format("Area: {0}", _area);
        }

        public AreaType AreaType
        {
            get { return _area; }
            set { _area = value; }
        }
    }
}