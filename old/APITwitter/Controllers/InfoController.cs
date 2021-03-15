using LibraryTweeter;
using LibraryTweeter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace APITwitter.Controllers
{
    public class InfoController : ApiController
    {
        // GET api/values/5
        public InfoService Get()
        {
            return TwitterService.GetInfoService();
        }
    }
}
