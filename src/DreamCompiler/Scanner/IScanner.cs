namespace DreamCompiler.Scan
{
    public interface IScanner
    {
        IToken ReadToken();
        void PutTokenBack();
        void BufferInitialLoad();
        bool TryReadBufferBytes();
    }
}
