using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace YouggyTw
{
    public class LimitManager
    {
        private List<Tuple<int, int>> limitsAndDurationInMinutes;

        public LimitManager(List<Tuple<int, int>> limitAndDurationInMinute)
        {
            limitsAndDurationInMinutes = limitAndDurationInMinute;
        }

        private List<DateTime> call = new List<DateTime>();

        public void AddCall()
        {
            // Remove old 
            int higherDuration = limitsAndDurationInMinutes.Max(x => x.Item2);

            call = call.Where(x => (DateTime.Now - x).TotalMinutes < higherDuration).ToList();

            foreach (Tuple<int, int> limitAndDurationInMinute in limitsAndDurationInMinutes)
            {
                // check limit only for the good duration
                List<DateTime> FilteredDateTime = new List<DateTime>();
                FilteredDateTime = call.Where(x => (DateTime.Now - x).TotalMinutes < limitAndDurationInMinute.Item2).ToList();

                if (FilteredDateTime.Count + 1 > limitAndDurationInMinute.Item1)
                {
                    Thread.Sleep(limitAndDurationInMinute.Item2 * 60000);
                }
            }            

            call.Add(DateTime.Now);
        }

        public void WaitBeforeNewTry()
        {
            Thread.Sleep(limitsAndDurationInMinutes.Max(x => x.Item2));
        }
    }
}
