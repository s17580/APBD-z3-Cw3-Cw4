using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Cw3.Services;
using Cw3.Middlewares;
using Microsoft.AspNetCore.Http;


namespace Cw3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)

        {

            services.AddSingleton<IStudentsDal, SqlServerDbDal>();
            //services.AddScoped<IStudentsDal, SqlServerDbDal>();
            services.AddTransient<ILogMiddleware, FileLogMiddleware>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStudentsDal dbService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ILogMiddleware>();

            app.Use(async (contx, nex) =>
            {
                if (!contx.Request.Headers.ContainsKey("Index"))
                {
                    contx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await contx.Response.WriteAsync("W nag³owkach nie znajduje siê numer indexu studenta.");
                    return;
                }

                var idx = contx.Request.Headers["Index"].ToString();
                if (!dbService.IsStudentExist(idx))
                {
                    contx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await contx.Response.WriteAsync("Student o podanym numerze indexu nie istnieje");
                    return;
                }

                await nex();
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
