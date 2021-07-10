using System;
using System.IO;
using System.Text;
using com.mobiquity.packer;
using com.mobiquity.packer.Validations;
using Xunit;

namespace PackerTests
{
    public class LineValidationTests : TestBase
    {
        [Theory]
        [InlineData("81:items")]
        [InlineData("81.1:items")]
        [InlineData("81.1 :items")]
        [InlineData("81.1 : items")]
        public void Should_accept_float_max_weight(string line)
        {
            line.ValidateLineFormatAndGetMaxWeight(0, Config);
        }

        [Theory]
        // Greater than max weight 100
        [InlineData("101:items")]
        // Negative number
        [InlineData("-81.1:items")]
        public void Float_max_weight_should_be_in_range(string line)
        {
            Assert.Throws<APIException>(() => line.ValidateLineFormatAndGetMaxWeight(0, Config));
        }

        [Theory]
        [InlineData("81:items")]
        [InlineData("81.1 : ()")]
        [InlineData("81.1:(items)")]
        // Index must be a whole number
        [InlineData("81.1 :(1.1,2,3)")]
        public void Should_not_find_invalid_groups(string line)
        {
            Assert.Throws<APIException>(() => line.ValidateAndParseItems(0, Config));
        }

        [Theory]
        [InlineData("81:(1,1,€1)")]
        // Currency symbol should be optional
        [InlineData("81:(1,1,1)")]
        public void Should_find_groups(string line)
        {
            line.ValidateAndParseItems(0, Config);
        }
    }
}