using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorApp3.Models {
    public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler {
        public CustomAuthorizationMessageHandler(IAccessTokenProvider provider,
            NavigationManager navigationManager)
            : base(provider, navigationManager) {
            ConfigureHandler(
                authorizedUrls: new[] { "https://localhost:5001" },
                scopes: new[] { "Api1", "Api1.*.Get*", "Api1.*.Edit*", "Api1.*.Delete*" });
        }


    }
}