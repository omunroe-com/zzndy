using System;

namespace UITestDsl.Exceptions
{
    /// <summary>
    /// Base exception for all exceptions risen by UIT DSL code.
    /// </summary>
    public class UitException : ApplicationException
    {
        /// <summary>
        /// Initializes new isntance of <see cref="UitException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public UitException( string message )
            : base( String.Format( message ) )
        {
        }

        /// <summary>
        /// Initializes new isntance of <see cref="UitException"/> class with 
        /// a specified error message and a reference to the inner exception 
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public UitException( string message, Exception innerException )
            : base( String.Format( message), innerException ) 
        {
        }

        /// <summary>
        /// Initializes new isntance of <see cref="UitException"/> class with
        /// an error message constructed by formatting fiven format string with 
        /// specified arguments.
        /// </summary>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An <see cref="Object"/> array containing zero or more objects to format.</param>
        public UitException( string format, params object[] args )
            : base( String.Format( format, args ) )
        {
        }

        /// <summary>
        /// Initializes new isntance of <see cref="UitException"/> class with
        /// an error message constructed by formatting fiven format string with 
        /// specified arguments and a reference to the inner exception 
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An <see cref="Object"/> array containing zero or more objects to format.</param>
        public UitException( Exception innerException, string format, params object[] args )
            : base( String.Format( format, args ), innerException )
        {
        }
    }
}