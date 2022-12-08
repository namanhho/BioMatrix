///
///    Experimented By : Ozesh Thapa
///    Email: dablackscarlet@gmail.com
///
using BioMetrixCore.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

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

        public ICollection<MachineInfo> GetLogData(ZkemClient objZkeeper, int machineNumber, ref string message)
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

            ICollection<MachineInfo> lstEnrollData = new List<MachineInfo>();

            var a = Utility.GetAppSetting("ReadType");
            int readType = 1;
            int.TryParse(a, out readType);
            switch (readType)
            {
                case 1:
                    objZkeeper.ReadAllGLogData(machineNumber);
                    break;
                case 2:
                    objZkeeper.ReadGeneralLogData(machineNumber);
                    break;
                case 3:
                    objZkeeper.ReadSuperLogData(machineNumber);
                    break;
                case 4:
                    objZkeeper.ReadAllSLogData(machineNumber);
                    break;
                case 5:
                    objZkeeper.ReadNewGLogData(machineNumber);
                    break;
                case 6:
                    objZkeeper.ReadAllBellSchData(machineNumber);
                    break;
                default:
                    break;
            };

            int number = 0;
            string str = string.Empty;
            int dwEnrollNumber = 0;

            a = Utility.GetAppSetting("type");
            int type = 1;
            int.TryParse(a, out type);
            switch (type)
            {
                case 1:
                    while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))


                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type1 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        int dwEnroll = 0;
                        int.TryParse(dwEnrollNumber1, out dwEnroll);
                        objInfo.IndRegID = dwEnroll;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type1 - Count: {lstEnrollData.Count}";
                    break;
                case 2:
                    while (objZkeeper.GetGeneralLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref dwVerifyMode, ref dwInOutMode, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type2 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        objInfo.IndRegID = dwEnrollNumber;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type2 - Count: {lstEnrollData.Count}";
                    break;
                case 3:
                    while (objZkeeper.GetSuperLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type3 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        objInfo.IndRegID = dwEnrollNumber;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type3 - Count: {lstEnrollData.Count}";
                    break;
                case 4:
                    while (objZkeeper.GetAllSLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type4 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        objInfo.IndRegID = dwEnrollNumber;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type4 - Count: {lstEnrollData.Count}";
                    break;
                case 5:
                    while (objZkeeper.GetAllGLogData(machineNumber, ref number, ref dwEnrollNumber, ref number, ref dwVerifyMode, ref dwInOutMode, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type5 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        objInfo.IndRegID = dwEnrollNumber;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type5 - Count: {lstEnrollData.Count}";
                    break;
                case 6:
                    while (objZkeeper.GetGeneralExtLogData(machineNumber, ref dwEnrollNumber, ref dwVerifyMode, ref dwInOutMode, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond, ref dwWorkCode, ref number))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type6 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        objInfo.IndRegID = dwEnrollNumber;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type6 - Count: {lstEnrollData.Count}";
                    break;
                case 7:
                    while (objZkeeper.SSR_OutPutHTMLRep(machineNumber, dwEnrollNumber1, str, str, str, str, str, BYear, BMonth, BDay, BHour, BMinute, number, EYear, EMonth, EDay, EHour, EMinute, number, str, str, number, number, str))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type7 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        int dwEnroll = 0;
                        int.TryParse(dwEnrollNumber1, out dwEnroll);
                        objInfo.IndRegID = dwEnroll;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type7 - Count: {lstEnrollData.Count}";
                    break;
                case 8:
                    while (objZkeeper.GetSuperLogData2(machineNumber, ref number, ref dwEnrollNumber, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref number))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type8 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        objInfo.IndRegID = dwEnrollNumber;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type8 - Count: {lstEnrollData.Count}";
                    break;
                case 9:
                    while (objZkeeper.ReadLastestLogData(machineNumber, number, dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type9 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        objInfo.IndRegID = dwEnrollNumber;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type9 - Count: {lstEnrollData.Count}";
                    break;
                case 10:
                    while (objZkeeper.ReadSuperLogDataEx(machineNumber, BYear, BMonth, BDay, BHour, BMinute, number, EYear, EMonth, EDay, EHour, EMinute, number, number))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type10 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        objInfo.IndRegID = dwEnrollNumber;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type10 - Count: {lstEnrollData.Count}";
                    break;
                case 11:
                    while (objZkeeper.GetSuperLogDataEx(machineNumber, ref dwEnrollNumber1, ref number, ref number, ref number, ref number, ref number, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type11 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        int dwEnroll = 0;
                        int.TryParse(dwEnrollNumber1, out dwEnroll);
                        objInfo.IndRegID = dwEnroll;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type11 - Count: {lstEnrollData.Count}";
                    break;
                case 12:
                    while (objZkeeper.SSR_GetGeneralLogDataEx(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, out str))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type12 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        int dwEnroll = 0;
                        int.TryParse(dwEnrollNumber1, out dwEnroll);
                        objInfo.IndRegID = dwEnroll;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type12 - Count: {lstEnrollData.Count}";
                    break;
                case 13:
                    string TimeStr = string.Empty;
                    while (objZkeeper.GetGeneralLogDataStr(machineNumber, ref dwEnrollNumber, ref dwVerifyMode, ref dwInOutMode, ref TimeStr))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type13 - {TimeStr}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        objInfo.IndRegID = dwEnrollNumber;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type13 - Count: {lstEnrollData.Count}";
                    break;
                case 14:
                    string time = string.Empty;
                    while (objZkeeper.SSR_GetSuperLogData(machineNumber, out number, out str, out str, out number, out time, out number, out number, out number))
                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type14 - {time}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type14 - Count: {lstEnrollData.Count}";
                    break;
                default:
                    while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))


                    {
                        //string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
                        string inputDate = $"Type1 - {dwDay}/{dwMonth}/{dwYear} {dwHour}:{dwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        int dwEnroll = 0;
                        int.TryParse(dwEnrollNumber1, out dwEnroll);
                        objInfo.IndRegID = dwEnroll;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    message += $"Type1 - Count: {lstEnrollData.Count}";
                    break;
            };


            if (type == 15)
            {
                int ret = 0;
                int idwErrorCode = 0;
                var fromTime = Utility.GetAppSetting("FromTime");
                var toTime = Utility.GetAppSetting("ToTime");
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


                if (objZkeeper.ReadTimeGLogData(machineNumber, fromTime, toTime))
                {
                    while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out sdwEnrollNumber, out idwVerifyMode,
                                out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                    {
                        string inputDate = $"Type15 - {idwDay}/{idwMonth}/{idwYear} {idwHour}:{idwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        int dwEnroll = 0;
                        int.TryParse(sdwEnrollNumber, out dwEnroll);
                        objInfo.IndRegID = dwEnroll;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    ret = 1;
                }
                else
                {
                    objZkeeper.GetLastError(ref idwErrorCode);
                    ret = idwErrorCode;

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
            else if (type == 16)
            {
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

                if (objZkeeper.ReadGeneralLogData(machineNumber))
                {
                    while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out sdwEnrollNumber, out idwVerifyMode,
                                out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                    {
                        string inputDate = $"Type16 - {idwDay}/{idwMonth}/{idwYear} {idwHour}:{idwMinute}";

                        MachineInfo objInfo = new MachineInfo();
                        objInfo.MachineNumber = machineNumber;
                        int dwEnroll = 0;
                        int.TryParse(sdwEnrollNumber, out dwEnroll);
                        objInfo.IndRegID = dwEnroll;
                        objInfo.DateTimeRecord = inputDate;

                        lstEnrollData.Add(objInfo);
                    }
                    ret = 1;
                }
                else
                {
                    objZkeeper.GetLastError(ref idwErrorCode);
                    ret = idwErrorCode;

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
