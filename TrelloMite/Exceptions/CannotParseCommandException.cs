using System;
using System.Runtime.Serialization;

namespace TrelloMite.Exceptions
{
    [Serializable]
    public class CannotParseCommandException : ApplicationException
    {
        public CannotParseCommandException()
        {
        }

        public CannotParseCommandException(string message) : base(message)
        {
        }

        public CannotParseCommandException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CannotParseCommandException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}