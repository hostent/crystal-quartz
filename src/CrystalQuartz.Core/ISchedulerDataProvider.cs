using System;
using System.Collections.Generic;

namespace CrystalQuartz.Core
{
    using Domain;

    /// <summary>
    /// Translates Quartz.NET entyties to CrystalQuartz objects graph.
    /// </summary>
    public interface ISchedulerDataProvider
    {
        SchedulerData Data { get; }

        JobDetailsData GetJobDetailsData(string name, string group);

        IEnumerable<ActivityEvent> GetJobEvents(string name, DateTime minDateUtc, DateTime maxDateUtc);
    }
}