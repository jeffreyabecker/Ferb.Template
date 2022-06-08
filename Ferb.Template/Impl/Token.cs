namespace Ferb.Template.Impl;

internal record Token(string Content, TokenType Type, int Line, int Id)
{
    private static Dictionary<int, (int line, int col)> CreateLineIndex(string code)
    {
        var result = new Dictionary<int, (int line, int col)>();
        var line = 1;
        var lineStart = 0;
        for (var i = 0; i < code.Length; i++)
        {
            result.Add(i, (line, i - lineStart));
            if (code[i] == '\n')
            {
                lineStart = i + 1;
                line++;
            }
        }
        return result;
    }

    public static IEnumerable<Token> Parse(string code, IRecognizeBoundries boundries)
    {
        var id = 0;
        var i = 0;
        var lastEnd = 0;
        var index = CreateLineIndex(code);

        while (i < code.Length)
        {
            var open = boundries.IsOpen(i, code);
            if (open.success)
            {
                var contentLen = i - lastEnd;
                if (contentLen > 0)
                {
                    yield return new Token(code.Substring(lastEnd, i - lastEnd), TokenType.Content, index[i].line, id++);
                }
                var close = boundries.FindClose(i, code);

                if (close.position == -1)
                {
                    throw new Exception("Unmatched close boundry");
                }

                var startOfOpen = i;
                var lengthOfOpen = open.length;
                var startOfCode = startOfOpen + lengthOfOpen;
                var lengthOfCode = close.position - startOfCode;
                var startOfClose = close.position;
                var lengthOfClose = close.length;


                yield return new Token(code.Substring(startOfCode, lengthOfCode), open.type, index[startOfCode].line, -1);

                lastEnd = startOfClose + lengthOfClose;
                i = lastEnd;
            }
            else
            {
                i++;
            }
        }
        var rest = code.Substring(lastEnd, code.Length - lastEnd);
        if (!String.IsNullOrEmpty(rest))
        {
            yield return new Token(rest, TokenType.Content, index[lastEnd].line, id++);
        }
    }
}