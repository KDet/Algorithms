namespace GameAlgorithms.Lands
{
    public enum AreaType
    {
        Walkable = 0,
        Wall = 2,
        Water = 4,
        UnWalkable = Wall| Water
    }
}