using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LocationSvc.Models;

namespace LocationSvc.Controllers
{
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
    }
}
