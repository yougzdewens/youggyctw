using LibraryTweeter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTestTweeter
{
    class Program
    {
        static void Main(string[] args)
        {
            //test
            //LibraryTweeter.TwitterService twitterService = new LibraryTweeter.TwitterService();

            //Task task0 = Task.Factory.StartNew(() => { twitterService.DeleteAllDataInDB(); });
            //Task task4 = Task.Factory.StartNew(() => twitterService.Start());

            //twitterService.StartCycle();
            //twitterService.StartSearchCitation();
            //twitterService.StartSearchDirectMessage();

            //Task task1 = Task.Factory.StartNew(() => { twitterService.StartCycle(); });
            //Task task2 = Task.Factory.StartNew(() => { twitterService.StartSearchCitation(); });
            //Task task3 = Task.Factory.StartNew(() => { twitterService.StartSearchDirectMessage(); });

            TwitterService twitterService = TwitterService.Instance;

            Task task1 = Task.Factory.StartNew(() => { twitterService.StartAll(); });
            
            Console.ReadLine();
        }
    }
}