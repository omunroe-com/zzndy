using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Ranorex;

using UITestDsl.Actions;

namespace UITestDsl
{
    public class Tester : IContext
    {
        private readonly Queue<BaseAction> _actions;
        private readonly Stack<Form> _forms;
        private readonly Dictionary<string, Form> _aliases;
        private int _timeout = 200;

        public void RunScript()
        {
            foreach ( BaseAction action in _actions )
            {
                action.Execute( this );
                Thread.Sleep( _timeout );
            }

            if ( Form != null )
            {
                Form.Close();
            }
        }

        public Tester( Queue<BaseAction> actions )
        {
            _actions = actions;
            _forms = new Stack<Form>();
            _aliases = new Dictionary<string, Form>();
        }

        public static int RanorexMain( string[] args )
        {
            Form _form;
            int error = 0;

            try
            {
                Environment.CurrentDirectory =
                    @"D:\workspace\enerdeq-tfs\branches\AssetBank_2.1_C7\AssetBank\build\bin\Debug\";
                Process.Start( "Enerdeq.exe" );


                MenuButtonClick query = new MenuButtonClick( "IHS AssetBank" );
                query.Click( "Query" );
                _form = Application.FindFormTitle( "Query - Query1 - IHS AssetBank" );
                _form.Close();

                MenuButtonClick vg = new MenuButtonClick( "IHS AssetBank" );
                vg.Click( "View Globals" );
                Application.Sleep( 25000 );
                _form = Application.FindFormTitle( "DataCard - IHS AssetBank" );
                _form.Close();

                MenuButtonClick newfield = new MenuButtonClick( "IHS AssetBank" );
                newfield.Click( "New Field" );
                _form = Application.FindFormTitle( "DataCard 1 - IHS AssetBank" );
                _form.Close();

                MenuButtonClick newblock = new MenuButtonClick( "IHS AssetBank" );
                newblock.Click( "New Block" );
                _form = Application.FindFormTitle( "DataCard 2 - IHS AssetBank" );
                _form.Close();

                MenuButtonClick newbcompany = new MenuButtonClick( "IHS AssetBank" );
                newbcompany.Click( "New Company" );
                _form = Application.FindFormTitle( "DataCard 3 - IHS AssetBank" );
                _form.Close();

                MenuButtonClick newcomplex = new MenuButtonClick( "IHS AssetBank" );
                newcomplex.Click( "New Complex" );
                _form = Application.FindFormTitle( "DataCard 4 - IHS AssetBank" );
                _form.Close();

                MenuClick file = new MenuClick( "IHS AssetBank" );
                file.Click( "File", "New" );

                MenuClick view = new MenuClick( "IHS AssetBank" );
                view.Click( "View", "Toolbars" );

                MenuClick tools = new MenuClick( "IHS AssetBank" );
                tools.Click( "Tools", "Change Unit System" );

                MenuClick help = new MenuClick( "IHS AssetBank" );
                help.Click( "Help", "Help Content" );
            }
            catch ( ValidationException )
            {
                Logger.Error( "Validation failed!" );
                error = -1;
            }
            catch ( RanorexException e )
            {
                Logger.Error( e.ToString() );
                error = -1;
            }
            finally
            {
                Logger.OpenLogViewer();
            }

            return error;
        }

        #region Implementation of IContext

        /// <summary>
        /// Gets current form.
        /// </summary>
        public Form Form
        {
            get
            {
                return _forms.Peek();
            }
        }

        public int Timeout
        {
            get
            {
                return _timeout;
            }
        }

        /// <summary>
        /// Add <see cref="Ranorex.Form"/> provided to the form stack and sets it as current.
        /// </summary>
        /// <param name="form"></param>
        public void AddForm( Form form )
        {
            if ( form == null )
            {
                throw new ArgumentNullException( "form" );
            }

            _forms.Push( form );
        }

        /// <summary>
        /// Add <see cref="Ranorex.Form"/> provided to the aliases collection aliases
        /// with given alias.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="alias"></param>
        public void AddForm( Form form, string alias )
        {
            if ( form == null )
            {
                throw new ArgumentNullException( "form" );
            }
            if ( String.IsNullOrEmpty( alias ) )
            {
                throw new ArgumentNullException( "alias" );
            }

            _aliases[ alias ] = form;
        }

        /// <summary>
        /// Get <see cref="Ranorex.Form"/> aliased by <paramref name="alias"/>.
        /// </summary>
        /// <param name="alias"></param>
        public Form GetForm( string alias )
        {
            if ( String.IsNullOrEmpty( alias ) )
            {
                throw new ArgumentNullException( "alias" );
            }

            Form form = null;
            if ( _aliases.ContainsKey( alias ) )
            {
                form = _aliases[ alias ];
            }

            return form;
        }

        #endregion
    }
}