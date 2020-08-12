using AphrieTask.BE;
using AphrieTask.Manager;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AphrieTask.CustomMiddlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        private IBaseRepository<Log> _logRepository;
        private LogManager _logManager;


        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory
                      .CreateLogger<ExceptionMiddleware>(); ;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IBaseRepository<Log> logRepository)
        {
            _logRepository = logRepository;
            _logManager = new LogManager(_logRepository);

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");


                string userClaims = "";
                string userInfo = "";

                if (httpContext.User.Claims.Count() > 0)
                {
                    foreach (var item in httpContext.User.Claims)
                    {
                        userClaims += "Type: " + item.Type + " , Value: " + item.Value + ". ";
                    }

                    userInfo = @"Info: " + userClaims;
                }


                string MSG = @"Internal Server Error from the custom middleware: " + Environment.NewLine + "["
                         + "Status Code: " + (int)HttpStatusCode.InternalServerError + Environment.NewLine
                         + "Exception Mesage: " + ex.Message
                         + " ]";
                var log = _logManager.GetLog(Entities.Enums.LogLevel.ERROR, MSG, "", "", userInfo);
                if (log != null)
                    _logManager.addLog(log);

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware."
            }.ToString());
        }
    }
}
