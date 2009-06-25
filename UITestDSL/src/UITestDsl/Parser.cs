using QUT.Gppg;

using UITestDsl;

public partial class Parser : ShiftReduceParser<int, LexLocation>
{
    public Parser( AbstractScanner<int, LexLocation> scanner ) : base( scanner )
    {
    }

    private void Add< TAction >( params string[] arg )
         where TAction:Action
    {
    }
}