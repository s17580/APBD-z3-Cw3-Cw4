using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cw3
{
    public class CustMiddleware
    {
        private RequestDelegate _next;
        public CustMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Add("Cos", "123");

            await _next.Invoke(context); //odpalam kolejny middleware
        }
    }
}
