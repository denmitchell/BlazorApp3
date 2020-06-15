using Api1;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Api {
    public class Startup {
        readonly IConfiguration Configuration;

        readonly string AllowedOrigins = "_allowedOrigins";

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();

            var clients = new List<string>();
            Configuration.GetSection("Clients").Bind(clients);

            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowedOrigins,
                                  builder => {
                                      builder
                                        .AllowAnyOrigin()
                                        //.WithOrigins(clients.ToArray())
                                        .AllowAnyMethod()
                                        .AllowAnyHeader();
                                        //.WithHeaders(HeaderNames.ContentType);
                                  });
            });


            var apis = new Dictionary<string, string>();
            Configuration.GetSection("Apis").Bind(apis);
            var oidcApi = apis.Single(a => a.Key == "OidcProvider").Value;

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options => {
                    options.Authority = $"{oidcApi}";
                    options.ApiName = "Api1";
                    //options.ApiSecret = "secret";
                });

            services.AddAuthorization(
            options =>
            {
                options.AddPolicy("GetPolicy", policy =>
                    policy.RequireClaim("scope", "Api1.*.Get*"));
                options.AddPolicy("EditPolicy", policy =>
                    policy.RequireClaim("scope", "Api1.*.Edit*"));
                options.AddPolicy("DeletePolicy", policy =>
                    policy.RequireClaim("scope", "Api1.*.Delete*"));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

            app.Use(async (context, next) => {

                Debug.WriteLine(context.Request.Path.Value.ToString());

                await next.Invoke();
            });
            
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(AllowedOrigins);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
