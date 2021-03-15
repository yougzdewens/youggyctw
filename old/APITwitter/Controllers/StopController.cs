using LibraryTweeter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace APITwitter.Controllers
{
    public class StopController : ApiController
    {
        // GET api/values/5
        public string Get()
        {
            TwitterService.Instance.IsStopping = true;

            while(!TwitterService.Instance.IsStopped)
            {
                
            }

            return "stopped";
        }
    }
}
