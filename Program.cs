using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Progress
{
    public interface IProgress
    {
        event EventHandler Changed;
        float Progress { get; }
        void Run();
    }

    public abstract class ProgressBase : IProgress
    {
        private float _progress;

        #region Implementation of IProgress

        public event EventHandler Changed;

        public float Progress
        {
            get
            {
                return _progress;
            }
            protected set
            {
                _progress = value;
                EventHandler handler = Changed;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        public abstract void Run();

        #endregion
    }

    public class FakeTask : ProgressBase
    {
        private readonly int _durationMs;
        private float _progress;

        #region Implementation of IProgress

        public override void Run()
        {
            int duration = _durationMs;
            Progress = 0;
            do
            {
                Thread.Sleep(50);
                duration -= 50;
                Progress = (_durationMs - duration) * 100f / _durationMs;
            } while (duration > 0);

            Progress = 100;
        }

        #endregion

        public FakeTask(int durationMs)
        {
            _durationMs = durationMs;
        }
    }

    public class ParallelProgress : SerialProgress
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

            Task.WaitAll();
        }
    }

    public class SerialProgress : ProgressBase
    {
        protected readonly IEnumerable<Tuple<int, IProgress>> _tasks;

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

        private readonly int _total;
        protected readonly List<TaskRecord> progresses;

        public SerialProgress(IEnumerable<Tuple<int, IProgress>> tasks)
        {
            _tasks = tasks;
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

        public override void Run()
        {
            foreach (var taskRecord in _tasks)
            {
                taskRecord.Item2.Run();
            }
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            FakeTask t1 = new FakeTask(5000);
            FakeTask t2 = new FakeTask(1000);
            FakeTask t3 = new FakeTask(2500);

            SerialProgress p = new SerialProgress(
                new[]
                    {
                        new Tuple<int, IProgress>(10, t1),
                        new Tuple<int, IProgress>(2, t2),
                        new Tuple<int, IProgress>(5, t3)
                    }
                );

            p.Changed += t_Changed;

            Stopwatch watch = Stopwatch.StartNew();
            p.Run();
            watch.Stop();

            Console.WriteLine();
            Console.WriteLine("{0}ms elapsed.", watch.ElapsedMilliseconds);
        }

        private static void t_Changed(object sender, EventArgs e)
        {
            IProgress task = sender as IProgress;
            int pad = 11;
            int width = Console.WindowWidth - pad;
            int progress = (int) (width * task.Progress) / 100;

            Console.Write("\r{0,6:0.00}% [{1}{2}]", task.Progress, new String('=', progress),
                          new String(' ', width - progress));
        }
    }
}
