using Ferb.Template.Impl;
using Xunit;

namespace TestProject1
{
    public class TemplateResolverTests
    {
        [Fact]
        public void CanLoadFilesFromDir()
        {
            var dir = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(Path.GetTempFileName()));
            Directory.CreateDirectory(dir);
            var templateFile = Path.Combine(dir, "views\\expected.txt");
            Directory.CreateDirectory(Path.Combine(dir, "views"));
            File.WriteAllText(templateFile, "expected");
            var system = new FilesystemTemplateResolver(dir);
            var actual = system.Resolve("~/views/expected.txt");
            Assert.Equal("expected", actual);
            Directory.Delete(dir, true);

        }

        [Fact]
        public void CanResolveResources()
        {
            var resolver = EmbededResourceTemplateResolver.Create<TemplateResolverTests>();
            var actual = resolver.Resolve("/Views/expected.txt");
            Assert.Equal("expected", actual);
        }

    }
}