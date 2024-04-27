namespace Schemes.Dashboard
{
    public interface IGridPathNode<T> : IGridElement<T> where T: class
    {
        public int gCost { get; set; }
        public int hCost { get; set; }
        public int fCost { get; set; }
        
        public T nodeCameFrom { get; set;}

        public bool IsWalkable { get;  }
        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}