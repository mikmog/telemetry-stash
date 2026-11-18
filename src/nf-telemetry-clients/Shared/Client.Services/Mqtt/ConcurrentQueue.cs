using System.Collections;

namespace TelemetryStash.NfClient.Services
{
    public class ConcurrentQueue
    {
        private readonly Queue _queue = new();

        public int Count
        {
            get
            {
                lock (_queue)
                {
                    return _queue.Count;
                }
            }
        }

        public object Dequeue()
        {
            lock (_queue)
            {
                return _queue.Dequeue();
            }
        }

        public void Enqueue(object item)
        {
            lock (_queue)
            {
                _queue.Enqueue(item);
            }
        }
    }
}
