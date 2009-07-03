﻿using System;

namespace UITestDsl.Exceptions
{
    public class ButtonNotFoundException : UitException
    {
        /// <summary>
        /// Initializes new isntance of <see cref="ButtonNotFoundException"/> class with
        /// a specified missing button name.
        /// </summary>
        /// <param name="buttonName">The name of a button that was not found.</param>
        public ButtonNotFoundException( string buttonName ) : base( String.Format( "Could not find button {0}", buttonName ) )
        {
        }

        /// <summary>
        /// Initializes new isntance of <see cref="ButtonNotFoundException"/> class with 
        /// a specified error message and a reference to the inner exception 
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public ButtonNotFoundException( string message, Exception innerException ) : base( message, innerException )
        {
        }

        /// <summary>
        /// Initializes new isntance of <see cref="ButtonNotFoundException"/> class with
        /// an error message constructed by formatting fiven format string with 
        /// specified arguments.
        /// </summary>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An <see cref="Object"/> array containing zero or more objects to format.</param>
        public ButtonNotFoundException( string format, params object[] args ) : base( format, args )
        {
        }

        /// <summary>
        /// Initializes new isntance of <see cref="ButtonNotFoundException"/> class with
        /// an error message constructed by formatting fiven format string with 
        /// specified arguments and a reference to the inner exception 
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An <see cref="Object"/> array containing zero or more objects to format.</param>
        public ButtonNotFoundException( Exception innerException, string format, params object[] args )
            : base( innerException, format, args )
        {
        }
    }
}