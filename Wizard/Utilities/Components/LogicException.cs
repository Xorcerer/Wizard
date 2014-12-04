using System;

namespace Camp.WallBreaker
{
    /// <summary>
    /// logic exception.
    /// Message from this exception (not including the inner one),
    /// is permitted and necessary to inform client.
    /// </summary>
    [Serializable]
    public class LogicException : Exception
    {
        public const int UnknownError = 1;
        public int ErrorEnumNumber { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LogicException"/> class
        /// </summary>
        /// <param name="errorEnumNumber">A <see cref="T:int"/> that represent the error in protocol enum. </param>
        public LogicException(int errorEnumNumber)
        {
            ErrorEnumNumber = errorEnumNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LogicException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        /// <param name="errorEnumNumber">A <see cref="T:int"/> that represent the error in protocol enum. </param>
        public LogicException(string message, int errorEnumNumber = UnknownError) : base (message)
        {
            ErrorEnumNumber = errorEnumNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LogicException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        /// <param name="inner">The exception that is the cause of the current exception. </param>
        public LogicException(string message, Exception inner) : base (message, inner)
        {
        }
    }
}

