namespace MarkdownWebsite.Infrastructure.CommandProcessing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Serilog;

    public class ExceptionLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse response = default;

            try
            {
                response = await next();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception handling {RequestName} when called with request {@Request}", typeof(TRequest).FullName, request);
            }

            return response;
        }
    }
}