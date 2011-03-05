using System;

namespace Progress
{
    /// <summary>
    /// Provides basic progress reporting functionality.
    /// </summary>
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
}