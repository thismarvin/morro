using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Morro.Core.Exceptions
{
    class MissingMethodException : Exception
    {
        public MissingMethodException() : base("A particular method was expected to be called, but it appears it never was.")
        {
        }

        public MissingMethodException(string message) : base(message)
        {
        }

        public MissingMethodException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingMethodException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
