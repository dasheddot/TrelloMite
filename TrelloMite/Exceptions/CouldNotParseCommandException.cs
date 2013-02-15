using System;
using System.Runtime.Serialization;

namespace TrelloMite
{
    [Serializable]
    public class CouldNotParseCommandException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public CouldNotParseCommandException()
        {
        }

        public CouldNotParseCommandException(string message) : base(message)
        {
        }

        public CouldNotParseCommandException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CouldNotParseCommandException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}