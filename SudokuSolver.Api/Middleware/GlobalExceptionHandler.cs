using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SudokuSolver.Api.Exceptions;
using SudokuSolver.Api.Models;

namespace SudokuSolver.Api.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
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
        var correlationId = context.TraceIdentifier;
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            SudokuException sudokuEx => HandleSudokuException(sudokuEx, correlationId),
            System.ComponentModel.DataAnnotations.ValidationException validationEx => HandleValidationException(validationEx, correlationId),
            _ => HandleGenericException(exception, correlationId)
        };

        response.StatusCode = errorResponse.StatusCode;
        
        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }

    private static (int StatusCode, ApiResponse<object> Response) HandleSudokuException(SudokuException exception, string correlationId)
    {
        var errorResponse = ApiResponse<object>.ErrorResponse(
            exception.Message,
            exception.ErrorCode,
            correlationId
        );

        return exception switch
        {
            UnsolvablePuzzleException => (StatusCodes.Status422UnprocessableEntity, errorResponse),
            InvalidGridException => (StatusCodes.Status400BadRequest, errorResponse),
            SolvingTimeoutException => (StatusCodes.Status408RequestTimeout, errorResponse),
            PuzzleGenerationException => (StatusCodes.Status500InternalServerError, errorResponse),
            InvalidDifficultyException => (StatusCodes.Status400BadRequest, errorResponse),
            HintUnavailableException => (StatusCodes.Status404NotFound, errorResponse),
            _ => (StatusCodes.Status500InternalServerError, errorResponse)
        };
    }

    private static (int StatusCode, ApiResponse<object> Response) HandleValidationException(System.ComponentModel.DataAnnotations.ValidationException exception, string correlationId)
    {
        var errorResponse = ApiResponse<object>.ErrorResponse(
            "Validation failed",
            "VALIDATION_ERROR",
            correlationId
        );

        if (errorResponse.Error != null)
        {
            errorResponse.Error.Details = new Dictionary<string, object>
            {
                { "field", "unknown" },
                { "value", "null" }
            };
        }

        return (StatusCodes.Status400BadRequest, errorResponse);
    }

    private (int StatusCode, ApiResponse<object> Response) HandleGenericException(Exception exception, string correlationId)
    {
        _logger.LogError(exception, "Unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);

        var errorResponse = ApiResponse<object>.ErrorResponse(
            "An unexpected error occurred",
            "INTERNAL_SERVER_ERROR",
            correlationId
        );

        return (StatusCodes.Status500InternalServerError, errorResponse);
    }
}

/// <summary>
/// Extension method to register the global exception handler
/// </summary>
public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandler>();
    }
} 