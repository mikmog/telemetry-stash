using System;
using System.Threading;

namespace TelemetryStash.Shared
{
    public class TimerRunner : IDisposable
    {
        private readonly TimeSpan _notificationInterval;
        private Timer _notifyTimer;

        public TimerRunner(TimeSpan notificationInterval)
        {
            _notificationInterval = notificationInterval;
        }

        public void Start()
        {
            _notifyTimer = new Timer(OnNotificationEvent, null, TimeSpan.FromSeconds(1), _notificationInterval);
        }

        public void Stop()
        {
            _notifyTimer.Dispose();
            _notifyTimer = null;
        }

        public void Dispose()
        {
            Stop();
        }

        private void OnNotificationEvent(object state)
        {
            TimerElapsed?.Invoke(DateTime.UtcNow);
        }

        public delegate void TimerElapsedEventHandler(DateTime timestamp);
        public event TimerElapsedEventHandler TimerElapsed;
    }
}
