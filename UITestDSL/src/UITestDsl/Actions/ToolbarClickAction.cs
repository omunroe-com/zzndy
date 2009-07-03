using Ranorex;

using UITestDsl.Exceptions;

namespace UITestDsl.Actions
{
    public class ToolbarClickAction : BaseAction
    {
        private readonly string _name;

        public ToolbarClickAction( string name )
        {
            _name = name;
        }

        public override string ToString()
        {
            return "Click " + _name;
        }

        public override void Execute( IContext ctx )
        {
            Control toolbar = ctx.Form.FindControlName( "Standard" );

            Element x = toolbar.Element.FindChild( Role.PushButton, _name );

            if ( x == null )
            {
                throw new ToolbarButtonNotFoundException( _name );
            }

            Mouse.ClickElement( x );
        }
    }
}