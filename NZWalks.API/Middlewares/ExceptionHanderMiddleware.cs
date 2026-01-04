using System.Net;

namespace NZWalks.API.Middlewares
{
    public class ExceptionHanderMiddleware
    {
        private readonly ILogger<ExceptionHanderMiddleware> logger;
        private readonly RequestDelegate next;

        public ExceptionHanderMiddleware(ILogger<ExceptionHanderMiddleware> logger,
            RequestDelegate next) // RequestDelegate represents the next middleware in the pipeline
        {
            this.logger = logger;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Call the next middleware in the pipeline
                await next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();

                // Log the exception
                logger.LogError(ex, $"{errorId} : {ex.Message}" );

                // Return a generic error response
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Internal Server Error
                httpContext.Response.ContentType = "application/json";

                var response = new
                {
                    Id = errorId,
                    ErrorMessage = "An unexpected error occurred. Please try again later."
                };

                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }

    }
}
