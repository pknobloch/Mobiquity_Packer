using System.Collections.Generic;

namespace com.mobiquity.packer.Contracts
{
    internal class SummedItems
    {
        public float TotalWeight { get; init; }
        public float TotalCost { get; init; }
        public List<Item> Items { get; init; }

        public override string ToString()
        {
            return $"{{Weight:'{TotalWeight}',Cost:'{TotalCost}',Items:{string.Join(", ", Items)}}}";
        }
    }
}