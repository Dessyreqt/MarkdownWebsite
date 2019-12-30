namespace MarkdownWebsite.Infrastructure.CommandProcessing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using MediatR;

    public class ExceptionLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse response = default(TResponse);

            try
            {
                response = await next();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Exception caught: {Exception}", ex);
                Console.WriteLine($"Error: {ex.Message}");
            }

            return response;
        }
    }
}