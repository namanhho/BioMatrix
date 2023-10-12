using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore
{
    public class SyncTimeConfig
    {
        public int Hour { get; set; } = 10;
        public int Minute { get; set; } = 0;
        public string SyncTime { get { return $"{Hour:00}:{Minute:00}";  } }
    }
}
