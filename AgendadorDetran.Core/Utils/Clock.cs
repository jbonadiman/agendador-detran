using System;
using System.Diagnostics;
using System.Threading;
using AgendadorDetran.Core.Interfaces;

namespace AgendadorDetran.Core.Utils
{
    public class Clock : IClock
    {
        public TimeSpan Tick { get; set; }
        
        public Clock(TimeSpan tick)
        {
            this.Tick = tick;
        }
        
        // /// <summary>
        // /// In milliseconds
        // /// </summary>
        // private const ushort TimeTick = 100;

        /// <summary>
        /// Stops the execution and waits for the amount of time specified
        /// </summary>
        /// <param name="duration">A <see cref="TimeSpan"/> object representing the duration</param>
        public void StopFor(TimeSpan duration)
        {
            void Callback(object state)
            {
                duration -= this.Tick;
                if (duration.CompareTo(TimeSpan.Zero) < 1)
                {
                    ((AutoResetEvent) state).Set();
                }
            }

            var autoEvent = new AutoResetEvent(false);
            var timer = new Timer(Callback, autoEvent, TimeSpan.Zero, this.Tick);
            
            autoEvent.WaitOne();
            
            timer.Dispose();
        }

        /// <summary>
        /// Waits for a specified condition to be true
        /// </summary>
        /// <param name="predicate">The condition that should be tested every <see cref="Clock"/> tick</param>
        /// <param name="timeout">The timeout as a <see cref="TimeSpan"/></param>
        /// <returns>The elapsed time</returns>
        public TimeSpan StopUntilCondition(Func<bool> predicate, TimeSpan timeout)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.Elapsed <= timeout)
            {
                if (predicate())
                {
                    stopwatch.Stop();
                    return stopwatch.Elapsed;
                }

                this.StopFor(this.Tick);
            }

            stopwatch.Stop();
            throw new TimeoutException(
                $"After waiting for {timeout.TotalSeconds} second(s) the expected condition was not fulfilled");
        }
    }
}