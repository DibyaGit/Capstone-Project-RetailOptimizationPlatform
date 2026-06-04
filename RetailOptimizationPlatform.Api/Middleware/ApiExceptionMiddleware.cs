using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Api.Middleware
{
    /// <summary>
    /// Centralized exception handling middleware for the REST Web API.
    /// Catches all exceptions and returns standardized JSON error responses.
    /// </summary>
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
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
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found in database: {Message}", ex.Message);
                await HandleExceptionAsync(context, StatusCodes.Status404NotFound, "Not Found", ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failure: {Message}", ex.Message);
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, "Bad Request", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal system error in API project.");
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected system error occurred on the API server.");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string errorType, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var payload = new
            {
                Error = errorType,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }
    }
}
