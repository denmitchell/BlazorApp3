using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorApp3 {
    public class BearerTokenHandler : DelegatingHandler {

        private readonly IAccessTokenProvider _provider;
        private readonly NavigationManager _navigation;
        private AccessToken _lastToken;
        private AuthenticationHeaderValue _cachedHeader;
        private Uri[] _authorizedUris;
        private AccessTokenRequestOptions _tokenOptions;
        private IConfiguration _config;


        public BearerTokenHandler(
            IAccessTokenProvider provider,
            NavigationManager navigation, 
            IConfiguration config ) {
            _provider = provider;
            _navigation = navigation;
            _config = config;
        }


        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) {
            var accessToken = await GetAccessTokenAsync();

            if (!string.IsNullOrWhiteSpace(accessToken)) {
                request.SetBearerToken(accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        public async Task<string> GetAccessTokenAsync() {

            var scopes = new List<string>();
            _config.GetSection("OidcProvider:DefaultScopes").Bind(scopes);

            var tokenResult = await _provider.RequestAccessToken(new AccessTokenRequestOptions {
                Scopes = scopes
            });

            if (tokenResult.TryGetToken(out var token))
                return token.Value;
            else
                throw new ApplicationException("Cannot get access token");

        }


        /// <summary>
        /// Configures this handler to authorize outbound HTTP requests using an access token. The access token is only attached if at least one of
        /// <paramref name="authorizedUrls" /> is a base of <see cref="HttpRequestMessage.RequestUri" />.
        /// </summary>
        /// <param name="authorizedUrls">The base addresses of endpoint URLs to which the token will be attached.</param>
        /// <param name="scopes">The list of scopes to use when requesting an access token.</param>
        /// <param name="returnUrl">The return URL to use in case there is an issue provisioning the token and a redirection to the
        /// identity provider is necessary.
        /// </param>
        public BearerTokenHandler ConfigureHandler(
            IEnumerable<string> authorizedUrls,
            IEnumerable<string> scopes = null,
            string returnUrl = null) {
            if (_authorizedUris != null) {
                throw new InvalidOperationException("Handler already configured.");
            }

            if (authorizedUrls == null) {
                throw new ArgumentNullException(nameof(authorizedUrls));
            }

            var uris = authorizedUrls.Select(uri => new Uri(uri, UriKind.Absolute)).ToArray();
            if (uris.Length == 0) {
                throw new ArgumentException("At least one URL must be configured.", nameof(authorizedUrls));
            }

            _authorizedUris = uris;
            var scopesList = scopes?.ToArray();
            if (scopesList != null || returnUrl != null) {
                _tokenOptions = new AccessTokenRequestOptions {                    
                    Scopes = scopesList,
                    ReturnUrl = returnUrl                    
                };
            }

            return this;
        }
    }
}

