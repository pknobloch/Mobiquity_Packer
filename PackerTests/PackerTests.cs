using System;
using System.IO;
using System.Linq;
using System.Text;
using com.mobiquity.packer;
using Xunit;

namespace PackerTests
{
    public class PackerTests : TestBase
    {
        [Theory]
        [InlineData("4", "81 : (1,53.38,€45) (2,88.62,€98) (3,78.48,€3) (4,72.30,€76) (5,30.18,€9) (6,46.34,€48)")]
        // Greater weight than the package weight
        [InlineData("-", "8 : (1,15.3,€34)")]
        [InlineData("2,7", "75 : (1,85.31,€29) (2,14.55,€74) (3,3.98,€16) (4,26.24,€55) (5,63.69,€52) (6,76.25,€75) (7,60.02,€74) (8,93.18,€35) (9,89.95,€78)")]
        [InlineData("8,9", "56 : (1,90.72,€13) (2,33.80,€40) (3,43.15,€10) (4,37.97,€16) (5,46.81,€36) (6,48.77,€79) (7,81.80,€45) (8,19.36,€79) (9,6.76,€64)")]
        // You would prefer to send a package which weighs less in case there is more than one package with the same price
        [InlineData("1,2", "100 : (1,49,€1) (2,49,€1) (3,50,€1) (4,50,€1)")]
        public void Should_pack_valid_lines(string expected, string line)
        {
            var output = Packer.PackLine(line, 0, Config);
            Assert.Equal(expected, output);
        }
        
        [Theory]
        // Duplicate index
        [InlineData("8 : (1,15.3,€34) (1,15.3,€34)")]
        // Max item weight exceeded
        [InlineData("8 : (1,101,€34)")]
        // Max package weight exceeded
        [InlineData("101 : (1,10,€34)")]
        public void Should_not_pack_invalid_lines(string line)
        {
            Assert.Throws<APIException>(() => Packer.PackLine(line, 0, Config));
        }
        
        [Fact]
        public void Should_not_allow_more_than_max_items()
        {
            var line = "100 : " + string.Join("", Enumerable.Range(1, 16).Select(i => $" ({i},1,€1)"));
            Assert.Throws<APIException>(() => Packer.PackLine(line, 0, Config));
        }
    }
}