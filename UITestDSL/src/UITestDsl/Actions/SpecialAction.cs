using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UITestDsl.Actions
{
    internal enum ESpecialAction
    {
        Close
    }

    class SpecialAction:BaseAction
    {
        private readonly ESpecialAction _action;

        public SpecialAction(ESpecialAction action)
        {
            _action = action;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void Execute( IContext ctx )
        {
            switch ( _action )
            {
                case ESpecialAction.Close:
                    ctx.Form.Close();
                    break;
            }
        }
    }
}
