using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RetailOptimizationPlatform.Core.Exceptions;
using System;
using System.Threading.Tasks;

namespace RetailOptimizationPlatform.Web.Middleware
{
    /// <summary>
    /// Global middleware to catch all unhandled exceptions, log them, and present a clean error page.
    /// Fulfills the requirement for centralized error handling.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Proceed to the next middleware or controller normally
                await _next(context);
            }
            catch (InventoryNotFoundException ex)
            {
                _logger.LogWarning(ex, "Domain Resource Not Found: {Message}", ex.Message);
                
                // For HTML requests, redirect to standard MVC error page
                context.Response.Redirect($"/Home/Error?message={Uri.EscapeDataString(ex.Message)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request execution.");
                
                // For HTML requests, redirect to standard MVC error page
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}