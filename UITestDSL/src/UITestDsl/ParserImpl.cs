using System.Collections.Generic;
using System.Diagnostics;

using QUT.Gppg;

using UITestDsl.Actions;

namespace UITestDsl
{

    public partial class Parser : ShiftReduceParser<ValueType, LexLocation>
    {
        readonly Queue<Action> _actions = new Queue<Action>();

        public Parser( AbstractScanner<ValueType, LexLocation> scanner )
            : base( scanner )
        {
        }

        private void AddClickToolbarAction( string buttonName )
        {
            _actions.Enqueue( new ClickAction( buttonName ) );
        }

        private void AddClickSpecialAction( string name )
        {
            _actions.Enqueue( new ClickAction( name ) );
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
            _actions.Enqueue( new AssertFormAction(name, var) );
        }

        private void RunApplication(string path)
        {
            _actions.Enqueue( new RunAppAction(path) );
        }

        private void trace (string msg)
        {
            Trace.WriteLine( msg );
        }

        public Queue<Action> GetActions()
        {
            return _actions;
        }
    }
}