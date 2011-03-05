using System;

namespace Progress
{
    public interface IProgress
    {
        event EventHandler Changed;
        float Progress { get; }
        void Run();
    }
}