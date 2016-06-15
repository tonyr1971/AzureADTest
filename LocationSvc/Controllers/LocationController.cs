using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using LocationSvc.Models;

namespace LocationSvc.Controllers
{
    [Authorize]
    public class LocationController : ApiController
    {
        public Location GetLocation(string cityName)
        {
            return new Location
            {
                Latitude = 10,
                Longitude = 20
            };
        }

        //Use this in the trust client example (and might have to remove [Authorize] tag)
        //private static readonly string TrustedCallerClientId = ConfigurationManager.AppSettings["ida:trustedCallerClientId"];
        //public Location GetLocation(string cityName)
        //{
        //    string currentCallerClientId = ClaimsPrincipal.Current.FindFirst("appid").Value;
        //    if (currentCallerClientId == TrustedCallerClientId)
        //    {
        //        return new Location
        //        {
        //            Latitude = 10,
        //            Longitude = 20
        //        };
        //    }

        //    throw new HttpResponseException(
        //        new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.Unauthorized,
        //            ReasonPhrase = "Only trusted callers are allowed. Your identity: " + currentCallerClientId
        //        });
        //}
    }
}
