using System;

namespace UITestDsl.Exceptions
{
    internal class FormNotFoundException : UitException
    {
        /// <summary>
        /// Initializes new isntance of <see cref="FormNotFoundException"/> class with
        /// a specified missing form name.
        /// </summary>
        /// <param name="formName">A name of the form that was not found.</param>
        public FormNotFoundException( string formName ) : base( String.Format( "Could not find form {0}", formName ) )
        {
        }

        /// <summary>
        /// Initializes new isntance of <see cref="FormNotFoundException"/> class with 
        /// a specified error message and a reference to the inner exception 
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public FormNotFoundException( string message, Exception innerException ) : base( message, innerException )
        {
        }

        /// <summary>
        /// Initializes new isntance of <see cref="FormNotFoundException"/> class with
        /// an error message constructed by formatting fiven format string with 
        /// specified arguments.
        /// </summary>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An <see cref="Object"/> array containing zero or more objects to format.</param>
        public FormNotFoundException( string format, params object[] args ) : base( format, args )
        {
        }

        /// <summary>
        /// Initializes new isntance of <see cref="FormNotFoundException"/> class with
        /// an error message constructed by formatting fiven format string with 
        /// specified arguments and a reference to the inner exception 
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An <see cref="Object"/> array containing zero or more objects to format.</param>
        public FormNotFoundException( Exception innerException, string format, params object[] args )
            : base( innerException, format, args )
        {
        }
    }
}