namespace DreamCompiler.Scanner
{
    public interface IScanner
    {
        IToken ReadToken();
        void PutTokenBack();
        void BufferInitialLoad();
        bool TryReadBufferBytes();
    }
}
