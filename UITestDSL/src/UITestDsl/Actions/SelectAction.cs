using System;

namespace UITestDsl.Actions
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

        public override string ToString()
        {
            return String.Format( "Select {0} in {1}", _item, _name );
        }

        public override void Execute( IContext ctx )
        {
            throw new System.NotImplementedException();
        }
    }
}