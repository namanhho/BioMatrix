using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore
{
    public class Setting
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class UserOption
    {
        public SyncOption SyncOption { get; set; } = new SyncOption();
    }
    public class SyncOption
    {
        public List<SyncTimeConfig> SyncTimes { get; set; } = new List<SyncTimeConfig>();
    }
}
