using Ranorex;

public class MenuClick
{
    private readonly Form _form;
    private readonly Control _mainMenu;

    public MenuClick( string formTitle )
    {
        _form = Application.FindFormTitle( "IHS AssetBank", SearchMatchMode.MatchExact, true, 5000 );
        _form.Activate();

        _mainMenu = _form.FindControlName( "Main Menu" );
    }

    public void Click( params string[] items )
    {
        _form.Activate();
        _mainMenu.Focus();
        Element item = _mainMenu.Element;


        foreach ( string itemName in items )
        {
            item = item.FindChild( Role.MenuItem, itemName );
            Mouse.ClickElement( item );
            //Element[] children = item.FindChildren( Role.MenuItem );

            if ( itemName != null )
            {
                break;
            }
        }
        Mouse.ClickElement( item );
    }
}

public class MenuButtonClick
{
    private readonly Form _form;
    private readonly Control _mainMenu;

    public MenuButtonClick( string formTitle )
    {
        _form = Application.FindFormTitle( "IHS AssetBank", SearchMatchMode.MatchExact, true, 5000 );
        _form.Activate();

        _mainMenu = _form.FindControlName( "Standard" );
    }

    public void Click( params string[] items )
    {
        _form.Activate();
        _mainMenu.Focus();
        Element item = _mainMenu.Element;

        foreach ( string itemName in items )
        {
            item = item.FindChild( Role.PushButton, itemName );
            Mouse.ClickElement( item );
        }
        Mouse.ClickElement( item );
    }
}