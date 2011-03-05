using System;
using System.Collections.Generic;
using System.Linq;

namespace Progress
{
    /// <summary>
    /// Provides basic task execution progress aggregation functionality.
    /// </summary>
    public abstract class AggregateProgressBase : ProgressBase
    {
        protected IEnumerable<Tuple<int, IProgress>> _tasks;
        private readonly int _total;
        protected List<TaskRecord> progresses;

        protected AggregateProgressBase(IEnumerable<Tuple<int, IProgress>> tasks)
        {
            this._tasks = tasks;
            _total = tasks.Sum(p => p.Item1);
            progresses = new List<TaskRecord>();

            foreach (var tuple in tasks)
            {
                int work = tuple.Item1;
                IProgress task = tuple.Item2;

                TaskRecord handle = new TaskRecord((float) work / _total, task.Progress);
                task.Changed += Handle(handle);
                progresses.Add(handle);
            }
        }

        protected sealed class TaskRecord
        {
            public readonly float magnitude;
            public float progress;

            public TaskRecord(float work, float progress)
            {
                magnitude = work;
                this.progress = progress;
            }
        }

        private void Update()
        {
            Progress = progresses.Sum(p => p.progress * p.magnitude);
        }

        private EventHandler Handle(TaskRecord handle)
        {
            return (sender, args) =>
                       {
                           IProgress task = sender as IProgress;
                           handle.progress = task.Progress;
                           Update();
                       };
        }
    }
}