using Ferb.Template;
using Ferb.Template.Impl;
using Xunit;

namespace TestProject1
{
    public class BoundryRecogniserTests
    {
        private readonly BoundryRecognizer _recognizer = new BoundryRecognizer(BoundryStyle.Asp);
        private void CanRecognise(int index, string code, (bool success, int length, TokenType type) expected)
        {
            var actual = _recognizer.IsOpen(index, code);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void CanRegoniseEnd()
        {

        }
        [Fact]
        public void CanRecogniseAppend() => CanRecognise(0, "<%=", (true, 3, TokenType.Append));
        [Fact]
        public void CanRecogniseDirective() => CanRecognise(0, "<%#", (true, 3, TokenType.Directive));
        [Fact]
        public void CanRecogniseMember() => CanRecognise(0, "<%@", (true, 3, TokenType.Member));
        [Fact]
        public void CanRecogniseInvoke() => CanRecognise(0, "<%&", (true, 3, TokenType.Invoke));
        [Fact]
        public void CanRecogniseCode() => CanRecognise(0, "<% this.Foo", (true, 2, TokenType.Code));
        [Fact]
        public void CanRecogniseContent() => CanRecognise(0, "<html><body>", (false, 0, TokenType.Content));
    }
}