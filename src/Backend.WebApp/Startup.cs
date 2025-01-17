﻿// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ExtCore.Data.EntityFramework;
using ExtCore.WebApplication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace WebApplication
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly string _extensionsPath;

        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this._configuration = configuration;
            this._extensionsPath = hostingEnvironment.ContentRootPath + this._configuration["Extensions:Path"];
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddExtCore(this._extensionsPath);
            services.Configure<StorageContextOptions>(options =>
                {
                    options.ConnectionString = this._configuration.GetConnectionString("SQlServer");
                    options.MigrationsAssembly = typeof(DesignTimeStorageContextFactory).GetTypeInfo().Assembly.FullName;

                }
            );

            DesignTimeStorageContextFactory.Initialize(services.BuildServiceProvider());
        }

        public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
                applicationBuilder.UseDatabaseErrorPage();
            }

            applicationBuilder.UseExtCore();
        }
    }
}