using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Cw3.Middlewares
{
    public class FileLogMiddleware : ILogMiddleware
    {


        public const string LogFilePath = @"acc.log";

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate nex)
        {
            var meth = httpContext.Request.Method;
            var path = httpContext.Request.Path.ToString();
            var query = httpContext.Request.QueryString.ToString();

            bool isBodUs = false;
            var body = "";

            string[] methsWithBod = { "POST", "PUT", "PATCH" };
            if (methsWithBod.Contains(meth))
            {
                isBodUs = true;
                using (StreamReader strRead = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, false))
                {
                    body = await strRead.ReadToEndAsync();
                    strRead.Close();
                }
            }

            await nex(httpContext);

            using (TextWriter textWr = new StreamWriter(LogFilePath, true))
            {
                await textWr.WriteLineAsync($"{meth} {path}{query}");
                if (isBodUs)
                {
                    await textWr.WriteLineAsync($"{body}");
                }

                textWr.Close();
            }
        }
    }

    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FileLogMiddleware>();
        }


    }
}
