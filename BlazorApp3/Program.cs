using BlazorApp3.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorApp3 {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var apis = new Dictionary<string,string>();
            builder.Configuration.Bind("Apis",apis);

            //builder.Services.AddTransient<CustomAuthorizationMessageHandler>();


            //builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<CustomAuthorizationMessageHandler>(); //BearerTokenHandler >();// 

            //builder.Services.AddHttpClient("IDPClient", client =>
            //{
            //    client.BaseAddress = new Uri(builder.Configuration["OidcProvider:Authority"]);
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            //});

            foreach (var api in apis) {
                builder.Services.AddHttpClient(api.Key, client =>
                {
                    client.BaseAddress = new Uri(api.Value);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
                }).AddHttpMessageHandler<CustomAuthorizationMessageHandler>(); //BearerTokenHandler >();// 
            }

            builder.Services.AddOidcAuthentication(options => {
                // see https://aka.ms/blazor-standalone-auth
                builder.Configuration.Bind("OidcProvider", options.ProviderOptions);
            });

            await builder.Build().RunAsync();
        }
    }
}
