using System.Collections.Generic;
using System.Diagnostics;

using QUT.Gppg;

namespace UITestDsl
{

    public partial class Parser : ShiftReduceParser<ValueType, LexLocation>
    {
        readonly Queue<Action> _actions = new Queue<Action>();

        public Parser( AbstractScanner<ValueType, LexLocation> scanner )
            : base( scanner )
        {
        }

        private void AddClickAction( string buttonName )
        {
            _actions.Enqueue( new ClickAction( buttonName ) );
        }

        private void AddSwitchAction( string formName )
        {
            _actions.Enqueue( new SwitchAction( formName ) );
        }

        private void AddSelectAction( string listItem, string listName )
        {
            _actions.Enqueue( new SelectAction( listItem, listName ) );
        }

        private void AddNewFormExpectation( string name, string var)
        {
            
        }

        private void trace (string msg)
        {
            Trace.WriteLine( msg );
        }

    }
}