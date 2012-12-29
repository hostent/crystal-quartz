using System;
using Quartz;
using Quartz.Job;
using Spring.Context.Support;

namespace CrystalQuartz.Web.Demo
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var context = ContextRegistry.GetContext();
            var scheduler = (IScheduler) context.GetObject("scheduler");

            scheduler.Start();

            var jobDetail = new JobDetail("myJob", null, typeof(NoOpJob));
            var trigger = TriggerUtils.MakeHourlyTrigger();
            trigger.StartTimeUtc = DateTime.UtcNow;
            trigger.Name = "myTrigger";
            scheduler.ScheduleJob(jobDetail, trigger); 
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}