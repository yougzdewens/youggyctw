using ConcoursTwitter.Core;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcoursTwitter
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcoursTwitterCore concoursTwitter = new ConcoursTwitterCore();
            concoursTwitter.StartAll();
            //_client.DefaultRequestHeaders.Clear();
            //_client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");


            //var session = OAuth.AuthorizeAsync("kI5aGFtrAfGJd43jpGkVxsbMH", "7gpOSlkdWEZvUMpxq97QvND2qffdQE4TBwdmzs487Jtl59vi6H").GetAwaiter().GetResult();

            //string uriauth = session.AuthorizeUri.AbsoluteUri;

            //string htmlTest = GetHtmlFromPage(uriauth);

            //string pin = Console.ReadLine();

            //var tokens = OAuth.GetTokensAsync(session, pin).GetAwaiter().GetResult();
        }
    }
}
