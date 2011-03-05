using System;
using System.Collections.Generic;
using System.Diagnostics;
using Progress;

namespace ConsoleProgress
{
    internal class Program
    {
        private static void Main()
        {
            FakeTask t1 = new FakeTask(5000);
            FakeTask t2 = new FakeTask(1000);
            FakeTask t3 = new FakeTask(2500);

            Tuple<int, IProgress>[] tasks 
                = new[]
                                                {
                                                    new Tuple<int, IProgress>(10, t1),
                                                    new Tuple<int, IProgress>(2, t2),
                                                    new Tuple<int, IProgress>(5, t3)
                                                };

            Console.WriteLine("Serial:");
            RunTasks(tasks, true);
            Console.WriteLine();

            Console.WriteLine("Parallel:");
            RunTasks(tasks, false);
        }

        private static void RunTasks(IEnumerable<Tuple<int, IProgress>> tasks, bool serail)
        {
            IProgress p = GetProgress(serail, tasks);

            p.Changed += OnProgressChanged;

            Stopwatch watch = Stopwatch.StartNew();
            p.Run();
            watch.Stop();

            Console.WriteLine();
            Console.WriteLine("{0}ms elapsed.", watch.ElapsedMilliseconds);
        }

        private static IProgress GetProgress(bool serial, IEnumerable<Tuple<int, IProgress>> tasks)
        {
            return serial ? new SerialProgress(tasks) : new ParallelProgress(tasks) as IProgress;
        }

        private static void OnProgressChanged(object sender, EventArgs e)
        {
            IProgress task = sender as IProgress;
            if (task != null)
            {
                int pad = 11;
                int width = Console.WindowWidth - pad;

                int progress = (int) (width * task.Progress) / 100;

                Console.Write("\r{0,6:0.00}% [{1}{2}]", task.Progress, new String('=', progress),
                              new String(' ', width - progress));
            }
        }
    }
}
