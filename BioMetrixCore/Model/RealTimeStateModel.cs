using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Model
{
    public class RealTimeStateModel
    {
        private string _deviceSN;
        public string DeviceSN
        {
            get { return _deviceSN; }
            set { _deviceSN = value; }
        }
        private string _time;
        public string Time
        {
            get { return _time; }
            set { _time = value; }
        }

        private string _sensor;
        public string Sensor
        {
            get { return _sensor; }
            set { _sensor = value; }
        }

        private string _relay;
        public string Relay
        {
            get { return _relay; }
            set { _relay = value; }
        }

        private string _alarm;
        public string Alarm
        {
            get { return _alarm; }
            set { _alarm = value; }
        }
    }
}
