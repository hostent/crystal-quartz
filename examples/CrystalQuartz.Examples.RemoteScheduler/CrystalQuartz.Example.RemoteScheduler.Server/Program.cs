namespace CrystalQuartz.Example.RemoteScheduler.Server
{
    using System;
    using System.Collections.Specialized;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Job;
    using CrystalQuartz.ExampleJob;
    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {

            HostFactory.Run(x =>                                 //1
            {
                x.Service<SchedulerServer>(s =>                        //2
                {
                    s.ConstructUsing(name => new SchedulerServer());     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                });
                x.RunAsLocalSystem();                            //6
                //x.UseNLog();
                x.SetDescription("SchedulerServer Topshelf Host");        //7
                x.SetDisplayName("SchedulerServer");                       //8
                x.SetServiceName("SchedulerServer");                       //9
            });                                          
        }
    }
}
