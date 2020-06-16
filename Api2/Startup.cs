using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Api2 {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string AllowedOrigins = "_allowedOrigins";


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();

            var clients = new List<string>();
            Configuration.GetSection("Clients").Bind(clients);

            services.AddCors(options => {
                options.AddPolicy(name: AllowedOrigins,
                                  builder => {
                                      builder
                                        .WithOrigins(clients.ToArray())
                                        .AllowAnyMethod()
                                        .AllowAnyHeader();
                                  });
            });

            var apis = new Dictionary<string, string>();
            Configuration.GetSection("Apis").Bind(apis);
            var oidcApi = apis.Single(a => a.Key == "OidcProvider").Value;

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options => {
                    options.Authority = $"{oidcApi}";
                    options.ApiName = "Api2";
                    //options.ApiSecret = "secret";
                });

            //todo: add support for default policies
            services.AddAuthorization(
            options => {
                options.AddPolicy("GetPolicy", policy =>
                    policy.RequireClaim("scope", "Api2.*.Get*"));
                options.AddPolicy("EditPolicy", policy =>
                    policy.RequireClaim("scope", "Api2.*.Edit*"));
                options.AddPolicy("DeletePolicy", policy =>
                    policy.RequireClaim("scope", "Api2.*.Delete*"));
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

            app.UseHttpsRedirection();

            app.UseCors(AllowedOrigins);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
