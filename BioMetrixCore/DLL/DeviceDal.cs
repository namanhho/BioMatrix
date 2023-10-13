using BioMetrixCore.Model;
using BioMetrixCore.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.DLL
{
    /// <summary>
    /// 设备
    /// </summary>
    public class DeviceDal
    {
        /// <summary>GetAll
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <returns></returns>
        public DataTable GetAll(string sqlWhere)
        {
            sqlWhere = string.IsNullOrEmpty(sqlWhere) ? "" : "where " + sqlWhere;
            string sql = string.Format(@"
select * from Device {0};", sqlWhere);

            return SqliteHelper.GetDataTable(sql);
        }
        /// <summary>Get one model
        /// </summary>
        /// <param name="devSN"></param>
        /// <returns></returns>
        public DeviceModel Get(string devSN)
        {
            DataTable dt = GetAll(string.Format("DevSN='{0}'", devSN));
            if (dt == null || dt.Rows.Count == 0)
                return null;

            DeviceModel device = new DeviceModel();
            device.ID = Tools.TryConvertToInt32(dt.Rows[0]["ID"]);
            device.DeviceSN = dt.Rows[0]["DevSN"].ToString();
            device.RegistryCode = dt.Rows[0]["RegistryCode"].ToString();
            device.SessionID = dt.Rows[0]["SessionID"].ToString();
            device.TransTimes = dt.Rows[0]["TransTimes"].ToString();
            device.Encrypt = dt.Rows[0]["Encrypt"].ToString();
            device.LastRequestTime = Convert.ToDateTime(dt.Rows[0]["LastRequestTime"].ToString());
            device.IPAddress = dt.Rows[0]["DevIP"].ToString();
            device.DevMac = dt.Rows[0]["DevMac"].ToString();
            device.FirmVer = dt.Rows[0]["FirmVer"].ToString();
            device.UserCount = Tools.TryConvertToInt32(dt.Rows[0]["UserCount"].ToString());
            device.AccCount = Tools.TryConvertToInt32(dt.Rows[0]["AccCount"].ToString());
            device.DeviceName = dt.Rows[0]["DevName"].ToString();
            device.Timeout = Tools.TryConvertToInt32(dt.Rows[0]["Timeout"].ToString());
            device.SyncTime = Tools.TryConvertToInt32(dt.Rows[0]["SyncTime"].ToString());
            device.ErrorDelay = dt.Rows[0]["ErrorDelay"].ToString();
            device.Delay = dt.Rows[0]["Delay"].ToString();
            device.TransTables = dt.Rows[0]["TransTables"].ToString();
            device.Realtime = dt.Rows[0]["Realtime"].ToString();
            device.VendorName = dt.Rows[0]["VendorName"].ToString();
            device.DateFmtFunOn = Tools.TryConvertToInt32(dt.Rows[0]["DateFmtFunOn"].ToString());
            device.MachineType = dt.Rows[0]["MachineType"].ToString();
            device.AccSupportFunList = dt.Rows[0]["AccSupportFunList"].ToString();
            device.IRTempDetectionFunOn = dt.Rows[0]["IRTempDetectionFunOn"].ToString();
            device.MaskDetectionFunOn = dt.Rows[0]["MaskDetectionFunOn"].ToString();

            device.MultiBioDataSupport = dt.Rows[0]["MultiBioDataSupport"].ToString();
            device.MultiBioPhotoSupport = dt.Rows[0]["MultiBioPhotoSupport"].ToString();
            device.MultiBioVersion = dt.Rows[0]["MultiBioVersion"].ToString();
            device.MultiBioCount = dt.Rows[0]["MultiBioCount"].ToString();
            device.MaxMultiBioDataCount = dt.Rows[0]["MaxMultiBioDataCount"].ToString();
            device.MaxMultiBioPhotoCount = dt.Rows[0]["MaxMultiBioPhotoCount"].ToString();

            return device;
        }
        /// <summary>Add
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public int Add(DeviceModel device)
        {
            string sql = string.Format(@"
insert into Device(
        DevSN,RegistryCode,TransInterval,TransTimes,Encrypt,LastRequestTime,DevIP,DevMac,FirmVer
        ,UserCount,AccCount,DevName,Timeout,SyncTime,PushVersion,MachineType,AccSupportFunList
        ,ErrorDelay,Delay,TransTables,Realtime,VendorName,DateFmtFunOn,IRTempDetectionFunOn,MaskDetectionFunOn
   ) values (
        @DevSN,@RegistryCode,@TransInterval,@TransTimes,@Encrypt,@LastRequestTime,@DevIP,@DevMac,@FirmVer
        ,@UserCount,@AccCount,@DevName,@Timeout,@SyncTime,@PushVersion,@MachineType,@AccSupportFunList
        ,@ErrorDelay,@Delay,@TransTables,@Realtime,@VendorName,@DateFmtFunOn,@IRTempDetectionFunOn,@MaskDetectionFunOn
);");

            SQLiteParameter[] parameters = {
                        new SQLiteParameter("@DevSN", device.DeviceSN) ,
                        new SQLiteParameter("@RegistryCode", device.RegistryCode) ,
                        new SQLiteParameter("@TransInterval", device.TransInterval) ,
                        new SQLiteParameter("@TransTimes", device.TransTimes) ,
                        new SQLiteParameter("@Encrypt", device.Encrypt) ,
                        new SQLiteParameter("@LastRequestTime", device.LastRequestTime) ,
                        new SQLiteParameter("@DevIP", device.IPAddress) ,
                        new SQLiteParameter("@DevMac", device.DevMac),
                        new SQLiteParameter("@MachineType",device.MachineType),
                        new SQLiteParameter("@AccSupportFunList",device.AccSupportFunList),
                        new SQLiteParameter("@FirmVer", device.FirmVer) ,
                        new SQLiteParameter("@PushVersion", device.PushVersion) ,
                        new SQLiteParameter("@UserCount", device.UserCount) ,
                        new SQLiteParameter("@AccCount", device.AccCount) ,
                        new SQLiteParameter("@DevName", device.DeviceName) ,
                        new SQLiteParameter("@Timeout", device.Timeout) ,
                        new SQLiteParameter("@SyncTime", device.SyncTime) ,
                        new SQLiteParameter("@ErrorDelay", device.ErrorDelay) ,
                        new SQLiteParameter("@Delay",device.Delay) ,
                        new SQLiteParameter("@TransTables", device.TransTables) ,
                        new SQLiteParameter("@Realtime", device.Realtime) ,
                        new SQLiteParameter("@VendorName", device.VendorName),
                        new SQLiteParameter("@DateFmtFunOn", device.DateFmtFunOn),
                        new SQLiteParameter("@IRTempDetectionFunOn", device.IRTempDetectionFunOn),
                        new SQLiteParameter("@MaskDetectionFunOn", device.MaskDetectionFunOn)

            };
            return SqliteHelper.ExecuteNonQuery(sql, parameters);
        }
        /// <summary>Delete
        /// </summary>
        /// <param name="devSN"></param>
        /// <returns></returns>
        public int Delete(string devSN)
        {
            string sql = string.Format(@"
delete from Device where DevSN='{0}';", devSN);

            return SqliteHelper.ExecuteNonQuery(sql);
        }
        /// <summary>Update
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public int Update(DeviceModel device)
        {
            string sql = string.Format(@"
update Device set     
    TransInterval = @TransInterval , 
    TransTimes = @TransTimes , 
    Encrypt = @Encrypt , 
    LastRequestTime = @LastRequestTime , 
    DevIP = @DevIP , 
    DevMac = @DevMac , 
    FirmVer = @FirmVer , 
    PushVersion= @PushVersion , 
    UserCount = @UserCount , 
    AccCount = @AccCount , 
    DevName = @DevName , 
    Timeout = @Timeout , 
    SyncTime = @SyncTime , 
    ErrorDelay = @ErrorDelay , 
    Delay = @Delay , 
    TransTables = @TransTables , 
    Realtime = @Realtime,  
    VendorName = @VendorName,
    DateFmtFunOn=@DateFmtFunOn,
    MachineType=@MachineType,
    IRTempDetectionFunOn = @IRTempDetectionFunOn,  
    MaskDetectionFunOn = @MaskDetectionFunOn,  
    MultiBioDataSupport = @MultiBioDataSupport,  
    MultiBioPhotoSupport = @MultiBioPhotoSupport,  
    MultiBioVersion = @MultiBioVersion,  
    MultiBioCount = @MultiBioCount,  
    MaxMultiBioDataCount = @MaxMultiBioDataCount,  
    MaxMultiBioPhotoCount = @MaxMultiBioPhotoCount  
where DevSN=@DevSN 
");

            SQLiteParameter[] parameters = {
                        new SQLiteParameter("@DevSN",  device.DeviceSN) ,
                        new SQLiteParameter("@TransInterval", device.TransInterval) ,
                        new SQLiteParameter("@TransTimes", device.TransTimes) ,
                        new SQLiteParameter("@Encrypt", device.Encrypt) ,
                        new SQLiteParameter("@LastRequestTime", device.LastRequestTime) ,
                        new SQLiteParameter("@DevIP", device.IPAddress) ,
                        new SQLiteParameter("@DevMac",device.DevMac) ,
                        new SQLiteParameter("@FirmVer", device.FirmVer) ,
                        new SQLiteParameter("@PushVersion", device.PushVersion) ,
                        new SQLiteParameter("@UserCount",device.UserCount) ,
                        new SQLiteParameter("@AccCount", device.AccCount) ,
                        new SQLiteParameter("@DevName", device.DeviceName) ,
                        new SQLiteParameter("@Timeout", device.Timeout) ,
                        new SQLiteParameter("@SyncTime", device.SyncTime) ,
                        new SQLiteParameter("@ErrorDelay",device.ErrorDelay) ,
                        new SQLiteParameter("@Delay", device.Delay) ,
                        new SQLiteParameter("@TransTables", device.TransTables) ,
                        new SQLiteParameter("@Realtime", device.Realtime),
                        new SQLiteParameter("@VendorName", device.VendorName),
                        new SQLiteParameter("@MachineType", device.MachineType),
                        new SQLiteParameter("@DateFmtFunOn", device.DateFmtFunOn),
                        new SQLiteParameter("@IRTempDetectionFunOn", device.IRTempDetectionFunOn),
                        new SQLiteParameter("@MaskDetectionFunOn", device.MaskDetectionFunOn),
                        new SQLiteParameter("@MultiBioDataSupport", device.MultiBioDataSupport),
                        new SQLiteParameter("@MultiBioPhotoSupport", device.MultiBioPhotoSupport),
                        new SQLiteParameter("@MultiBioVersion", device.MultiBioVersion),
                        new SQLiteParameter("@MultiBioCount", device.MultiBioCount),
                        new SQLiteParameter("@MaxMultiBioDataCount", device.MaxMultiBioDataCount),
                        new SQLiteParameter("@MaxMultiBioPhotoCount", device.MaxMultiBioPhotoCount)
            };
            return SqliteHelper.ExecuteNonQuery(sql, parameters);
        }
        /// <summary>
        /// 更新SessionID
        /// </summary>
        /// <param name="devSN"></param>
        public int UpdateSession(string sessinoID, string devSN)
        {
            string sql = string.Format(@"
update Device set 
      SessionID = @SessionID 
 where DevSN=@DevSN;
");

            SQLiteParameter[] parameters = {
                new SQLiteParameter("@SessionID", sessinoID),
                new SQLiteParameter("@DevSN",  devSN)
            };
            return SqliteHelper.ExecuteNonQuery(sql, parameters);
        }
        /// <summary>
        /// 考勤日志时间戳清理
        /// </summary>
        /// <param name="listDevSn"></param>
        public void SetZeroAttLogStamp(List<string> listDevSn)
        {
            string sql = string.Format(@"
update Device set    
    ATTLOGStamp = '0', 
 where DevSN in ({0})
", Tools.UnionString(listDevSn));

            SqliteHelper.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 更新操作日志时间戳
        /// </summary>
        /// <param name="stamp"></param>
        /// <param name="devSN"></param>
        public void UpdateOperLogStamp(string stamp, string devSN)
        {
            string sql = string.Format(@"
update Device set 
      OPERLOGStamp = @OPERLOGStamp 
 where DevSN=@DevSN;
");

            SQLiteParameter[] parameters = {
                new SQLiteParameter("@OPERLOGStamp",stamp ),
                new SQLiteParameter("@DevSN",  devSN)
            };
            SqliteHelper.ExecuteNonQuery(sql, parameters);
        }
        /// <summary>
        /// 更新错误日志时间戳
        /// </summary>
        /// <param name="stamp"></param>
        /// <param name="devSN"></param>
        public void UpdateErrorLogStamp(string stamp, string devSN)
        {
            string sql = string.Format(@"
update Device set 
      ERRORLOGStamp = @ERRORLOGStamp 
 where DevSN=@DevSN;
");

            SQLiteParameter[] parameters = {
                new SQLiteParameter("@ERRORLOGStamp",stamp ),
                new SQLiteParameter("@DevSN",  devSN)
            };
            SqliteHelper.ExecuteNonQuery(sql, parameters);
        }
        /// <summary>
        /// 更新考勤图像时间戳
        /// </summary>
        /// <param name="stamp"></param>
        /// <param name="devSN"></param>
        public void UpdateAttPhotoStamp(string stamp, string devSN)
        {
            string sql = string.Format(@"
update Device set 
      ATTPHOTOStamp = @ATTPHOTOStamp 
 where DevSN=@DevSN;
");

            SQLiteParameter[] parameters = {
                new SQLiteParameter("@ATTPHOTOStamp",stamp ),
                new SQLiteParameter("@DevSN",  devSN)
            };
            SqliteHelper.ExecuteNonQuery(sql, parameters);
        }
        /// <summary>
        /// 清零时间戳
        /// </summary>
        /// <param name="listDevSn"></param>
        public void SetZeroStamp(List<string> listDevSn)
        {
            string sql = string.Format(@"
update Device set 
    OPERLOGStamp = '0', 
    ATTLOGStamp = '0', 
    ATTPHOTOStamp = '0' 
 where DevSN in ({0})
", Tools.UnionString(listDevSn));

            SqliteHelper.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 更新最后请求时间
        /// </summary>
        /// <param name="DevSN"></param>
        public void SetLastRequestTime(string DevSN)
        {
            string sql = string.Format(@"
update Device set 
      LastRequestTime = @LastRequestTime 
 where DevSN=@DevSN;
");

            SQLiteParameter[] parameters = {
                new SQLiteParameter("@LastRequestTime",Tools.GetDateTimeNowString() ),
                new SQLiteParameter("@DevSN",  DevSN)
            };
            SqliteHelper.ExecuteNonQuery(sql, parameters);
        }
        /// <summary>
        /// 更新供应商名称
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="vendorName"></param>
        public void UpdateVendorName(string sn, string vendorName)
        {
            string sql = string.Format(@"
update Device set 
      VendorName = @VendorName 
 where DevSN=@DevSN;
");

            SQLiteParameter[] parameters = {
                new SQLiteParameter("@VendorName",vendorName ),
                new SQLiteParameter("@DevSN",  sn)
            };
            SqliteHelper.ExecuteNonQuery(sql, parameters);
        }
        /// <summary>
        /// 获取所有的DevSN
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllDevSN()
        {
            string sql = string.Format(@"select DevSN from Device;");

            List<string> listDevSN = new List<string>();
            DataTable dt = SqliteHelper.GetDataTable(sql);
            if (dt == null)
                return listDevSN;

            foreach (DataRow row in dt.Rows)
            {
                listDevSN.Add(row[0].ToString());
            }

            return listDevSN;
        }
    }
}
