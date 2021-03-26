using System;
using System.Collections.Generic;
using System.Text;

namespace ConcoursTwitter.Tools
{
    public class ConfigurationTools
    {
        public static string ConnectionStringDB
        {
            get
            {
                return Environment.GetEnvironmentVariable("CONNECTIONSTRINGDB");
            }
        }

        public static string PathOfSimlPackage
        {
            get
            {
                return Environment.GetEnvironmentVariable("PATHOFSIMLPACKAGE");
            }
        }

        public static string ResetMode
        {
            get
            {
                return Environment.GetEnvironmentVariable("RESETMODE");
            }
        }

        public static string SearchAndFollow
        {
            get
            {
                return Environment.GetEnvironmentVariable("SEARCHANDFOLLOW");
            }
        }

        public static string ConsumerKey { get
            {
                return Environment.GetEnvironmentVariable("CONSUMERKEY");
            }
        }

        public static string ConsumerSecret
        {
            get
            {
                return Environment.GetEnvironmentVariable("CONSUMERSECRET");
            }
        }

        public static string AccessToken
        {
            get
            {
                return Environment.GetEnvironmentVariable("ACCESSTOKEN");
            }
        }

        public static string AccessTokenSecret
        {
            get
            {
                return Environment.GetEnvironmentVariable("ACCESSTOKENSECRET");
            }
        }
    }
}
