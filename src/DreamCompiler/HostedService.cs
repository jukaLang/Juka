using DreamCompiler.Lexer;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace DreamCompiler
{
    class HostedService : IHostedService
    {
        private ILexicalAnalysis lexicalAnalysis;

        public HostedService(
            ILexicalAnalysis lexicalAnalysis)
        {
            this.lexicalAnalysis = lexicalAnalysis;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.lexicalAnalysis.Analyze();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
