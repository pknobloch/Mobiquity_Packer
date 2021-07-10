using System;
using System.IO;
using System.Text;
using com.mobiquity.packer;
using com.mobiquity.packer.Validations;
using Xunit;

namespace PackerTests
{
    public class ConfigTests : TestBase
    {
        [Theory]
        [InlineData("invalid_example")]
        [InlineData("not_root\\invalid_example")]
        public void Should_not_accept_relative_paths(string filePath)
        {
            var validator = new FileValidation(filePath, Config);
            Assert.Throws<APIException>(() => validator.ValidateFilePathIsAbsolute());
        }

        [Fact]
        public void Should_not_throw_error_when_UTF8_encoding()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, ExampleInputPath);
            var validation = new FileValidation(filePath, Config);
            validation.ValidateEncoding();
        }
    }
}