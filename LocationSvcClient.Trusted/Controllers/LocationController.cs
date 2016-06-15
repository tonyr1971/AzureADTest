using System.Configuration;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace LocationSvcClient.Trusted.Controllers
{
    [Authorize]
    public class LocationController : Controller
    {
        private static readonly string Audience = ConfigurationManager.AppSettings["ida:audience"];
        private static readonly string ClientId = ConfigurationManager.AppSettings["ida:clientId"];
        private static readonly string AppKey = ConfigurationManager.AppSettings["ida:appKey"];
        private static readonly string AadInstance = ConfigurationManager.AppSettings["ida:aadInstance"];
        private static readonly string Tenant = ConfigurationManager.AppSettings["ida:tenant"];
        private static readonly string Authority = string.Format(CultureInfo.InvariantCulture, AadInstance, Tenant);

        private static readonly AuthenticationContext AuthContext = new AuthenticationContext(Authority);
        //private static readonly ClientCredential ClientCredential = new ClientCredential(ClientId, AppKey);
        private static readonly ClientCredential ClientCredential = new ClientCredential(Audience, AppKey);

        public static string ServiceResourceId = ConfigurationManager.AppSettings["ida:serviceResourceId"];
        public static string ServiceBaseAddress = "https://localhost:44302/";

        // GET: Location
        public async Task<ActionResult> Index()
        {
            AuthenticationResult result = null;

            int retryCount = 0;
            bool retry = false;
            do
            {
                retry = false;
                try
                {
                    result = await AuthContext.AcquireTokenAsync(ServiceResourceId, ClientCredential);
                }
                catch (AdalException ex)
                {
                    if(ex.ErrorCode == "temporarily_unavailable")
                    {
                        retry = true;
                        retryCount++;
                        Thread.Sleep(3000);
                    }
                }
            } while (retry && retryCount < 3);

            if(result == null)
            {
                ViewBag.ErrorMessage = "UnexpectedError";
                return View("Index");
            }

            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, ServiceBaseAddress + "api/location?cityName=Omaha");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            var response = await httpClient.SendAsync(request);

            if(response.IsSuccessStatusCode)
            {
                string r = await response.Content.ReadAsStringAsync();
                ViewBag.Results = r;
                return View("Index");
            }
            else
            {
                string r = await response.Content.ReadAsStringAsync();
                if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    AuthContext.TokenCache.Clear();
                }
                ViewBag.ErrorMessage = "AuthorizationRequired";
                return View("Index");
            }
        }
    }
}