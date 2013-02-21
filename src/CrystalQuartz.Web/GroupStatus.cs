using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrystalQuartz.Web
{
    public class GroupStatus
    {
        public string Group { get; set; }

        public bool IsJobGroupPaused { get; set; }

        public bool IsTriggerGroupPaused { get; set; }
    }
}
