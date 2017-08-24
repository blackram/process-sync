using FileLock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkSharing
{
    public class SingleScheduler
    {
        string LockPath { get; }
        Func<IWorker> NextWorker { get; }

        public SingleScheduler(Func<IWorker> nextWorker, string lockPath)
        {
            NextWorker = nextWorker;
            LockPath = lockPath;
        }

        public bool ScheduleWork(CancellationToken mainToken, TimeSpan until, int milliSecondsBeforeMoreWork)
        {
            var fileLock = SimpleFileLock.Create(LockPath, until);
            var acquired = fileLock.TryAcquireLock();
            if (!acquired)
                return false;

            var worker = NextWorker();

            var ending = DateTime.Now.Add(until);
            var workerTokenSource = new CancellationTokenSource(until.Add(TimeSpan.FromSeconds(2)));

            using (var combinedSource = CancellationTokenSource.CreateLinkedTokenSource(mainToken, workerTokenSource.Token))
            {
                try
                {
                    var task = Task.Run(async () =>
                    {
                        bool cancelled = false;
                        while (DateTime.Now < ending && !cancelled)
                        {
                            Console.WriteLine("Work, Work");
                            try
                            {
                                worker.Do();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                throw new Exception("While running Worker.Do", ex);
                            }

                            try
                            {
                                await Task.Delay(milliSecondsBeforeMoreWork, mainToken);
                            }
                            catch (TaskCanceledException)
                            {
                                Console.WriteLine("Simulated delay caused by work was cancelled");
                                cancelled = true;
                            }
                        };

                        Console.WriteLine("Work complete!");
                    }
                    , combinedSource.Token);

                    task.Wait();
                }
                catch (OperationCanceledException)
                {
                    if (mainToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Main process timed-out");
                    }
                    else if (workerTokenSource.IsCancellationRequested)
                    {
                        Console.WriteLine("Worker process took longer than allotted");
                    }
                }

            }

            fileLock.ReleaseLock(); // not strictly required in this case but does remove the lock file
            return true;
        }
    }
}
