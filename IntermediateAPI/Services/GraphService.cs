using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using B2CIntermediateAPI.Models.Graph;
using B2CIntermediateAPI.Models.Services.Graph;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace B2CIntermediateAPI.Services
{
    // Service used to connect to Microsoft Graph
    public class GraphService
    {
        //private static AuthenticationResult AccessToken;
        private static readonly SemaphoreSlim Semaphore = new(1);
        private static DateTimeOffset _previousErrorTime = DateTimeOffset.MinValue;
        public static string[] Scopes = { "https://graph.microsoft.com/.default" };

        private readonly HttpClient _client;
        private readonly IConfidentialClientApplication _confidentialClient;

        public GraphService(
            IOptions<AzureAdB2COptions> b2COptions,
            IHttpClientFactory factory
        )
        {
            AzureAdB2COptions b2COptions1 = b2COptions.Value;

            _confidentialClient = ConfidentialClientApplicationBuilder.Create(b2COptions1.ClientId)
                .WithClientSecret(b2COptions1.ClientSecret)
                .WithAuthority(new Uri(b2COptions1.Authority))
                .Build();

            _client = factory.CreateClient();
        }

        public static string ApiUrl => "https://graph.microsoft.com/";

        /// <summary>
        ///     API Wrapper for MSGraph to get the Software OAuth Methods defined for user
        /// </summary>
        /// <param name="userId">B2C User Object ID</param>
        /// <returns>Task wrapped object of parsed Software OAuth Methods defined for user</returns>
        public async Task<SoftwareOAuthMethods?> GetSoftwareOAuthMethods(string? userId)
        {
            var url = $"/users/{userId}/authentication/softwareOathMethods";
            string result = await SendBetaGraphRequest(url, null, HttpMethod.Get);

            return JsonSerializer.Deserialize<SoftwareOAuthMethods>(result);
        }

        /// <summary>
        ///     API Wrapper for MSGraph to delete the Software OAuth Methods defined for user
        /// </summary>
        /// <param name="userId">B2C User Object ID</param>
        /// <param name="methodIds">Method IDs defined for user to delete</param>
        /// <returns>Task</returns>
        public async Task DeleteSoftwareOAuthMethods(string? userId, IEnumerable<string>? methodIds)
        {
            IEnumerable<Task>? tasks = methodIds?.Select(async id =>
            {
                var url = $"/users/{userId}/authentication/softwareOathMethods/{id}";
                await SendBetaGraphRequest(url, null, HttpMethod.Delete);
            });

            if (tasks != null)
                await Task.WhenAll(tasks);
        }


        /// <summary>
        /// </summary>
        /// <param name="api">API endpoint to call</param>
        /// <param name="data">Body payload to call API with</param>
        /// <param name="method">Http Method</param>
        /// <returns>Task / string result object of API call</returns>
        private async Task<string> SendBetaGraphRequest(string api, string? data, HttpMethod method)
        {
            var url = $"{ApiUrl}beta{api}";

            return await SendGraphRequest(url, data, method);
        }


        /// <summary>
        ///     Handle Graph user API, support following HTTP methods: GET, POST and PATCH
        /// </summary>
        /// <param name="url">URL to call</param>
        /// <param name="data">body / data for API call</param>
        /// <param name="method">HTTP method to use (GET / POST / DELETE)</param>
        /// <param name="retry">retry flag</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<string> SendGraphRequest(string url, string? data, HttpMethod method, bool retry = true)
        {
            string accessToken = await AcquireAccessToken();

            //Trace.WriteLine($"Graph API call: {url}");
            using (var request = new HttpRequestMessage(method, url))
            {
                // Set the authorization header
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // For POST and PATCH set the request content 
                if (!string.IsNullOrEmpty(data))
                    //Trace.WriteLine($"Graph API data: {data}");
                    request.Content = new StringContent(data, Encoding.UTF8, "application/json");

                // This semaphore block is related to the call below - this will gate any calls into the graph API based on the throttling timeout
                Semaphore.Wait();

                if (_previousErrorTime > DateTimeOffset.UtcNow)
                {
                    TimeSpan duration = _previousErrorTime - DateTime.UtcNow;

                    if (duration > TimeSpan.Zero)
                        await Semaphore.WaitAsync((int)Math.Max(0, duration.TotalMilliseconds));
                }

                Semaphore.Release();

                // Send the request to Graph API endpoint
                using (HttpResponseMessage response = await _client.SendAsync(request))
                {
                    string error = await response.Content.ReadAsStringAsync();

                    // Check the result for error
                    if (!response.IsSuccessStatusCode)
                    {
                        // Throw server busy error message
                        if (response.StatusCode == (HttpStatusCode)429)
                        {
                            TimeSpan? retryTime = response.Headers.RetryAfter?.Delta;

                            if (!retryTime.HasValue) retryTime = TimeSpan.FromSeconds(200);

                            retryTime += TimeSpan.FromSeconds(15);

                            _previousErrorTime = DateTimeOffset.UtcNow.Add((TimeSpan)retryTime);

                            // Retry the request for failed only once
                            if (retry) return await SendGraphRequest(url, data, method, false);
                        }

                        throw new Exception(error);
                    }

                    // Return the response body, usually in JSON format
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        /// <summary>
        ///     Acquire access token to be used for MS Graph call
        /// </summary>
        /// <returns>Access token</returns>
        private async Task<string> AcquireAccessToken()
        {
            AuthenticationResult? accessToken = await _confidentialClient.AcquireTokenForClient(Scopes).ExecuteAsync();

            return accessToken.AccessToken;
        }
    }
}