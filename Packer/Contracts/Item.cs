namespace com.mobiquity.packer.Contracts
{
    public class Item
    {
        public string Index { get; }
        public float Weight { get; }
        public float Cost { get; }

        public Item(string index, float weight, float cost)
        {
            Index = index;
            Weight = weight;
            Cost = cost;
        }

        public override string ToString()
        {
            return $"{{Index:'{Index}',Weight:'{Weight}',Cost:'{Cost}'}}";
        }
    }
}