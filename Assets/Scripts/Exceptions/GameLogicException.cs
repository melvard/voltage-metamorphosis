using System;

namespace Exceptions
{
    public class GameLogicException : ApplicationException
    {
        private static string DefaultMessage => "Unexpected initialization from inspector.";

        public GameLogicException() : base(DefaultMessage)
        {
        }

        public GameLogicException(string message)
            : base(message)
        {
        }

        public GameLogicException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}