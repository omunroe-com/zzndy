using System.Threading;

namespace Progress
{
    /// <summary>
    /// Simulate busy task by <see cref="Thread.Sleep(int)"/>-ing specified amount of time.
    /// </summary>
    public sealed class FakeTask : ProgressBase
    {
        private readonly int _durationMs;

        public FakeTask(int durationMs)
        {
            _durationMs = durationMs;
        }

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
    }
}