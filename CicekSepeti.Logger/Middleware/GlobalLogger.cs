using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CicekSepeti.Logger.Middleware
{
    public class GlobalLogger
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public GlobalLogger(RequestDelegate next)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            await HandleRequestAsync(context);
            await HandleResponseAsync(context);
        }

        private async Task HandleRequestAsync(HttpContext context)
        {
            if (context?.Request?.Path.Value.Contains("swagger") == true)
                return;

            using (var requestStream = _recyclableMemoryStreamManager.GetStream())
            {
                await context.Request.Body.CopyToAsync(requestStream);

                var body = ReadStreamInChunks(requestStream);

                var queryString =
                           $"Http Request QueryString: " +
                           $"{context.Request.Scheme}://" +
                           $"{context.Request.Host}" +
                           $"{context.Request.Path}" +
                           $"{context.Request.QueryString}";

                if (!string.IsNullOrWhiteSpace(body))
                {
                    body = $"{Environment.NewLine}Http Request Body: {body}";
                    queryString += body;
                }

                Log.Information(queryString);

                context.Request.Body.Position = 0;
            }
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
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            }

            while (readChunkLength > 0);

            return textWriter.ToString();
        }

        private async Task HandleResponseAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using (var responseBody = _recyclableMemoryStreamManager.GetStream())
            {
                context.Response.Body = responseBody;

                await _next(context);
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                if (context?.Request?.Path.Value.Contains("swagger") == false && !string.IsNullOrWhiteSpace(text))
                    Log.Information($"Http Response Body: {text}");

                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }
}
