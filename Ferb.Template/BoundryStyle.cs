namespace Ferb.Template;

public record BoundryStyle(string OpenSigil, string CloseSigil, string AppendModifier = "=", string DirectiveModifier = "#", string MemberModifier = "@", string InvokeModifier = "&")
{
    public static readonly BoundryStyle Asp = new BoundryStyle("<%", "%>");
    public static readonly BoundryStyle Php = new BoundryStyle("<?", "?>");
    public static readonly BoundryStyle T4 = new BoundryStyle("<#", "#>", DirectiveModifier: "!");
    public static readonly BoundryStyle Handlebars = new BoundryStyle("{{", "}}");
    public static readonly BoundryStyle TemplateToolkit = new BoundryStyle("[%", "%]");


}