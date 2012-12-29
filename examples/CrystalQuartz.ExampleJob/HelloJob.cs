using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace CrystalQuartz.ExampleJob
{
    public class HelloJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hello, CrystalQuartz!");
        }
    }
}
