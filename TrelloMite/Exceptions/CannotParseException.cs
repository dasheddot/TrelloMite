using System;
using System.Runtime.Serialization;

namespace TrelloMite
{
    [Serializable]
    public class CannotParseException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

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