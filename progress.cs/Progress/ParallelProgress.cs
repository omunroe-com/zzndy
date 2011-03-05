using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Progress
{
    /// <summary>
    /// Runs provided tasks simultaneously.
    /// </summary>
    public sealed class ParallelProgress : AggregateProgressBase
    {
        public ParallelProgress(IEnumerable<Tuple<int, IProgress>> tasks)
            : base(tasks)
        {
        }

        public override void Run()
        {
            Task[] tasks = new Task[progresses.Count];
            int i = 0;

            foreach (var tuple in _tasks)
            {
                Tuple<int, IProgress> tuple1 = tuple;
                tasks[i++] = Task.Factory.StartNew(() => tuple1.Item2.Run());
            }

            Task.WaitAll(tasks);
        }
    }
}