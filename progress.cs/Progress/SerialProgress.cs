using System;
using System.Collections.Generic;

namespace Progress
{
    /// <summary>
    /// Runs given tasks one by one.
    /// </summary>
    public sealed class SerialProgress : AggregateProgressBase
    {
        public SerialProgress(IEnumerable<Tuple<int, IProgress>> tasks)
            : base(tasks)
        {
        }

        public override void Run()
        {
            foreach (var taskRecord in _tasks)
            {
                taskRecord.Item2.Run();
            }
        }
    }
}