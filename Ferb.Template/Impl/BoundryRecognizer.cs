namespace Ferb.Template.Impl;

public class BoundryRecognizer : IRecognizeBoundries
{
    private readonly BoundryStyle _style;

    public BoundryRecognizer(BoundryStyle style)
    {
        if (string.IsNullOrEmpty(style.OpenSigil))
        {
            throw new ArgumentException($"'style.OpenSigil' cannot be null or empty.", "style.OpenSigil)");
        }

        if (string.IsNullOrEmpty(style.CloseSigil))
        {
            throw new ArgumentException($"'style.CloseSigil' cannot be null or empty.", "style.CloseSigil)");
        }

        if (string.IsNullOrEmpty(style.AppendModifier))
        {
            throw new ArgumentException($"'style.AppendModifier' cannot be null or empty.", "style.AppendModifier)");
        }

        if (string.IsNullOrEmpty(style.DirectiveModifier))
        {
            throw new ArgumentException($"'style.DirectiveModifier' cannot be null or empty.", "style.DirectiveModifier)");
        }

        if (string.IsNullOrEmpty(style.MemberModifier))
        {
            throw new ArgumentException($"'style.MemberModifier' cannot be null or empty.", "style.MemberModifier)");
        }

        if (string.IsNullOrEmpty(style.InvokeModifier))
        {
            throw new ArgumentException($"'style.style.InvokeModifier' cannot be null or empty.", "style.style.InvokeModifier)");
        }
        _style = style;
        //_style = new BoundryStyle(style.OpenSigil, style.CloseSigil, style.OpenSigil + style.AppendModifier, style.OpenSigil + style.DirectiveModifier, style.OpenSigil + style.MemberModifier, style.OpenSigil + style.InvokeModifier);

    }

    public (int position, int length) FindClose(int index, string code)
    {
        var position = code.IndexOf(_style.CloseSigil, index);
        return (position, _style.CloseSigil.Length);
    }
    public (bool success, int length, TokenType type) IsOpen(int index, string code)
    {

        bool matches(string value, int? idx)
        {
            return String.CompareOrdinal(code, idx ?? index, value, 0, value.Length) == 0;
        }
        bool matchesModifier(string modifier)
        {
            return matches(modifier, index + _style.OpenSigil.Length);
        }
        if (matches(_style.OpenSigil, index))
        {
            if (matchesModifier(_style.AppendModifier))
            {
                return (true, _style.OpenSigil.Length + _style.AppendModifier.Length, TokenType.Append);
            }
            if (matchesModifier(_style.DirectiveModifier))
            {
                return (true, _style.OpenSigil.Length + _style.DirectiveModifier.Length, TokenType.Directive);
            }
            if (matchesModifier(_style.InvokeModifier))
            {
                return (true, _style.OpenSigil.Length + _style.InvokeModifier.Length, TokenType.Invoke);
            }
            if (matchesModifier(_style.MemberModifier))
            {
                return (true, _style.OpenSigil.Length + _style.MemberModifier.Length, TokenType.Member);
            }
            return (true, _style.OpenSigil.Length, TokenType.Code);
        }
        return (false, 0, TokenType.Content);
    }
}