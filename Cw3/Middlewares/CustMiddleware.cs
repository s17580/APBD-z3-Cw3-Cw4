using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cw3
{
    public class CustMiddleware
    {
        private RequestDelegate _nex;
        public CustMiddleware(RequestDelegate nex)
        {
            _nex = nex;
        }

        public async Task InvokeAsync(HttpContext cont)
        {
            cont.Response.Headers.Add("Coś tam", "12345");

            await _nex.Invoke(cont); 
        }
    }
}
