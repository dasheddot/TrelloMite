using System;
using System.Runtime.Serialization;

namespace TrelloMite.Exceptions
{
    [Serializable]
    public class CannotParseException : ApplicationException
    {
        public CannotParseException()
        {
        }

        public CannotParseException(string message) : base(message)
        {
        }

        public CannotParseException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CannotParseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}