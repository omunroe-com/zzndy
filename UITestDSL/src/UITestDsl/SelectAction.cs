using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UITestDsl
{
    public class SelectAction : Action
    {
        private readonly string _item;
        private readonly string _name;

        public SelectAction( string item, string name )
        {
            _item = item;
            _name = name;
        }
    }
}
