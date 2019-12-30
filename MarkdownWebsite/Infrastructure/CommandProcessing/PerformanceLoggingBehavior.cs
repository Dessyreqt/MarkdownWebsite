namespace MarkdownWebsite.Infrastructure.CommandProcessing
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using MediatR;

    public class PerformanceLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var watch = new Stopwatch();
            watch.Start();
            Logger.Instance.Debug("Handling {RequestName}", typeof(TRequest).Name);

            TResponse response;

            try
            {
                response = await next();
            }
            finally
            {
                watch.Stop();
                Logger.Instance.Debug("Handled {RequestName} in {Elapsed:000}ms", typeof(TRequest).Name, watch.ElapsedMilliseconds);
            }

            return response;
        }
    }
}