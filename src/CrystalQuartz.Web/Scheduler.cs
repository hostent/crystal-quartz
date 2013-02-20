using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace CrystalQuartz.Web
{
    
        public class Scheduler
        {
            public readonly IScheduler Instance;
            public string Address { get; private set; }
            public string JobName { get; set; }
            public string JobGroup { get; set; }
            public int Priority { get; set; }
            public string CronExpression { get; set; }

            private readonly ISchedulerFactory _schedulerFactory;


            public Scheduler(string server, int port, string scheduler)
            {
                Address = string.Format("tcp://{0}:{1}/{2}", server, port, scheduler);
                _schedulerFactory = new StdSchedulerFactory(GetProperties(Address));

                try
                {
                    Instance = _schedulerFactory.GetScheduler();

                    if (!Instance.IsStarted)
                        Instance.Start();
                }
                catch (SchedulerException ex)
                {
                    throw new Exception(string.Format("Failed: {0}", ex.Message));
                }
            }

            private static NameValueCollection GetProperties(string address)
            {
                var properties = new NameValueCollection();
                properties["quartz.scheduler.instanceName"] = "ServerScheduler";
                properties["quartz.scheduler.proxy"] = "true";
                properties["quartz.threadPool.threadCount"] = "0";
                properties["quartz.scheduler.proxy.address"] = address;
                return properties;
            }

            public IScheduler GetScheduler()
            {
                return Instance;
            }

            public List<GroupStatus> GetGroups()
            {
                var results = new List<GroupStatus>();
                foreach (var gp in Instance.GetJobGroupNames())
                {
                    results.Add(new GroupStatus()
                    {
                        Group = gp,
                        IsJobGroupPaused = Instance.IsJobGroupPaused(gp),
                        IsTriggerGroupPaused = Instance.IsTriggerGroupPaused(gp)
                    });
                }
                return results;
            }

            public JobSchedule GetSchedule()
            {
                var jobKey = new JobKey(JobName, JobGroup);

                var trigger = Instance.GetTriggersOfJob(jobKey).FirstOrDefault();

                var js = new JobSchedule();

                if (trigger != null)
                {
                    js.Name = trigger.Key.Name;
                    js.Group = trigger.Key.Group;
                    js.Description = trigger.Description;
                    js.Priority = trigger.Priority;
                    js.TriggerType = trigger.GetType().Name;
                    js.TriggerState = Instance.GetTriggerState(trigger.Key).ToString();

                    DateTimeOffset? startTime = trigger.StartTimeUtc;
                    js.StartTime = TimeZone.CurrentTimeZone.ToLocalTime(startTime.Value.DateTime);

                    var nextFireTime = trigger.GetNextFireTimeUtc();
                    if (nextFireTime.HasValue)
                    {
                        js.NextFire = TimeZone.CurrentTimeZone.ToLocalTime(nextFireTime.Value.DateTime);
                    }

                    var previousFireTime = trigger.GetPreviousFireTimeUtc();
                    if (previousFireTime.HasValue)
                    {
                        js.LastFire = TimeZone.CurrentTimeZone.ToLocalTime(previousFireTime.Value.DateTime);
                    }
                }

                return js;
            }

            public List<JobSchedule> GetSchedules()
            {
                var jcs = new List<JobSchedule>();

                foreach (var group in Instance.GetJobGroupNames())
                {
                    var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                    var jobKeys = Instance.GetJobKeys(groupMatcher);

                    foreach (var jobKey in jobKeys)
                    {
                        var triggers = Instance.GetTriggersOfJob(jobKey);
                        foreach (var trigger in triggers)
                        {
                            var js = new JobSchedule();
                            js.Name = jobKey.Name;
                            js.Group = jobKey.Group;
                            js.TriggerType = trigger.GetType().Name;
                            js.TriggerState = Instance.GetTriggerState(trigger.Key).ToString();
                            js.Priority = trigger.Priority;

                            DateTimeOffset? startTime = trigger.StartTimeUtc;
                            js.StartTime = TimeZone.CurrentTimeZone.ToLocalTime(startTime.Value.DateTime);

                            DateTimeOffset? nextFireTime = trigger.GetNextFireTimeUtc();
                            if (nextFireTime.HasValue)
                            {
                                js.NextFire = TimeZone.CurrentTimeZone.ToLocalTime(nextFireTime.Value.DateTime);
                            }

                            DateTimeOffset? previousFireTime = trigger.GetPreviousFireTimeUtc();
                            if (previousFireTime.HasValue)
                            {
                                js.LastFire = TimeZone.CurrentTimeZone.ToLocalTime(previousFireTime.Value.DateTime);
                            }

                            jcs.Add(js);
                        }
                    }
                }
                return jcs;
            }

            public List<JobSchedule> GetSchedules(string groupName)
            {
                var jcs = new List<JobSchedule>();

                var groupMatcher = GroupMatcher<JobKey>.GroupContains(groupName);
                var jobKeys = Instance.GetJobKeys(groupMatcher);

                foreach (var jobKey in jobKeys)
                {
                    var triggers = Instance.GetTriggersOfJob(jobKey);
                    foreach (var trigger in triggers)
                    {
                        var js = new JobSchedule();
                        js.Name = jobKey.Name;
                        js.Description = trigger.Description;
                        js.Group = jobKey.Group;
                        js.TriggerType = trigger.GetType().Name;
                        js.TriggerState = Instance.GetTriggerState(trigger.Key).ToString();
                        js.Priority = trigger.Priority;

                        DateTimeOffset? startTime = trigger.StartTimeUtc;
                        js.StartTime = TimeZone.CurrentTimeZone.ToLocalTime(startTime.Value.DateTime);

                        DateTimeOffset? nextFireTime = trigger.GetNextFireTimeUtc();
                        if (nextFireTime.HasValue)
                        {
                            js.NextFire = TimeZone.CurrentTimeZone.ToLocalTime(nextFireTime.Value.DateTime);
                        }

                        DateTimeOffset? previousFireTime = trigger.GetPreviousFireTimeUtc();
                        if (previousFireTime.HasValue)
                        {
                            js.LastFire = TimeZone.CurrentTimeZone.ToLocalTime(previousFireTime.Value.DateTime);
                        }

                        jcs.Add(js);
                    }
                }
                return jcs;
            }

            public string GetMetaData()
            {
                var metaData = Instance.GetMetaData();

                return string.Format(
                    "{0}Name: '{1}'{0}Version: '{2}'{0}ThreadPoolSize: '{3}'{0}IsRemote: '{4}'{0}JobStoreName: '{5}'                 {0}SupportsPersistance: '{6}'{0}IsClustered: '{7}'",
                    Environment.NewLine, metaData.SchedulerName, metaData.Version, metaData.ThreadPoolSize,
                    metaData.SchedulerRemote, metaData.JobStoreType.Name, metaData.JobStoreSupportsPersistence,
                    metaData.JobStoreClustered);
            }

            public bool UnscheduleJob()
            {
                var jobKey = new JobKey(JobName, JobGroup);

                if (Instance.CheckExists(jobKey))
                {
                    return Instance.UnscheduleJob(new TriggerKey(JobName, JobGroup));
                }
                return false;
            }

            public bool UnscheduleAll()
            {
                foreach (var group in Instance.GetTriggerGroupNames())
                {
                    var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                    var jobKeys = Instance.GetJobKeys(groupMatcher);

                    foreach (var triggers in jobKeys.Select(jobKey => Instance.GetTriggersOfJob(jobKey)))
                    {
                        return Instance.UnscheduleJobs(triggers.Select(t => t.Key).ToList());
                    }
                }
                return false;
            }

            public void DeleteAll()
            {
                Instance.Clear();
            }

            public void RescheduleJob()
            {
                // Build new trigger
                var trigger = (ICronTrigger)TriggerBuilder.Create()
                    .WithIdentity(JobName, JobGroup)
                    .WithCronSchedule(CronExpression)
                    .WithPriority(Priority)
                    //.StartAt(StartAt.ToUniversalTime())
                    .Build();

                Instance.RescheduleJob(new TriggerKey(JobName, JobGroup), trigger);
            }
        }
}
