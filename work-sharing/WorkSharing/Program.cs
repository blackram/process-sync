using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkSharing
{
    class Program
    {
        static void Main(string[] args)
        {
            var counterPath = args[0];
            var outPath = args[1];

            // Initialise the counter at given path
            System.IO.File.WriteAllText(counterPath, 0.ToString());

            // Clear all matching content from the outpath
            foreach(var file in System.IO.Directory.EnumerateFiles(outPath, "_*_"))
            {
                System.IO.File.Delete(file);
            }

            // Initialise a worker
            var w = new Worker(counterPath, outPath);

            // Some demo work
            for(var i=0;i<10;i++)
            {
                w.Do();
            }
        }
    }
}
