using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ranorex;

namespace UITestDsl
{
    class Tester
    {
        public static int RanorexMain( string[] args )
        {
            Form _form;
            int error = 0;

            try
            {
                System.Environment.CurrentDirectory = @"D:\workspace\enerdeq-tfs\branches\AssetBank_2.1_C7\AssetBank\build\bin\Debug\";
                System.Diagnostics.Process.Start( "Enerdeq.exe" );


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
    }
}
