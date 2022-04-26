using Backend.Data;
using Backend.Helper;
using Backend.Repository;
using Backend.Repository.IRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Backend.TrailerMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Backend
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
            services.AddCors();

            services.AddDbContext<ApplicationDbContext>(Options => Options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IRepositoryUser, RepositoryUser>();
            services.AddScoped<IRepositoryTrailer, RepositoryTrailer>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.
                    GetBytes(Configuration.GetSection("AppSettings:Token").Value)),

                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAutoMapper(typeof(TrailerMappers));


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("users", new OpenApiInfo 
                {
                    Title = "Trailer API",
                    Version = "V1",
                    Contact = new OpenApiContact()
                    {
                        Email = "davmontmontas@gmail.com.com",
                        Name = "Davinci Montas",
                        Url = new Uri("https://google.com")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT Lincese",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });
                c.SwaggerDoc("trailer", new OpenApiInfo
                {
                    Title = "Trailer API",
                    Version = "V1",
                    Contact = new OpenApiContact()
                    {
                        Email = "davmontmontas@gmail.com.com",
                        Name = "Davinci Montas",
                        Url = new Uri("https://google.com")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT Lincese",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });
                c.AddSecurityDefinition("Bearer",
                   new OpenApiSecurityScheme
                   {
                       Description = "Autenticacion JWT (Bearer)",
                       Type = SecuritySchemeType.Http,
                       Scheme = "Bearer"
                   });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                     {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                });

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(x => x.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/users/swagger.json", "Users v1");
                    c.SwaggerEndpoint("/swagger/trailer/swagger.json", "trailer v1");
                });
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if (error != null)
                        {
                            context.Response.AddAplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }

                    });

                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/users/swagger.json", "Users v1");
                        c.SwaggerEndpoint("/swagger/trailer/swagger.json", "trailer v1");

                        c.RoutePrefix = "";
                    });
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
