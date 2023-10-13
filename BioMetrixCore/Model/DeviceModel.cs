using BioMetrixCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Model
{
    /// <summary>
    /// 设备
    /// </summary>
    public class DeviceModel
    {
        public DeviceModel()
        {
            this.DeviceName = "xFace600";
            this.Delay = "30";
            this.ErrorDelay = "60";
            this.FirmVer = "";
            this.IPAddress = "192.168.1.201";
            this.Encrypt = "0";
            this.Realtime = "1";
            this.SyncTime = 0;
            this.Timeout = 120;
            this.TransInterval = 2;
            this.TransTimes = "00:00\t14:30";
            this.UserCount = 10000;
            this.VendorName = "ZK";
            this.LastRequestTime = Convert.ToDateTime("1900-01-01 00:00:00");
            this.IRTempDetectionFunOn = "0";
            this.MaskDetectionFunOn = "0";
            this.MultiBioDataSupport = "1:1:1:1:1:1:1:1:1:1";
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
        /// <summary>
        /// DevSN
        /// </summary>		
        private string _deviceSN;
        public string DeviceSN
        {
            get { return _deviceSN; }
            set { _deviceSN = value; }
        }
        /// <summary>
        /// DevName
        /// </summary>		
        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; }
        }

        /// <summary>
        /// RegisterCode 服务器生成随机数， 最长32个字节
        /// </summary>
        private string _registryCode;
        public string RegistryCode
        {
            get { return _registryCode; }
            set { _registryCode = value; }
        }

        /// <summary>
        /// PUSH 通信会话 ID, 今后设备的请求都需要此字段来计算新的 Token
        /// </summary>
        public string _sessionID = "";
        public string SessionID
        {
            get { return _sessionID; }
            set { _sessionID = value; }
        }

        /// <summary>
        /// 设备类型：考勤、安防
        /// </summary>		
        private string _deviceType;
        public string DeviceType
        {
            get { return _deviceType; }
            set { _deviceType = value; }
        }
        /// <summary>
        /// 设备类型：控制器、一体机
        /// </summary>		
        private string _machineType;
        public string MachineType
        {
            get { return _machineType; }
            set { _machineType = value; }
        }
        /// <summary>
        /// 支持参数
        /// </summary>		
        private string _accSupportFunList;
        public string AccSupportFunList
        {
            get { return _accSupportFunList; }
            set { _accSupportFunList = value; }
        }

        /// <summary>
        /// 固件版本号
        /// </summary>		
        private string _firmVer;
        public string FirmVer
        {
            get { return _firmVer; }
            set { _firmVer = value; }
        }
        /// <summary>
        /// 设备mac地址
        /// </summary>		
        private string _devMac;
        public string DevMac
        {
            get { return _devMac; }
            set { _devMac = value; }
        }
        /// <summary>
        /// DevIP
        /// </summary>		
        private string _pushVersion;
        public string PushVersion
        {
            get { return _pushVersion; }
            set { _pushVersion = value; }
        }



        /// <summary>
        /// 通讯类型
        /// </summary>		
        private string _commType;
        public string CommType
        {
            get { return _commType; }
            set { _commType = value; }
        }

        /// <summary>
        /// 通讯包最大尺寸
        /// </summary>		
        private string _maxPackageSize;
        public string MaxPackageSize
        {
            get { return _maxPackageSize; }
            set { _maxPackageSize = value; }
        }
        /// <summary>
        /// 设备IP
        /// </summary>		
        private string _iPAddress;
        public string IPAddress
        {
            get { return _iPAddress; }
            set { _iPAddress = value; }
        }
        /// <summary>
        /// 支持的验证方式
        /// </summary>		
        private string _verifyStyles;
        public string VerifyStyles
        {
            get { return _verifyStyles; }
            set { _verifyStyles = value; }
        }

        /// <summary>
        /// 支持的验证方式
        /// </summary>		
        private string _eventTypes;
        public string EventTypes
        {
            get { return _eventTypes; }
            set { _eventTypes = value; }
        }



        /// <summary>
        ///  联网失败后客户端重新联接服务器的间隔时间（秒） ， 建议设置30~300秒， 如果不配置客户端默认值为30秒
        /// </summary>
        public string _errorDelay;
        public string ErrorDelay
        {
            get { return _errorDelay; }
            set { _errorDelay = value; }
        }

        /// <summary>
        ///  客户端获取命令请求间隔时间（秒） ， 如果不配置客户端默认值为30秒
        /// </summary>
        public int _requestDelay = 30;
        public int RequestDelay
        {
            get
            {
                if (MachineType == "10" || MachineType == "9" || MachineType == "8")
                    _requestDelay = 5;
                return _requestDelay;
            }
            set { _requestDelay = value; }
        }

        /// <summary>
        ///  定时检查传送新数据的时间点， 如果不配置默认为"12:30;14:30"
        /// </summary>
        public string _transTimes = "00:00\t14:30";//TODO "12:30\t14:30"
        public string TransTimes
        {
            get { return _transTimes; }
            set { _transTimes = value; }
        }

        /// <summary>
        ///  检查是否有新数据需要传送的间隔时间（分钟） ,如果不配置客户端默认为2分钟
        /// </summary>
        public int _transInterval = 2;
        public int TransInterval
        {
            get { return _transInterval; }
            set { _transInterval = value; }
        }

        /// <summary>
        /// 需要检查新数据并上传的数据， 默认为"User Transaction",需要自动上传用户和门禁记录
        /// </summary>
        public string _transTables = "User\tTransaction";//TODO "User\tTransaction"
        public string TransTables
        {
            get { return _transTables; }
            set { _transTables = value; }
        }

        /// <summary>
        /// 客户端是否实时传送新记录。 为1表示有新数据就传送到服务器， 为0表示按照 TransTimes 和 TransInterval规定的时间传送,如果不配置客户端默认值为1
        /// </summary>
        public string _realtime = "1";
        public string Realtime
        {
            get { return _realtime; }
            set { _realtime = value; }
        }




        /// <summary>
        ///设置网络超时时间， 如果不配置客户端默认 10秒
        /// </summary>
        public int _timeoutSec = 10;
        public int TimeoutSec
        {
            get { return _timeoutSec; }
            set { _timeoutSec = value; }
        }



        /// <summary>
        /// Stamp
        /// </summary>		
        private string _stamp;
        public string Stamp
        {
            get { return _stamp; }
            set { _stamp = value; }
        }



        /// <summary>
        /// Delay
        /// </summary>		
        private string _delay;
        public string Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        ///// <summary>
        ///// Realtime
        ///// </summary>		
        //private string _realtime;
        //public string Realtime
        //{
        //    get { return _realtime; }
        //    set { _realtime = value; }
        //}
        ///// <summary>
        ///// TransInterval
        ///// </summary>		
        //private string _transinterval;
        //public string TransInterval
        //{
        //    get { return _transinterval; }
        //    set { _transinterval = value; }
        //}
        ///// <summary>
        ///// TransTimes
        ///// </summary>		
        //private string _transtimes;
        //public string TransTimes
        //{
        //    get { return _transtimes; }
        //    set { _transtimes = value; }
        //}
        /// <summary>
        /// Encrypt
        /// </summary>		
        private string _encrypt;
        public string Encrypt
        {
            get { return _encrypt; }
            set { _encrypt = value; }
        }
        /// <summary>
        /// LastRequestTime
        /// </summary>		
        private DateTime _lastrequesttime;
        public DateTime LastRequestTime
        {
            get { return _lastrequesttime; }
            set { _lastrequesttime = value; }
        }

        /// <summary>
        /// UserCount
        /// </summary>		
        private int _usercount;
        public int UserCount
        {
            get { return _usercount; }
            set { _usercount = value; }
        }
        /// <summary>
        /// DateFmtFunOn
        /// </summary>		
        private int _datefmtfunon;
        public int DateFmtFunOn
        {
            get { return _datefmtfunon; }
            set { _datefmtfunon = value; }
        }
        /// <summary>
        /// AccCount
        /// </summary>		
        private int _acccount;
        public int AccCount
        {
            get { return _acccount; }
            set { _acccount = value; }
        }

        /// <summary>
        /// Timeout
        /// </summary>		
        private int _timeout;
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        /// <summary>
        /// SyncTime
        /// </summary>		
        private int _synctime;
        public int SyncTime
        {
            get { return _synctime; }
            set { _synctime = value; }
        }

        /// <summary>
        /// Vendor Name
        /// </summary>		
        private string _vendorName;
        public string VendorName
        {
            get { return _vendorName; }
            set { _vendorName = value; }
        }
        /// <summary>
        /// MaskDetectionFunOn
        /// </summary>
        private string _iRTempDetectionFunOn;
        public string IRTempDetectionFunOn
        {
            get { return _iRTempDetectionFunOn; }
            set { _iRTempDetectionFunOn = value; }
        }
        /// <summary>
        /// MaskDetectionFunOn
        /// </summary>
        private string _maskDetectionFunOn;
        public string MaskDetectionFunOn
        {
            get { return _maskDetectionFunOn; }
            set { _maskDetectionFunOn = value; }
        }

        /// <summary>
        /// 支持URL方式下发用户照片
        /// </summary>
        private string _userPicURLFunOn;
        public string UserPicURLFunOn
        {
            get { return _userPicURLFunOn; }
            set { _userPicURLFunOn = value; }
        }

        /// <summary>
        /// MultiBioDataSupport
        /// 支持多模态生物特征图片参数， type类型进行按位定义， 不同类型间用:冒号隔开，
        /// 0:不支持， 1表示支持： 支持版本号， 如： 0:1:1:0:0:0:0:0:0:0,表示支持指纹图片支持和近红外人
        ///脸图片支持
        /// 0 通用的 ,1 指纹,2 面部,3 声纹,4 虹膜,5 视网膜,6 掌纹,7 指静脉,8 手掌,9 可见光面部,
        /// </summary>
        private string _multiBioDataSupport;
        public string MultiBioDataSupport
        {
            get { return _multiBioDataSupport; }
            set { _multiBioDataSupport = value; }
        }
        /// <summary>
        /// 支持多模态生物特征图片参数， type类型进行按位定义， 不同类型间用:冒号隔开，
        /// 0:不支持， 1表示支持：如： 0:1:1:0:0:0:0:0:0:0,表示支持指纹图片支持和近红外人脸图片支持
        /// </summary>
        private string _multiBioPhotoSupport;
        public string MultiBioPhotoSupport
        {
            get { return _multiBioPhotoSupport; }
            set { _multiBioPhotoSupport = value; }
        }
        /// <summary>
        /// 多模态生物特征数据版本， 不同类型间用:冒号隔开， 0:不支持， 1表示支持： 
        /// 如： 0:10.0:7.0:0:0:0:0:0:0:0,表示支持指纹算法10.0和近红外人脸算法7.0
        /// </summary>
        private string _multiBioVersion;
        public string MultiBioVersion
        {
            get { return _multiBioVersion; }
            set { _multiBioVersion = value; }
        }
        /// <summary>
        /// 支持多模态生物特征数据版本参数， type类型进行按位定义， 不同类型间用:冒号隔开，
        /// 0:不支持， 1表示支持： 支持版本号， 如： 0:100:200:0:0:0:0:0:0:0,表示支持指纹数量100和近红外人脸数量200
        /// </summary>
        private string _multiBioCount;
        public string MultiBioCount
        {
            get { return _multiBioCount; }
            set { _multiBioCount = value; }
        }
        /// <summary>
        /// 支持多模态生物特征模板最大数量， type类型进行按位定义， 不同类型间用: 冒号隔开，
        /// 0:不支持， 1表示支持： 支持最大模板数量， 如： 0:10000:2000:0:0:0:0:0:0:0,表示支持指纹模板最大数量10000和近红外人脸模板最大数量2000
        /// </summary>
        private string _maxMultiBioDataCount;
        public string MaxMultiBioDataCount
        {
            get { return _maxMultiBioDataCount; }
            set { _maxMultiBioDataCount = value; }
        }
        /// <summary>
        /// 支持多模态生物特征照片最大数量， type类型进行按位定义， 不同类型间用:冒号隔开，
        /// 0:不支持， 1表示支持： 支持最大照片数量， 如： 0:10000:2000:0:0:0:0:0:0:0,表示支持指纹照片最大数量10000和近红外人脸照片最大数量2000
        /// </summary>
        private string _maxMultiBioPhotoCount;
        public string MaxMultiBioPhotoCount
        {
            get { return _maxMultiBioPhotoCount; }
            set { _maxMultiBioPhotoCount = value; }
        }

        public override string ToString()
        {
            return _deviceSN;
        }

        #region BioData Function
        /// <summary>
        /// 是否支持某多模态生物
        /// </summary>
        /// <param name="BioType">多模态生物类型</param>
        /// <returns></returns>
        public bool IsBioDataSupport(BioType BioType)
        {
            if (string.IsNullOrEmpty(MultiBioDataSupport))
                return false;

            string[] arr = MultiBioDataSupport.Split(':');
            if (arr.Length != 10 || (int)BioType >= arr.Length)
                return false;

            if (arr[(int)BioType] == "0" || arr[(int)BioType] == "")
                return false;

            return true;
        }
        /// <summary>
        /// 是否支持某多模态生物图片
        /// </summary>
        /// <param name="BioType">多模态生物类型</param>
        /// <returns></returns>
        public bool IsBioPhotoSupport(BioType BioType)
        {
            if (string.IsNullOrEmpty(MultiBioPhotoSupport))
                return false;

            string[] arr = MultiBioDataSupport.Split(':');
            if (arr.Length != 10 || (int)BioType >= arr.Length)
                return false;

            if (arr[(int)BioType] == "0" || arr[(int)BioType] == "")
                return false;

            return true;
        }
        /// <summary>
        /// 获取支持某多模态生物版本
        /// </summary>
        /// <param name="BioType">多模态生物类型</param>
        /// <returns></returns>
        public string GetBioVersion(BioType BioType)
        {
            string version = "";
            if (string.IsNullOrEmpty(MultiBioVersion))
                return version;

            string[] arr = MultiBioVersion.Split(':');
            if (arr.Length != 10 || (int)BioType >= arr.Length)
                return version;

            version = arr[(int)BioType];

            return version;
        }

        /// <summary>
        /// 获取某多模态生物数量
        /// </summary>
        /// <param name="BioType">多模态生物类型</param>
        /// <returns></returns>
        public int GetBioDataCount(BioType BioType)
        {
            int count = 0;
            if (string.IsNullOrEmpty(MultiBioCount))
                return count;

            string[] arr = MultiBioCount.Split(':');
            if (arr.Length != 10 || (int)BioType >= arr.Length)
                return count;

            count = Tools.TryConvertToInt32(arr[(int)BioType]);

            return count;
        }

        /// <summary>
        /// 获取某多模态生物图片数量
        /// </summary>
        /// <param name="BioType">多模态生物类型</param>
        /// <returns></returns>
        public int GetBioPhotoCount(BioType BioType)
        {
            int count = 0;
            if (string.IsNullOrEmpty(MaxMultiBioPhotoCount))
                return count;

            string[] arr = MaxMultiBioPhotoCount.Split(':');
            if (arr.Length != 10 || (int)BioType >= arr.Length)
                return count;

            count = Tools.TryConvertToInt32(arr[(int)BioType]);

            return count;
        }


        /// <summary>
        /// 获取某多模态生物最大数量
        /// </summary>
        /// <param name="BioType">多模态生物类型</param>
        /// <returns></returns>
        public int GetMaxBioDataCount(BioType BioType)
        {
            int count = 0;
            if (string.IsNullOrEmpty(MaxMultiBioDataCount))
                return count;

            string[] arr = MaxMultiBioDataCount.Split(':');
            if (arr.Length != 10 || (int)BioType >= arr.Length)
                return count;

            count = Tools.TryConvertToInt32(arr[(int)BioType]);

            return count;
        }
        /// <summary>
        /// 获取某多模态生物最大数量
        /// </summary>
        /// <param name="BioType">多模态生物类型</param>
        /// <returns></returns>
        public int GetMaxBioPhotoCount(BioType BioType)
        {
            int count = 0;
            if (string.IsNullOrEmpty(MaxMultiBioPhotoCount))
                return count;

            string[] arr = MaxMultiBioPhotoCount.Split(':');
            if (arr.Length != 10 || (int)BioType >= arr.Length)
                return count;

            count = Tools.TryConvertToInt32(arr[(int)BioType]);

            return count;
        }
        #endregion End-BioData Function

    }

    /// <summary>
    /// 多模态生物类型
    /// </summary>
    public enum BioType
    {
        /// <summary>
        /// 0 通用的
        /// </summary>
        Comm = 0,
        /// <summary>
        /// 1 指纹
        /// </summary>
        FingerPrint = 1,
        /// <summary>
        /// 2 面部
        /// </summary>
        Face = 2,
        /// <summary>
        /// 3 声纹
        /// </summary>
        VocalPrint = 3,
        /// <summary>
        /// 4 虹膜
        /// </summary>
        Iris = 4,
        /// <summary>
        /// 5 视网膜
        /// </summary>
        Retina = 5,
        /// <summary>
        /// 6 掌纹
        /// </summary>
        PalmPrint = 6,
        /// <summary>
        /// 7 指静脉
        /// </summary>
        FingerVein = 7,
        /// <summary>
        /// 8 手掌
        /// </summary>
        Palm = 8,
        /// <summary>
        /// 9 可见光面部 
        /// </summary>
        VisilightFace = 9
    }
}
