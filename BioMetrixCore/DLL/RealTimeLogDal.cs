using BioMetrixCore.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.DLL
{
    public class RealTimeLogDal
    {
        /// <summary>
        /// 获取实施事件记录
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="userID"></param>
        /// <param name="devSN"></param>
        /// <returns></returns>
        public DataTable GetByTime(DateTime startTime, DateTime endTime, string userID, string devSN)
        {
            string wherePin = (string.IsNullOrEmpty(userID)) ? "" : " and r.PIN=@PIN";
            string whereDevSN = (string.IsNullOrEmpty(devSN)) ? "" : " and r.DevSN=@DevSN";

            string sql = string.Format(@"
select r.*,a.event as eventDescribe,v.type as verifyTypeDescribe from RealTimeLog r left join AccType a on r.event=a.eventcode 
 left join VerifyType v on r.verifytype=v.typecode where r.Time>@StartTime and r.Time<@EndTime
      {0}
      {1}
  order by r.Time desc
", wherePin, whereDevSN);

            SQLiteParameter[] parameters = {
                 new SQLiteParameter("@StartTime", startTime),
                 new SQLiteParameter("@EndTime", endTime),
                 new SQLiteParameter("@PIN", userID),
                 new SQLiteParameter("@DevSN", devSN)
            };
            return SqliteHelper.GetDataTable(sql, parameters);
        }

        public DataTable GetAccType()
        {
            string sql = "select * from AccType";

            return SqliteHelper.GetDataTable(sql);
        }
        public DataTable GetVerifyType()
        {
            string sql = "select * from VerifyType";

            return SqliteHelper.GetDataTable(sql);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(RealTimeLogModel model)
        {
            string sql = string.Format(@"
insert into RealTimeLog(Time,Pin,CardNo,EventAddr,Event,InOutStatus,VerifyType,DevIndex,DevSN,MaskFlag,Temperature)
 values(@Time,@Pin,@CardNo,@EventAddr,@Event,@InOutStatus,@VerifyType,@DevIndex,@DevSN,@MaskFlag,@Temperature);
");

            SQLiteParameter[] parameters = {
                        new SQLiteParameter("@Time", model.Time) ,
                        new SQLiteParameter("@Pin", model.Pin) ,
                        new SQLiteParameter("@CardNo",model.CardNo) ,
                        new SQLiteParameter("@EventAddr", model.EventAddr) ,
                        new SQLiteParameter("@Event", model.Event) ,
                        new SQLiteParameter("@InOutStatus", model.InOutStatus),
                        new SQLiteParameter("@VerifyType", model.VerifyType),
                        new SQLiteParameter("@DevIndex", model.DevIndex),
                        new SQLiteParameter("@DevSN", model.DevSN),
                        new SQLiteParameter("@MaskFlag", model.MaskFlag),
                        new SQLiteParameter("@Temperature", model.Temperature)
                    };
            return SqliteHelper.ExecuteNonQuery(sql.ToString(), parameters);
        }
        public int Add(List<RealTimeLogModel> modelList)
        {
            List<ManySql> transSqls = new List<ManySql>();
            string sql = string.Empty;

            foreach (var item in modelList)
            {
                sql = string.Format(@"
insert into RealTimeLog(Time,Pin,CardNo,EventAddr,Event,InOutStatus,VerifyType,DevIndex,DevSN,MaskFlag,Temperature)
 values(@Time,@Pin,@CardNo,@EventAddr,@Event,@InOutStatus,@VerifyType,@DevIndex,@DevSN,@MaskFlag,@Temperature);
");
                SQLiteParameter[] parameters = {
                        new SQLiteParameter("@Time", item.Time) ,
                        new SQLiteParameter("@Pin", item.Pin) ,
                        new SQLiteParameter("@CardNo", item.CardNo) ,
                        new SQLiteParameter("@EventAddr", item.EventAddr) ,
                        new SQLiteParameter("@Event", item.Event) ,
                        new SQLiteParameter("@InOutStatus", item.InOutStatus),
                        new SQLiteParameter("@VerifyType", item.VerifyType),
                        new SQLiteParameter("@DevIndex", item.DevIndex),
                        new SQLiteParameter("@DevSN", item.DevSN),
                        new SQLiteParameter("@MaskFlag", item.MaskFlag),
                        new SQLiteParameter("@Temperature", item.Temperature)
                    };
                transSqls.Add(new ManySql(sql, parameters));
            }

            return SqliteHelper.ExecuteManySql(transSqls);
        }

        /// <summary>
        /// 清空
        /// </summary>
        /// <returns></returns>
        public int ClearAll(DateTime starttime, DateTime endtime, string userid, string devsn)
        {
            string wherePin = (string.IsNullOrEmpty(userid)) ? "" : " and r.PIN=@PIN";
            string whereDevSN = (string.IsNullOrEmpty(devsn)) ? "" : " and r.DevSN=@DevSN";
            string sql = string.Format(@"delete from RealTimeLog where Time>@StartTime and Time<@EndTime
      {0}
      {1}", whereDevSN, wherePin);
            SQLiteParameter[] parameters = {
                 new SQLiteParameter("@StartTime", starttime),
                 new SQLiteParameter("@EndTime", endtime),
                 new SQLiteParameter("@PIN", userid),
                 new SQLiteParameter("@DevSN", devsn)
            };
            return SqliteHelper.ExecuteNonQuery(sql, parameters);
        }
    }
}
