using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace LocationSvcClient.Daemon
{
    class Program
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:clientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:appKey"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:aadInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:tenant"];
        private static string ServiceResourceId = ConfigurationManager.AppSettings["ida:serviceResourceId"];

        private static string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        private static AuthenticationContext authContext = new AuthenticationContext(authority);
        private static ClientCredential clientCredential = new ClientCredential(clientId, appKey);

        private static void Main(string[] args)
        {
            Console.WriteLine("Press enter to start...");
            Console.Read();

            AuthenticationResult result = null;
            int retryCount = 0;
            bool retry = false;
            do
            {
                retry = false;
                try
                {
                    result = authContext.AcquireTokenAsync(ServiceResourceId, clientCredential).Result;
                }
                catch (AdalException ex)
                {
                    if (ex.ErrorCode == "temporarily_unavailable")
                    {
                        retry = true;
                        retryCount++;
                        Thread.Sleep(3000);
                    }
                }
            } while (retry && retryCount < 3);

            if (result == null)
            {
                Console.WriteLine("Cancelling attempt...");
                return;
            }

            Console.WriteLine("Authenticated successfully... making HTTPS call..");

            MakeHttpsCall(result.AccessToken).Wait();
        }

        private static async Task MakeHttpsCall(string accessToken)
        {
            const string serviceBaseAddress = "https://localhost:44302/";

            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, serviceBaseAddress + "api/location?cityName=Omaha");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string r = await response.Content.ReadAsStringAsync();
                Console.WriteLine(r);
            }
            else
            {
                string r = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    authContext.TokenCache.Clear();
                }
                Console.WriteLine("Access Denied");
            }
        }
    }
}
