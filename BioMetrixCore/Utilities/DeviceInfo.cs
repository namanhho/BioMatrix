using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Utilities
{
    public class DeviceInfo
    {
        #region Property
        //public DeviceType DeviceType { get; set; }
        public string DeviceName { get; set; }
        //public ConnectType ConnectType { get; set; }
        public string SerialNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string APIUrl { get; set; }
        #region Hanet
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string DeviceID { get; set; }
        public string PlaceID { get; set; }
        #endregion
        // oracle connection
        public string HostName { get; set; }
        public string ServiceName { get; set; }
        #region TCP
        public string IPAddress { get; set; }
        public int Port { get; set; } = 4370;
        #endregion

        #region COM
        public int? COMKey { get; set; } = 0;
        public int? Baudrate { get; set; } = 115200;
        public string SerialPort { get; set; }
        public int MachineNumber { get; set; } = 1;
        #endregion

        #region USB
        public int? USBDeviceID { get; set; }
        #endregion
        #endregion
    }
}
