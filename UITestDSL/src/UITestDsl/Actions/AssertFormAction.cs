using System;

using Ranorex;

using UITestDsl.Exceptions;

namespace UITestDsl.Actions
{
    internal class AssertFormAction : Action
    {
        private readonly string _name;
        private readonly string _var;

        public AssertFormAction( string name, string var )
        {
            _name = name;
            _var = var;
        }

        public override string ToString()
        {
            return String.Format( "Expecting form {0} to open as ${1}", _name, _var );
        }

        public override void Execute( IContext ctx )
        {
            Form form = Application.FindFormTitle( _name, SearchMatchMode.MatchFromStart );
            if ( form == null )
                throw new FormNotFoundException( _name );

            ctx.AddForm( form, _var );
        }
    }
}