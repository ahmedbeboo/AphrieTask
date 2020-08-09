using AphrieTask.Manager;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AphrieTask.CustomMiddlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        private IBaseRepository<Log> _logRepository;
        private LogManager _logManager;


        public RequestResponseLoggingMiddleware(RequestDelegate next,
                                                ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory
                      .CreateLogger<RequestResponseLoggingMiddleware>();
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

        }

        public async Task Invoke(HttpContext context, IBaseRepository<Log> logRepository)
        {
            _logRepository = logRepository;
            _logManager = new LogManager(_logRepository);

            //await LogRequest(context);
            //await LogResponse(context);


            // ------------ Log Requests -----------------------------
            context.Request.EnableBuffering();

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);

            string RequestInfo = @"Http Request Information:-" + Environment.NewLine + "[" +
                                   "Schema: " + context.Request.Scheme + Environment.NewLine +
                                   "Host: " + context.Request.Host + Environment.NewLine +
                                   "Path: " + context.Request.Path + Environment.NewLine +
                                   "QueryString: " + context.Request.QueryString + Environment.NewLine +
                                   "Request Body: " + ReadStreamInChunks(requestStream) + "]";

            _logger.LogInformation(RequestInfo);
            context.Request.Body.Position = 0;



            // ------------ Log Responses -----------------------------
            var originalBodyStream = context.Response.Body;

            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);


            string ResponseInfo = @"Http Response Information:" + Environment.NewLine + "[" +
                       "Schema: " + context.Request.Scheme + Environment.NewLine +
                       "Host: " + context.Request.Host + Environment.NewLine +
                       "Path: " + context.Request.Path + Environment.NewLine +
                       "QueryString: " + context.Request.QueryString + Environment.NewLine +
                       "Response Body: " + text + Environment.NewLine +
                       "Status Code: " + context.Response.StatusCode + " ]";
            _logger.LogInformation(ResponseInfo);


            //------------ TODO: Save log to chosen datastore

            string userClaims = "";
            foreach (var item in context.User.Claims)
            {
                userClaims += "Type: " + item.Type + " , Value: " + item.Value + ". ";
            }
            string userInfo = @"Info: " + userClaims;

            Log log = new Log();
            log.CreatedDate = DateTime.Now;
            log.Level = Entities.Enums.LogLevel.INFO;
            log.requestInfo = RequestInfo;
            log.responseInfo = ResponseInfo;
            log.userInfo = userInfo;
            _logManager.addLog(log);

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }



    }

}
