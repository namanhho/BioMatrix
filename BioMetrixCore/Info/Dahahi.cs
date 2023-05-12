using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore
{
    public class Dahahi
    {
        public class ResponseData
        {
            public string ErrorCode { get; set; }
            public string ErrorMessage { get; set; }
            public int Total { get; set; }
            public List<DahahiData> Data { get; set; }
        }
        public class DahahiData
        {
            public string EmployeeCode { get; set; }
            public string EmployeeName { get; set; }
            public string CheckinTimeStr { get; set; }
        }
        public class ResponseDataMachine
        {
            public string ErrorCode { get; set; }
            public string ErrorMessage { get; set; }
            public int Total { get; set; }
            public List<DahahiMachine> Data { get; set; }
        }
        public class DahahiMachine
        {
            public string MachineBoxId { get; set; }
        }
    }

}
