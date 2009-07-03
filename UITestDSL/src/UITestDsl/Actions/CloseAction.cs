using Ranorex;

namespace UITestDsl.Actions
{
    internal class CloseAction : BaseAction
    {
        private readonly string _formId;

        public CloseAction( string formId )
        {
            _formId = formId;
        }

        public override void Execute( IContext ctx )
        {
            Element button = ctx.GetForm( _formId ).Element.FindChild( Role.PushButton, "Close" );

            if ( button == null )
            {
                ctx.GetForm( _formId ).Close();
            }
            else
            {
                Mouse.ClickElement( button );
            }
        }

        public override string ToString()
        {
            return "Close " + _formId;
        }
    }
}