using Ranorex;

using UITestDsl.Exceptions;

namespace UITestDsl.Actions
{
    public class ClickAction : Action
    {
        private readonly string _name;

        public ClickAction( string name )
        {
            _name = name;
        }

        public override string ToString()
        {
            return "Click " + _name;
        }

        public override void Execute( IContext ctx )
        {
            Element x = null;

            switch ( _name )
            {
                case "X":
                    x = ctx.Form.FindButton( 5 ).Element;
                    break;

                default:
                    Control toolbar = ctx.Form.FindControlName( "Standard" );
                    x = toolbar.Element.FindChild( Role.PushButton, _name );
                    break;
            }


            if ( x == null )
            {
                throw new ToolbarButtonNotFoundException( _name );
            }

            Mouse.ClickElement( x );
        }
    }
}