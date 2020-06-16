using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EDennis.AspNet.Base {

    //Add this handler to HttpClient DI configuration when the child API 
    //has writeable operations on a database
    //TODO: how will this work with WebAssembly client? ... no IHttpContextAccessor
    public class TransactionCookiePropagatingHandler : DelegatingHandler {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionCookiePropagatingHandler(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            if(_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("Transaction", out var transaction)) {
                request.Headers.Add("Cookie", $"Transaction={transaction}");
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
