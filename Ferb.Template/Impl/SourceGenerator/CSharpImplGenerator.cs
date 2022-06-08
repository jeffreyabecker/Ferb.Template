using Ferb.Template.Impl;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ferb.Template.Impl.SourceGenerator;

internal class CSharpImplGenerator : Ferb.Template.TemplateBase<Ferb.Template.Impl.GenerationConext>
{
    private readonly static string[] usings = new string[] { "System", "System.Collections.Generic", "System.Linq", "System.Text" };

    void BeginCode(int line) => Output.WriteLine($"#line {line}");
    void EndCode() => Output.WriteLine($"#line hidden");
    void Emit(string content) => Output.Write(content);
    void EmitLine(string content = "") => Output.WriteLine(content);
    int level = 0;
    void BeginBlock()
    {
        EmitIndent();
        Output.WriteLine("{");
        level++;
    }
    void EndBlock(params string[] includeLines)
    {
        level--;
        EmitIndent();
        Output.Write("}");
        foreach (var l in includeLines)
        {
            Output.WriteLine(l);
        }
        Output.WriteLine();

    }
    void EmitIndent() => Output.Write(new String(' ', 2 * level));
#line hidden
    protected override void ExecInternal()
    {
        Output.WriteLine("#line hidden");
        foreach (var u in usings)
        {
            Output.WriteLine($"using {u};");
        }


        foreach (var d in Context.Tokens.Where(t => t.Type == Ferb.Template.Impl.TokenType.Directive))
        {
            BeginCode(d.Line);
            EmitLine(d.Content);
            EndCode();
        }
        EmitLine($"namespace {Context.Namespace}");
        BeginBlock();

        var templateClassName = Context.IsPrecompiled ? typeof(PrecompiledTemplate<>).Name : typeof(TemplateBase<>).Name;
        EmitIndent();
        EmitLine($"public partial class {Context.ClassName} : global::Ferb.Template.{templateClassName}<{Context.ContextClassName}>");
        BeginBlock();

        var lookup = EmitChunkIndex();
        if (!Context.IsPrecompiled)
        {
            EmitIndent();
            EmitLine($"public {Context.ClassName} (global::Ferb.Template.Impl.IInstantiateTemplates engine) : base(engine) {{ }}");
        }
        else
        {

            EmitIndent();
            EmitLine($"public static {Context.ClassName} () {{ global::Ferb.Template.Impl.Precompilation.PrecompiledTemplateRegistry.Instance.Register<{Context.ClassName}, {Context.ContextClassName}>(\"{EscapeString(Context.FileName)}\"); }}");
        }

        foreach (var d in Context.Tokens.Where(t => t.Type == TokenType.Member))
        {
            BeginCode(d.Line);
            Emit(d.Content);
            EmitLine();
            EndCode();

        }
        EmitLine();

        EmitIndent();
        EmitLine("protected override void ExecInternal()");
        BeginBlock();



        var tokens = Context.Tokens.Where(x => x.Type != TokenType.Directive && x.Type != TokenType.Member).ToList();


        foreach (var t in tokens)
        {
            if (t.Type == TokenType.Content)
            {
                BeginCode(t.Line);
                EmitIndent();
                EmitLine($"Output.Write(__literal_chunks[{lookup[t.Id]}]);");
                EndCode();

            }
            else if (t.Type == TokenType.Append)
            {
                BeginCode(t.Line);
                EmitIndent();
                EmitLine($"Output.Write(({t.Content}));");
                EndCode();

            }
            else if (t.Type == TokenType.Invoke)
            {
                var parts = t.Content.Split(new[] { ',' }, 2).Select(x => x.Trim()).ToArray();
                BeginCode(t.Line);
                EmitIndent();
                EmitLine($"Output.Write(Call(\"{EscapeString(parts[0])}\", ({parts[1]})));");
                EndCode();
            }
            else
            {
                BeginCode(t.Line);
                EmitLine(t.Content);
                EndCode();
            }
        }
        Output.Write("\r\n      }\r\n      \r\n    }\r\n}");
        EndBlock();// Override
        EndBlock(); // class
        EndBlock(); // namespace
    }

    public CSharpImplGenerator() : base(new FakeEngine()) { }

    private Dictionary<int, int> EmitChunkIndex()
    {
        Dictionary<int, int> lookup;
        var foo = Context.Tokens.Where(t => t.Type == TokenType.Content)
          .GroupBy(x => x.Content)
          .Select((g, i) =>
          new
          {
              Content = g.Key,
              Index = i,
              Ids = g.Select(x => x.Id).ToList()

          }).ToList();
        lookup = foo.SelectMany(f => f.Ids.Select(id => new { Id = id, Index = f.Index })).ToDictionary(x => x.Id, x => x.Index);
        EmitIndent();


        EmitLine("protected static readonly string[] __literal_chunks = new []");
        BeginBlock();
        foreach (var item in foo)
        {
            EmitIndent();
            Emit("\"");
            Emit(EscapeString(item.Content));
            Emit("\",");
            EmitLine();
        }
        EndBlock(";");
        return lookup;
    }

    protected string EscapeString(string src) => Microsoft.CodeAnalysis.CSharp.SyntaxFactory.LiteralExpression(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StringLiteralExpression, Microsoft.CodeAnalysis.CSharp.SyntaxFactory.Literal(src)).GetText().ToString();


    public static Microsoft.CodeAnalysis.SyntaxTree GenerateFor(GenerationConext ctx)
    {

        var tmpl = new CSharpImplGenerator();
        var src = tmpl.Exec(ctx);
        return CSharpSyntaxTree.ParseText(src, path: ctx.FileName, encoding: tmpl.Output.Encoding);

    }
}