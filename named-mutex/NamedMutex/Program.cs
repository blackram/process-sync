using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Locker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Starting");

            using (var mutex = new Mutex(false, "Testicle"))
            {
                Console.WriteLine("Mutex initialised");

                bool acquired = false;
                try
                {
                    acquired = mutex.WaitOne(new TimeSpan(0, 1, 0));
                }
                catch (AbandonedMutexException)
                {
                    Console.Write("a mutex was abandoned without being released");
                }

                if (!acquired)
                {
                    Console.WriteLine("Didn't acquire the mutex in time");
                }
                else
                {
                    Console.WriteLine("Work, Work");

                    Thread.Sleep(10 * 1000); // 10 seconds of work

                    mutex.ReleaseMutex(); // finish

                    Console.WriteLine("Work Complete!");
                }
            }

            Console.WriteLine($"Finished");
        }
    }
}
