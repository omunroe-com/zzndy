using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UITestDsl.Actions
{
    public abstract class Action
    {
        abstract public void Execute( IContext ctx );
    }
}