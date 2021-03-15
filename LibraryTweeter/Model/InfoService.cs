using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryTweeter.Model
{
    public class InfoService
    {
        public DateTime DateLastCheck { get; set; }

        public int NbFollowed { get; set; }

        public int NBLastRetweet { get; set; }
    }
}