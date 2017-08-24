﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkSharing
{
    public class Worker
    {
        private string CounterPath { get; }
        private string OutFolder { get; }

        public Worker(string counterPath, string outFolder)
        {
            CounterPath = counterPath;
            OutFolder = outFolder;
        }

        public void Do()
        {
            int? counter = null;

            using (var cf = System.IO.File.Open(CounterPath, System.IO.FileMode.OpenOrCreate))
            {
                using (var sr = new System.IO.StreamReader(cf))
                {
                    counter = Convert.ToInt32(sr.ReadToEnd());
                }
                
                counter = counter + 1;

                using (var sw = new System.IO.StreamWriter(cf))
                {
                    sw.Write(Convert.ToString(counter));
                    sw.Flush();
                }
            }

            var targetFile = System.IO.Path.Combine(OutFolder, $"_{counter}_");

            System.IO.File.WriteAllText(targetFile, counter.ToString());
        }
    }
}