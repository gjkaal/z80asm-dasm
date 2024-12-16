using AutoFixture;
using System.Diagnostics;

namespace Konamiman.Z80dotNet.Tests
{
    public class ClockSynchronizationHelperTests : IDisposable
    {
        private const int MinMicrosecondsToWait = 10 * 1000;
        private bool disposedValue;

        private ClockSynchronizer Sut { get; set; }
        private Fixture Fixture { get; set; }

        [SetUp]
        [Repeat(1000)]
        public void Setup()
        {
            Sut = new ClockSynchronizer();
            Sut.EffectiveClockFrequencyInMHz = 1;
            Fixture = new Fixture();
        }

        [Test]
        public void TryWait_works_with_repeated_short_intervals()
        {
            var expected = 50;
            var totalCyclesToWait = 50000;

            Sut.Start();

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < (totalCyclesToWait / 5); i++)
            {
                Sut.TryWait(5);
            }
            sw.Stop();

            var actual = sw.ElapsedMilliseconds;
            Assert.That(actual >= expected - 30 && actual <= expected + 30, $"Expected : {expected} (+/- 30) Actual value: {actual}");
        }

        [Test]
        public void TryWait_works_with_one_long_interval()
        {
            Sut.Start();

            var sw = new Stopwatch();
            sw.Start();

            var totalCyclesToWait = 50000;
            var expected = 50;

            Sut.TryWait(totalCyclesToWait);

            sw.Stop();

            var actual = sw.ElapsedMilliseconds;
            Assert.That(actual >= expected - 15 && actual <= expected + 15, "Actual value: " + actual);
        }

        [Test]
        public void TryWait_works_repeatedly()
        {
            Sut.Start();

            var sw = new Stopwatch();

            var totalCyclesToWait = 50000;
            var expected = 50;

            sw.Start();
            Sut.TryWait(totalCyclesToWait);
            sw.Stop();
            var actual = sw.ElapsedMilliseconds;

            sw.Restart();
            Sut.TryWait(totalCyclesToWait);
            sw.Stop();
            var actual2 = sw.ElapsedMilliseconds;

            Assert.That(actual >= expected - 15 && actual <= expected + 15, $"Expected : {expected} (+/- 15) Actual value 1: {actual}");

            actual = sw.ElapsedMilliseconds;
            Assert.That(actual2 >= expected - 15 && actual2 <= expected + 15, "Actual value 2: " + actual2);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Sut.Dispose();
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
