using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;
using System.Threading;

namespace Helpers
{
    public class AuthHandler : DelegatingHandler
    {
        private IAuthenticationProvider _authenicationProvider;

        public AuthHandler(IAuthenticationProvider authenicationProvider, HttpMessageHandler innerHandler)
        {
            InnerHandler = innerHandler;
            _authenicationProvider = authenicationProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await _authenicationProvider.AuthenticateRequestAsync(request);
            return await base.SendAsync(request, cancellationToken);
        }
    }

}