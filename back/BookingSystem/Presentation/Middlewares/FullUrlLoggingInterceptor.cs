using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpLogging;

namespace BookingSystem.Presentation.Middlewares;

// Adds the full request URL (scheme, host, path and query) to the HTTP log entry
public class FullUrlLoggingInterceptor : IHttpLoggingInterceptor
{
    public ValueTask OnRequestAsync(HttpLoggingInterceptorContext logContext)
    {
        logContext.AddParameter("Url", logContext.HttpContext.Request.GetDisplayUrl());
        return ValueTask.CompletedTask;
    }

    public ValueTask OnResponseAsync(HttpLoggingInterceptorContext logContext) => ValueTask.CompletedTask;
}
