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
using System.Threading;
using System.Windows.Forms;

namespace BioMetrixCore.Access
{
    /// <summary>Server Listen
    /// </summary>
    public class ListenClient
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MD5_CTX
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] state;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] buffer;
        };
        [DllImport("libmd5.dll", EntryPoint = "MD5_init", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void MD5_init(ref MD5_CTX context);

        [DllImport("libmd5.dll", EntryPoint = "MD5_update", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void MD5_update(ref MD5_CTX context, string input, uint inputLen);
        [DllImport("libmd5.dll", EntryPoint = "MD5_fini", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void MD5_fini(ref byte digest, ref MD5_CTX context);
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
            if (string.IsNullOrWhiteSpace(strReceive))
                return;
            if (null != OnReceiveDataEvent)
            {
                OnReceiveDataEvent(strReceive);
            }

            //是否为未知消息
            bool unknownMsg = false;
            try
            {
                if (strReceive.Substring(0, 3).ToUpper() == "GET")
                {
                    if (strReceive.IndexOfEx("cdata?") > 0 && strReceive.IndexOfEx("options=all", 0) > 0)
                    {//设备第一次请求
                        DevFirstRequest(strReceive, endsocket);
                    }
                    else if (strReceive.IndexOfEx("rtdata?") > 0 && strReceive.IndexOfEx("type=time", 0) > 0)
                    {
                        DevGetDateTime(endsocket);
                    }
                    else if (strReceive.IndexOfEx("getrequest?") > 0)
                    {//设备下载缓存命令
                        DevGetRequest(bReceive, endsocket);
                    }
                    else if (strReceive.IndexOfEx("ping?") > 0)
                    {
                        SendDataToDevice("200 OK", "OK\r\n", ref endsocket);
                    }
                    else if (strReceive.IndexOfEx("/iclock/file") > 0)
                    {
                        DevUpgrade(bReceive, endsocket);
                    }
                    else if (strReceive.IndexOfEx("push?") > 0)
                    {//设备下载配置参数
                        DevGetParam(strReceive, endsocket);
                    }
                    else
                    {
                        unknownMsg = true;
                    }
                }
                else if (strReceive.Substring(0, 4).ToUpper() == "POST")
                {
                    if (strReceive.IndexOfEx("cdata?") > 0 || strReceive.IndexOfEx("querydata?") > 0)
                    {//设备发送数据
                        DevSendData(bReceive, endsocket);
                    }
                    else if (strReceive.IndexOfEx("registry?") > 0)
                    {//设备注册
                        DevRegistry(strReceive, endsocket);
                    }
                    else if (strReceive.IndexOfEx("push?") > 0)
                    {//设备下载配置参数
                        DevGetParam(strReceive, endsocket);
                    }
                    else if (strReceive.IndexOfEx("devicecmd?") > 0)
                    {//设备返回命令结果
                        DevCmdProcess(bReceive, endsocket);
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
        private void CreateSyncTimeCmd(string DeviceSN)
        {
            Logger.LogError($"\n CreateSyncTimeCmd: Bat dau");
            DeviceCmdModel deviceCmdModel = new DeviceCmdModel();
            deviceCmdModel.CommitTime = DateTime.Now;
            deviceCmdModel.Content = string.Format(Commands.Command_SetOptioins, $" MachineTZ = {Tools.GetServerTZ()}");
            deviceCmdModel.DevSN = DeviceSN;
            DeviceCmdBll deviceCmdBll = new DeviceCmdBll();
            deviceCmdBll.Add(deviceCmdModel);
            deviceCmdModel = new DeviceCmdModel();
            deviceCmdModel.CommitTime = DateTime.Now;
            deviceCmdModel.Content = string.Format(Commands.Command_SetOptioins, $"DateTime={Tools.OldEncodeTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second)}");
            deviceCmdModel.DevSN = DeviceSN;
            deviceCmdBll.Add(deviceCmdModel);

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
        /// 设备注册
        /// </summary>
        /// <param name="strReceive"></param>
        /// <param name="remoteSocket"></param>
        private void DevRegistry(string strReceive, Socket remoteSocket)
        {
            Logger.LogError($"\n DevRegistry: Bat dau");
            string devSN = GetValueByNameInPushHeader(strReceive, "SN");
            DeviceModel device = DeviceBll.Get(devSN);
            int attindex = strReceive.IndexOfEx("\r\n\r\n", 1);
            string attstr = strReceive.Substring(attindex + 4);
            string registryCode = string.Empty;
            if (null == device)
            {
                //未注册
                DeviceModel newDevice = new DeviceModel();
                newDevice.RegistryCode = GetRegistryCode();
                newDevice.DeviceSN = devSN;
                FormatBioData(ref attstr);
                Tools.InitModel(newDevice, attstr);
                if (TempDeviceModelList.Exists(x => x.DeviceSN == devSN) == false)
                    TempDeviceModelList.Add(newDevice);

                //if (DeviceBll.Add(newDevice) > 0)
                //    OnNewMachine?.Invoke(newDevice);
            }
            else
            {
                registryCode = device.RegistryCode;

                if (string.IsNullOrEmpty(registryCode))
                {
                    //must have registrycode item when add manually
                    return;
                }
                //UpdateDeivce(device, attstr, devSN);
                FormatBioData(ref attstr);
                Tools.InitModel(device, attstr);
                DeviceBll.Update(device);
            }


            string sessionID = GetSessionID();
            //更新数据库设备session
            DeviceBll.UpdateSession(sessionID, devSN);
            string strCode = string.IsNullOrEmpty(registryCode) ? "406 Not acceptable" : "200 OK";
            string strReply = string.IsNullOrEmpty(registryCode) ? "406" : string.Format("RegistryCode={0}", registryCode);
            byte[] bData = _encoding.GetBytes(strReply);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("HTTP/1.1 {0}\r\n", strCode);
            sb.AppendFormat("Server: C#demo/1.1\r\n");
            sb.AppendFormat("Set-Cookie: JSESSIONID={0}; Path=/; HttpOnly\r\n", sessionID);
            sb.AppendFormat("Content-Type: text/plain;charset=UTF-8\r\n");
            sb.AppendFormat("Date: {0}\r\n", Tools.GetDateTimeNow().ToUniversalTime().ToString("r"));
            sb.AppendFormat("Content-Length: {0}\r\n\r\n", bData.Length);

            byte[] header = _encoding.GetBytes(sb.ToString());
            byte[] buffer = new byte[bData.Length + header.Length];
            header.CopyTo(buffer, 0);
            bData.CopyTo(buffer, header.Length);
            SendToBrowser(buffer, ref remoteSocket);

        }

        private void UpdateDeivce(DeviceModel device, string attstr, string devSN)
        {
            try
            {
                string[] strlist = attstr.Split(',');
                string[] arr;
                device.DeviceSN = devSN;
                device.Stamp = Tools.GetDateTimeNowString();
                foreach (var item in strlist)
                {
                    arr = new string[2];
                    arr = item.Split('=');
                    if (arr.Length < 1)
                        continue;
                    if (arr[0].Equals("~DeviceName"))
                        device.DeviceName = arr[1];
                    else if (arr[0].Equals("DeviceType"))
                        device.DeviceType = arr[1];
                    else if (arr[0].Equals("FirmVer"))
                        device.FirmVer = arr[1];
                    else if (arr[0].Equals("PushVersion"))
                        device.PushVersion = arr[1];
                    else if (arr[0].Equals("IPAddress"))
                        device.IPAddress = arr[1];
                    else if (arr[0].Equals("MaxPackageSize"))
                        device.MaxPackageSize = arr[1];
                }


            }
            catch (Exception ex)
            {
                Logger.LogError($"\nUpdateDeivce Exception: {ex.Message}");
                if (OnError != null)
                {
                    OnError(ex.Message);
                }
            }
        }
        /// <summary>
        /// 设备下载配置参数
        /// </summary>
        /// <param name="strReceive"></param>
        /// <param name="remoteSocket"></param>
        private void DevGetParam(string strReceive, Socket remoteSocket)
        {
            Logger.LogError($"\n DevGetParam: Bat dau");
            string devSN = GetValueByNameInPushHeader(strReceive, "SN");
            DeviceModel device = DeviceBll.Get(devSN);
            if (null == device)
            {//未注册

                return;
            }

            string timestamp = GetValueByNameInPushHeader(strReceive, "timestamp");
            string token = GetValueByNameInPushHeader(strReceive, "token");

            try
            {
                if (timestamp != null)
                {
                    if (0 != MD5Verify(token, new List<string>() { device.RegistryCode, device.DeviceSN, timestamp }))
                    {
                        //MD5 check failed
                        //MessageBox.Show("MD5 check failed!");
                        return;
                    }
                }

            }
            catch (Exception e)
            {
                Logger.LogError($"\n DevGetParam Exception: {e.Message}");
                MessageBox.Show("MD5 call failed:" + e.Message + e.Source + "\n" + e.StackTrace);
                return;
            }
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("ServerVersion={0}\n", ServerInfo.ServerVersion);
            sb.AppendFormat("ServerName={0}\n", ServerInfo.ServerName);
            sb.AppendFormat("PushVersion={0}\n", ServerInfo.PushProtVer);
            sb.AppendFormat("ErrorDelay={0}\n", device.ErrorDelay);
            sb.AppendFormat("RequestDelay={0}\n", device.RequestDelay);
            sb.AppendFormat("TransTimes={0}\n", device.TransTimes);
            sb.AppendFormat("TransInterval={0}\n", device.TransInterval);
            sb.AppendFormat("TransTables={0}\n", device.TransTables);
            sb.AppendFormat("Realtime={0}\n", device.Realtime);
            sb.AppendFormat("SessionID={0}\n", device.SessionID);
            sb.AppendFormat("TimeoutSec={0}\n", device.TimeoutSec);
            //sb.AppendFormat("BioPhotoFun={0}\r\n", device.BioPhotoFun);
            //sb.AppendFormat("BioDataFun={0}\r\n", device.BioDataFun);
            sb.AppendFormat("MultiBioDataSupport={0}\r\n", "1:1:1:1:1:1:1:1:1:1");
            sb.AppendFormat("MultiBioPhotoSupport={0}\r\n", "1:1:1:1:1:1:1:1:1:1");

            string strReply = sb.ToString();
            SendDataToDevice("200 OK", strReply, ref remoteSocket);
            CreateSyncTimeCmd(devSN);
            return;
        }
        private int MD5Verify(string token, List<string> verifyList)
        {
            Logger.LogError($"\n MD5Verify: Bat dau");
            byte[] digest = new byte[16];
            MD5_CTX md5c = new MD5_CTX();
            MD5_init(ref md5c);
            foreach (var item in verifyList)
            {
                MD5_update(ref md5c, item, (uint)item.Length);
            }

            MD5_fini(ref digest[0], ref md5c);
            string chk_token = Tools.ToHexString(digest);
            return String.Compare(chk_token, token, true);

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
                replyString = DeviceCmdBll.Send(SN, (device.MachineType == "10" || device.MachineType == "9" || device.MachineType == "8") ? true : false);
                replyString = replyString + "\r\n";
            }
            SendDataToDevice(ReplyCode, replyString, ref remoteSocket);

        }
        private void DevUpgrade(byte[] bReceive, Socket remoteSocket)
        {
            Logger.LogError($"\n DevUpgrade: Bat dau");
            string sBuffer = _encoding.GetString(bReceive);
            string url = GetValueByNameInPushHeader(sBuffer, "url");
            string fileName = url.Replace($"http*//localhost*{port}/", "");

            FileInfo finfo = new FileInfo(Application.StartupPath + $@"\Upload\{fileName}");
            int fileSize = (int)finfo.Length;

            byte[] fileData = new byte[fileSize];
            FileStream fs = new FileStream(Application.StartupPath + $@"\Upload\{fileName}", FileMode.Open);
            fs.Read(fileData, 0, fileSize);
            fs.Close();
            string base64Str = EncryptString(fileData);

            string replyString = base64Str;

            string ReplyCode = "200 OK";

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

            string strReply = "OK";

            //options 推送配置信息
            if (strReceive.IndexOfEx("table=options", 1) > 0) // Upload options Info
            {
                Options(strReceive);
            }
            else if (strReceive.IndexOfEx("table=rtlog", 1) > 0)
            {//上传实时事件
                int count = 0;
                RealTimeLog(strReceive, ref count);
            }
            else if (strReceive.IndexOfEx("table=rtstate", 1) > 0)
            {//上传实时状态
                RealTimeState(strReceive);
            }
            else if (strReceive.IndexOfEx("table=tabledata&tablename=userpic", 1) > 0)
            {//上传用户照片
                string count = GetValueByNameInPushHeader(strReceive, "count");
                strReply = "userpic=" + count;
                UserPicLog(strReceive);
            }
            else if (strReceive.IndexOfEx("table=tabledata&tablename=user", 1) > 0)
            {//上传用户信息
                string count = GetValueByNameInPushHeader(strReceive, "count");
                strReply = "user=" + count;
                UserInfo(strReceive);
            }
            else if (strReceive.IndexOfEx("table=tabledata&tablename=face", 1) > 0)
            {//上传人脸模板
                string count = GetValueByNameInPushHeader(strReceive, "count");
                string tableName = GetValueByNameInPushHeader(strReceive, "tablename");
                strReply = tableName + "=" + count;
                FacePrint(strReceive);
            }
            else if (strReceive.IndexOfEx("table=tabledata&tablename=template", 1) > 0)
            {//上传指纹模板
                string count = GetValueByNameInPushHeader(strReceive, "count");
                string tableName = GetValueByNameInPushHeader(strReceive, "tablename");
                strReply = tableName + "=" + count;
                FingerPrint(strReceive);
            }
            else if (strReceive.IndexOfEx("table=tabledata&tablename=biophoto", 1) > 0)
            {//上传比对照片
                string count = GetValueByNameInPushHeader(strReceive, "count");
                strReply = "biophoto=" + count;
                BioPhoto(strReceive);
            }
            else if (strReceive.IndexOfEx("table=tabledata&tablename=ATTPHOTO", 1) > 0)
            {//上传抓拍照片
                string count = GetValueByNameInPushHeader(strReceive, "count");
                strReply = "ATTPHOTO=" + count;
                AttPhoto(bReceive);
            }

            else if (strReceive.IndexOfEx("table=tabledata&tablename=biodata", 1) > 0)
            {//上传一体化模板
                string count = GetValueByNameInPushHeader(strReceive, "count");
                strReply = "biodata=" + count;
                BioData(strReceive);
            }
            else if (strReceive.IndexOfEx("table=tabledata&tablename=errorlog", 1) > 0)
            {//上传异常日志
                Errorlog(strReceive);
            }
            else if (strReceive.IndexOfEx("AuthType=device", 1) > 0)
            {//后台验证
                RemoteVerify(strReceive, ref strReply);

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
        /// Response Device cmd request for iclock Devices
        /// </summary>
        /// <param name="bReceive"></param>
        private void DevCmdProcess(byte[] bReceive, Socket remoteSocket)
        {
            Logger.LogError($"\n DevCmdProcess: Bat dau");
            string strReceive = Encoding.UTF8.GetString(bReceive).TrimEnd('\0');

            int index = strReceive.IndexOfEx("\r\n\r\n", 1);
            string strContent = strReceive.Substring(index + 4);
            SendDataToDevice("200 OK", "OK", ref remoteSocket);
            index = strContent.IndexOfEx("ID=");
            _deviceCmdBll.Update(strContent.Substring(index));





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

        DeviceCmdBll _deviceCmdBll = null;
        public DeviceCmdBll DeviceCmdBll
        {
            get
            {
                if (_deviceCmdBll == null)
                    _deviceCmdBll = new DeviceCmdBll();
                return _deviceCmdBll;
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
                    Logger.LogError($"\n SendToBrowser-----OnSendDataEvent: Bat dau---bSendData: {bSendData}");
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

        #region resolve data of table=biophoto
        private void BioPhoto(string bioPhoto)
        {
            Logger.LogError($"\n BioPhoto: Bat dau");
            string machineSN = bioPhoto.Substring(bioPhoto.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];
            string token = GetValueByNameInPushHeader(bioPhoto, "token");
            DeviceModel device = DeviceBll.Get(SN);
            if (device == null)
                return;
            try
            {

                if (0 != MD5Verify(token, new List<string>() { device.RegistryCode, device.DeviceSN, device.SessionID }))
                {
                    //MD5 check failed
                    //MessageBox.Show("MD5 check failed!");
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"\n BioPhoto Exception:{e.Message}");
                MessageBox.Show("MD5 call failed:" + e.Message + e.Source + "\n" + e.StackTrace);
                return;
            }
            SaveBioPhoto(bioPhoto);
        }
        private void SaveBioPhoto(string bioPhoto)
        {
            Logger.LogError($"\n SaveBioPhoto: Bat dau");
            if (bioPhoto.IndexOfEx("PIN") >= 0)
            {
                if (OnNewBioPhoto != null)
                {
                    OnNewBioPhoto(CreatBioPhoto(bioPhoto));
                }
            }
        }
        //封装比对照片
        private List<TmpBioPhotoModel> CreatBioPhoto(string bioPhotoString)
        {
            Logger.LogError($"\n CreatBioPhoto: Bat dau");
            List<TmpBioPhotoModel> modelList = new List<TmpBioPhotoModel>();

            int index = bioPhotoString.IndexOfEx("\r\n\r\n", 1);
            string template = bioPhotoString.Substring(index + 4);
            template = Tools.Replace(template, "biophoto", "");
            string[] arr = template.Split('\n');
            Dictionary<string, string> dic = null;
            foreach (var item in arr)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                dic = Tools.GetKeyValues(item);

                TmpBioPhotoModel biophoto = new TmpBioPhotoModel();
                biophoto.Pin = Tools.GetValueFromDic(dic, "PIN");
                biophoto.FileName = Tools.GetValueFromDic(dic, "FileName");
                biophoto.Type = Tools.GetValueFromDic(dic, "Type");
                biophoto.Size = Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "Size"));
                biophoto.Content = Tools.GetValueFromDic(dic, "Content");
                modelList.Add(biophoto);
            }
            return modelList;
        }
        #endregion
        #region resolve data of table=templatev10
        private void FingerPrint(string fingerPrint)
        {
            Logger.LogError($"\n FingerPrint: Bat dau");
            string machineSN = fingerPrint.Substring(fingerPrint.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];
            string token = GetValueByNameInPushHeader(fingerPrint, "token");
            DeviceModel device = DeviceBll.Get(SN);
            if (device == null)
                return;
            try
            {

                if (0 != MD5Verify(token, new List<string>() { device.RegistryCode, device.DeviceSN, device.SessionID }))
                {
                    //MD5 check failed
                    //MessageBox.Show("MD5 check failed!");
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"\n FingerPrint Exception:{e.Message}");
                MessageBox.Show("MD5 call failed:" + e.Message + e.Source + "\n" + e.StackTrace);
                return;
            }
            SaveFP(fingerPrint, SN, false);
        }
        /// <summary>
        /// save  FP
        /// </summary>
        /// <param name="enfplog">指纹数据</param>
        /// <param name="SN">设备序列号</param>
        /// <param name="isBioData">是否一体化模板</param>
        private void SaveFP(string enfplog, string SN, bool isBioData)
        {
            Logger.LogError($"\n SaveFP: Bat dau");
            if (enfplog.IndexOfEx("PIN", 0, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (OnNewFP != null)
                {
                    OnNewFP(CreatFP(enfplog, SN, isBioData));
                }
            }
        }
        //封装通用指纹信息
        private List<TmpFPModel> CreatFP(string enfplog, string SN, bool isBioData)
        {
            Logger.LogError($"\n CreatFP: Bat dau");
            List<TmpFPModel> templateModelList = new List<TmpFPModel>();
            int index = enfplog.IndexOfEx("\r\n\r\n", 1);
            TmpFPModel templateModel;

            string template = enfplog.Substring(index + 4);
            Dictionary<string, string> dic = null;
            if (isBioData)
            {
                string[] arr = template.Split('\n');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (string.IsNullOrEmpty(arr[i]))
                        continue;
                    arr[i] = Tools.Replace(arr[i], "BIODATA", "");
                    dic = Tools.GetKeyValues(arr[i]);
                    templateModel = new TmpFPModel();
                    templateModel.Pin = Tools.GetValueFromDic(dic, "PIN");
                    templateModel.Fid = Tools.GetValueFromDic(dic, "No");
                    templateModel.Valid = Tools.GetValueFromDic(dic, "Valid");
                    templateModel.Duress = Tools.GetValueFromDic(dic, "Duress");
                    string MajorVer = Tools.GetValueFromDic(dic, "MajorVer");

                    //从数据库中获取此设备支持的指纹版本号
                    if (string.IsNullOrEmpty(MajorVer))
                    {
                        DeviceModel model = _deviceBll.Get(SN);
                        if (null != model)
                        {
                            MajorVer = model.GetBioVersion(BioType.FingerPrint).Split('.')[0];
                        }
                    }
                    templateModel.MajorVer = MajorVer;
                    templateModel.Tmp = Tools.GetValueFromDic(dic, "TMP");
                    templateModelList.Add(templateModel);
                }

            }
            else
            {
                string Ver = string.Empty;
                string[] arr = template.Split('\n');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (string.IsNullOrEmpty(arr[i]))
                        continue;
                    templateModel = new TmpFPModel();
                    if (arr[i].IndexOfEx("templatev10") >= 0)
                    {
                        arr[i] = Tools.Replace(arr[i], "templatev10", "");
                        Ver = "10";
                    }
                    else
                    {
                        arr[i] = Tools.Replace(arr[i], "templatev09", "");
                        Ver = "9";
                    }
                    if (string.IsNullOrEmpty(arr[i]))
                        continue;
                    dic = Tools.GetKeyValues(arr[i]);
                    templateModel.Pin = Tools.GetValueFromDic(dic, "pin");
                    templateModel.Fid = Tools.GetValueFromDic(dic, "fingerid");
                    templateModel.Size = Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "size"));
                    templateModel.Valid = Tools.GetValueFromDic(dic, "valid");
                    templateModel.Tmp = Tools.GetValueFromDic(dic, "template");
                    templateModel.Duress = Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "valid")) > 15 ? "1" : "0";
                    templateModel.MajorVer = Ver;
                    templateModelList.Add(templateModel);
                }
            }

            return templateModelList;
        }
        #endregion
        #region resolve data of face
        private void FacePrint(string facePrint)
        {
            Logger.LogError($"\n FacePrint: Bat dau");
            string machineSN = facePrint.Substring(facePrint.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];
            string token = GetValueByNameInPushHeader(facePrint, "token");
            DeviceModel device = DeviceBll.Get(SN);
            if (device == null)
                return;
            try
            {

                if (0 != MD5Verify(token, new List<string>() { device.RegistryCode, device.DeviceSN, device.SessionID }))
                {
                    //MD5 check failed
                    MessageBox.Show("MD5 check failed!");
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"\n FacePrint Exception: {e.Message}");
                MessageBox.Show("MD5 call failed:" + e.Message + e.Source + "\n" + e.StackTrace);
                return;
            }
            SaveFace(facePrint, false);
        }
        /// <summary>
        /// save face
        /// </summary>
        /// <param name="enfacelog">面部数据</param>
        /// <param name="isBioData">是否一体化模板</param>
        private void SaveFace(string enfacelog, bool isBioData)
        {
            Logger.LogError($"\n SaveFace: Bat dau");
            if (enfacelog.IndexOfEx("PIN", 0, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (OnNewFace != null)
                {
                    OnNewFace(CreatFace(enfacelog, isBioData));
                }
            }
        }
        //封装Face模板信息
        private List<TmpFaceModel> CreatFace(string template, bool isBioData)
        {
            Logger.LogError($"\n CreatFace: Bat dau");
            List<TmpFaceModel> tmpFaceList = new List<TmpFaceModel>();
            int index = template.IndexOfEx("\r\n\r\n", 1);
            template = template.Substring(index + 4);
            Dictionary<string, string> dic = null;
            TmpFaceModel tmpFaceModel;
            if (isBioData)
            {
                string[] arr = template.Split('\n');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (string.IsNullOrEmpty(arr[i]))
                        continue;
                    arr[i] = Tools.Replace(arr[i], "BIODATA", "");
                    dic = Tools.GetKeyValues(arr[i]);
                    tmpFaceModel = new TmpFaceModel();
                    tmpFaceModel.Pin = Tools.GetValueFromDic(dic, "PIN");
                    tmpFaceModel.Fid = Tools.GetValueFromDic(dic, "No");
                    tmpFaceModel.Valid = Tools.GetValueFromDic(dic, "Valid");
                    string MajorVer = Tools.GetValueFromDic(dic, "MajorVer");
                    string MinorVer = Tools.GetValueFromDic(dic, "MinorVer");
                    tmpFaceModel.Ver = $"{MajorVer}.{MinorVer}";
                    tmpFaceModel.Tmp = Tools.GetValueFromDic(dic, "TMP");
                    tmpFaceList.Add(tmpFaceModel);
                }

            }
            else
            {
                string[] arr = template.Split('\n');
                string[] arrTmp;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (string.IsNullOrEmpty(arr[i]))
                        continue;
                    arrTmp = arr[i].Split(' ');

                    dic = Tools.GetKeyValues(arrTmp[1]);
                    tmpFaceModel = new TmpFaceModel
                    {
                        Pin = Tools.GetValueFromDic(dic, "PIN"),
                        Fid = Tools.GetValueFromDic(dic, "FaceID"),
                        Size = Tools.TryConvertToInt32(Tools.GetValueFromDic(dic, "SIZE")),
                        Valid = Tools.GetValueFromDic(dic, "VALID"),
                        Tmp = Tools.GetValueFromDic(dic, "face"),
                        Ver = arrTmp[0]
                    };
                    tmpFaceList.Add(tmpFaceModel);
                }

            }

            return tmpFaceList;
        }
        #endregion
        #region resolve data of palm
        /// <summary>
        /// save Palm
        /// </summary>
        /// <param name="enfacelog">面部数据</param>
        /// <param name="isBioData">是否一体化模板</param>
        private void SavePalm(string enPalmlog)
        {
            Logger.LogError($"\n SavePalm: Bat dau---enPalmlog: {enPalmlog}");
            if (enPalmlog.IndexOfEx("PIN", 0, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (OnNewPalm != null)
                {
                    OnNewPalm(CreateBioTmp(enPalmlog));
                }
            }
        }


        #endregion
        #region resolve data of table=biodata
        TmpBioDataBll _tmpBioDataBll = new TmpBioDataBll();
        private void SaveBioData(string biodataTmp)
        {
            Logger.LogError($"\n SaveBioData: Bat dau---biodataTmp: {biodataTmp}");
            //可见光面部模板，本程序使用比对照片用作下发模板
            TmpBioDataModel tmpBioDataModel = CreateBioTmp(biodataTmp);
            _tmpBioDataBll.Add(tmpBioDataModel);
        }
        //封装掌一体化模板信息
        private TmpBioDataModel CreateBioTmp(string template)
        {
            Logger.LogError($"\n CreateBioTmp: Bat dau---template: {template}");
            template = Tools.Replace(template, "BIODATA", "");
            Dictionary<string, string> dic = Tools.GetKeyValues(template);

            TmpBioDataModel tmpBio = new TmpBioDataModel();
            tmpBio.Pin = Tools.GetValueFromDic(dic, "PIN");
            tmpBio.No = Tools.GetValueFromDic(dic, "No");
            tmpBio.Index = Tools.GetValueFromDic(dic, "Index");
            tmpBio.Valid = Tools.GetValueFromDic(dic, "Valid");
            tmpBio.Duress = Tools.GetValueFromDic(dic, "Duress");
            tmpBio.Type = Tools.GetValueFromDic(dic, "Type");
            tmpBio.MajorVer = Tools.GetValueFromDic(dic, "MajorVer");
            tmpBio.MinorVer = Tools.GetValueFromDic(dic, "MinorVer");
            tmpBio.Format = Tools.GetValueFromDic(dic, "Format");
            tmpBio.Tmp = Tools.GetValueFromDic(dic, "TMP");

            return tmpBio;
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
            try
            {

                if (0 != MD5Verify(token, new List<string>() { device.RegistryCode, device.DeviceSN, device.SessionID }))
                {
                    //MD5 check failed
                    MessageBox.Show("MD5 check failed!");
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"\n UserInfo Exception: {e.Message}");
                MessageBox.Show("MD5 call failed:" + e.Message + e.Source + "\n" + e.StackTrace);
                return;
            }

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

        #region resolve data of table=biodata
        //处理table=BIODATA的数据
        private void BioData(string sBuffer)
        {
            Logger.LogError($"\n BioData: Bat dau--sBuffer: {sBuffer}");
            string machineSN = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];

            int bioindex = sBuffer.IndexOfEx("\r\n\r\n", 1);
            string biostr = sBuffer.Substring(bioindex + 4);

            SeparateBioData(biostr, SN);
        }
        //将table=BIODATA后面的数据进行分解
        private void SeparateBioData(string datastr, string SN)
        {
            Logger.LogError($"\n SeparateBioData: Bat dau---datastr: {datastr}");
            try
            {
                string[] strlist = datastr.Split('\n');
                foreach (string i in strlist)
                {
                    string tmpstr = i.ToString();
                    if (string.IsNullOrEmpty(tmpstr))
                        continue;
                    string bioTypeStr = tmpstr.Split('\t')[5].Split('=')[1];
                    BioType bioType = (BioType)Enum.Parse(typeof(BioType), bioTypeStr);
                    switch (bioType)
                    {
                        case BioType.Comm://通用
                            break;
                        case BioType.FingerPrint://指纹
                            SaveFP(tmpstr, SN, true);
                            break;
                        case BioType.Face://面部
                            SaveFace(tmpstr, true);
                            break;
                        case BioType.VocalPrint://声纹
                            break;
                        case BioType.Iris://虹膜
                            break;
                        case BioType.Retina://视网膜
                            break;
                        case BioType.PalmPrint://掌纹
                            break;
                        case BioType.FingerVein://指静脉
                            break;
                        case BioType.Palm://手掌
                            SavePalm(tmpstr);
                            break;
                        case BioType.VisilightFace://可见光面部
                            SaveBioData(tmpstr);
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n SeparateBioData Exception: {ex.Message}");
                if (OnError != null)
                {
                    OnError(ex.Message);
                }
            }
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
        private void RemoteVerify(string sBuffer, ref string strReply)
        {
            Logger.LogError($"\n RemoteVerify: Bat dau---sBuffer: {sBuffer}---strReply: {strReply}");
            string machineSN = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];

            int index = sBuffer.IndexOfEx("\r\n\r\n", 1);
            string verifystr = sBuffer.Substring(index + 4);

            RemoteVerifyProcess(verifystr, SN, ref strReply);
        }
        UserInfoBll _userInfoBll = new UserInfoBll();

        private void RemoteVerifyProcess(string datastr, string SN, ref string strReply)
        {
            Logger.LogError($"\n RemoteVerifyProcess: Bat dau");
            string Auth = string.Empty;
            try
            {
                string[] strlist = datastr.Split('\t');
                string pin = strlist[1];

                if (null != _userInfoBll.Get(pin))
                    Auth = "SUCCESS";
                else
                    Auth = "FAILED";
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n RemoteVerifyProcess Exception: {ex.Message}");
                Auth = "TIMEOUT";
                if (OnError != null)
                {
                    OnError(ex.Message);
                }
            }
            finally
            {
                strReply = String.Format(Commands.Command_RemoteVerify, Auth, datastr, " 1 1 1 6");
            }

        }
        private void GetQueryData(string sBuffer, string tableName, ref string strReply, ref int count)
        {
            Logger.LogError($"\n GetQueryData: Bat dau---tableName: {tableName}");
            switch (tableName)
            {
                case "user":
                    UserInfo(sBuffer);
                    break;
                case "templatev10":
                    FingerPrint(sBuffer);
                    break;
                case "transaction":
                    RealTimeLog(sBuffer, ref count);
                    break;
                case "biophoto":
                    BioPhoto(sBuffer);
                    break;
                case "biodata":
                    BioData(sBuffer);
                    break;
                default:
                    break;
            }
        }


        private void QueryDataProcess(string datastr, string SN, ref string strReply)
        {
            Logger.LogError($"\n QueryDataProcess: Bat dau");
            string Auth = string.Empty;
            try
            {
                string[] strlist = datastr.Split('\t');
                string pin = strlist[1];

                if (null != _userInfoBll.Get(pin))
                    Auth = "SUCCESS";
                else
                    Auth = "FAILED";
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n QueryDataProcess Exception: {ex.Message}");
                Auth = "TIMEOUT";
                if (OnError != null)
                {
                    OnError(ex.Message);
                }
            }
            finally
            {
                strReply = String.Format(Commands.Command_RemoteVerify, Auth, datastr, " 1 1 1 6");
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
            try
            {

                if (0 != MD5Verify(token, new List<string>() { device.RegistryCode, device.DeviceSN, device.SessionID }))
                {
                    //MD5 check failed
                    //MessageBox.Show("MD5 check failed!");
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"\n RealTimeLog Exception: {e.Message}");
                MessageBox.Show("MD5 call failed:" + e.Message + e.Source + "\n" + e.StackTrace);
                return;
            }

            RealTimeLogProcess(attstr, SN, ref count);
        }


        private void RealTimeLogProcess(string attstr, string machineSN, ref int count)
        {
            Logger.LogError($"\n RealTimeLogProcess: Bat dau---attstr: {attstr}");
            try
            {
                string[] strlist = attstr.Split('\n', '\r');
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
            try
            {

                if (0 != MD5Verify(token, new List<string>() { device.RegistryCode, device.DeviceSN, device.SessionID }))
                {
                    //MD5 check failed
                    //MessageBox.Show("MD5 check failed!");
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"\n RealTimeState Exception: {e.Message}");
                MessageBox.Show("MD5 call failed:" + e.Message + e.Source + "\n" + e.StackTrace);
                return;
            }

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

        #region resolve data of table=ATTPHOTO
        /* save AttPhoto */
        private void AttPhoto(byte[] bReceive)
        {
            Logger.LogError($"\n AttPhoto: Bat dau");
            string strReceive = Encoding.UTF8.GetString(bReceive);
            int index = strReceive.IndexOfEx("\r\n\r\n", 1);
            string photoStr = strReceive.Substring(index + 4);
            string[] arrPhoto = photoStr.Split('\n');
            foreach (var item in arrPhoto)
            {
                string[] tmpstr = item.Split('\t');
                string strImageNumber = "";
                if (tmpstr.Length < 2)
                    continue;
                foreach (string str in tmpstr)
                {
                    if (str.IndexOfEx("PIN=") >= 0)
                    {
                        strImageNumber = str;
                        break;
                    }
                }

                string photo = tmpstr[3].Split('=')[1];
                string dummyData = photo.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                if (dummyData.Length % 4 > 0)
                {
                    dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                }
                byte[] imgReceive = Convert.FromBase64String(dummyData);
                string path = System.Environment.CurrentDirectory + "\\Capture";
                path += "\\" + strImageNumber.Replace("pin=", "");
                System.IO.File.WriteAllBytes(path, imgReceive);
            }


        }
        #endregion

        #region resolve data of table=USERPIC
        public void UserPicLog(string sBuffer)
        {
            Logger.LogError($"\n UserPicLog: Bat dau--sBuffer: {sBuffer}");
            string machineSN = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];

            int usinindex = sBuffer.IndexOfEx("\r\n\r\n", 1);
            string usinstr = sBuffer.Substring(usinindex + 4);
            string token = GetValueByNameInPushHeader(sBuffer, "token");
            DeviceModel device = DeviceBll.Get(SN);
            if (device == null)
                return;
            try
            {

                if (0 != MD5Verify(token, new List<string>() { device.RegistryCode, device.DeviceSN, device.SessionID }))
                {
                    //MD5 check failed
                    MessageBox.Show("MD5 check failed!");
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"\n UserPicLog Exception: {e.Message}");
                MessageBox.Show("MD5 call failed:" + e.Message + e.Source + "\n" + e.StackTrace);
                return;
            }

            userpiclogprocess(usinstr);
        }

        public void userpiclogprocess(string sBuffer)
        {
            Logger.LogError($"\n userpiclogprocess: Bat dau--sBuffer: {sBuffer}");
            try
            {
                int usinindex = sBuffer.IndexOfEx("\n", 1);
                string usinstr = "";

                if (usinindex > 0)
                {
                    usinstr = sBuffer.Substring(0, usinindex);
                }
                saveuserpic(usinstr);
                string endop = sBuffer.Substring(usinindex + 1);
                if (endop != "")
                {
                    userpiclogprocess(endop);
                }
            }
            catch
            {
            }

        }

        private void Base64StringToImage(string txtFileName)
        {
            Logger.LogError($"\n Base64StringToImage: Bat dau--txtFileName: {txtFileName}");
            try
            {
                FileStream ifs = new FileStream(txtFileName, FileMode.Open, FileAccess.Read);
                //System.IO.FileStream ifs = new System.IO.FileStream(txtFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(ifs);

                String inputStr = sr.ReadToEnd();

                byte[] arr = Convert.FromBase64String(inputStr);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);

                string temp = txtFileName.Substring(0, txtFileName.Length - 4);
                bmp.Save(temp + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                ms.Close();
                sr.Close();
                ifs.Close();
                //this.picUser.Image = bmp;
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n Base64StringToImage Exception:{ex.Message}");
                MessageBox.Show("Base64StringToImage 转换失败/nException：" + ex.Message);
            }
        }
        public void saveuserpic(string usinlog)
        {
            Logger.LogError($"\n saveuserpic: Bat dau");
            if (usinlog.IndexOfEx("PIN", 0) > 0 && usinlog.IndexOfEx("FileName", 0) > 0)
            {
                string UsInid = usinlog.Substring(0, usinlog.IndexOfEx("\t", 0));
                string stillusin1 = usinlog.Substring(usinlog.IndexOfEx("\t", 0) + 1);
                string usinnum1 = stillusin1.Substring(0, stillusin1.IndexOfEx("\t", 0));
                string stillusin2 = stillusin1.Substring(stillusin1.IndexOfEx("\t", 0) + 1);
                string usinnum2 = stillusin2.Substring(0, stillusin2.IndexOfEx("\t", 0));
                string stillusin3 = stillusin2.Substring(stillusin2.IndexOfEx("\t", 0) + 1);
                string stillusin4 = stillusin3.Substring(8);
                string path = System.Environment.CurrentDirectory + "\\Photo";
                string name = usinnum1.Replace("filename=", "");
                string name1 = name.Substring(0, name.Length - 4);
                path += "\\" + name1 + ".txt";  //Photo\\
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine(stillusin4);
                sw.Close();
                Base64StringToImage("Photo\\" + name1 + ".txt");
                //this.WriteUserPic(UsInid.Replace("USER PIN=", ""), usinnum1.Replace("FileName=", ""), usinnum2.Replace("Size=", ""));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                /* if (tabOptions.SelectedIndex == 10) // User Info Logs Count
                 {
                     lblRecords.Text = "UserPic Count:";
                     lblCount.Text = lvUserPic.Items.Count.ToString();
                 }*/
                // MessageBox.Show("转换成功！");
            }
        }
        #endregion

        #region table=options
        /// <summary>
        /// Parse Options for Device
        /// </summary>
        /// <param name="sBuffer"></param>
        private void Options(string sBuffer)
        {
            Logger.LogError($"\n Options: Bat dau---sBuffer: {sBuffer}");
            string machineSN = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];
            if (string.IsNullOrEmpty(SN))
                return;

            int attindex = sBuffer.IndexOfEx("\r\n\r\n", 1);
            string strOptions = sBuffer.Substring(attindex + 4);

            if (string.IsNullOrEmpty(strOptions))
                return;

            DeviceModel device = GetDeviceModelByOptions(SN, strOptions);
            if (device == null)
                OnError?.Invoke("AttDevice?");
            else
                DeviceBll.Update(device);
        }

        /// <summary>
        /// 根据参数获取设备实例
        /// </summary>
        /// <param name="strOptions"></param>
        /// <returns></returns>
        private DeviceModel GetDeviceModelByOptions(string devSN, string strOptions)
        {
            Logger.LogError($"\n GetDeviceModelByOptions: Bat dau");
            DeviceModel device = DeviceBll.Get(devSN);
            //Tools.InitModel(device, strOptions);

            ////MultiBioDataSupport属性为空，则设备不支持混合识别协议
            //if (!string.IsNullOrEmpty(device.MultiBioDataSupport))
            //    return device;

            FormatBioData(ref strOptions);
            Tools.InitModel(device, strOptions);

            return device;
        }

        /// <summary>
        /// 参数字符串转化，转化为混合识别协议格式的
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private void FormatBioData(ref string options)
        {
            Logger.LogError($"\n FormatBioData: Bat dau");
            if (string.IsNullOrEmpty(options))
                return;

            //0 通用的
            //1 指纹
            //2 面部
            //3 声纹
            //4 虹膜
            //5 视网膜
            //6 掌纹
            //7 指静脉
            //8 手掌
            //9 可见光面部 
            string valMultiBioDataSupport = "0:0:0:0:0:0:0:0:0:0";
            string valMultiBioPhotoSupport = "0:0:0:0:0:0:0:0:0:0";
            string valMultiBioVersion = "0:0:0:0:0:0:0:0:0:0";
            string valMultiBioCount = "0:0:0:0:0:0:0:0:0:0";
            string valMaxMultiBioDataCount = "0:0:0:0:0:0:0:0:0:0";
            string valMaxMultiBioPhotoCount = "0:0:0:0:0:0:0:0:0:0";

            options = options.Replace("~", "");
            string[] arrInfo = options.Split(",\t".ToCharArray());
            foreach (string info in arrInfo)
            {
                string[] arrKeyVal = info.Split('=');
                if (arrKeyVal.Length != 2)
                    continue;
                string key = arrKeyVal[0].Trim();
                string val = arrKeyVal[1].Trim();
                if (val == "" || val == "0")
                    continue;

                #region 指纹
                if (string.Equals(key, "FingerFunOn", StringComparison.OrdinalIgnoreCase))
                {//支持
                    UpdateOptionVal(BioType.FingerPrint, val, ref valMultiBioDataSupport);
                }
                else if (string.Equals(key, "FPVersion", StringComparison.OrdinalIgnoreCase))
                {//版本
                    UpdateOptionVal(BioType.FingerPrint, val, ref valMultiBioVersion);
                }
                else if (string.Equals(key, "FPCount", StringComparison.OrdinalIgnoreCase))
                {//数量
                    UpdateOptionVal(BioType.FingerPrint, val, ref valMultiBioCount);
                }
                else if (string.Equals(key, "MaxFingerCount", StringComparison.OrdinalIgnoreCase))
                {//最大数量
                    UpdateOptionVal(BioType.FingerPrint, val, ref valMaxMultiBioDataCount);
                }
                #endregion end-指纹
                #region 人脸
                else if (string.Equals(key, "FaceFunOn", StringComparison.OrdinalIgnoreCase))
                {//支持
                    UpdateOptionVal(BioType.Face, val, ref valMultiBioDataSupport);
                }
                else if (string.Equals(key, "FaceVersion", StringComparison.OrdinalIgnoreCase))
                {//版本
                    UpdateOptionVal(BioType.Face, val, ref valMultiBioVersion);
                }
                else if (string.Equals(key, "FaceCount", StringComparison.OrdinalIgnoreCase))
                {//数量
                    UpdateOptionVal(BioType.Face, val, ref valMultiBioCount);
                }
                else if (string.Equals(key, "MaxFaceCount", StringComparison.OrdinalIgnoreCase))
                {//最大数量
                    UpdateOptionVal(BioType.Face, val, ref valMaxMultiBioDataCount);
                }
                #endregion end-人脸
                #region 指静脉
                else if (string.Equals(key, "FvFunOn", StringComparison.OrdinalIgnoreCase))
                {//支持
                    UpdateOptionVal(BioType.FingerVein, val, ref valMultiBioDataSupport);
                }
                else if (string.Equals(key, "FvVersion", StringComparison.OrdinalIgnoreCase))
                {//版本
                    UpdateOptionVal(BioType.FingerVein, val, ref valMultiBioVersion);
                }
                else if (string.Equals(key, "FvCount", StringComparison.OrdinalIgnoreCase))
                {//数量
                    UpdateOptionVal(BioType.FingerVein, val, ref valMultiBioCount);
                }
                else if (string.Equals(key, "MaxFvCount", StringComparison.OrdinalIgnoreCase))
                {//最大数量
                    UpdateOptionVal(BioType.FingerVein, val, ref valMaxMultiBioDataCount);
                }
                #endregion end-指静脉
                #region 掌静脉-手掌
                else if (string.Equals(key, "PvFunOn", StringComparison.OrdinalIgnoreCase))
                {//支持
                    UpdateOptionVal(BioType.Palm, val, ref valMultiBioDataSupport);
                }
                else if (string.Equals(key, "PvVersion", StringComparison.OrdinalIgnoreCase))
                {//版本
                    UpdateOptionVal(BioType.Palm, val, ref valMultiBioVersion);
                }
                else if (string.Equals(key, "PvCount", StringComparison.OrdinalIgnoreCase))
                {//数量
                    UpdateOptionVal(BioType.Palm, val, ref valMultiBioCount);
                }
                else if (string.Equals(key, "MaxPvCount", StringComparison.OrdinalIgnoreCase))
                {//最大数量
                    UpdateOptionVal(BioType.Palm, val, ref valMaxMultiBioDataCount);
                }
                #endregion end-掌静脉-手掌
                #region 可见光
                else if (string.Equals(key, "VisilightFun", StringComparison.OrdinalIgnoreCase))
                {//支持
                    UpdateOptionVal(BioType.VisilightFace, val, ref valMultiBioDataSupport);
                }
                //else if (string.Equals(key, "PvVersion", StringComparison.OrdinalIgnoreCase))
                //{//版本
                //    UpdateOptionVal(BioType.FingerVein, val, ref valMultiBioVersion);
                //}
                //else if (string.Equals(key, "PvCount", StringComparison.OrdinalIgnoreCase))
                //{//数量
                //    UpdateOptionVal(BioType.FingerVein, val, ref valMultiBioCount);
                //}
                //else if (string.Equals(key, "MaxPvCount", StringComparison.OrdinalIgnoreCase))
                //{//最大数量
                //    UpdateOptionVal(BioType.FingerVein, val, ref valMaxMultiBioDataCount);
                //}
                #endregion end-可见光
            }

            options += ",MultiBioDataSupport=" + valMultiBioDataSupport
                    + ",MultiBioPhotoSupport=" + valMultiBioPhotoSupport
                    + ",MultiBioVersion=" + valMultiBioVersion
                    + ",MultiBioCount=" + valMultiBioCount
                    + ",MaxMultiBioDataCount=" + valMaxMultiBioDataCount
                    + ",MaxMultiBioPhotoCount=" + valMaxMultiBioPhotoCount;
        }
        /// <summary>
        /// 更新参数值
        /// </summary>
        /// <param name="BioType">类型</param>
        /// <param name="val">更新值</param>
        /// <param name="vals">值组</param>
        /// <returns></returns>
        private string UpdateOptionVal(BioType BioType, string val, ref string vals)
        {
            Logger.LogError($"\n UpdateOptionVal: Bat dau");
            string[] arrVal = vals.Split(':');
            int t = (int)BioType;
            if (t >= arrVal.Length)
                return vals;

            arrVal[t] = val;

            vals = string.Join(":", arrVal);

            return vals;
        }
        #endregion end-table=options

    }
}
