using System.Collections.Generic;
using System.Diagnostics;

using QUT.Gppg;

using UITestDsl.Actions;

namespace UITestDsl
{
    public partial class Parser : ShiftReduceParser<ValueType, LexLocation>
    {
        private readonly Queue<BaseAction> _actions = new Queue<BaseAction>();

        public Parser( AbstractScanner<ValueType, LexLocation> scanner )
            : base( scanner )
        {
        }

        private void AddClickToolbarAction( string buttonName )
        {
            _actions.Enqueue( new ToolbarClickAction( buttonName ) );
        }

        private void AddClickSpecialAction( ESpecialAction action )
        {
            _actions.Enqueue( new SpecialAction( action ) );
        }

        private void AddSwitchAction( string formName )
        {
            _actions.Enqueue( new SwitchAction( formName ) );
        }

        private void AddSelectAction( string listItem, string listName )
        {
            _actions.Enqueue( new SelectAction( listItem, listName ) );
        }

        private void AddNewFormExpectation( string name, string var )
        {
            _actions.Enqueue( new AssertFormAction( name, var ) );
        }

        private void RunApplication( string path )
        {
            _actions.Enqueue( new RunAppAction( path ) );
        }

        private void AddWaitAction( int timeout )
        {
            _actions.Enqueue( new WaitAction( timeout ) );
        }

        private void AddCloseAction( string formId )
        {
            _actions.Enqueue( new CloseAction( formId ) );
        }

        private void trace( string msg )
        {
            Trace.WriteLine( msg );
        }

        public Queue<BaseAction> GetActions()
        {
            return _actions;
        }
    }
}