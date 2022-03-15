using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using ExpenseTracker.Identity.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using ExpenseTracker.Rest.Models;
using ExpenseTracker.Core.Exceptions;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Rest.Middlewares
{
    public class ExceptionsToResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionsToResponseMiddleware(RequestDelegate next) 
        {
            this._next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                context.Response.ContentType = "application/json";
                Response response = null;

                switch (exception)
                {
                    case AuthenticationException ex:
                        response = new Response(StatusCodes.Status401Unauthorized);
                        break;
                    case RegistrationException ex:
                        response = new Response(StatusCodes.Status400BadRequest, ex.Errors);
                        break;
                    case ValidationException ex:
                        response = new Response(StatusCodes.Status400BadRequest, ex.ValidationErrors);
                        break;
                    case BadRequestException ex:
                        response = new Response(StatusCodes.Status400BadRequest, error : ex.Message);
                        break;
                    case ArgumentException ex:
                        response = new Response(StatusCodes.Status400BadRequest, error: ex.Message);
                        break;
                    case NotFoundException ex:
                        response = new Response(StatusCodes.Status404NotFound, error: ex.Message);
                        break;
                    case Exception ex: 
                        response = new Response(StatusCodes.Status500InternalServerError);
                        break;
                }

                context.Response.StatusCode = response.StatusCode;
                await context.Response.WriteAsync
                    (
                        JsonSerializer.Serialize
                        (
                            response,
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                        )
                    );
            }
        }
    }

    public static class ExceptionsToResponseMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionsToResponse(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ExceptionsToResponseMiddleware>();
            return builder;
        }
    }
}
