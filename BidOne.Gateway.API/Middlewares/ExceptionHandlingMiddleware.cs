using BidOne.Gateway.Domain.Enums;
using BidOne.Gateway.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace BidOne.Gateway.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); 
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "An unexpected error occurred.";

            switch (exception)
            {
                case BidOneException bidOneException:
                    switch(bidOneException.exceptionType)
                    {
                        case ExceptionType.Validation:
                            statusCode = HttpStatusCode.BadRequest;
                            break;
                        case ExceptionType.NotFound:
                            statusCode = HttpStatusCode.NotFound;
                            break;                
                        default:
                            statusCode = HttpStatusCode.InternalServerError;
                            break;
                    }
                    message = bidOneException.Message;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;                
                    break;
            }

            _logger.LogError(exception, message); // log unexpected errors

            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                error = message,
                statusCode = (int)statusCode
            });

            await context.Response.WriteAsync(result);
        }
    }

}
