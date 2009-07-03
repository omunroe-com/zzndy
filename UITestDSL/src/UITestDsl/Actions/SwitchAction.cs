using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UITestDsl.Actions
{
    public class SwitchAction:Action
    {
        private readonly string _name;

        public SwitchAction( string name )
        {
            _name = name;
        }

        public override string ToString()
        {
            return "Switch to $" + _name;
        }

        public override void Execute( IContext ctx )
        {
            var form = ctx.GetForm( _name );
            ctx.AddForm(  form);
            form.Activate();

        }
    }
}