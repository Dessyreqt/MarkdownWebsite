namespace MarkdownWebsite.Infrastructure.CommandProcessing
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Serilog;

    public class PerformanceLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var watch = Stopwatch.StartNew();
            Log.Debug("Handling {RequestName} with reqest {@Request}", typeof(TRequest).FullName, request);

            TResponse response;

            try
            {
                response = await next();
            }
            finally
            {
                watch.Stop();
                Log.Debug("Handled {RequestName} in {Elapsed:000}ms", typeof(TRequest).FullName, watch.ElapsedMilliseconds);
            }

            return response;
        }
    }
}