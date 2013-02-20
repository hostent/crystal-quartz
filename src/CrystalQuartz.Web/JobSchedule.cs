using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalQuartz.Web
{
    public class JobSchedule
    {
        public string Name { get; set; }

        public string Group { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public string TriggerType { get; set; }

        public string TriggerState { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime NextFire { get; set; }

        public DateTime LastFire { get; set; }
    }
}
