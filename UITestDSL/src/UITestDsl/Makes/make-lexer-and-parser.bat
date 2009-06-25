echo ====================
echo  Generating Scanner
echo ====================

..\..\bin\gplex.exe /out:- Grammar\uit.lexer > Scanner.cs

echo ===================
echo  Generating Parser
echo ===================

..\..\bin\gppg.exe /nolines Grammar\uit.parser > Parser.cs