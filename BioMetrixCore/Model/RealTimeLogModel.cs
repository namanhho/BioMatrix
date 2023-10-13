using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Model
{
    public class RealTimeLogModel
    {
        public RealTimeLogModel()
        {

        }
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _devSN;
        public string DevSN
        {
            get { return _devSN; }
            set { _devSN = value; }
        }
        private DateTime _time;
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }
        private string _pin;
        public string Pin
        {
            get { return _pin; }
            set { _pin = value; }
        }
        private string _cardno;
        public string CardNo
        {
            get { return _cardno; }
            set { _cardno = value; }
        }
        private int _eventaddr;
        public int EventAddr
        {
            get { return _eventaddr; }
            set { _eventaddr = value; }
        }
        private int _event;
        public int Event
        {
            get { return _event; }
            set { _event = value; }
        }
        private int _inoutstatus;
        public int InOutStatus
        {
            get { return _inoutstatus; }
            set { _inoutstatus = value; }
        }
        private int _verifytype;
        public int VerifyType
        {
            get { return _verifytype; }
            set { _verifytype = value; }
        }
        private int _devIndex;
        public int DevIndex
        {
            get { return _devIndex; }
            set { _devIndex = value; }
        }
        /// <summary>
        /// 口罩
        /// </summary>
        private int _maskflag;
        public int MaskFlag
        {
            get { return _maskflag; }
            set { _maskflag = value; }
        }
        /// <summary>
        /// 体温
        /// </summary>
        private string _temperature;
        public string Temperature
        {
            get { return _temperature; }
            set { _temperature = value; }
        }
    }
}
