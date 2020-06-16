using BlazorApp3.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorApp3 {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var apis = new Dictionary<string,string>();
            builder.Configuration.Bind("Apis",apis);

            var oidcProviderOptions = new OidcProviderOptions();
            builder.Configuration.GetSection("OidcProvider").Bind(oidcProviderOptions);

            builder.Services.AddTransient<CustomAuthorizationMessageHandler>();

            //THIS DOES NOT APPEAR TO WORK
            //builder.Services.AddTransient(sp => {
            //    var handler = sp.GetRequiredService<AuthorizationMessageHandler>()
            //        .ConfigureHandler(
            //            authorizedUrls: apis.Values.ToArray(),
            //            scopes: oidcProviderOptions.DefaultScopes);
            //    return handler;
            //});


            foreach (var api in apis) {
                builder.Services.AddHttpClient(api.Key, client =>
                {
                    client.BaseAddress = new Uri(api.Value);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
                }).AddHttpMessageHandler<CustomAuthorizationMessageHandler>(); 
            }


            builder.Services.AddOidcAuthentication(options => {
                // see https://aka.ms/blazor-standalone-auth
                builder.Configuration.Bind("OidcProvider", options.ProviderOptions);
            });

            await builder.Build().RunAsync();
        }
    }
}
