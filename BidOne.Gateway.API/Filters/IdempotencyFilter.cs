using BidOne.Gateway.API.Helpers;
using BidOne.Gateway.Application.Interfaces;
using BidOne.Gateway.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace BidOne.Gateway.API.Filters
{
    public class IdempotencyFilter : IAsyncActionFilter
    {
        private readonly IIdempotencyService _idempotencyService;


        public IdempotencyFilter(IIdempotencyService idempotencyService)
        {
            _idempotencyService = idempotencyService;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Idempotency-Key", out var key))
            {
                var response = APIResponse.BadRequest(StringConstants.Idempotency_MissingKey, (int)HttpStatusCode.ExpectationFailed);
                context.Result = new JsonResult(response) { StatusCode = (int)HttpStatusCode.ExpectationFailed };
                return;
            }


            var method = context.HttpContext.Request.Method;
            var bodyStr = await ReadBodyAsync(context.HttpContext.Request);
            var endpoint = context.HttpContext.Request.Path;
            string hash = ComputeBodyHash(bodyStr + endpoint);


            var stored = await _idempotencyService.GetStoredResponseAsync(key, method, hash);
            if (stored != null)
            {
                if (stored.Value.IsProcessing)
                {
                    var processingResponse = APIResponse.BadRequest(StringConstants.Idempotency_DuplicateRequest, (int)HttpStatusCode.Conflict);
                    context.Result = new ConflictObjectResult(processingResponse);
                    return;
                }


                context.Result = new JsonResult(stored.Value.Response)
                {
                    StatusCode = stored.Value.StatusCode
                };
                return;
            }


            await _idempotencyService.MarkProcessingAsync(key, method, hash);

            var executedContext = await next();


            if (executedContext.Result is ObjectResult result)
            {
                await _idempotencyService.SaveResponseAsync(key, method, hash, result.Value, result.StatusCode ?? 200);
            }
            else
            {
                await _idempotencyService.RemoveKeyAsync(key, method, hash);
            }
        }


        private async Task<string> ReadBodyAsync(HttpRequest req)
        {
            if (!req.Body.CanSeek)
                req.EnableBuffering();


            req.Body.Position = 0;
            using var reader = new StreamReader(req.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            req.Body.Position = 0;
            return body;
        }


        private static string ComputeBodyHash(string body)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(body)));
        }
    }
}