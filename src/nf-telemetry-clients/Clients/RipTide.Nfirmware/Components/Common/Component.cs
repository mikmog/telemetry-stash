using System.Threading;

namespace RipTide.Nfirmware.Components.Common
{
    public abstract class Component
    {
        protected readonly ErrorHandler _errorHandler;
        private Thread _runnerThread;

        protected Component(ErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        protected void Start(ThreadStart threadStart)
        {
            _runnerThread = new Thread(threadStart);
            _runnerThread.Start();

            OnInitialized?.Invoke();
        }

        public abstract void Initialize(AppSettings appSettings);

        public delegate void InitializedEventArgs();
        public event InitializedEventArgs OnInitialized;
    }
}
