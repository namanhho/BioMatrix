using BioMetrixCore.DLL;
using BioMetrixCore.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.BLL
{
    public class RealTimeLogBll
    {
        RealTimeLogDal _dal = new RealTimeLogDal();
        /// <summary>
        /// 获取记录
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="userid"></param>
        /// <param name="devsn"></param>
        /// <returns></returns>
        public DataTable GetByTime(DateTime starttime, DateTime endtime, string userid, string devsn)
        {
            return _dal.GetByTime(starttime, endtime, userid, devsn);
        }


        public DataTable GetAccType()
        {
            return _dal.GetAccType();
        }
        public DataTable GetVerifyType()
        {
            return _dal.GetVerifyType();
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(RealTimeLogModel model)
        {
            return _dal.Add(model);
        }
        /// <summary>
        /// 新增多个
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(List<RealTimeLogModel> modelList)
        {
            return _dal.Add(modelList);
        }

        /// <summary>
        /// 清空
        /// </summary>
        /// <returns></returns>
        public int ClearAll(DateTime starttime, DateTime endtime, string userid, string devsn)
        {
            return _dal.ClearAll(starttime, endtime, userid, devsn);
        }
    }
}
