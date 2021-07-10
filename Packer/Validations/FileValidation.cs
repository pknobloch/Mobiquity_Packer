using System.IO;
using System.Text;

namespace com.mobiquity.packer.Validations
{
    public class FileValidation
    {
        private readonly string _filePath;
        private readonly Config _config;

        public FileValidation(string filePath, Config config)
        {
            _filePath = filePath;
            _config = config;
        }
        
        /// <summary>
        /// Check the file based on configuration.
        /// </summary>
        /// <exception cref="com.mobiquity.packer.APIException">Thrown when file path or encoding is invalid.</exception>
        public void Validate()
        {
            ValidateFilePathIsAbsolute();
            ValidateEncoding();
        }

        /// <summary>
        /// Check the file encoding.
        /// </summary>
        /// <exception cref="APIException">Thrown when file encoding is invalid.</exception>
        public void ValidateEncoding()
        {
            if (_config.ExpectedFileEncoding == null) return;
            var encoding = GetEncoding(_filePath);
            if (!_config.ExpectedFileEncoding.Equals(encoding))
            {
                throw new APIException($"Expected file encoding to be '{_config.ExpectedFileEncoding}' but it was '{encoding}'.");
            }
        }

        /// <summary>
        /// Check the file path.
        /// </summary>
        /// <exception cref="APIException">Thrown when file path is invalid</exception>
        public void ValidateFilePathIsAbsolute()
        {
            if (_config.OnlyAllowAbsolutePaths && !Path.IsPathRooted(_filePath))
            {
                throw new APIException($"Absolute path expected. Invalid path: '{_filePath}'.");
            }
        }
        
        private static Encoding GetEncoding(string filePath)
        {
            using var file = new StreamReader(filePath);
            return file.CurrentEncoding;
        }
    }
}