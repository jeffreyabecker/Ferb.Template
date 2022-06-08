using Ferb.Template;
using Ferb.Template.Precompiled;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace TestProject1
{

    public class PrecompiledTestContext
    {

    }
    [Template("/views/Hello.html.txt", typeof(PrecompiledTestContext))]
    public partial class PrecompiledTest : PrecompiledTemplate<PrecompiledTestContext>
    {
#pragma warning disable IDE1006 // Naming Styles
        private const string SourceFile = @"/views/Hello.html.txt";


#pragma warning restore IDE1006 // Naming Styles
    }

    public class UnitTest1
    {


        [Fact]
        public void Test1()
        {
            var cs = File.ReadAllText(@"c:\ode\foo.txt");
            var tree = CSharpSyntaxTree.ParseText(cs, path: @"c:\ode\foo.txt");
            var declaration = tree.GetRoot().DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();


        }


    }


}