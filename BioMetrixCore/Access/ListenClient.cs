using BioMetrixCore.BLL;
using BioMetrixCore.Model;
using BioMetrixCore.Utilities;
using BioMetrixCore.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace BioMetrixCore.Access
{
    /// <summary>Server Listen
    /// </summary>
    public class ListenClient
    {
        TcpListener tcp;
        const int MAXBUFFERSIZE = 1024 * 1024 * 2;
        /// <summary>
        /// listenling port
        /// </summary>
        private int port = 80;
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// server IP
        /// </summary>
        private string serverIP = string.Empty;
        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }

        /// <summary>
        /// is listenling
        /// </summary>
        private static bool listening = false;
        public static bool Listening
        {
            get { return listening; }
        }
        private static object lockObj = new object();
        private static List<DeviceModel> _tempDeviceModelList;
        public static List<DeviceModel> TempDeviceModelList
        {
            get
            {
                if (null == _tempDeviceModelList)
                {
                    lock (lockObj)
                    {
                        if (null == _tempDeviceModelList)
                            _tempDeviceModelList = new List<DeviceModel>();
                    }
                }
                return _tempDeviceModelList;
            }
            set { _tempDeviceModelList = value; }
        }

        /// <summary>
        /// 字符编码
        /// </summary>
        private Encoding _encoding = Encoding.GetEncoding("UTF-8");

        #region event

        /// <summary>
        /// error event
        /// </summary>
        /// <param name="imgReceive"></param>
        /// <param name="empPin"></param>
        public event Action<string> OnError;

        /// <summary>
        /// new punch event
        /// </summary>
        public event Action<RealTimeLogModel> OnNewRealTimeLog;

        /// <summary>
        /// new punch event
        /// </summary>
        public event Action<RealTimeStateModel> OnNewRealTimeState;



        /// <summary>
        /// new oplog event
        /// </summary>
        public event Action<ErrorLogModel> OnNewErrorLog;

        /// <summary>
        /// new user event
        /// </summary>
        /// <param name="user"></param>
        public event Action<UserInfoModel> OnNewUser;

        /// <summary>
        /// new bio fp template
        /// </summary>

        public event Action<List<TmpFPModel>> OnNewFP;

        /// <summary>
        /// new face template
        /// </summary>

        public event Action<List<TmpFaceModel>> OnNewFace;
        /// <summary>
        /// new Palm template
        /// </summary>

        public event Action<TmpBioDataModel> OnNewPalm;
        /// <summary>
        /// new biophoto
        /// </summary>

        public event Action<List<TmpBioPhotoModel>> OnNewBioPhoto;



        /// <summary>
        /// new machine
        /// </summary>

        public event Action<DeviceModel> OnNewMachine;

        /// <summary>
        /// Device Info Sync
        /// </summary>

        public event Action<DeviceModel> OnDeviceSync;


        /// <summary>
        /// Receive Data event
        /// </summary>
        public event Action<string> OnReceiveDataEvent;

        /// <summary>
        /// Send Data event
        /// </summary>
        public event Action<string> OnSendDataEvent;

        #endregion

        #region Main TCP listening process

        /// <summary>
        /// Start listening
        /// </summary>
        public void StartListening()
        {
            Logger.LogError($"\nStartListening: Bat dau");
            try
            {
                if (tcp == null)
                {
                    this.tcp = new TcpListener(IPAddress.Parse(serverIP), port);
                }
                Logger.LogError($"\nStartListening: Start-Bat dau");
                this.tcp.Start();
                listening = true;
                Socket mySocket = null;
                byte[] bReceive;

                while (listening)
                {

                    // Blocks until a client has connected to the server 
                    try
                    {
                        Logger.LogError($"\nStartListening: AcceptSocket-Bat dau");
                        mySocket = this.tcp.AcceptSocket();
                        Logger.LogError($"\nStartListening: AcceptSocket-Xong");
                        mySocket.ReceiveBufferSize = MAXBUFFERSIZE;
                        mySocket.SendBufferSize = MAXBUFFERSIZE;
                        bReceive = new byte[MAXBUFFERSIZE];
                        int len = 0;
                        int i = 1;
                        byte[] databyte = new Byte[MAXBUFFERSIZE];
                        while (mySocket.Available > 0)
                        {
                            Logger.LogError($"\nStartListening: Receive-Bat dau");
                            i = mySocket.Receive(bReceive);
                            for (int j = 0; j < i; j++)
                            {
                                databyte[len + j] = bReceive[j];
                            }
                            len += i;
                            Array.Clear(bReceive, 0, bReceive.Length);
                            Thread.Sleep(10);
                        }

                        string strReceive = Encoding.UTF8.GetString(databyte).TrimEnd('\0');//utf8转字符串

                        if (strReceive.IndexOf("100-continue") >= 0)
                        {
                            int length = 0;
                            string HttpContinue = "HTTP/1.1 100 Continue\r\n\r\n";
                            Thread.Sleep(1);
                            byte[] bContinue = new byte[32];
                            bContinue = System.Text.Encoding.Default.GetBytes(HttpContinue);
                            mySocket.Send(bContinue);
                            Thread.Sleep(50);
                            Array.Clear(bReceive, 0, bReceive.Length);
                            i = mySocket.Receive(bReceive);
                            length += i;
                            for (int j = 0; j < length; j++)
                            {
                                databyte[len + j] = bReceive[j];
                            }
                            strReceive = Encoding.UTF8.GetString(databyte);
                        }

                        Logger.LogError($"\nStartListening: Analysis-Bat dau");
                        Analysis(databyte, strReceive, mySocket);

                        Thread.Sleep(10);
                        mySocket.Close();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"\nStartListening Exception1: {ex.Message}");
                        mySocket.Close();
                        if (OnError != null)
                        {
                            OnError(ex.Message + ":" + ex.StackTrace);
                        }
                    }
                }

                this.tcp.Stop();
            }
            catch (Exception ex)
            {
                Logger.LogError($"\nStartListening Exception2: {ex.Message}");
                listening = false;
                string errMessage = string.Format("Please be sure that you are listening to the port number of your own PC." +
                "And {0} port is not occupied by other application or stopped by firewall.", port);

                if (OnError != null)
                {
                    OnError(errMessage);
                }
                Logger.LogError($"\nStartListening Exception2: {errMessage}");
            }

            //try
            //{
            //    string strReceive = $"POST /iclock/cdata?SN=0782232260009&table=ATTLOG&Stamp=9999 HTTP/1.1\nHost: 192.168.1.240:8080\nUser - Agent: iClock Proxy/ 1.09\nConnection: close\nAccept: */*\nContent-Type: text/plain \nContent-Length: 44\r\n\r\n3	2023-10-16 08:38:16	255	15	0	0	0	255	0	0	\n";
            //    var logData = Regex.Replace(Regex.Replace(strReceive, "\r", @"\r"), "\n", @"\n");
            //    Logger.LogError($"\n StartListening----logData: {logData}");
            //    int count = 0;
            //    if (strReceive.Substring(0, 4).ToUpper() == "POST" && strReceive.IndexOfEx("SN=") > 0 && strReceive.IndexOfEx("cdata?") > 0 && strReceive.IndexOfEx("table=ATTLOG", 1) > 0)
            //    {
            //        RealTimeLog(strReceive, ref count);
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }

        /// <summary>
        /// stop listening
        /// </summary>
        public void StopListening()
        {
            Logger.LogError($"\n StopListening: Bat dau");
            if (listening)
            {
                listening = false;
                this.tcp.Stop();
            }
        }

        #region 处理设备消息
        /// <summary>
        /// Analysis which kind of request from the iclock Devices
        /// </summary>
        /// <param name="bReceive"></param>
        /// <param name="endsocket"></param>
        private void Analysis(byte[] bReceive, string strReceive, Socket endsocket)
        {
            Logger.LogError($"\n Analysis: Bat dau----strReceive: {strReceive}");
            var logData = Regex.Replace(Regex.Replace(strReceive, "\r", @"\r"), "\n", @"\n");
            Logger.LogError($"\n Analysis----logData: {logData}");
            if (string.IsNullOrWhiteSpace(strReceive))
                return;
            if (null != OnReceiveDataEvent)
            {
                OnReceiveDataEvent(strReceive);
            }

            bool unknownMsg = false;
            try
            {
                if (strReceive.Substring(0, 3).ToUpper() == "GET")
                {
                    if (strReceive.IndexOfEx("cdata?") > 0 && strReceive.IndexOfEx("options=all", 0) > 0)
                    {
                        DevFirstRequest(strReceive, endsocket);
                    }
                    else if (strReceive.IndexOfEx("rtdata?") > 0 && strReceive.IndexOfEx("type=time", 0) > 0)
                    {
                        DevGetDateTime(endsocket);
                    }
                    else if (strReceive.IndexOfEx("getrequest?") > 0)
                    {
                        DevGetRequest(bReceive, endsocket);
                    }
                    else if (strReceive.IndexOfEx("ping?") > 0)
                    {
                        SendDataToDevice("200 OK", "OK\r\n", ref endsocket);
                    }
                    else
                    {
                        unknownMsg = true;
                    }
                }
                else if (strReceive.Substring(0, 4).ToUpper() == "POST")
                {
                    if (strReceive.IndexOfEx("cdata?") > 0 || strReceive.IndexOfEx("querydata?") > 0)
                    {
                        DevSendData(bReceive, endsocket);
                    }
                    else
                    {
                        unknownMsg = true;
                    }
                }
                else
                {
                    unknownMsg = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n Analysis Exception: {ex.Message}");
                throw ex;
            }

            //处理未知消息
            if (unknownMsg)
            {
                UnknownCmdProcess(strReceive, endsocket);
            }
        }

        /// <summary>
        /// 设备第一次请求
        /// </summary>
        /// <param name="strReceive"></param>
        /// <param name="remoteSocket"></param>
        private void DevFirstRequest(string strReceive, Socket remoteSocket)
        {
            Logger.LogError($"\n DevFirstRequest: Bat dau---strReceive: {strReceive}");
            string devSN = GetValueByNameInPushHeader(strReceive, "SN");

            string strReply = InitDeviceConnect(devSN);
            SendDataToDevice("200 OK", strReply, ref remoteSocket);

        }
        private void DevGetDateTime(Socket remoteSocket)
        {
            Logger.LogError($"\n DevGetDateTime: Bat dau");
            string strReply = $"DateTime={Tools.OldEncodeTime(DateTime.UtcNow.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.UtcNow.Hour, DateTime.Now.Minute, DateTime.Now.Second)}";
            strReply += ",ServerTZ=" + Tools.GetServerTZ();
            SendDataToDevice("200 OK", strReply, ref remoteSocket);
        }
        /// <summary>
        /// Inilialize Device connection
        /// </summary>
        /// <param name="devSN"></param>
        /// <returns></returns>
        private string InitDeviceConnect(string devSN)
        {
            Logger.LogError($"\n InitDeviceConnect: Bat dau--devSN: {devSN}");
            string strReply = "OK";

            DeviceModel device = DeviceBll.Get(devSN);
            if (null == device)
            {//未注册

            }
            else
            {//已注册
                OnDeviceSync?.Invoke(device);
                strReply = GetDeviceInitInfo(device).ToString();

            }
            return strReply;
        }
        /// <summary>
        /// Reponse get request for iclock Devices
        /// </summary>
        /// <param name="bReceive"></param>
        /// <param name="remoteSocket"></param>
        private void DevGetRequest(byte[] bReceive, Socket remoteSocket)
        {
            Logger.LogError($"\n DevGetRequest: Bat dau");
            string sBuffer = _encoding.GetString(bReceive);
            string replyString = "OK\r\n";
            string SN = GetValueByNameInPushHeader(sBuffer, "SN");
            string ReplyCode = "200 OK";
            DeviceModel device = DeviceBll.Get(SN);

            if (null == device)
            {

                ReplyCode = "401 Unauthorized";
                replyString = "Device Unauthorized";
            }
            else
            {
                if (null != OnDeviceSync)
                {
                    OnDeviceSync(device);
                }
            }
            SendDataToDevice(ReplyCode, replyString, ref remoteSocket);

        }
        /// <summary>
        /// 设备发送数据
        /// </summary>
        /// <param name="bReceive"></param>
        /// <param name="remoteSocket"></param>
        private void DevSendData(byte[] bReceive, Socket remoteSocket)
        {
            string strReceive = Encoding.ASCII.GetString(bReceive).TrimEnd().TrimEnd('\0');
            Logger.LogError($"\n DevSendData: Bat dau---strReceive: {strReceive}");
            var logData = Regex.Replace(Regex.Replace(strReceive, "\r", @"\r"), "\n", @"\n");
            Logger.LogError($"\n DevSendData----logData: {logData}");

            string strReply = "OK";

            if (strReceive.IndexOfEx("table=rtlog", 1) > 0)
            {//上传实时事件
                int count = 0;
                RealTimeLog(strReceive, ref count);
            }
            else if (strReceive.IndexOfEx("table=rtstate", 1) > 0)
            {//上传实时状态
                RealTimeState(strReceive);
            }
            else if (strReceive.IndexOfEx("table=tabledata&tablename=user", 1) > 0)
            {//上传用户信息
                string count = GetValueByNameInPushHeader(strReceive, "count");
                strReply = "user=" + count;
                UserInfo(strReceive);
            }
            else if (strReceive.IndexOfEx("table=tabledata&tablename=errorlog", 1) > 0)
            {//上传异常日志
                Errorlog(strReceive);
            }
            else if (strReceive.IndexOfEx("type=tabledata&cmdid", 1) > 0)
            {
                string tableName = GetValueByNameInPushHeader(strReceive, "tablename");
                string count = GetValueByNameInPushHeader(strReceive, "count");
                if (strReceive.IndexOfEx("datafmt") > 0)
                {
                    strReply = tableName + "=" + count;
                }
                else
                {
                    //查询数据
                    int dataCount = Tools.TryConvertToInt32(count);
                    GetQueryData(strReceive, tableName, ref strReply, ref dataCount);
                    strReply = tableName + "=" + count;
                }

            }
            else if (strReceive.IndexOfEx("desc=overview", 1) > 0)
            {
                strReply = "OK";
            }

            string replyCode = "200 OK";
            SendDataToDevice(replyCode, strReply, ref remoteSocket);
        }


        /// <summary>
        /// 处理未知消息
        /// </summary>
        /// <param name="remoteSocket"></param>
        private void UnknownCmdProcess(string strReceive, Socket remoteSocket)
        {
            Logger.LogError($"\n UnknownCmdProcess: Bat dau");
            SendDataToDevice("401 Unknown", "Unknown DATA", ref remoteSocket);


            if (OnError != null)
            {
                OnError("UnKnown message from device: " + strReceive);
            }
        }
        public string EncryptString(byte[] bInput)
        {
            Logger.LogError($"\n EncryptString: Bat dau");
            try
            {
                return System.Convert.ToBase64String(bInput, 0, bInput.Length);
            }
            catch (System.ArgumentNullException)
            {
                //binary data is NULL. 
                return "";
            }
            catch (System.ArgumentOutOfRangeException)
            {
                //length error
                return "";
            }

        }
        #endregion end-处理设备消息

        /// <summary>
        /// Get Stamp
        /// </summary>
        /// <param name="sBuffer"></param>
        /// <param name="numberstr"></param>
        private void GetTimeNumber(string sBuffer, ref string numberstr)
        {
            numberstr = "";

            for (int i = 0; i < sBuffer.Length; i++)
            {
                if (sBuffer[i] > 47 && sBuffer[i] < 58)
                {
                    numberstr += sBuffer[i];
                }
                else
                {
                    break;
                }
            }
        }
        /// <summary>
        /// Get Value By name, Value=Name
        /// </summary>
        private string GetValueByNameInPushHeader(string buffer, string Name)
        {
            Logger.LogError($"\n GetValueByNameInPushHeader: Bat dau");
            string[] splitStr = buffer.Split('&', '?', ' ', '\n', '\r', ',');
            if (splitStr.Length <= 0)
            {
                return null;
            }

            foreach (string tmpStr in splitStr)
            {
                if (tmpStr.IndexOfEx(Name + "=") >= 0)
                {
                    return tmpStr.Substring(tmpStr.IndexOfEx(Name + "=") + Name.Length + 1);
                }
            }
            return null;
        }

        private int GetTimeFormTimeZone(string timezone)
        {
            string[] spistr = null;

            try
            {
                if ('-' == timezone[0])
                {
                    timezone = timezone.Substring(1);
                    spistr = timezone.Split(':');
                    if (spistr.Length == 2)
                    {
                        return -1 * (Convert.ToInt32(spistr[0]) * 60 + Convert.ToInt32(spistr[1]));
                    }
                    else if (spistr.Length == 1)
                    {
                        return -1 * (Convert.ToInt32(spistr[0]) * 60);
                    }
                }
                else
                {
                    spistr = timezone.Split(':');
                    if (spistr.Length == 2)
                    {
                        return Convert.ToInt32(spistr[0]) * 60 + Convert.ToInt32(spistr[1]);
                    }
                    else if (spistr.Length == 1)
                    {
                        return Convert.ToInt32(spistr[0]) * 60;
                    }
                }
            }
            catch { }
            return 0;
        }

        /// <summary>
        /// Get Device Init Info
        /// </summary>
        private string GetDeviceInitInfo(DeviceModel device)
        {
            Logger.LogError($"\n GetDeviceInitInfo: Bat dau");
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("registry=ok\n");
            sb.AppendFormat("RegistryCode={0}\n", device.RegistryCode);
            sb.AppendFormat("ServerVersion={0}\n", ServerInfo.ServerVersion);
            sb.AppendFormat("ServerName={0}\n", ServerInfo.ServerName);
            sb.AppendFormat((device.MachineType == "10" || device.MachineType == "9" || device.MachineType == "8") ? "PushVersion={0}\n" : "PushProtVer={0}\n", ServerInfo.PushProtVer);
            sb.AppendFormat("ErrorDelay={0}\n", device.ErrorDelay);
            sb.AppendFormat("RequestDelay={0}\n", device.RequestDelay);
            sb.AppendFormat("TransTimes={0}\n", device.TransTimes);
            sb.AppendFormat("TransInterval={0}\n", device.TransInterval);
            sb.AppendFormat("TransTables={0}\n", device.TransTables);
            sb.AppendFormat("Realtime={0}\n", device.Realtime);

            string sessionID = GetSessionID();
            //更新数据库设备session
            int nRtn = DeviceBll.UpdateSession(sessionID, device.DeviceSN);

            sb.AppendFormat("SessionID={0}\n", sessionID);
            sb.AppendFormat("TimeoutSec={0}\n", device.TimeoutSec);

            return sb.ToString();
        }

        private string GetRegistryCode()
        {
            Logger.LogError($"\n GetRegistryCode: Bat dau");
            string registryCode = null;
            int index = 0;
            Random random = null;
            //int [] index = new int[10];

            List<string> strList = new List<string>();

            for (int i = 65; i <= 90; i++)
            {
                char aa = (char)i;
                strList.Add(aa.ToString());
            }

            char[] number = new char[10];
            for (int i = 48; i <= 57; i++)
            {
                char aa = (char)i;
                strList.Add(aa.ToString());
                number[i - 48] = aa;
            }

            for (int i = 97; i <= 122; i++)
            {
                char aa = (char)i;
                strList.Add(aa.ToString());
            }

            random = new Random();
            for (int i = 0; i < 10; i++)
            {
                index = random.Next(strList.Count);
                registryCode = registryCode + strList[index];
                //strList.RemoveAt(index);
            }

            return registryCode;
        }
        /// <summary>
        /// 生成Session
        /// </summary>
        /// <returns></returns>
        private string GetSessionID()
        {
            Logger.LogError($"\n GetSessionID: Bat dau");
            string sessionID = null;
            int index = 0;
            Random random = null;

            List<string> strList = new List<string>();


            char[] number = new char[10];
            for (int i = 48; i <= 57; i++)
            {
                char aa = (char)i;
                strList.Add(aa.ToString());
                number[i - 48] = aa;
            }

            for (int i = 97; i <= 122; i++)
            {
                char aa = (char)i;
                strList.Add(aa.ToString());
            }

            random = new Random();
            for (int i = 0; i < 10; i++)
            {
                index = random.Next(strList.Count);
                sessionID = sessionID + strList[index];

            }

            return sessionID;
        }

        DeviceBll _deviceBll = null;
        public DeviceBll DeviceBll
        {
            get
            {
                if (_deviceBll == null)
                    _deviceBll = new DeviceBll();
                return _deviceBll;
            }
        }

        /// <summary>
        /// Server reponses to iclock Devices
        /// </summary>
        /// <param name="iTotBytes"></param>
        /// <param name="sStatusCode"></param>
        /// <param name="mySocket"></param>
        /// Date, Format: Thu, 19 Feb 2020 15:52:10 GMT+08:00
        private void SendDataToDevice(string sStatusCode, string sDataStr, ref Socket mySocket)
        {
            Logger.LogError($"\n SendDataToDevice: Bat dau---sStatusCode: {sStatusCode}---sDataStr:{sDataStr}");
            byte[] bData = _encoding.GetBytes(sDataStr);
            string sHeader = "HTTP/1.1 " + sStatusCode + "\r\n";
            sHeader = sHeader + "Content-Type:application/push;charset=UTF-8\r\n";
            //sHeader = sHeader + "Accept-Ranges: bytes\r\n";
            sHeader = sHeader + "Date: " + Tools.GetDateTimeNow().ToUniversalTime().ToString("r") + "\r\n";
            sHeader = sHeader + "Content-Length: " + bData.Length + "\r\n\r\n";
            byte[] header = _encoding.GetBytes(sHeader);
            byte[] buffer = new byte[bData.Length + header.Length];
            header.CopyTo(buffer, 0);
            bData.CopyTo(buffer, header.Length);
            //SendToBrowser(_encoding.GetBytes(sHeader), ref mySocket);
            //SendToBrowser(bData, ref mySocket);
            SendToBrowser(buffer, ref mySocket);

        }



        private void SendToBrowser(Byte[] bSendData, ref Socket mySocket)
        {
            Logger.LogError($"\n SendToBrowser: Bat dau");
            int numBytes = 0;
            string errMessage = string.Empty;

            try
            {
                if (mySocket.Connected)
                {
                    Logger.LogError($"\n SendToBrowser-----OnSendDataEvent: Bat dau---bSendData: {Encoding.UTF8.GetString(bSendData)}");
                    if (null != OnSendDataEvent)
                    {
                        OnSendDataEvent(Encoding.UTF8.GetString(bSendData));
                    }

                    if ((numBytes = mySocket.Send(bSendData, bSendData.Length, 0)) == -1)
                    {
                        errMessage = "Socket Error: Cannot Send Packet";
                    }
                    else
                    {
                    }
                }
                else
                {
                    errMessage = "Link Failed...";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n SendToBrowser: {ex.Message}");
                errMessage = ex.Message;
            }

            if (!string.IsNullOrEmpty(errMessage))
            {
                if (OnError != null)
                {
                    OnError(errMessage);
                }
            }
        }
        #endregion
        #region resolve data of table=user
        /// <summary>
        /// Parse UserInfo for acc Device
        /// </summary>
        /// <param name="sBuffer"></param>
        private void UserInfo(string sBuffer)
        {
            Logger.LogError($"\n UserInfo: Bat dau---sBuffer: {sBuffer}");
            string machineSN = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];

            int attindex = sBuffer.IndexOfEx("\r\n\r\n", 1);
            string attstr = sBuffer.Substring(attindex + 4);
            string token = GetValueByNameInPushHeader(sBuffer, "token");
            DeviceModel device = DeviceBll.Get(SN);
            if (device == null)
                return;

            UserInfoProcess(attstr);
        }
        private void UserInfoProcess(string attstr)
        {
            Logger.LogError($"\n UserInfoProcess: Bat dau---attstr: {attstr}");
            try
            {
                string[] strlist = attstr.Split('\n');
                foreach (string i in strlist)
                {
                    if (string.IsNullOrEmpty(i))
                        continue;
                    SaveUserinfo(i.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n UserInfoProcess Exception: {ex.Message}");
                if (OnError != null)
                {
                    OnError(ex.Message);
                }
            }
        }
        /// <summary>
        /// save user
        /// </summary>
        /// <param name="usinlog"></param>
        /// <param name="stamp"></param>
        private void SaveUserinfo(string usinlog)
        {
            Logger.LogError($"\n SaveUserinfo: Bat dau---usinlog: {usinlog}");
            if (usinlog.IndexOfEx("PIN") >= 0)
            {
                if (OnNewUser != null)
                {
                    OnNewUser(CreatUserInfo(usinlog));
                }
            }
        }
        //封装用户信息
        private UserInfoModel CreatUserInfo(string userstring)
        {
            Logger.LogError($"\n CreatUserInfo: Bat dau---userstring: {userstring}");
            userstring = Tools.Replace(userstring, "USER", "");
            Dictionary<string, string> dic = Tools.GetKeyValues(userstring);

            UserInfoModel user = new UserInfoModel();
            user.PIN = Tools.GetValueFromDic(dic, "PIN");
            user.UserName = Tools.GetValueFromDic(dic, "Name");
            user.Pri = Tools.GetValueFromDic(dic, "privilege");
            user.Passwd = Tools.GetValueFromDic(dic, "password");
            user.IDCard = Tools.GetValueFromDic(dic, "CardNo");
            user.Grp = Tools.GetValueFromDic(dic, "Group");
            string time = Tools.GetValueFromDic(dic, "StartTime");
            try
            {
                DateTime dtStartTime;
                DateTime dtEndTime;
                try
                {
                    dtStartTime = Convert.ToDateTime(time);
                }
                catch (Exception)
                {
                    dtStartTime = Tools.GetDateTimeBySeconds(Tools.TryConvertToInt32(time));
                }
                time = Tools.GetValueFromDic(dic, "EndTime");
                try
                {
                    dtEndTime = Convert.ToDateTime(time);
                }
                catch (Exception)
                {
                    dtEndTime = Tools.GetDateTimeBySeconds(Tools.TryConvertToInt32(time));
                }
                user.StartTime = Tools.GetValueFromDic(dic, "StartTime") == "0" ? "0" : dtStartTime.ToString();
                user.EndTime = Tools.GetValueFromDic(dic, "EndTime") == "0" ? "0" : dtEndTime.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n CreatUserInfo Exception: {ex.Message}");
                user.StartTime = "0";
                user.EndTime = "0";
            }
            user.Disable = Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "Disable"));
            user.Verify = Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "Verify"));
            return user;
        }
        #endregion

        #region resolve data of table=errorlog
        //处理table=ERRORLOG的数据
        private void Errorlog(string sBuffer)
        {
            Logger.LogError($"\n Errorlog: Bat dau---sBuffer: {sBuffer}");
            string machineSN = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];

            int errorindex = sBuffer.IndexOfEx("\r\n\r\n", 1);
            string errorstr = sBuffer.Substring(errorindex + 4);

            ErrorLogProcess(errorstr, SN);
        }

        private void ErrorLogProcess(string datastr, string SN)
        {
            Logger.LogError($"\n ErrorLogProcess: Bat dau---datastr: {datastr}---SN: {SN}");
            try
            {
                string[] strlist = datastr.Split('\n');
                foreach (string i in strlist)
                {
                    if (string.IsNullOrEmpty(i))
                        continue;
                    SaveErrorLog(i.ToString(), SN);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n ErrorLogProcess Exception: {ex.Message}");
                if (OnError != null)
                {
                    OnError(ex.Message);
                }
            }
        }
        /// <summary>
        /// save ErrorLog
        /// </summary>
        /// <param name="erlog"></param>
        private void SaveErrorLog(string erlog, string machineSN)
        {
            Logger.LogError($"\n SaveErrorLog: Bat dau---erlog: {erlog}---machineSN: {machineSN}");
            if (OnNewErrorLog != null)
            {
                OnNewErrorLog(CreateErrorlog(erlog, machineSN));
            }
        }
        //将设备传来的字符串分割为各个字段之后再封装为一个操作记录的对象
        private ErrorLogModel CreateErrorlog(string erlog, string machineSN)
        {
            Logger.LogError($"\n CreateErrorlog: Bat dau---erlog: {erlog}---machineSN: {machineSN}");
            erlog = Tools.Replace(erlog, "ERRORLOG", "");
            Dictionary<string, string> dic = Tools.GetKeyValues(erlog);

            ErrorLogModel erlogModel = new ErrorLogModel();
            erlogModel.ErrCode = Tools.GetValueFromDic(dic, "ErrCode");
            erlogModel.ErrMsg = Tools.GetValueFromDic(dic, "ErrMsg");
            erlogModel.DataOrigin = Tools.GetValueFromDic(dic, "DataOrigin");
            erlogModel.CmdId = Tools.GetValueFromDic(dic, "CmdId");
            erlogModel.Additional = Tools.GetValueFromDic(dic, "Additional");
            erlogModel.DeviceID = machineSN;

            return erlogModel;
        }
       
        private void GetQueryData(string sBuffer, string tableName, ref string strReply, ref int count)
        {
            Logger.LogError($"\n GetQueryData: Bat dau---tableName: {tableName}");
            switch (tableName)
            {
                case "user":
                    UserInfo(sBuffer);
                    break;
                case "transaction":
                    RealTimeLog(sBuffer, ref count);
                    break;
                default:
                    break;
            }
        }


        #endregion

        #region resolve data of table=rtlog

        /// <summary>
        /// Parse RealTimeLog for acc Device
        /// </summary>
        /// <param name="sBuffer"></param>
        private void RealTimeLog(string sBuffer, ref int count)
        {
            Logger.LogError($"\n RealTimeLog: Bat dau---sBuffer: {sBuffer}");
            string machineSN = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];

            int attindex = sBuffer.IndexOfEx("\r\n\r\n", 1);
            string attstr = sBuffer.Substring(attindex + 4);
            string token = GetValueByNameInPushHeader(sBuffer, "token");
            DeviceModel device = DeviceBll.Get(SN);
            if (device == null)
                return;

            RealTimeLogProcess(attstr, SN, ref count);
        }


        private void RealTimeLogProcess(string attstr, string machineSN, ref int count)
        {
            Logger.LogError($"\n RealTimeLogProcess: Bat dau---attstr: {attstr}");
            try
            {
                string[] strlist = attstr.Split('\n', '\r');
                Logger.LogError($"\n RealTimeLogProcess: Split xong---strlist: {Converter.JsonSerialize(strlist)}");
                count = 0;
                foreach (string i in strlist)
                {
                    if (string.IsNullOrEmpty(i))
                        continue;
                    count++;
                    SaveRealTimeLog(i.ToString(), machineSN);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n RealTimeLogProcess Exception: {ex.Message}");
                if (OnError != null)
                {
                    OnError(ex.Message);
                }
            }
        }
        /// <summary>
        /// save RealTimeLog
        /// </summary>
        /// <param name="realtimelog"></param>
        private void SaveRealTimeLog(string realtimelog, string machineSN)
        {
            Logger.LogError($"\n SaveRealTimeLog: Bat dau---realtimelog: {realtimelog}");
            if (OnNewRealTimeLog != null)
            {
                bool isTanrsaction = realtimelog.Contains("transaction");
                RealTimeLogModel realTimeLogModel = isTanrsaction ? CreateTransactionlog(realtimelog, machineSN) : CreateRealTimelog(realtimelog, machineSN);
                OnNewRealTimeLog(realTimeLogModel);
            }
        }
        //封装实时事件信息
        private RealTimeLogModel CreateRealTimelog(string realtimelog, string machineSN)
        {
            Logger.LogError($"\n CreateRealTimelog: Bat dau---realtimelog: {realtimelog}");
            realtimelog = realtimelog.TrimEnd('\t');
            string[] realtimelogstr = realtimelog.Split('\t');
            Logger.LogError($"\n CreateRealTimelog: Split xong---strlist: {Converter.JsonSerialize(realtimelogstr)}");
            for (int i = 0; i < realtimelogstr.Length; i++)
            {
                realtimelogstr[i] = realtimelogstr[i].Split('=')[1];
            }
            RealTimeLogModel realTimeLog = new RealTimeLogModel();


            realTimeLog.Time = Convert.ToDateTime(realtimelogstr[0]);
            realTimeLog.Pin = realtimelogstr[1];
            realTimeLog.CardNo = realtimelogstr[2];
            realTimeLog.EventAddr = Tools.TryConvertToInt32(realtimelogstr[3]);
            realTimeLog.Event = Tools.TryConvertToInt32(realtimelogstr[4]);
            realTimeLog.InOutStatus = Tools.TryConvertToInt32(realtimelogstr[5]);
            realTimeLog.VerifyType = Tools.TryConvertToInt32(realtimelogstr[6]);
            realTimeLog.DevIndex = Tools.TryConvertToInt32(realtimelogstr[7]);
            realTimeLog.DevSN = machineSN;
            if (realtimelogstr.Length > 9)
            {
                bool isConvert = int.TryParse(realtimelogstr[realtimelogstr.Length - 2], out int nResult);
                if (isConvert)
                    realTimeLog.MaskFlag = nResult;
                realTimeLog.Temperature = realtimelogstr[realtimelogstr.Length - 1];
            }
            return realTimeLog;
        }
        private RealTimeLogModel CreateTransactionlog(string realtimelog, string machineSN)
        {
            Logger.LogError($"\n CreateTransactionlog: Bat dau----realtimelog: {realtimelog}----machineSN: {machineSN}");
            realtimelog = realtimelog.TrimEnd('\t').Replace("transaction", "");

            RealTimeLogModel realTimeLog = new RealTimeLogModel();
            Dictionary<string, string> dic = Tools.GetKeyValues(realtimelog);
            realTimeLog.Time = Tools.GetDateTimeBySeconds(Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "time_second")));
            realTimeLog.Pin = Tools.GetValueFromDic(dic, "pin");
            realTimeLog.CardNo = Tools.GetValueFromDic(dic, "cardno");
            realTimeLog.EventAddr = Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "doorid"));
            realTimeLog.Event = Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "eventtype"));
            realTimeLog.InOutStatus = Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "inoutstate"));
            realTimeLog.VerifyType = Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "verified"));
            realTimeLog.DevIndex = Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "index"));
            realTimeLog.DevSN = machineSN;
            if (dic.Count > 9)
            {
                bool isConvert = int.TryParse(Tools.GetValueFromDic(dic, "maskflag"), out int nResult);
                if (isConvert)
                    realTimeLog.MaskFlag = nResult;
                realTimeLog.Temperature = Tools.GetValueFromDic(dic, "temperature");
            }
            return realTimeLog;
        }
        #endregion

        #region resolve data of table=rtstate
        /// <summary>
        /// Parse RealTimeState for acc Device
        /// </summary>
        /// <param name="sBuffer"></param>
        private void RealTimeState(string sBuffer)
        {
            Logger.LogError($"\n RealTimeState: Bat dau---sBuffer: {sBuffer}");
            string machineSN = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];

            int attindex = sBuffer.IndexOfEx("\r\n\r\n", 1);
            string attstr = sBuffer.Substring(attindex + 4);
            string token = GetValueByNameInPushHeader(sBuffer, "token");
            DeviceModel device = DeviceBll.Get(SN);
            if (device == null)
                return;

            RealTimeStateProcess(attstr, SN);
        }


        private void RealTimeStateProcess(string attstr, string machineSN)
        {
            Logger.LogError($"\n RealTimeStateProcess: Bat dau---attstr: {attstr}");
            try
            {
                string[] strlist = attstr.Split('\n', '\r');
                foreach (string i in strlist)
                {
                    if (string.IsNullOrEmpty(i))
                        continue;
                    SaveRealTimeState(i.ToString(), machineSN);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n RealTimeStateProcess Exception: {ex.Message}");
                if (OnError != null)
                {
                    OnError(ex.Message);
                }
            }
        }

        //封装实时状态信息
        private RealTimeStateModel CreateRealTimeState(string realStateLog, string machineSN)
        {
            Logger.LogError($"\n CreateRealTimeState: Bat dau---realStateLog: {realStateLog}");
            realStateLog = realStateLog.TrimEnd('\t');
            string[] realtimestatestr = realStateLog.Split('\t');
            for (int i = 0; i < realtimestatestr.Length; i++)
            {
                realtimestatestr[i] = realtimestatestr[i].Split('=')[1];
            }
            RealTimeStateModel realTimeState = new RealTimeStateModel();
            realTimeState.DeviceSN = machineSN;
            realTimeState.Time = realtimestatestr[0];
            realTimeState.Sensor = realtimestatestr[1];
            realTimeState.Relay = realtimestatestr[2];
            realTimeState.Alarm = realtimestatestr[3];

            return realTimeState;
        }

        /// <summary>
        /// save RealTimeState
        /// </summary>
        /// <param name="realtimelog"></param>
        private void SaveRealTimeState(string realtimelog, string machineSN)
        {
            Logger.LogError($"\n SaveRealTimeState: Bat dau---realtimelog: {realtimelog}");
            if (OnNewRealTimeState != null)
            {
                OnNewRealTimeState(CreateRealTimeState(realtimelog, machineSN));
            }
        }
        #endregion

    }
}
