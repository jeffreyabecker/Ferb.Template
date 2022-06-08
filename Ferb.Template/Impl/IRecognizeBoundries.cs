namespace Ferb.Template.Impl;

public interface IRecognizeBoundries
{
    (bool success, int length, TokenType type) IsOpen(int index, string code);

    (int position, int length) FindClose(int index, string code);

}