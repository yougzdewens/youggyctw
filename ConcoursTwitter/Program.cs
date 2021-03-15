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
        }
    }
}
