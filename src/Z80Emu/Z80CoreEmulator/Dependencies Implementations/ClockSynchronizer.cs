using System.Diagnostics;

namespace Konamiman.Z80dotNet
{
    /// <summary>
    /// Default implementation of <see cref="IClockSynchronizer"/>.
    /// </summary>
    public class ClockSynchronizer : IClockSynchronizer, IDisposable
    {
        private const decimal MinMicrosecondsToWait = 10 * 1000;

        public decimal EffectiveClockFrequencyInMHz { get; set; }

        private readonly bool waiting;
        private readonly Stopwatch stopwatch;

        private decimal accummulatedMicroseconds;
        private bool disposedValue;

        public ClockSynchronizer()
        {
            stopwatch = new Stopwatch();
        }

        public void Start()
        {
            stopwatch.Reset();
        }

        public void Stop()
        {
            stopwatch.Stop();
        }

        public void TryWait(int periodLengthInCycles)
        {
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            accummulatedMicroseconds += periodLengthInCycles / EffectiveClockFrequencyInMHz;
            var accummulatedMicrosecondsLong = accummulatedMicroseconds;
            var microsecondsPending = accummulatedMicrosecondsLong - elapsedMilliseconds;

            if (microsecondsPending >= MinMicrosecondsToWait)
            {
                var sleeptime = microsecondsPending / 1000;
                if (sleeptime > 1)
                {
                    Thread.Sleep((int)sleeptime);
                }
                else
                {
                    Thread.Sleep(1);
                }
                accummulatedMicroseconds = 0;
                stopwatch.Restart();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // nothing to dispose
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
