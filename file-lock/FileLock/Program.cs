using FileLock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileLocking
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Starting");

            var lockFolder = args[0];

            var path = System.IO.Path.Combine(lockFolder, "file.lock");
            var fileLock = SimpleFileLock.Create(path, TimeSpan.FromMinutes(1));

            Console.WriteLine("Lock Initialised");

            var cts = new CancellationTokenSource();
            cts.CancelAfter(new TimeSpan(0, 1, 0));

            bool acquired = false;

            var task = Task.Run(async () =>
            {
                while (!acquired)
                {
                    Console.WriteLine("Trying to acquire");
                    acquired = fileLock.TryAcquireLock();
                    if (!acquired)
                    {
                        Console.WriteLine("Waiting before trying again");
                        await Task.Delay(1000, cts.Token);
                    }
                }
            }, cts.Token);

            task.Wait();

            if (!acquired)
            {
                Console.WriteLine("Didn't acquire the lock in time");
            }
            else
            {
                var stillAcquired = fileLock.TryAcquireLock();
                Console.WriteLine($"Still acquired: {stillAcquired}");

                Console.WriteLine("Work, Work");
                Thread.Sleep(10 * 1000); // 10 seconds of work
                fileLock.ReleaseLock();
                Console.WriteLine("Work Complete!");
            }

            Console.WriteLine("Finished");
        }
    }
}
