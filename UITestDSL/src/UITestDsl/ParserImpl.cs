using QUT.Gppg;

using UITestDsl;

public partial class Parser : ShiftReduceParser<int, LexLocation>
{
    public Parser( AbstractScanner<int, LexLocation> scanner ) : base( scanner )
    {
    }

    private void Add< TAction >( params int[] arg )
         where TAction:Action
    {
    }
}