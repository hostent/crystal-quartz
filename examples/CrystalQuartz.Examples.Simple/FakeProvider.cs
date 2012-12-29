namespace CrystalQuartz.Web.Demo
{
    using System;
    using Core.SchedulerProviders;
    using Quartz;

    public class FakeProvider : StdSchedulerProvider
    {
        protected override System.Collections.Specialized.NameValueCollection GetSchedulerProperties()
        {
            var properties = base.GetSchedulerProperties();
            properties.Add("test1", "test1value");
            return properties;
        }

        protected override void InitScheduler(IScheduler scheduler)
        {

            // construct job info
            IJobDetail jobDetail = JobBuilder.Create<HelloJob>()
                .WithIdentity(new JobKey("myJob", "group1"))
                .Build();
            // fire every hour
            //jobDetail. = true;
            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                     .WithIdentity("myTrigger", "group1")
                                                     .WithCronSchedule("0/20 * * * * ?")
                                                     .Build();


            //ITrigger trigger = TriggerUtils.MakeMinutelyTrigger(1, 2);
            //// start on the next even hour
            //trigger.StartTimeUtc = DateTime.UtcNow;
            //trigger.Name = "myTrigger";
            scheduler.ScheduleJob(jobDetail, trigger);

            // construct job info
            IJobDetail jobDetail2 = JobBuilder.Create<HelloJob>()
                .WithIdentity("myJob2", "group1")
                .Build();

            ITrigger trigger2 = (ICronTrigger)TriggerBuilder.Create()
                                        .WithIdentity("myTrigger2", "group1")
                                        .WithCronSchedule("15 0/2 * * * ?")
                                        .Build();

            //// fire every hour
            //Trigger trigger2 = TriggerUtils.MakeMinutelyTrigger(1, 2);
            //// start on the next even hour
            //trigger2.StartTimeUtc = DateTime.UtcNow;
            //trigger2.Name = "myTrigger2";
            scheduler.ScheduleJob(jobDetail2, trigger2);

            //Trigger trigger3 = TriggerUtils.MakeSecondlyTrigger(5, 5);
            //// start on the next even hour
            //trigger3.StartTimeUtc = DateTime.UtcNow;
            //trigger3.Name = "myTrigger3";
            //trigger3.JobName = "myJob2";
            //scheduler.ScheduleJob(trigger3);

            //// construct job info
            //JobDetail jobDetail4 = new JobDetail("myJob4", null, typeof(HelloJob));
            //jobDetail4.Group = "MyOwnGroup";
            //jobDetail4.JobDataMap.Add("key1", "value1");
            //jobDetail4.JobDataMap.Add("key2", "value2");
            //jobDetail4.JobDataMap.Add("key3", 11);
            //jobDetail4.JobDataMap.Add("key4", 1d);
            //// fire every hour
            //ITrigger trigger4 = TriggerBuilder.Create().MakeMinutelyTrigger(1, 1);
            //// start on the next even hour
            //trigger4.StartTimeUtc = DateTimeOffset.UtcNow;
            //trigger4.Name = "myTrigger4";
            //trigger4.Group = jobDetail4.Group;
            //scheduler.ScheduleJob(jobDetail4, trigger4);

            //scheduler.PauseJob(new JobKey("myJob4", "MyOwnGroup"));
            //scheduler.PauseTrigger(new TriggerKey("myTrigger3", "DEFAULT"));
        }
    }
}