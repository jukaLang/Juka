using DreamCompiler.Lexer;
using DreamCompiler.Scan;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace DreamCompiler
{
    class HostedService : IHostedService
    {
        private ILexicalAnalysis lexicalAnalysis;
        private IScanner scanner;

        public HostedService(
            ILexicalAnalysis lexicalAnalysis,
            IScanner scanner)
        {
            this.lexicalAnalysis = lexicalAnalysis;
            this.scanner = scanner;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.lexicalAnalysis.Analyze(this.scanner);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
