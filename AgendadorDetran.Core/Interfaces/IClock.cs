using System;
using System.Threading.Tasks;

namespace AgendadorDetran.Core.Interfaces
{
    public interface IClock
    {
        TimeSpan Tick { get; set; }
        
        void StopFor(TimeSpan duration);
        Task<TimeSpan> StopUntilConditionAsync(Func<Task<bool>> predicate, TimeSpan timeout);
        TimeSpan StopUntilCondition(Func<bool> predicate, TimeSpan timeout);
    }
}