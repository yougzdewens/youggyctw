using LibraryTweeter;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ServiceConcoursTwitter
{
    public partial class Service1 : ServiceBase
    {
        //TwitterService twitterService = new TwitterService();

        public Service1()
        {
            InitializeComponent();            

            //Task task0 = Task.Factory.StartNew(() => { twitterService.DeleteAllDataInDB(); });
            //Task task4 = Task.Factory.StartNew(() => twitterService.Start());

            //twitterService.StartCycle();

            //Task task1 = Task.Factory.StartNew(() => { twitterService.StartCycle(); });
            //Task task2 = Task.Factory.StartNew(() => { twitterService.StartSearchCitation(); });
            //Task task3 = Task.Factory.StartNew(() => { twitterService.StartSearchDirectMessage(); });
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
            //twitterService.IsStopping = true;
        }
    }
}
