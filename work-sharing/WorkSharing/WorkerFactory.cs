using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkSharing
{
    public class WorkerFactory
    {
        int ProcessId { get; }

        public WorkerFactory(int processId, string counterPath, string outPath)
        {
            ProcessId = processId;
            CounterPath = counterPath;
            OutPath = outPath;
        }

        string CounterPath { get; }
        string OutPath { get; }

        public IWorker Create()
        {
            return new Worker(CounterPath, OutPath, ProcessId.ToString());
        }
    }
}
