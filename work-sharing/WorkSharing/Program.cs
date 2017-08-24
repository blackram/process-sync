using FileLock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkSharing
{
    class Program
    {
        const int TotalMinutesDuration = 2;
        const string FileLock = "file.lock";
        const int SecondsLeewayForCancellationSource = 5;
        const int SecondsForScheduledWorkToBeCarriedOut = 10;
        const int DelayInSecondsBeforeSchedulingAgain=3;

        static void Main(string[] args)
        {
            var counterPath = args[0];
            var outPath = args[1];

            // only one process should reset - not part of the demo

            if (args.Length > 2 && args[2] == "reset")
            {
                ResetDemoEnvironment(counterPath, outPath);
                return;
            }
            else
                SetupDemo(counterPath, outPath);
        }

        static void ResetDemoEnvironment(string counterPath, string outPath)
        {
            // Initialise the counter at given path
            System.IO.File.WriteAllText(counterPath, 0.ToString());

            // Clear all matching content from the outpath
            foreach (var file in System.IO.Directory.EnumerateFiles(outPath, "_*_*"))
            {
                System.IO.File.Delete(file);
            }
        }

        static void SetupDemo(string counterPath, string outPath)
        {
            var lockPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(counterPath), FileLock);

            var duration = TimeSpan.FromMinutes(TotalMinutesDuration);
            var runUntil = DateTime.Now.Add(duration);

            var workerFactory = new WorkerFactory(System.Diagnostics.Process.GetCurrentProcess().Id, counterPath, outPath);
            var singleScheduler = new SingleScheduler(workerFactory.Create, lockPath);

            using (var mainTokenSource = new CancellationTokenSource(duration.Add(TimeSpan.FromSeconds(SecondsLeewayForCancellationSource))))
            {
                try
                {
                    var mainTask = Task.Run(async () =>
                    {
                        Console.WriteLine("Starting Main Processing");

                        while (DateTime.Now < runUntil)
                        {
                            await StartDemo(mainTokenSource.Token, singleScheduler);
                        }

                    }, mainTokenSource.Token);

                    mainTask.Wait();
                }
                catch (OperationCanceledException)
                {
                    if (mainTokenSource.IsCancellationRequested)
                    {
                        Console.WriteLine("Main process cancelled in main loop");
                    }
                }
            }
        }      

        static async Task StartDemo(CancellationToken mainToken, SingleScheduler scheduler)
        {
            Console.WriteLine("About to try to schedule work ");

            var until = TimeSpan.FromSeconds(SecondsForScheduledWorkToBeCarriedOut);
            var scheduled = scheduler.ScheduleWork(mainToken, until, 30);

            if (scheduled)
            {
                Console.WriteLine($"Scheduled task completed");
            }
            else
            {
                Console.WriteLine("Waiting before trying to get a lock again");

                try
                {
                    await Task.Delay(DelayInSecondsBeforeSchedulingAgain*1000, mainToken);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("blocking delay in progres when task was cancelled");
                }
            }
        }
    }
}
