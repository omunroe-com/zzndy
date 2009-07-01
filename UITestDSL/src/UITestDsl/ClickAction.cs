using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UITestDsl
{
    public class ClickAction : Action
    {
        private readonly string _name;

        public ClickAction( string name )
        {
            _name = name;
        }
    }
}
