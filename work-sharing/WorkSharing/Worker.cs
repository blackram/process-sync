using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkSharing
{
    public class Worker : IWorker
    {
        private string CounterPath { get; }
        private string OutFolder { get; }
        private string Label { get; }

        public Worker(string counterPath, string outFolder, string label)
        {
            CounterPath = counterPath;
            OutFolder = outFolder;
            Label = label;
        }

        public void Do()
        {
            int? counter = null;
            
            using (var cf = System.IO.File.Open(CounterPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read))
            {
                using (var sr = new System.IO.StreamReader(cf))
                {
                    counter = Convert.ToInt32(sr.ReadToEnd());
                }
            }
                
            counter = counter + 1;

            using (var cf = System.IO.File.Open(CounterPath, System.IO.FileMode.Open, System.IO.FileAccess.Write))
            {
                using (var sw = new System.IO.StreamWriter(cf))
                {
                    cf.Seek(0,System.IO.SeekOrigin.Begin);
                    sw.Write(counter);
                    sw.Flush();
                }
            }

            // for this demo, the file names will collide and cause the demo to stop if the locking has failed

            var targetFile = System.IO.Path.Combine(OutFolder, $"_{counter}_");
            
            System.IO.File.WriteAllText(targetFile, $"{Label} - {counter}");
        }
    }
}
