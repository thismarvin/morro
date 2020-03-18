using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Morro.Core
{
    class MorroException : Exception
    {
        public MorroException() : base("Something went wrong within the Morro Engine.")
        {
        }

        public MorroException(string message) : base(message)
        {
        }

        public MorroException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MorroException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
