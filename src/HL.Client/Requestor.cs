using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HL.Client
{
    /// <summary>
    /// Defines the general class for making any request.
    /// </summary>
    public class Requestor
    {
        #region Fields
        private HttpClient _httpClient;
        private CookieContainer _cookies;
        #endregion

        #region Methods
        private async Task<HttpResponseMessage> RequestAsync(HttpMethod method, string path, HttpContent content, CancellationToken cancelationToken)
        {
            cancelationToken.ThrowIfCancellationRequested();

            // Build the request
            HttpRequestMessage request = new HttpRequestMessage(method, path);

            if (method != HttpMethod.Get)
                request.Content = content;

            // Send the request
            HttpResponseMessage response = await _httpClient.SendAsync(request, cancelationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return response;
            } else
            {
                // Figure out error handling here.
                throw new Exception("Error!");
                return response;
            }
        }

        /// <summary>
        /// Performs a get request.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RequestAsync(HttpMethod.Get, path, null, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Performs a post request.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string path, HttpContent content = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RequestAsync(HttpMethod.Post, path, content, cancellationToken).ConfigureAwait(false);
        }

        public async Task GetCookies()
        {
            var x = _cookies;
        }
        #endregion

        #region Constructor
        public Requestor()
        {
            // Build the HTTP Client
            HttpClientHandler handler = new HttpClientHandler();
            _cookies = new CookieContainer();
            handler.CookieContainer = _cookies;
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(Constants.BaseUrl)
            };
        }
        #endregion
    }
}
