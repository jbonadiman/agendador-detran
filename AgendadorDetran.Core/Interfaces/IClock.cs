using System;

namespace AgendadorDetran.Core.Interfaces
{
    public interface IClock
    {
        TimeSpan Tick { get; set; }
        
        void StopFor(TimeSpan duration);
        TimeSpan StopUntilCondition(Func<bool> predicate, TimeSpan timeout);
    }
}