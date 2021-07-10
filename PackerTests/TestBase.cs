using com.mobiquity.packer;

namespace PackerTests
{
    public class TestBase
    {
        protected readonly Config Config = new();
        protected const string ExampleInputPath = "Resources\\example_input.txt";
    }
}