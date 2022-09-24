﻿using BlazorSozluk.Common.Infrastructure.Exceptions;
using BlazorSozluk.Common.Infrastructure.Results;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace BlazorSozluk.WebApi.Infrastructure.Extensions
{
    /// <summary>
    /// 11.video 10.dk
    /// </summary>
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder ConfigureExceptionHandling(this IApplicationBuilder app,
          bool includeExceptionDetails = false,
          bool useDefaultHandlingResponse = true,
          Func<HttpContext, Exception, Task> handleException = null)
        {
            app.UseExceptionHandler(options =>
            {
                options.Run(context =>
                {
                    var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();

                    if (!useDefaultHandlingResponse && handleException == null)
                        throw new ArgumentException(nameof(handleException),
                            $"{nameof(handleException)} cannot be null when {nameof(useDefaultHandlingResponse)} is false.");

                    if (!useDefaultHandlingResponse && handleException != null)
                        return handleException(context, exceptionObject.Error);

                    return DefaultHandleException(context, exceptionObject.Error, includeExceptionDetails);
                });
            });

            return app;
        }

        /// <summary>
        /// Exception meydana gelince yakalayan extension görevi görür.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <param name="includeExceptionDetails"></param>
        /// <returns></returns>
        private static async Task DefaultHandleException(HttpContext context, Exception exception, bool includeExceptionDetails)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "Internal server error occured!";

            if (exception is UnauthorizedAccessException)
                statusCode = HttpStatusCode.Unauthorized;

            if (exception is DatabaseValidationException)
            {
                statusCode = HttpStatusCode.BadRequest;
                var validationResponse = new ValidationResponseModel(exception.Message);
                await WriteResponse(context, statusCode, validationResponse);
                return;
            }

            var res = new
            {
                HttpStatusCode = (int)statusCode,
                Detail = includeExceptionDetails ? exception.ToString() : message
            };

            await WriteResponse(context, statusCode, res);
        }

        private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, object responseObj)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsJsonAsync(responseObj);
        }
    }
}
