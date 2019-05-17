﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpMemoServer.Web.Middlewares;

namespace SharpMemoServer
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var staticContentRoot = System.Environment.GetEnvironmentVariable("STATIC_CONTENT_ROOT");
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                staticContentRoot = staticContentRoot == null ? "/Users/atchijov/Work/Spikes/SharpMemo.Core/SharpMemoUI.HTML" : staticContentRoot;
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                if (staticContentRoot == null)
                {
                    throw new Exception("STATIC_CONTENT_ROOT not set");
                }
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(staticContentRoot),
                RequestPath = ""
            });
            
            app.UseHttpsRedirection();
            app.UseMiddleware<OptionsMiddleware>();
            app.UseMvc();
        }
    }
}
