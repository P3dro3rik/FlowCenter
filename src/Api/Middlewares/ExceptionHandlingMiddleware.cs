using System.Net;
using FlowCenter.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FlowCenter.Api.Middlewares;

/// <summary>
/// Middleware global de tratamento de exceções.
/// Converte violações de regras de negócio (<see cref="DomainException"/>) em
/// respostas HTTP 400, e qualquer outra falha não tratada em HTTP 500. (RB12, RNF08)
/// </summary>
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
        catch (DomainException ex)
        {
            await EscreverProblemaAsync(context, HttpStatusCode.BadRequest, "Requisição inválida", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado ao processar a requisição.");
            await EscreverProblemaAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Erro interno do servidor",
                "Ocorreu um erro inesperado ao processar a requisição.");
        }
    }

    private static async Task EscreverProblemaAsync(
        HttpContext context, HttpStatusCode statusCode, string title, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
