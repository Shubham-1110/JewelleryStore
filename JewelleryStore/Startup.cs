using JewelleryStore.BusinessService.Implementation;
using JewelleryStore.BusinessService.Interface;
using JewelleryStore.Common;
using JewelleryStore.Enum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using static JewelleryStore.Delegates.Delegates;

namespace JewelleryStore
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
            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseInMemoryDatabase("JewelleryStore");
            });

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", config =>
                {
                    var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.Secret));
                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Constants.Issuer,
                        ValidAudience = Constants.Audience,
                        IssuerSigningKey = symmetricKey
                    };
                });

            services.AddControllers();

            services.AddScoped(typeof(IGoldPriceEstimation), typeof(GoldPriceEstimation));
            services.AddScoped<PrintToFile>();
            services.AddScoped<PrintToPaper>();

            services.AddTransient<ServiceResolver>(serviceProvider => printOption =>
            {
                switch (printOption)
                {
                    case PrintOptions.PrintToFile:
                        return serviceProvider.GetService<PrintToFile>();
                    case PrintOptions.PrintToPaper:
                        return serviceProvider.GetService<PrintToPaper>();
                    default:
                        return null;
                }
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Jewellery Store Management", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Use the JWT bearer authorization token  in the header to access this API. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            HelperClass.InitializeDatabase(app);

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context2 =>
                {
                    var ex = context2.Features.Get<IExceptionHandlerPathFeature>().Error;

                    if (ex == null)
                    {
                        return;
                    }

                    context2.Response.StatusCode = 500;

                    var error = new
                    {
                        message = ex.Message
                    };

                    context2.Response.ContentType = "application/json";

                    using (var writer = new StreamWriter(context2.Response.Body))
                    {
                        new JsonSerializer().Serialize(writer, error);
                        await writer.FlushAsync().ConfigureAwait(false);
                    }
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "Jewellery Store Management API");
            });
        }
    }
}
