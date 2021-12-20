using JukaCompiler.Lexer;
using JukaCompiler.Scan;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace JukaCompiler
{
    class HostedService : IHostedService
    {
        private readonly ILexicalAnalysis lexicalAnalysis;
        private readonly IScanner scanner;

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
