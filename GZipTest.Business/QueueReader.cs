using System;
using System.Collections.Generic;
using System.Threading;
using GZipTest.Abstractions;
using GZipTest.Data;

namespace GZipTest.Business
{
    public class QueueReader: IQueueReader
    {
        private readonly object _locker = new object();
        private readonly Queue<ByteBlock> _queue = new Queue<ByteBlock>();
        bool _isDead;
        private int _blockId;
        
        public void EnqueueForCompressing(byte[] buffer)
        {
            lock (_locker)
            {
                if (_isDead)
                    throw new InvalidOperationException("Queue stopped");

                while (_queue.Count > Constants.QueueCountLimit && !_isDead)
                {
                    Monitor.Wait(_locker);
                }
                var block = new ByteBlock
                {
                    Id = _blockId,
                    Buffer = buffer
                };
                _queue.Enqueue(block);
                _blockId++;
                Monitor.PulseAll(_locker);
            }
        }

        public void EnqueueForDecompressing(ByteBlock block)
        {
            int id = block.Id;
            lock (_locker)
            {
                if (_isDead)
                    throw new InvalidOperationException("Queue stopped");

                while (_queue.Count > Constants.QueueCountLimit && !_isDead)
                {
                    Monitor.Wait(_locker);
                }

                while (id != _blockId)
                {
                    Monitor.Wait(_locker);
                }

                _queue.Enqueue(block);
                _blockId++;
                Monitor.PulseAll(_locker);
            }

            GC.Collect();
        }

        public ByteBlock Dequeue()
        {
            lock (_locker)
            {
                while (_queue.Count == 0 && !_isDead)
                {
                    Monitor.Wait(_locker);
                }

                if (_queue.Count == 0)
                    return null;

                Monitor.PulseAll(_locker);
                return _queue.Dequeue();
            }
        }

        public void Stop()
        {
            lock (_locker)
            {
                _isDead = true;
                Monitor.PulseAll(_locker);
            }
        }
    }
}
