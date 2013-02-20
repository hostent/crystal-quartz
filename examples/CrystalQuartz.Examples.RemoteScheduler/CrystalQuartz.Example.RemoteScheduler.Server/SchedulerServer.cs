using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;
using Quartz.Impl;
using Quartz;
using CrystalQuartz.ExampleJob;

namespace CrystalQuartz.Example.RemoteScheduler.Server
{
    public class SchedulerServer
    {
        StdSchedulerFactory schedulerFactory;

        public SchedulerServer()
        {
            schedulerFactory = new StdSchedulerFactory();
        }

        public void Start()
        {
            Console.WriteLine("Starting scheduler...");
            
            var scheduler = schedulerFactory.GetScheduler();

            // define the job and ask it to run
            var map = new JobDataMap();
            map.Put("msg", "Some message!");

            var job = JobBuilder.Create<HelloJob>()
                .WithIdentity("localJob", "default")
                .UsingJobData(map).RequestRecovery(true).Build();

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                    .WithIdentity("remotelyAddedTrigger", "default")
                                                    .WithCronSchedule("/5 * * ? * *")
                                                    .StartAt(DateTimeOffset.UtcNow)
                                                    .ForJob(job)                            
                                                    .Build();
            if (!scheduler.CheckExists(job.Key))
            {
                scheduler.ScheduleJob(job, trigger);
            }

            scheduler.Start();                            
        }

        public void Stop()
        {
            if (schedulerFactory != null)
            {
                var scheduler = schedulerFactory.GetScheduler();
                scheduler.Shutdown(true);
            }
        }
    }
}
