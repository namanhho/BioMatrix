using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore
{
    public class Hikvision
    {
        public class EventInfo
        {
            public string major;
            public string minor;
            public string time;
            public string employeeNoString;
            public string cardNo;
            public string name;
        }

        public class EventSearchRoot
        {
            public EventSearchResult AcsEvent { get; set; }
        }
        public class EventSearchResult
        {
            public string searchID { get; set; }
            public string numOfMatches { get; set; }
            public string totalMatches { get; set; }
            public string responseStatusStrg { get; set; }
            public List<EventInfo> InfoList { get; set; }
        }
    }
}
