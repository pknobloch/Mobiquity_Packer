using System;

namespace com.mobiquity.packer
{
    // ReSharper disable once InconsistentNaming
    public class APIException : Exception
    {
        public APIException(string message): base(message)
        {
        }
    }
}