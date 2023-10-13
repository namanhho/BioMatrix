using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Model
{
    /// <summary>
    /// ÓÃ»§ÐÅÏ¢
    /// </summary>
    public class UserInfoModel
    {
        public UserInfoModel()
        {
            this.DevSN = "1";
            this.Grp = "1";
            this.IDCard = "0";
            this.Passwd = "0";
            this.Pri = "0";
            this.UserName = "";
            this.StartTime = "0";
            this.EndTime = "0";
        }
        /// <summary>
        /// ID
        /// </summary>		
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _devsn;
        public string DevSN
        {
            get { return _devsn; }
            set { _devsn = value; }
        }

        /// <summary>
        /// PIN
        /// </summary>		
        private string _pin;
        public string PIN
        {
            get { return _pin; }
            set { _pin = value; }
        }
        /// <summary>
        /// UserName
        /// </summary>		
        private string _username;
        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }
        /// <summary>
        /// Passwd
        /// </summary>		
        private string _passwd;
        public string Passwd
        {
            get { return _passwd; }
            set { _passwd = value; }
        }
        /// <summary>
        /// IDCard
        /// </summary>		
        private string _idcard;
        public string IDCard
        {
            get { return _idcard; }
            set { _idcard = value; }
        }
        /// <summary>
        /// Grp
        /// </summary>		
        private string _grp;
        public string Grp
        {
            get { return _grp; }
            set { _grp = value; }
        }
        /// <summary>
        /// StartTime
        /// </summary>		
        private string _startTime;
        public string StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }
        /// <summary>
        /// EndTime
        /// </summary>		
        private string _endTime;
        public string EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }
        /// <summary>
        /// Pri
        /// </summary>		
        private string _pri;
        public string Pri
        {
            get { return _pri; }
            set { _pri = value; }
        }
        /// <summary>
        /// Disable
        /// </summary>		
        private int _disable;
        public int Disable
        {
            get { return _disable; }
            set { _disable = value; }
        }
        /// <summary>
        /// Verify
        /// </summary>		
        private int _verify;
        public int Verify
        {
            get { return _verify; }
            set { _verify = value; }
        }
        /// <summary>
        /// AccessLevelID
        /// </summary>		
        private int _accessLevelID;
        public int AccessLevelID
        {
            get { return _accessLevelID; }
            set { _accessLevelID = value; }
        }
    }
}
