///
///    Experimented By : Ozesh Thapa
///    Email: dablackscarlet@gmail.com
///
using BioMetrixCore.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace BioMetrixCore
{
    internal class DeviceManipulator
    {

        public ICollection<UserInfo> GetAllUserInfo(ZkemClient objZkeeper, int machineNumber)
        {
            string sdwEnrollNumber = string.Empty, sName = string.Empty, sPassword = string.Empty, sTmpData = string.Empty;
            int iPrivilege = 0, iTmpLength = 0, iFlag = 0, idwFingerIndex;
            bool bEnabled = false;

            ICollection<UserInfo> lstFPTemplates = new List<UserInfo>();

            objZkeeper.ReadAllUserID(machineNumber);
            objZkeeper.ReadAllTemplate(machineNumber);

            while (objZkeeper.SSR_GetAllUserInfo(machineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
            {
                for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                {
                    if (objZkeeper.GetUserTmpExStr(machineNumber, sdwEnrollNumber, idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))
                    {
                        UserInfo fpInfo = new UserInfo();
                        fpInfo.MachineNumber = machineNumber;
                        fpInfo.EnrollNumber = sdwEnrollNumber;
                        fpInfo.Name = sName;
                        fpInfo.FingerIndex = idwFingerIndex;
                        fpInfo.TmpData = sTmpData;
                        fpInfo.Privelage = iPrivilege;
                        fpInfo.Password = sPassword;
                        fpInfo.Enabled = bEnabled;
                        fpInfo.iFlag = iFlag.ToString();

                        lstFPTemplates.Add(fpInfo);
                    }
                }

            }
            return lstFPTemplates;
        }

        public ICollection<MachineInfo> GetLogData(ZkemClient objZkeeper, int machineNumber, ref string message, DateTime fromDate, DateTime toDate, int readType = 1, int readLog = 1)
        {
            ICollection<MachineInfo> lstEnrollData = new List<MachineInfo>();
            var fromTime = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
            var toTime = toDate.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                string dwEnrollNumber1 = "";
                int dwVerifyMode = 0;
                int dwInOutMode = 0;
                int dwYear = 0;
                int dwMonth = 0;
                int dwDay = 0;
                int dwHour = 0;
                int dwMinute = 0;
                int dwSecond = 0;
                int dwWorkCode = 0;

                int BYear = 0, BMonth = 0, BDay = 0, BHour = 0, BMinute = 0, EYear = 0, EMonth = 0, EDay = 0, EHour = 0, EMinute = 0;

                //var a = Utility.GetAppSetting("ReadType");
                //int readType = 1;
                //int.TryParse(a, out readType);
                message += $"\nBat dau ReadType";
                Logger.LogInfo($"\nBat dau ReadType");
                var success = false;
                switch (readType)
                {
                    case 1:
                        success = objZkeeper.ReadAllGLogData(machineNumber);
                        break;
                    case 2:
                        success = objZkeeper.ReadGeneralLogData(machineNumber);
                        break;
                    case 3:
                        success = objZkeeper.ReadSuperLogData(machineNumber);
                        break;
                    case 4:
                        success = objZkeeper.ReadAllSLogData(machineNumber);
                        break;
                    case 5:
                        success = objZkeeper.ReadNewGLogData(machineNumber);
                        break;
                    case 6:
                        success = objZkeeper.ReadAllBellSchData(machineNumber);
                        break;
                    case 7:
                        success = objZkeeper.ReadTimeGLogData(machineNumber, fromTime, toTime);
                        break;
                    default:
                        break;
                };

                message += $"\nReadType_{readType} xong: success_{success}_ machineNumber: {machineNumber}===============";
                Logger.LogInfo($"\nReadType_{readType} xong: success_{success}_ machineNumber: {machineNumber}===============");


                int number = 0;
                string str = string.Empty;
                int dwEnrollNumber = 0;

                //a = Utility.GetAppSetting("type");
                //int type = 1;
                //int.TryParse(a, out type);
                Logger.LogInfo($"\nBat dau ReadLog");
                switch (readLog)
                {
                    case 0:
                        Logger.LogInfo($"\n Bat dau ReadLog 1");
                        message += $"\n ReadLog1======";
                        while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog1 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            int dwEnroll = 0;
                            int.TryParse(dwEnrollNumber1, out dwEnroll);
                            objInfo.IndRegID = dwEnroll;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog1 xong - Count: {lstEnrollData.Count}===========";
                        Logger.LogInfo($"\n ReadLog1 xong - Count: {lstEnrollData.Count}===========");

                        Logger.LogInfo($"\n Bat dau ReadLog 2");
                        message += $"\n ReadLog2==========";
                        while (objZkeeper.GetGeneralLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref dwVerifyMode, ref dwInOutMode, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog2 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog2 xong - Count: {lstEnrollData.Count}===========";
                        Logger.LogInfo($"\n ReadLog2 xong - Count: {lstEnrollData.Count}===========");

                        Logger.LogInfo($"\n Bat dau ReadLog 3");
                        message += $"\n ReadLog3======";
                        while (objZkeeper.GetSuperLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog3 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog3 xong - Count: {lstEnrollData.Count}===========";
                        Logger.LogInfo($"\n ReadLog3 xong - Count: {lstEnrollData.Count}===========");

                        Logger.LogInfo($"\n Bat dau ReadLog 4");
                        message += $"\n ReadLog4==========";
                        while (objZkeeper.GetAllSLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog4 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog4 xong - Count: {lstEnrollData.Count}==============";
                        Logger.LogInfo($"\n ReadLog4 xong - Count: {lstEnrollData.Count}===========");

                        Logger.LogInfo($"\n Bat dau ReadLog 5");
                        message += $"\n ReadLog5==========";
                        while (objZkeeper.GetAllGLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref dwVerifyMode, ref dwInOutMode, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog5 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog5 xong - Count: {lstEnrollData.Count}===============";
                        Logger.LogInfo($"\n ReadLog5 xong - Count: {lstEnrollData.Count}===========");

                        Logger.LogInfo($"\n Bat dau ReadLog 6");
                        message += $"\n ReadLog6==========";
                        while (objZkeeper.GetGeneralExtLogData(machineNumber, ref dwEnrollNumber, ref dwVerifyMode, ref dwInOutMode, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond, ref dwWorkCode, ref number))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog6 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog6 xong - Count: {lstEnrollData.Count}============";
                        Logger.LogInfo($"\n ReadLog6 xong - Count: {lstEnrollData.Count}===========");

                        Logger.LogInfo($"\n Bat dau ReadLog 7");
                        message += $"\n ReadLog7=========";
                        //while (objZkeeper.SSR_OutPutHTMLRep(machineNumber, dwEnrollNumber1, str, str, str, str, str, BYear, BMonth, BDay, BHour, BMinute, number, EYear, EMonth, EDay, EHour, EMinute, number, str, str, number, number, str))
                        //{
                        //    //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        //    string inputDate = $"ReadLog7 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        //    MachineInfo objInfo = new MachineInfo();
                        //    objInfo.MachineNumber = machineNumber;
                        //    int dwEnroll = 0;
                        //    int.TryParse(dwEnrollNumber1, out dwEnroll);
                        //    objInfo.IndRegID = dwEnroll;
                        //    objInfo.DateTimeRecord = inputDate;

                        //    lstEnrollData.Add(objInfo);
                        //}
                        message += $"\n ReadLog7 xong - Count: {lstEnrollData.Count}==========";
                        Logger.LogInfo($"\n ReadLog7 xong - Count: {lstEnrollData.Count}===========");

                        Logger.LogInfo($"\n Bat dau ReadLog 8");
                        message += $"\n ReadLog8 ================";
                        while (objZkeeper.GetSuperLogData2(machineNumber, ref number, ref dwEnrollNumber, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref number))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog8 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog8 xong - Count: {lstEnrollData.Count}================";
                        Logger.LogInfo($"\n ReadLog8 xong - Count: {lstEnrollData.Count}===========");


                        Logger.LogInfo($"\n Bat dau ReadLog 9");
                        message += $"\n ReadLog9========";
                        while (objZkeeper.ReadLastestLogData(machineNumber, number, dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog9 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog9 xong - Count: {lstEnrollData.Count}=============";
                        Logger.LogInfo($"\n ReadLog9 xong - Count: {lstEnrollData.Count}===========");


                        Logger.LogInfo($"\n Bat dau ReadLog 10");
                        message += $"\n ReadLog10========";
                        while (objZkeeper.ReadSuperLogDataEx(machineNumber, BYear, BMonth, BDay, BHour, BMinute, number, EYear, EMonth, EDay, EHour, EMinute, number, number))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog10 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog10 xong - Count: {lstEnrollData.Count}=========";
                        Logger.LogInfo($"\n ReadLog10 xong - Count: {lstEnrollData.Count}===========");


                        Logger.LogInfo($"\n Bat dau ReadLog 11");
                        message += $"\n ReadLog11=========";
                        while (objZkeeper.GetSuperLogDataEx(machineNumber, ref dwEnrollNumber1, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog11 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            int dwEnroll = 0;
                            int.TryParse(dwEnrollNumber1, out dwEnroll);
                            objInfo.IndRegID = dwEnroll;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog11 xong - Count: {lstEnrollData.Count}==============";
                        Logger.LogInfo($"\n ReadLog11 xong - Count: {lstEnrollData.Count}===========");


                        Logger.LogInfo($"\n Bat dau ReadLog 12");
                        message += $"\n ReadLog12==============";
                        while (objZkeeper.SSR_GetGeneralLogDataEx(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, out str))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog12 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            int dwEnroll = 0;
                            int.TryParse(dwEnrollNumber1, out dwEnroll);
                            objInfo.IndRegID = dwEnroll;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog12 xong - Count: {lstEnrollData.Count}================";
                        Logger.LogInfo($"\n ReadLog12 xong - Count: {lstEnrollData.Count}===========");


                        Logger.LogInfo($"\n Bat dau ReadLog 13");
                        message += $"\n ReadLog13=============";
                        string TimeStr1 = string.Empty;
                        while (objZkeeper.GetGeneralLogDataStr(machineNumber, ref dwEnrollNumber, ref dwVerifyMode, ref dwInOutMode, ref TimeStr1))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog13 - {TimeStr1}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog13 xong - Count: {lstEnrollData.Count}==============";
                        Logger.LogInfo($"\n ReadLog13 xong - Count: {lstEnrollData.Count}===========");

                        Logger.LogInfo($"\n Bat dau ReadLog 14");
                        message += $"\n ReadLog14==========";
                        string time1 = string.Empty;
                        while (objZkeeper.SSR_GetSuperLogData(machineNumber, out number, out str, out str, out number, out time1, out number, out number, out number))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog14 - {time1}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog14 xong - Count: {lstEnrollData.Count}===============";
                        Logger.LogInfo($"\n ReadLog14 xong - Count: {lstEnrollData.Count}===========");

                        break;
                    case 1:
                        Logger.LogInfo($"\nBat dau ReadLog: 1");
                        while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog1 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            int dwEnroll = 0;
                            int.TryParse(dwEnrollNumber1, out dwEnroll);
                            objInfo.IndRegID = dwEnroll;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog1 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog1 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 2:
                        Logger.LogInfo($"\nBat dau ReadLog: 2");
                        while (objZkeeper.GetGeneralLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref dwVerifyMode, ref dwInOutMode, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog2 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog2  xong- Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog2 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 3:
                        Logger.LogInfo($"\nBat dau ReadLog: 3");
                        while (objZkeeper.GetSuperLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog3 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog3 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog3 xong- Count: {lstEnrollData.Count}");
                        break;
                    case 4:
                        Logger.LogInfo($"\nBat dau ReadLog: 4");
                        while (objZkeeper.GetAllSLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog4 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog4 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog4 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 5:
                        Logger.LogInfo($"\nBat dau ReadLog: 5");
                        while (objZkeeper.GetAllGLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref dwVerifyMode, ref dwInOutMode, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog5 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog5 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog5 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 6:
                        Logger.LogInfo($"\nBat dau ReadLog: 6");
                        while (objZkeeper.GetGeneralExtLogData(machineNumber, ref dwEnrollNumber, ref dwVerifyMode, ref dwInOutMode, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond, ref dwWorkCode, ref number))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog6 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog6 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog6 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 7:
                        Logger.LogInfo($"\nBat dau ReadLog: 7");
                        while (objZkeeper.SSR_OutPutHTMLRep(machineNumber, dwEnrollNumber1, str, str, str, str, str, BYear, BMonth, BDay, BHour, BMinute, number, EYear, EMonth, EDay, EHour, EMinute, number, str, str, number, number, str))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog7 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            int dwEnroll = 0;
                            int.TryParse(dwEnrollNumber1, out dwEnroll);
                            objInfo.IndRegID = dwEnroll;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog7 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog7 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 8:
                        Logger.LogInfo($"\nBat dau ReadLog: 8");
                        while (objZkeeper.GetSuperLogData2(machineNumber, ref number, ref dwEnrollNumber, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref number))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog8 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog8 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog8 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 9:
                        Logger.LogInfo($"\nBat dau ReadLog: 9");
                        while (objZkeeper.ReadLastestLogData(machineNumber, number, dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog9 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog9 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog9 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 10:
                        Logger.LogInfo($"\nBat dau ReadLog: 10");
                        while (objZkeeper.ReadSuperLogDataEx(machineNumber, BYear, BMonth, BDay, BHour, BMinute, number, EYear, EMonth, EDay, EHour, EMinute, number, number))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog10 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog10 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog10 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 11:
                        Logger.LogInfo($"\nBat dau ReadLog: 11");
                        while (objZkeeper.GetSuperLogDataEx(machineNumber, ref dwEnrollNumber1, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog11 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            int dwEnroll = 0;
                            int.TryParse(dwEnrollNumber1, out dwEnroll);
                            objInfo.IndRegID = dwEnroll;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog11 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog11 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 12:
                        Logger.LogInfo($"\nBat dau ReadLog: 12");
                        while (objZkeeper.SSR_GetGeneralLogDataEx(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, out str))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $" ReadLog12 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            int dwEnroll = 0;
                            int.TryParse(dwEnrollNumber1, out dwEnroll);
                            objInfo.IndRegID = dwEnroll;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog12 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog12 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 13:
                        Logger.LogInfo($"\nBat dau ReadLog: 13");
                        string TimeStr = string.Empty;
                        while (objZkeeper.GetGeneralLogDataStr(machineNumber, ref dwEnrollNumber, ref dwVerifyMode, ref dwInOutMode, ref TimeStr))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog13 - {TimeStr}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = dwEnrollNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog13 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog13 xong - Count: {lstEnrollData.Count}");
                        break;
                    case 14:
                        Logger.LogInfo($"\nBat dau ReadLog: 14");
                        string time = string.Empty;
                        while (objZkeeper.SSR_GetSuperLogData(machineNumber, out number, out str, out str, out number, out time, out number, out number, out number))
                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog14 - {time}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog14 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog14 xong - Count: {lstEnrollData.Count}");
                        break;
                    default:
                        Logger.LogInfo($"\nBat dau ReadLog: 1");
                        while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))


                        {
                            //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                            string inputDate = $"ReadLog1 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            int dwEnroll = 0;
                            int.TryParse(dwEnrollNumber1, out dwEnroll);
                            objInfo.IndRegID = dwEnroll;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        message += $"\n ReadLog1 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog1 xong - Count: {lstEnrollData.Count}");
                        break;
                };


                if (readLog == 15)
                {
                    Logger.LogInfo($"\nBat dau ReadLog: 15");
                    int ret = 0;
                    int idwErrorCode = 0;
                    objZkeeper.EnableDevice(machineNumber, false);//disable the device

                    string sdwEnrollNumber = "";
                    int idwVerifyMode = 0;
                    int idwInOutMode = 0;
                    int idwYear = 0;
                    int idwMonth = 0;
                    int idwDay = 0;
                    int idwHour = 0;
                    int idwMinute = 0;
                    int idwSecond = 0;
                    int idwWorkcode = 0;

                    Logger.LogInfo($"\nBat dau ReadTimeGLogData");
                    if (objZkeeper.ReadTimeGLogData(machineNumber, fromTime, toTime))
                    {
                        Logger.LogInfo($"\nBat dau SSR_GetGeneralLogData");
                        while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out sdwEnrollNumber, out idwVerifyMode,
                                    out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                        {
                            string inputDate = $"ReadLog15 - {idwDay}/{idwMonth}/{idwYear} {idwHour}:{idwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            int dwEnroll = 0;
                            int.TryParse(sdwEnrollNumber, out dwEnroll);
                            objInfo.IndRegID = dwEnroll;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        ret = 1;
                        message += $"\n ReadLog15 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog15 xong - Count: {lstEnrollData.Count}");
                    }
                    else
                    {
                        objZkeeper.GetLastError(ref idwErrorCode);
                        ret = idwErrorCode;
                        Logger.LogInfo($"\n ReadLog: 15====GetLastError: ErrorCode {idwErrorCode}");

                        if (idwErrorCode != 0)
                        {
                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = idwErrorCode;
                            objInfo.DateTimeRecord = "*Read attlog by period failed,ErrorCode: " + idwErrorCode.ToString();
                            lstEnrollData.Add(objInfo);
                        }
                        else
                        {
                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = idwErrorCode;
                            objInfo.DateTimeRecord = "No data from terminal returns!";
                            lstEnrollData.Add(objInfo);
                        }
                    }
                    objZkeeper.EnableDevice(machineNumber, true);//enable the device
                }
                else if (readLog == 16)
                {
                    Logger.LogInfo($"\nBat dau ReadLog: 16");
                    int ret = 0;
                    int idwErrorCode = 0;
                    objZkeeper.EnableDevice(machineNumber, false);//disable the device

                    string sdwEnrollNumber = "";
                    int idwVerifyMode = 0;
                    int idwInOutMode = 0;
                    int idwYear = 0;
                    int idwMonth = 0;
                    int idwDay = 0;
                    int idwHour = 0;
                    int idwMinute = 0;
                    int idwSecond = 0;
                    int idwWorkcode = 0;
                    Logger.LogInfo($"\nBat dau ReadGeneralLogData");
                    if (objZkeeper.ReadGeneralLogData(machineNumber))
                    {
                        Logger.LogInfo($"\nBat dau SSR_GetGeneralLogData");
                        while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out sdwEnrollNumber, out idwVerifyMode,
                                    out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                        {
                            string inputDate = $"ReadLog16 - {idwDay}/{idwMonth}/{idwYear} {idwHour}:{idwMinute}";

                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            int dwEnroll = 0;
                            int.TryParse(sdwEnrollNumber, out dwEnroll);
                            objInfo.IndRegID = dwEnroll;
                            objInfo.DateTimeRecord = inputDate;

                            lstEnrollData.Add(objInfo);
                        }
                        ret = 1;
                        message += $"\n ReadLog16 xong - Count: {lstEnrollData.Count}";
                        Logger.LogInfo($"\n ReadLog16 xong - Count: {lstEnrollData.Count}");
                    }
                    else
                    {
                        objZkeeper.GetLastError(ref idwErrorCode);
                        ret = idwErrorCode;
                        Logger.LogInfo($"\n ReadLog: 16====GetLastError: ErrorCode {idwErrorCode}");

                        if (idwErrorCode != 0)
                        {
                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = idwErrorCode;
                            objInfo.DateTimeRecord = "*Read attlog failed,ErrorCode: " + idwErrorCode.ToString();
                            lstEnrollData.Add(objInfo);
                        }
                        else
                        {
                            MachineInfo objInfo = new MachineInfo();
                            objInfo.MachineNumber = machineNumber;
                            objInfo.IndRegID = idwErrorCode;
                            objInfo.DateTimeRecord = "No data from terminal returns!";
                            lstEnrollData.Add(objInfo);
                        }
                    }

                    objZkeeper.EnableDevice(machineNumber, true);//enable the device
                }
            }
            catch (Exception ex)
            {
                message += $"\nLog Exception: {ex.Message}";
            }
            return lstEnrollData;
        }

        public ICollection<UserIDInfo> GetAllUserID(ZkemClient objZkeeper, int machineNumber)
        {
            int dwEnrollNumber = 0;
            int dwEMachineNumber = 0;
            int dwBackUpNumber = 0;
            int dwMachinePrivelage = 0;
            int dwEnabled = 0;

            ICollection<UserIDInfo> lstUserIDInfo = new List<UserIDInfo>();

            while (objZkeeper.GetAllUserID(machineNumber, ref dwEnrollNumber, ref dwEMachineNumber, ref dwBackUpNumber, ref dwMachinePrivelage, ref dwEnabled))
            {
                UserIDInfo userID = new UserIDInfo();
                userID.BackUpNumber = dwBackUpNumber;
                userID.Enabled = dwEnabled;
                userID.EnrollNumber = dwEnrollNumber;
                userID.MachineNumber = dwEMachineNumber;
                userID.Privelage = dwMachinePrivelage;
                lstUserIDInfo.Add(userID);
            }
            return lstUserIDInfo;
        }

        public void GetGeneratLog(ZkemClient objZkeeper, int machineNumber, string enrollNo)
        {
            string name = null;
            string password = null;
            int previlage = 0;
            bool enabled = false;
            byte[] byTmpData = new byte[2000];
            int tempLength = 0;

            int idwFingerIndex = 0;// [ <--- Enter your fingerprint index here ]
            int iFlag = 0;

            objZkeeper.ReadAllTemplate(machineNumber);

            while (objZkeeper.SSR_GetUserInfo(machineNumber, enrollNo, out name, out password, out previlage, out enabled))
            {
                if (objZkeeper.GetUserTmpEx(machineNumber, enrollNo, idwFingerIndex, out iFlag, out byTmpData[0], out tempLength))
                {
                    break;
                }
            }
        }


        public bool PushUserDataToDevice(ZkemClient objZkeeper, int machineNumber, string enrollNo)
        {
            string userName = string.Empty;
            string password = string.Empty;
            int privelage = 1;
            return objZkeeper.SSR_SetUserInfo(machineNumber, enrollNo, userName, password, privelage, true);
        }

        public bool UploadFTPTemplate(ZkemClient objZkeeper, int machineNumber, List<UserInfo> lstUserInfo)
        {
            string sdwEnrollNumber = string.Empty, sName = string.Empty, sTmpData = string.Empty;
            int idwFingerIndex = 0, iPrivilege = 0, iFlag = 1, iUpdateFlag = 1;
            string sPassword = "";
            string sEnabled = "";
            bool bEnabled = false;

            if (objZkeeper.BeginBatchUpdate(machineNumber, iUpdateFlag))
            {
                string sLastEnrollNumber = "";

                for (int i = 0; i < lstUserInfo.Count; i++)
                {
                    sdwEnrollNumber = lstUserInfo[i].EnrollNumber;
                    sName = lstUserInfo[i].Name;
                    idwFingerIndex = lstUserInfo[i].FingerIndex;
                    sTmpData = lstUserInfo[i].TmpData;
                    iPrivilege = lstUserInfo[i].Privelage;
                    sPassword = lstUserInfo[i].Password;
                    sEnabled = lstUserInfo[i].Enabled.ToString();
                    iFlag = Convert.ToInt32(lstUserInfo[i].iFlag);
                    bEnabled = true;

                    /* [ Identify whether the user 
                         information(except fingerprint templates) has been uploaded */

                    if (sdwEnrollNumber != sLastEnrollNumber)
                    {
                        if (objZkeeper.SSR_SetUserInfo(machineNumber, sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload user information to the memory
                            objZkeeper.SetUserTmpExStr(machineNumber, sdwEnrollNumber, idwFingerIndex, iFlag, sTmpData);//upload templates information to the memory
                        else return false;
                    }
                    else
                    {
                        /* [ The current fingerprint and the former one belongs the same user,
                        i.e one user has more than one template ] */
                        objZkeeper.SetUserTmpExStr(machineNumber, sdwEnrollNumber, idwFingerIndex, iFlag, sTmpData);
                    }

                    sLastEnrollNumber = sdwEnrollNumber;
                }

                return true;
            }
            else
                return false;
        }

        public object ClearData(ZkemClient objZkeeper, int machineNumber, ClearFlag clearFlag)
        {
            int iDataFlag = (int)clearFlag;

            if (objZkeeper.ClearData(machineNumber, iDataFlag))
                return objZkeeper.RefreshData(machineNumber);
            else
            {
                int idwErrorCode = 0;
                objZkeeper.GetLastError(ref idwErrorCode);
                return idwErrorCode;
            }
        }

        public bool ClearGLog(ZkemClient objZkeeper, int machineNumber)
        {
            return objZkeeper.ClearGLog(machineNumber);
        }


        public string FetchDeviceInfo(ZkemClient objZkeeper, int machineNumber)
        {
            StringBuilder sb = new StringBuilder();

            string returnValue = string.Empty;


            objZkeeper.GetFirmwareVersion(machineNumber, ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Firmware V: ");
                sb.Append(returnValue);
                sb.Append(",");
            }


            returnValue = string.Empty;
            objZkeeper.GetVendor(ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Vendor: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            string sWiegandFmt = string.Empty;
            objZkeeper.GetWiegandFmt(machineNumber, ref sWiegandFmt);

            returnValue = string.Empty;
            objZkeeper.GetSDKVersion(ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("SDK V: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            returnValue = string.Empty;
            objZkeeper.GetSerialNumber(machineNumber, out returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Serial No: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            returnValue = string.Empty;
            objZkeeper.GetDeviceMAC(machineNumber, ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Device MAC: ");
                sb.Append(returnValue);
            }

            return sb.ToString();
        }



    }
}
