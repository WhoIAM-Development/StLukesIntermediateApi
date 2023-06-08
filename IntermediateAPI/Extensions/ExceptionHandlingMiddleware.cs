using Serilog;
using Serilog.Context;

namespace IntermediateAPI.Extensions
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		///https://blog.bitscry.com/2019/01/14/logging-failed-requests-to-serilog-using-middleware/
		///Modified with async support for .NET 6
		public async Task Invoke(HttpContext httpContext)
		{
			string requestContent = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
			string requestMethod = httpContext.Request.Method;
			httpContext.Request.Body.Position = 0;

			Stream responseBody = httpContext.Response.Body;

			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					httpContext.Response.Body = memoryStream;

					await _next(httpContext);

					int responseStatus = httpContext.Response.StatusCode;

					if (!IsSuccessStatusCode(httpContext.Response.StatusCode) && requestMethod == "POST")
					{
						// Get response body if required
						//memoryStream.Position = 0;
						//string responseContent = new StreamReader(memoryStream).ReadToEnd();

						using (LogContext.PushProperty("RequestBody", requestContent))
						{
							Log.Warning("Invalid request ({@ResponseStatusCode}): {@RequestPath}", httpContext.Response.StatusCode, httpContext.Request.Path.Value);
						}
					}

					memoryStream.Position = 0;
					await memoryStream.CopyToAsync(responseBody);
				}
			}
			finally
			{
				httpContext.Response.Body = responseBody;
			}
		}

		public bool IsSuccessStatusCode(int statusCode)
		{
			if ((statusCode >= 200) && (statusCode <= 299))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class ExceptionHandlingMiddlewareExtensions
	{
		public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ExceptionHandlingMiddleware>();
		}
	}
}
