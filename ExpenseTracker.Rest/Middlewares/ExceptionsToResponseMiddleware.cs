using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using ExpenseTracker.Identity.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using ExpenseTracker.Rest.Models;
using ExpenseTracker.Core.Exceptions;

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
                    case ValidationException ex:
                        response = new Response(StatusCodes.Status400BadRequest, ex.ValidationErrors);
                        break;
                    case BadRequestException ex:
                        response = new Response(StatusCodes.Status400BadRequest, ex.Message);
                        break;
                    case ArgumentException ex:
                        response = new Response(StatusCodes.Status400BadRequest, ex.Message);
                        break;
                    case NotFoundException ex:
                        response = new Response(StatusCodes.Status404NotFound, ex.Message);
                        break;
                    case Exception ex: 
                        response = new Response(StatusCodes.Status500InternalServerError);
                        break;
                }

                context.Response.StatusCode = response.StatusCode;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
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
