using BioMetrixCore.Utilities;
using Microsoft.Exchange.WebServices.Data;
using NativeWifi;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BioMetrixCore
{
    public partial class ViewMain : Form
    {
        public ViewMain()
        {
            InitializeComponent();
            ToggleControls(false);
            ShowStatusBar(string.Empty, true);
            DisplayEmpty();
        }

        DeviceManipulator manipulator = new DeviceManipulator();
        public ZkemClient objZkeeper;
        private bool isDeviceConnected = false;

        public bool IsDeviceConnected
        {
            get { return isDeviceConnected; }
            set
            {
                isDeviceConnected = value;
                if (isDeviceConnected)
                {
                    ShowStatusBar("The device is connected !!", true);
                    btnConnect.Text = "Disconnect";
                    ToggleControls(true);
                }
                else
                {
                    ShowStatusBar("The device is diconnected !!", true);
                    objZkeeper.Disconnect();
                    btnConnect.Text = "Connect";
                    ToggleControls(true);
                }
            }
        }


        private void ToggleControls(bool value)
        {
            btnBeep.Enabled = value;
            btnDownloadFingerPrint.Enabled = value;
            btnPullData.Enabled = value;
            btnPowerOff.Enabled = value;
            btnRestartDevice.Enabled = value;
            btnGetDeviceTime.Enabled = value;
            btnEnableDevice.Enabled = value;
            btnDisableDevice.Enabled = value;
            btnGetAllUserID.Enabled = value;
            btnUploadUserInfo.Enabled = value;
            tbxMachineNumber.Enabled = !value;
            tbxPort.Enabled = !value;
            tbxDeviceIP.Enabled = !value;
            tbxPassWord.Enabled = !value;

        }


        /// <summary>
        /// Your Events will reach here if implemented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="actionType"></param>
        private void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case UniversalStatic.acx_Disconnect:
                    {
                        ShowStatusBar("The device is switched off", true);
                        DisplayEmpty();
                        btnConnect.Text = "Connect";
                        ToggleControls(false);
                        break;
                    }

                default:
                    break;
            }

        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                ShowStatusBar(string.Empty, true);

                if (IsDeviceConnected)
                {
                    IsDeviceConnected = false;
                    this.Cursor = Cursors.Default;

                    return;
                }

                string ipAddress = tbxDeviceIP.Text.Trim();
                string port = tbxPort.Text.Trim();
                if (ipAddress == string.Empty || port == string.Empty)
                    throw new Exception("The Device IP Address and Port is mandotory !!");

                int portNumber = 4370;
                if (!int.TryParse(port, out portNumber))
                    throw new Exception("Not a valid port number");

                bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The Device IP is invalid !!");

                isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The device at " + ipAddress + ":" + port + " did not respond!!");

                string pw = tbxPassWord.Text.Trim();
                int password = 0;
                if (!int.TryParse(pw, out password))
                    throw new Exception("Not a valid password number");

                objZkeeper = new ZkemClient(RaiseDeviceEvent);
                objZkeeper.Beep(5000);
                objZkeeper.SetCommPassword(password);
                IsDeviceConnected = objZkeeper.Connect_Net(ipAddress, portNumber);

                if (IsDeviceConnected)
                {
                    string deviceInfo = manipulator.FetchDeviceInfo(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()));
                    lblDeviceInfo.Text = deviceInfo;
                }

            }
            catch (Exception ex)
            {
                ShowStatusBar(ex.Message, false);
            }
            this.Cursor = Cursors.Default;

        }


        public void ShowStatusBar(string message, bool type)
        {
            if (message.Trim() == string.Empty)
            {
                lblStatus.Visible = false;
                return;
            }

            lblStatus.Visible = true;
            lblStatus.Text = message;
            lblStatus.ForeColor = Color.White;

            if (type)
                lblStatus.BackColor = Color.FromArgb(79, 208, 154);
            else
                lblStatus.BackColor = Color.FromArgb(230, 112, 134);
        }


        private void btnPingDevice_Click(object sender, EventArgs e)
        {
            ShowStatusBar(string.Empty, true);

            string ipAddress = tbxDeviceIP.Text.Trim();

            bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
            if (!isValidIpA)
                throw new Exception("The Device IP is invalid !!");

            isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
            if (isValidIpA)
                ShowStatusBar("The device is active", true);
            else
                ShowStatusBar("Could not read any response", false);
        }

        private void btnGetAllUserID_Click(object sender, EventArgs e)
        {
            try
            {
                ICollection<UserIDInfo> lstUserIDInfo = manipulator.GetAllUserID(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()));

                if (lstUserIDInfo != null && lstUserIDInfo.Count > 0)
                {
                    BindToGridView(lstUserIDInfo);
                    ShowStatusBar(lstUserIDInfo.Count + " records found !!", true);
                }
                else
                {
                    DisplayEmpty();
                    DisplayListOutput("No records found");
                }

            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }

        }

        private void btnBeep_Click(object sender, EventArgs e)
        {
            objZkeeper.Beep(100);
        }

        private void btnDownloadFingerPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ShowStatusBar(string.Empty, true);

                ICollection<UserInfo> lstFingerPrintTemplates = manipulator.GetAllUserInfo(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()));
                if (lstFingerPrintTemplates != null && lstFingerPrintTemplates.Count > 0)
                {
                    BindToGridView(lstFingerPrintTemplates);
                    ShowStatusBar(lstFingerPrintTemplates.Count + " records found !!", true);
                }
                else
                    DisplayListOutput("No records found");
            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }

        }


        private void btnPullData_Click(object sender, EventArgs e)
        {
            try
            {
                ShowStatusBar(string.Empty, true);
                string message = string.Empty;

                ICollection<MachineInfo> lstMachineInfo = manipulator.GetLogData(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()), ref message);

                if (lstMachineInfo != null && lstMachineInfo.Count > 0)
                {
                    BindToGridView(lstMachineInfo);
                    ShowStatusBar(lstMachineInfo.Count + " records found !!", true);
                }
                else
                    DisplayListOutput("No records found");
            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }

        }


        private void ClearGrid()
        {
            if (dgvRecords.Controls.Count > 2)
            {
                dgvRecords.Controls.RemoveAt(2);
            }
            dgvRecords.DataSource = null;
            dgvRecords.Controls.Clear();
            dgvRecords.Rows.Clear();
            dgvRecords.Columns.Clear();

            if (dgvLogData.Controls.Count > 2)
            {
                dgvLogData.Controls.RemoveAt(2);
            }
            dgvLogData.DataSource = null;
            dgvLogData.Controls.Clear();
            dgvLogData.Rows.Clear();
            dgvLogData.Columns.Clear();

            if (dgvListBSSID.Controls.Count > 2)
            {
                dgvListBSSID.Controls.RemoveAt(2);
            }
            dgvListBSSID.DataSource = null;
            dgvListBSSID.Controls.Clear();
            dgvListBSSID.Rows.Clear();
            dgvListBSSID.Columns.Clear();

            if (dgvListLogByHanetAI.Controls.Count > 2)
            {
                dgvListLogByHanetAI.Controls.RemoveAt(2);
            }
            dgvListLogByHanetAI.DataSource = null;
            dgvListLogByHanetAI.Controls.Clear();
            dgvListLogByHanetAI.Rows.Clear();
            dgvListLogByHanetAI.Columns.Clear();

            if (dgvListLogByHysoon.Controls.Count > 2)
            {
                dgvListLogByHysoon.Controls.RemoveAt(2);
            }
            dgvListLogByHysoon.DataSource = null;
            dgvListLogByHysoon.Controls.Clear();
            dgvListLogByHysoon.Rows.Clear();
            dgvListLogByHysoon.Columns.Clear();
        }
        private void BindToGridView(object list)
        {
            ClearGrid();

            dgvRecords.DataSource = list;
            dgvRecords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvRecords);

            dgvLogData.DataSource = list;
            dgvLogData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvLogData);

            dgvListBSSID.DataSource = list;
            dgvListBSSID.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvListBSSID);

            dgvListLogByHanetAI.DataSource = list;
            dgvListLogByHanetAI.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvListLogByHanetAI);

            dgvListLogByHysoon.DataSource = list;
            dgvListLogByHysoon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvListLogByHysoon);
        }



        private void DisplayListOutput(string message)
        {
            if (dgvRecords.Controls.Count > 2)
            { dgvRecords.Controls.RemoveAt(2); }

            ShowStatusBar(message, false);
        }

        private void DisplayEmpty()
        {
            ClearGrid();
            dgvRecords.Controls.Add(new DataEmpty());
        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        { UniversalStatic.DrawLineInFooter(pnlHeader, Color.FromArgb(204, 204, 204), 2); }



        private void btnPowerOff_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            var resultDia = DialogResult.None;
            resultDia = MessageBox.Show("Do you wish to Power Off the Device ??", "Power Off Device", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultDia == DialogResult.Yes)
            {
                bool deviceOff = objZkeeper.PowerOffDevice(int.Parse(tbxMachineNumber.Text.Trim()));

            }

            this.Cursor = Cursors.Default;
        }

        private void btnRestartDevice_Click(object sender, EventArgs e)
        {

            DialogResult rslt = MessageBox.Show("Do you wish to restart the device now ??", "Restart Device", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rslt == DialogResult.Yes)
            {
                if (objZkeeper.RestartDevice(int.Parse(tbxMachineNumber.Text.Trim())))
                    ShowStatusBar("The device is being restarted, Please wait...", true);
                else
                    ShowStatusBar("Operation failed,please try again", false);
            }

        }

        private void btnGetDeviceTime_Click(object sender, EventArgs e)
        {
            int machineNumber = int.Parse(tbxMachineNumber.Text.Trim());
            int dwYear = 0;
            int dwMonth = 0;
            int dwDay = 0;
            int dwHour = 0;
            int dwMinute = 0;
            int dwSecond = 0;

            bool result = objZkeeper.GetDeviceTime(machineNumber, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond);

            string deviceTime = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
            List<DeviceTimeInfo> lstDeviceInfo = new List<DeviceTimeInfo>();
            lstDeviceInfo.Add(new DeviceTimeInfo() { DeviceTime = deviceTime });
            BindToGridView(lstDeviceInfo);
        }


        private void btnEnableDevice_Click(object sender, EventArgs e)
        {
            // This is of no use since i implemented zkemKeeper the other way
            bool deviceEnabled = objZkeeper.EnableDevice(int.Parse(tbxMachineNumber.Text.Trim()), true);

        }



        private void btnDisableDevice_Click(object sender, EventArgs e)
        {
            // This is of no use since i implemented zkemKeeper the other way
            bool deviceDisabled = objZkeeper.DisableDeviceWithTimeOut(int.Parse(tbxMachineNumber.Text.Trim()), 3000);
        }

        private void tbxPort_TextChanged(object sender, EventArgs e)
        { UniversalStatic.ValidateInteger(tbxPort); }

        private void tbxMachineNumber_TextChanged(object sender, EventArgs e)
        { UniversalStatic.ValidateInteger(tbxMachineNumber); }
        private void tbxPassWord_TextChanged(object sender, EventArgs e)
        { UniversalStatic.ValidateInteger(tbxPassWord); }

        private void btnUploadUserInfo_Click(object sender, EventArgs e)
        {
            // Add you new UserInfo Here and uncomment the below code
            //List<UserInfo> lstUserInfo = new List<UserInfo>();
            //manipulator.UploadFTPTemplate(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()), lstUserInfo);
        }

        private void dgvRecords_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = string.Empty;
            string token = string.Empty;
            string mess = string.Empty;
            var success = login(tbxUserName.Text.Trim(), tbxPass.Text.Trim(), ref url, ref token, ref mess);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string url = string.Empty;
            string token = string.Empty;
            string mess = string.Empty;
            login(tbxUserName.Text.Trim(), tbxPass.Text.Trim(), ref url, ref token, ref mess);
            var fromDate = DateTime.Now.Date.AddDays(-1);
            var toDate = DateTime.Now.Date;
            var logs = GetDatas(fromDate, toDate, url, token, ref mess);
            var lstLog = new List<LogData>();
            foreach (var log in logs)
            {
                var data = new LogData();
                var userID = log.ContainsKey("person_id") ? log["person_id"].ToString() : "";
                var time = log.ContainsKey("time") ? log["time"].ToString() : "";
                var deviceID = log.ContainsKey("camera_id") ? log["camera_id"].ToString() : "";
                var checkTime = DateTime.Now;
                if (DateTime.TryParse(time, out checkTime) && !string.IsNullOrWhiteSpace(userID))
                {
                    data.CheckTime = checkTime;
                    data.UserID = userID;
                    data.DeviceID = deviceID;
                    lstLog.Add(data);
                }

            }
            if (lstLog.Count > 0)
            {
                BindToGridView(lstLog);
            }
        }
        private bool login(string email, string pass, ref string url, ref string token, ref string mess)
        {
            //var client = new RestClient("https://cloud.beetai.com/api/login_integrate");
            //client.Timeout = -1;
            //var request = new RestRequest(Method.POST);
            //var param = new
            //{
            // email = email,
            // password = pass
            //};
            //request.AddParameter("application/json", Converter.JsonSerialize(param), ParameterType.RequestBody);

            //IRestResponse response = client.Execute(request);
            //if (!response.IsSuccessful)
            //{
            // return;
            //}
            //var content = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
            //if (content.ContainsKey("status") && int.Parse(content["status"].ToString()) != 10000)
            //{
            // mess = content.ContainsKey("error message") ? content["error message"].ToString() : "";
            // return;
            //}
            //var data = content.ContainsKey("data") ? Converter.JsonDeserialize<Dictionary<string, object>>(content["data"].ToString()) : new Dictionary<string, object>();
            //url = data.ContainsKey("api_url") ? data["api_url"].ToString() : "";
            //token = data.ContainsKey("access_token") ? data["access_token"].ToString() : "";

            var param = new
            {
                email = email,
                password = pass
            };
            var res = CallTimesheetAPI<Dictionary<string, object>>("https://cloud.beetai.com/api/login_integrate", Method.POST, param, null, false);
            if (res.Success && res.status == 10000)
            {
                var data = res.Data;
                url = data.ContainsKey("api_url") ? data["api_url"].ToString() : "";
                token = data.ContainsKey("access_token") ? data["access_token"].ToString() : "";
                return true;
            }
            return false;
        }
        private List<Dictionary<string, object>> GetDatas(DateTime fromDate, DateTime toDate, string url, string token, ref string message)
        {
            var logs = new List<Dictionary<string, object>>();

            fromDate = fromDate.Date;
            toDate = toDate.AddDays(1).Date.AddSeconds(-1);
            message += $"\nLay du lieu BiFace:===Tu ngay: {fromDate}---Den ngay: {toDate}";
            if (fromDate.AddMonths(1) < toDate)
            {
                var logDatas = new List<Dictionary<string, object>>();
                var fromDateNew = fromDate.Date;
                var toDateNew = fromDate.AddMonths(1);
                while (toDateNew < toDate)
                {
                    message += $"\nLay du lieu BiFace chia nho:===Tu ngay: {fromDateNew}---Den ngay: {toDateNew}";
                    logDatas = GetData(fromDateNew, toDateNew, url, token, ref message);
                    message += $"\nLay du lieu BiFace chia nho:===Tu ngay: {fromDateNew}---Den ngay: {toDateNew}----SL: {logDatas.Count}";
                    logs.AddRange(logDatas);
                    // Tăng lên 1s để lấy dữ liệu liền kề tiếp
                    fromDateNew = toDateNew.AddDays(1).Date;
                    toDateNew = toDateNew.AddMonths(1);
                }
                logDatas = GetData(fromDateNew, toDate, url, token, ref message);
                message += $"\nLay du lieu BiFace chia nho:===Tu ngay: {fromDateNew}---Den ngay: {toDate}----SL: {logDatas.Count}";
                logs.AddRange(logDatas);
            }
            else
            {
                logs = GetData(fromDate, toDate, url, token, ref message);
            }
            message += $"\nTong du lieu tren may BiFace:===Tu ngay: {fromDate}---Den ngay: {toDate}----SL: {logs.Count}";
            return logs;
        }
        /// <summary>
        /// Hàm lấy dữ liệu
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetData(DateTime fromDate, DateTime toDate, string url, string token, ref string mess)
        {
            var logs = new List<Dictionary<string, object>>();
            //var client = new RestClient($"{url}/api/time_keeping/getListCheckin");
            //client.Timeout = -1;

            //var request = new RestRequest(Method.POST);
            //var param = new
            //{
            // company_code = tbxCompanyCode.Text.Trim(),
            // start_time = DateTime.Now.Date,
            // end_time = DateTime.Now.AddMonths(2)
            //};
            //request.AddParameter("application/json", Converter.JsonSerialize(param), ParameterType.RequestBody);

            //client.Authenticator = new JwtAuthenticator(token);
            //IRestResponse response = client.Execute(request);
            //if (!response.IsSuccessful)
            //{
            // return logs;
            //}
            //var content = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
            //if (content.ContainsKey("status") && int.Parse(content["status"].ToString()) != 10000)
            //{
            // mess += content.ContainsKey("error message") ? content["error message"]?.ToString() : "";
            // return logs;
            //}
            //logs = Converter.JsonDeserialize<List<Dictionary<string, object>>>(content["data"].ToString());


            var param = new
            {
                company_code = tbxCompanyCode.Text.Trim(),
                start_time = fromDate.Date,
                end_time = toDate.Date.AddDays(1).AddSeconds(-1)
            };
            var res = CallTimesheetAPI<List<Dictionary<string, object>>>($"{url}/api/time_keeping/getListCheckin", Method.POST, param, null, false, token: token);
            if (res.Success && res.status == 10000)
            {
                logs = res.Data;
            }
            return logs;
        }
        private ServiceResult<T> CallTimesheetAPI<T>(string apiPath, Method method, object jsonBody, Dictionary<string, object> queryParams, bool autoAddSessionCookies = true, bool getRaw = false, string token = null)
        {
            var result = new ServiceResult<T>();

            var client = new RestClient(apiPath);
            var request = new RestRequest(apiPath, method);

            //Cookies
            //request.AddCookie("x-deviceid", Utility.GetDeviceId());
            //if (autoAddSessionCookies)
            //{
            // request.AddCookie("x-sessionid", this.SessionId);
            // request.AddCookie("x-tenantid", this.TenantId);
            //}

            //Body
            if (jsonBody != null)
            {
                request.AddParameter("application/json", Converter.JsonSerialize(jsonBody), ParameterType.RequestBody);
            }

            //Authenticator
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.Authenticator = new JwtAuthenticator(token);
            }

            //QueryString
            if (queryParams != null)
            {
                foreach (var p in queryParams)
                {
                    request.AddParameter(p.Key, p.Value, ParameterType.QueryString);
                }
            }
            request.AddHeader("User-Agent", "");

            bool logService = false;
            Boolean.TryParse(Utility.GetAppSetting("LogService"), out logService);


            bool useProxy = false;
            Boolean.TryParse(Utility.GetAppSetting("useProxy"), out useProxy);
            if (useProxy)
            {
                if (!string.IsNullOrWhiteSpace(Utility.GetAppSetting("myProxyAddress")))
                {
                    client.Proxy = new WebProxy(Utility.GetAppSetting("myProxyAddress"));
                }
                else
                {
                    int myProxyPort = 0;
                    Int32.TryParse(Utility.GetAppSetting("myProxyPort"), out myProxyPort);
                    client.Proxy = new WebProxy(Utility.GetAppSetting("myProxyHost"), myProxyPort);
                }

                if (!string.IsNullOrWhiteSpace(Utility.GetAppSetting("domain")))
                {
                    client.Proxy.Credentials = new NetworkCredential(Utility.GetAppSetting("username"), Utility.GetAppSetting("password"), Utility.GetAppSetting("domain"));
                }
                else
                {
                    client.Proxy.Credentials = new NetworkCredential(Utility.GetAppSetting("username"), Utility.GetAppSetting("password"));
                }
            }
            //Execute
            var response = client.Execute(request);
            //Get response
            if (response.IsSuccessful)
            {
                if (!getRaw)
                {
                    var responseData = Converter.JsonDeserialize<ServiceResult<object>>(response.Content);

                    result.Success = responseData.Success;
                    result.Code = responseData.Code;
                    result.SubCode = responseData.SubCode;
                    result.UserMessage = responseData.UserMessage;
                    result.SystemMessage = responseData.SystemMessage;
                    var validateResult = string.Join(";", responseData.ValidateInfo.Select(x => x.ErrorMessage).ToList());
                    result.status = responseData.status;
                    result.error_message = responseData.error_message;
                    result.Data = Converter.JsonDeserialize<T>(Converter.JsonSerialize(responseData.Data));
                }
                else
                {
                    result.Success = true;
                    if (typeof(T) == typeof(byte[]))
                    {
                        result.Data = (T)Convert.ChangeType(response.RawBytes, typeof(T));
                    }
                }
            }
            else
            {
                result.Success = false;
                result.Code = (int)response.StatusCode;
                result.SystemMessage = response.ErrorMessage;
            }
            return result;
        }

        private async void btnGetBSSID_Click(object sender, EventArgs e)
        {
            try
            {
                //Clipboard.SetText("hnanh");
                var fileName = "cmd.exe";
                var cmdText = Utility.GetAppSetting("CMDTextWindows");
                //var operatingSystem = GetOperatingSystem();
                //if (operatingSystem == OSPlatform.Windows)
                //{
                //    fileName = "cmd.exe";
                //    cmdText = Utility.GetAppSetting("CMDTextWindows");
                //}
                //else if (operatingSystem == OSPlatform.OSX)
                //{
                //    fileName = "terminal.exe";
                //    cmdText = Utility.GetAppSetting("CMDTextOSX");
                //}
                //else
                //{
                //    return;
                //}


                var operatingSystem = GetOperatingSystemV2();
                if (operatingSystem == "Windows")
                {
                    fileName = "cmd.exe";
                    cmdText = Utility.GetAppSetting("CMDTextWindows");
                }
                else if (operatingSystem == "MacOSX")
                {
                    fileName = "terminal.exe";
                    cmdText = Utility.GetAppSetting("CMDTextOSX");
                }
                else
                {
                    return;
                }
                var args = !string.IsNullOrWhiteSpace(cmdText) ? cmdText : "/C netsh wlan show networks mode=Bssid";
                Process cmd = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = fileName,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        Arguments = args.Trim()
                    }
                };
                cmd.Start();
                var outPut = cmd.StandardOutput.ReadToEnd();
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();

                txtResult.Text = outPut;

                var apList = await GetSignalOfNetworks(fileName, args);
                if (apList.Count > 0)
                {
                    BindToGridView(apList);
                }
            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }
        }
        public static OSPlatform GetOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }

            throw new Exception("Cannot determine operating system!");
        }
        public static string GetOperatingSystemV2()
        {
            var os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return "Windows";
                case PlatformID.MacOSX:
                    return "MacOSX";
                case PlatformID.Unix:
                    return "Unix";
                default:
                    return "";
            }

            throw new Exception("Cannot determine operating system!");
        }
        private async Task<List<AccessPoint>> GetSignalOfNetworks(string fileName, string args)
        {
            string result = await ExecuteProcessAsync(fileName, args);

            return Regex.Split(result, @"[^B]SSID \d+").Skip(1).SelectMany(network => GetAccessPointFromNetwork(network)).ToList();
        }

        private static List<AccessPoint> GetAccessPointFromNetwork(string network)
        {
            string withoutLineBreaks = Regex.Replace(network, @"[\r\n]+", "@@@###%%%&&&").Trim();
            //string ssid = Regex.Replace(withoutLineBreaks, @"^:\s+(\S+).*$", "$1");
            string ssid = Regex.Replace(withoutLineBreaks, @"^:\s+([\w-*|\s*]*).*$", "$1");

            return Regex.Split(withoutLineBreaks, @"\s{4}BSSID \d+").Skip(1).Select(ap => GetAccessPoint(ssid, ap)).ToList();
        }

        private static AccessPoint GetAccessPoint(string ssid, string ap)
        {
            string withoutLineBreaks = Regex.Replace(ap, @"[\r\n]+", " ").Trim();
            string bssid = Regex.Replace(withoutLineBreaks, @"^:\s+([a-f0-9]{2}(:[a-f0-9]{2}){5}).*$", "$1").Trim();
            //byte signal = byte.Parse(Regex.Replace(withoutLineBreaks, @"^.*(Signal|Sinal)\s+:\s+(\d+)%.*$", "$2").Trim());

            return new AccessPoint
            {
                SSID = ssid,
                BSSID = bssid,
                //Signal = signal,
            };
        }

        private static async Task<string> ExecuteProcessAsync(string cmd, string args = null)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = args,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                }
            };

            process.Start();

            string result = await process.StandardOutput.ReadToEndAsync();

            process.WaitForExit();

            return result;
        }
       
        private string ListToStringCommaSeparator(List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.Append(item);
                sb.Append(',');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        public List<DeviceInfo> GetListDevice(ref string message)
        {
            List<DeviceInfo> listDevices = new List<DeviceInfo>();
            var client = new RestClient(HanetAPIPath.PartnerPath + "/device/get-list-device");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader(HanetAPIPath.ContentType, HanetAPIPath.UrlEncoded);
            request.AddParameter(HanetAPIPath.Token, tbAccessToken.Text);
            IRestResponse response = client.Execute(request);
            // kiem tra response tra ve co loi thi return null
            if (response.IsSuccessful == false)
            {
                message += response.StatusCode;
                return null;
            }
            message = response.Content;
            var bodyContent = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
            try
            {
                var returnCode = bodyContent["returnCode"].ToString();
                // nếu response trả về lỗi thì dùng refresh token xem có đúng ko
                if (returnCode.Equals("1"))
                {
                    // tra ve data thanh cong
                    var contentData = bodyContent["data"].ToString();
                    var listDevice = Converter.JsonDeserialize<List<Dictionary<string, object>>>(contentData);
                    // lay ve du lieu trong listDevice
                    if (listDevice != null && listDevice.Count() > 0)
                    {
                        // lay thong tin ve may cham cong
                        foreach (Dictionary<string, object> device in listDevice)
                        {
                            DeviceInfo keeperConfig = new DeviceInfo();
                            keeperConfig.ClientID = tbClientID.Text;
                            keeperConfig.ClientSecret = tbClientSecret.Text;
                            keeperConfig.AccessToken = tbAccessToken.Text;
                            keeperConfig.RefreshToken = tbRefreshToken.Text;
                            keeperConfig.DeviceID = device["deviceID"].ToString();
                            keeperConfig.PlaceID = device["placeID"].ToString();
                            listDevices.Add(keeperConfig);
                        }
                    }
                }
                else
                {
                    // co exception
                    message += " Error code: " + returnCode;
                    return null;
                }
                return listDevices;
            }
            catch (Exception e)
            {
                // co exception
                message += " Exception: " + e.ToString();
                return null;
            }
        }
        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
        /// <summary>
        /// Hàm lấy dữ liệu cho HanetAI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetLogByHanetAI_Click(object sender, EventArgs e)
        {
            var mess = "";
            int limit = 200;
            int.TryParse(Utility.GetAppSetting("LimitGetLogsByHanetAI"), out limit);
            var logs = GetLogs(dtFromDateHanetAI.Value, dtToDateHanetAI.Value, limit, ref mess);
            if (logs.Count > 0)
            {
                BindToGridView(logs);
            }
        }
        public List<LogData> GetLogs(DateTime? fromDate, DateTime? toDate, int? limit, ref string message)
        {
            var listLogs = new List<LogData>();
            try
            {
                // lấy list device
                List<DeviceInfo> listDevices = GetListDevice(ref message);
                if (listDevices == null || listDevices.Count == 0)
                {
                    message += "*No Device Found*";
                    return null;
                }
                // list device ID
                List<string> deviceIDs = listDevices.Select(item =>
                {
                    return item.DeviceID;
                }).ToList();
                // set place ID
                List<string> placeIDs = listDevices.Select(item =>
                {
                    return item.PlaceID;
                }).Distinct().ToList();

                var canGetLogs = false;
                // dung GetCheckinByPlaceIdInDay de lay du lieu theo from date va to date
                fromDate = fromDate.HasValue ? fromDate.Value.Date : DateTime.Now.AddMonths(-2);
                toDate = toDate.HasValue ? toDate.Value.Date.AddDays(1).AddMilliseconds(-1) : DateTime.Now;
                foreach (DateTime day in EachDay((DateTime)fromDate, (DateTime)toDate))
                {
                    foreach (var place in placeIDs)
                    {
                        var total = GetTotalCheckinByPlaceIdInDay(deviceIDs, place, day, ref message);
                        var page = 0;
                        var size = limit.Value;
                        while (page * size < total)
                        {
                            page++;
                            canGetLogs = GetCheckinByPlaceIdInDay(deviceIDs, place, ref listLogs, day, page, size, ref message);
                            if (!canGetLogs)
                            {
                                // log ra ngay ko lay duoc log
                                message += "Error could not get data: " + day.ToString();
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            message += "\n***Số lượng bản ghi: " + listLogs.Count;
            return listLogs.OrderBy(x => x.CheckTime).ToList();
        }
        private int GetTotalCheckinByPlaceIdInDay(List<string> deviceIDs, string place, DateTime dtToRead, ref string message)
        {
            var client = new RestClient(HanetAPIPath.PartnerPath + "/person/getTotalCheckinByPlaceIdInDay");
            client.Timeout = -1;
            string dtString = dtToRead.ToString("yyyy-MM-dd");
            string listDeviceIds = ListToStringCommaSeparator(deviceIDs);
            var request = new RestRequest(Method.POST);
            request.AddParameter(HanetAPIPath.Token, tbAccessToken.Text);
            request.AddParameter("placeID", place);
            request.AddParameter("date", dtString);
            request.AddParameter("devices", listDeviceIds);
            // 0 - nhan vien, 1 - khach hang
            request.AddParameter("type", "0");
            IRestResponse response = client.Execute(request);
            // check response tra ve false
            if (response.IsSuccessful == false)
            {
                message += response.StatusCode;
                return 0;
            }
            var bodyContent = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
            // check returnCode
            var returnCode = bodyContent["returnCode"] != null ? bodyContent["returnCode"].ToString() : "1";
            var returnMessage = bodyContent["returnMessage"] != null ? bodyContent["returnMessage"].ToString() : "";
            // nếu response trả về lỗi thì dùng refresh token xem có đúng ko
            var contentData = "0";
            if (returnCode.Equals("1"))
            {
                // tra ve data thanh cong
                contentData = bodyContent["data"] != null ? bodyContent["data"].ToString() : "0";
            }
            else
            {
                // co exception
                message += $" Error code: {returnCode}--- MessErr: {returnMessage}";
                return 0;
            }
            return int.Parse(contentData);
        }
        // truyen vao list deviceIDs, place, ngay, device info, tra lai dataLogs
        public bool GetCheckinByPlaceIdInDay(List<string> deviceIDs, string place, ref List<LogData> listLogs, DateTime dtToRead, int page, int size, ref string message)
        {
            var client = new RestClient(HanetAPIPath.PartnerPath + "/person/getCheckinByPlaceIdInDay");
            client.Timeout = -1;
            string dtString = dtToRead.ToString("yyyy-MM-dd");
            string listDeviceIds = ListToStringCommaSeparator(deviceIDs);
            var request = new RestRequest(Method.POST);
            request.AddParameter(HanetAPIPath.Token, tbAccessToken.Text);
            request.AddParameter("placeID", place);
            request.AddParameter("date", dtString);
            request.AddParameter("devices", listDeviceIds);
            request.AddParameter("page", page);
            request.AddParameter("size", size);
            // 0 - nhan vien, 1 - khach hang
            request.AddParameter("type", "0");
            IRestResponse response = client.Execute(request);
            // check response tra ve false
            if (response.IsSuccessful == false)
            {
                message += response.StatusCode;
                return false;
            }
            var bodyContent = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
            // check returnCode
            var returnCode = bodyContent["returnCode"].ToString();
            // nếu response trả về lỗi thì dùng refresh token xem có đúng ko
            if (returnCode.Equals("1"))
            {
                // tra ve data thanh cong
                var contentData = bodyContent["data"].ToString();
                var listLog = Converter.JsonDeserialize<List<Dictionary<string, object>>>(contentData);
                // lay ve du lieu trong listDevice
                if (listLog != null && listLog.Count() > 0)
                {
                    // co danh sach thiet bi
                    // lay thong tin ve may cham cong
                    foreach (Dictionary<string, object> item in listLog)
                    {
                        // try catch xử lí lỗi sai định dạng unix timestamp
                        try
                        {
                            var unixTimeStamp = Convert.ToDouble(item["checkinTime"].ToString());
                            var checktime = UnixTimeStampToDateTime(unixTimeStamp);
                            LogData log = new LogData();
                            log.CheckTime = checktime;
                            log.UserID = item["personID"].ToString();
                            log.FullName = item["deviceID"].ToString();
                            log.FullName = item["personName"].ToString();
                            listLogs.Add(log);
                        }
                        catch (Exception x)
                        {
                            message += x.ToString();
                        }
                    }
                }
            }
            else
            {
                // co exception
                message += "Error code: " + returnCode;
                return false;
            }
            return true;
        }

        private void btnGetLogByHanetAIV2_Click(object sender, EventArgs e)
        {
            var mess = "";
            int limit = 200;
            int.TryParse(Utility.GetAppSetting("LimitGetLogsByHanetAI"), out limit);
            var logs = GetLogs_V2(dtFromDateHanetAI.Value.Date, dtToDateHanetAI.Value.Date.AddDays(1).AddMilliseconds(-1), limit, ref mess);
            if (logs.Count > 0)
            {
                BindToGridView(logs);
            }
        }
        public List<LogData> GetLogs_V2(DateTime? fromDate, DateTime? toDate, int? limit, ref string message)
        {
            var listLogs = new List<LogData>();
            try
            {
                // lấy list device
                List<DeviceInfo> listDevices = GetListDevice(ref message);
                if (listDevices == null || listDevices.Count == 0)
                {
                    message += "*No Device Found*";
                    return null;
                }
                // list device ID
                List<string> deviceIDs = listDevices.Select(item =>
                {
                    return item.DeviceID;
                }).ToList();
                // set place ID
                List<string> placeIDs = listDevices.Select(item =>
                {
                    return item.PlaceID;
                }).Distinct().ToList();

                fromDate = fromDate.HasValue ? fromDate.Value.Date : DateTime.Now.AddMonths(-2);
                toDate = toDate.HasValue ? toDate.Value.Date.AddDays(1).AddMilliseconds(-1) : DateTime.Now;
                if (fromDate.Value.Month != toDate.Value.Month || fromDate.Value.Year != toDate.Value.Year)
                {
                    // Xử lý trường hợp api chỉ cho lấy dữ liệu FromDate, ToDate trong cùng 1 tháng
                    var fromDateNew = fromDate.Value.Date;
                    var toDateNew = new DateTime(fromDate.Value.Year, fromDate.Value.Month, DateTime.DaysInMonth(fromDate.Value.Year, fromDate.Value.Month)).Date.AddDays(1).AddMilliseconds(-1);
                    while (toDateNew < toDate)
                    {
                        foreach (var place in placeIDs)
                        { 
                            var total = GetTotalCheckinByPlaceIdInTimestamp(deviceIDs, place, fromDateNew, toDateNew, ref message);
                            var page = 0;
                            var size = limit.Value;
                            while(page * size < total)
                            {
                                page++;
                                var logs = GetCheckinByPlaceIdInTimestamp(deviceIDs, place, fromDateNew, toDateNew, page, size, ref message);
                                listLogs.AddRange(logs);
                            }
                        }
                        fromDateNew = toDateNew.AddDays(1).Date;
                        toDateNew = new DateTime(fromDateNew.Year, fromDateNew.Month, DateTime.DaysInMonth(fromDateNew.Year, fromDateNew.Month)).Date.AddDays(1).AddMilliseconds(-1);
                    }

                    foreach (var place in placeIDs)
                    {
                        var total = GetTotalCheckinByPlaceIdInTimestamp(deviceIDs, place, fromDateNew, toDate, ref message);
                        var page = 0;
                        var size = limit.Value;
                        while (page * size < total)
                        {
                            page++;
                            var logs = GetCheckinByPlaceIdInTimestamp(deviceIDs, place, fromDateNew, toDate, page, size, ref message);
                            listLogs.AddRange(logs);
                        }
                    }
                }
                else
                {
                    foreach (var place in placeIDs)
                    {
                        var total = GetTotalCheckinByPlaceIdInTimestamp(deviceIDs, place, fromDate, toDate, ref message);
                        var page = 0;
                        var size = limit.Value;
                        while (page * size < total)
                        {
                            page++;
                            var logs = GetCheckinByPlaceIdInTimestamp(deviceIDs, place, fromDate, toDate, page, size, ref message);
                            listLogs.AddRange(logs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            message += "\n***Số lượng bản ghi: " + listLogs.Count;
            return listLogs.OrderBy(x => x.CheckTime).ToList();
        }
        private int GetTotalCheckinByPlaceIdInTimestamp(List<string> deviceIDs, string place, DateTime? fromDate, DateTime? toDate, ref string message)
        {
            fromDate = fromDate.Value.Date;
            toDate = toDate.Value.Date.AddDays(1).AddMilliseconds(-1);
            var dt1 = new DateTimeOffset(fromDate.Value);
            long from = dt1.ToUnixTimeMilliseconds();
            var dt2 = new DateTimeOffset(toDate.Value);
            long to = dt2.ToUnixTimeMilliseconds();


            var from_v2 = DateTimeToUnixTimeStamp(fromDate.Value);
            var to_v2 = DateTimeToUnixTimeStamp(toDate.Value);

            var client = new RestClient(HanetAPIPath.PartnerPath + "/person/getTotalCheckinByPlaceIdInTimestamp");
            client.Timeout = -1;
            string listDeviceIds = ListToStringCommaSeparator(deviceIDs);
            var request = new RestRequest(Method.POST);
            request.AddParameter(HanetAPIPath.Token, tbAccessToken.Text);
            request.AddParameter("placeID", place);
            request.AddParameter("from", from_v2);
            request.AddParameter("to", to_v2);
            request.AddParameter("devices", listDeviceIds);
            // 0 - nhan vien, 1 - khach hang
            request.AddParameter("type", "0");
            IRestResponse response = client.Execute(request);
            // check response tra ve false
            if (response.IsSuccessful == false)
            {
                message += response.StatusCode;
                return 0;
            }
            var bodyContent = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
            // check returnCode
            var returnCode = bodyContent["returnCode"] != null ? bodyContent["returnCode"].ToString() : "1";
            var returnMessage = bodyContent["returnMessage"] != null ? bodyContent["returnMessage"].ToString() : "";
            // nếu response trả về lỗi thì dùng refresh token xem có đúng ko
            var contentData = "0";
            if (returnCode.Equals("1"))
            {
                // tra ve data thanh cong
                contentData = bodyContent["data"] != null ? bodyContent["data"].ToString() : "0";
            }
            else
            {
                // co exception
                message += $" Error code: {returnCode}--- MessErr: {returnMessage}";
                return 0;
            }
            return int.Parse(contentData);
        }
        private List<LogData> GetCheckinByPlaceIdInTimestamp(List<string> deviceIDs, string place, DateTime? fromDate, DateTime? toDate, int page, int size, ref string message)
        {
            fromDate = fromDate.Value.Date;
            toDate = toDate.Value.Date.AddDays(1).AddMilliseconds(-1);
            var dt1 = new DateTimeOffset(fromDate.Value);
            long from = dt1.ToUnixTimeMilliseconds();
            var dt2 = new DateTimeOffset(toDate.Value);
            long to = dt2.ToUnixTimeMilliseconds();

            var from_v2 = DateTimeToUnixTimeStamp(fromDate.Value);
            var to_v2 = DateTimeToUnixTimeStamp(toDate.Value);


            List<LogData> listLogs = new List<LogData>();
            var client = new RestClient(HanetAPIPath.PartnerPath + "/person/getCheckinByPlaceIdInTimestamp");
            client.Timeout = -1;
            string listDeviceIds = ListToStringCommaSeparator(deviceIDs);
            var request = new RestRequest(Method.POST);
            request.AddParameter(HanetAPIPath.Token, tbAccessToken.Text);
            request.AddParameter("placeID", place);
            request.AddParameter("from", from_v2);
            request.AddParameter("to", to_v2);
            request.AddParameter("devices", listDeviceIds);
            request.AddParameter("page", page);
            request.AddParameter("size", size);
            // 0 - nhan vien, 1 - khach hang
            request.AddParameter("type", "0");
            IRestResponse response = client.Execute(request);
            // check response tra ve false
            if (response.IsSuccessful == false)
            {
                message += response.StatusCode;
                return new List<LogData>();
            }
            var bodyContent = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
            // check returnCode
            var returnCode = bodyContent["returnCode"] != null ? bodyContent["returnCode"].ToString() : "1";
            var returnMessage = bodyContent["returnMessage"] != null ? bodyContent["returnMessage"].ToString() : "";
            // nếu response trả về lỗi thì dùng refresh token xem có đúng ko
            if (returnCode.Equals("1"))
            {
                // tra ve data thanh cong
                var contentData = bodyContent["data"].ToString();
                var listLog = Converter.JsonDeserialize<List<Dictionary<string, object>>>(contentData);
                // lay ve du lieu trong listDevice
                if (listLog != null && listLog.Count() > 0)
                {
                    // co danh sach thiet bi
                    // lay thong tin ve may cham cong
                    foreach (Dictionary<string, object> item in listLog)
                    {
                        // try catch xử lí lỗi sai định dạng unix timestamp
                        try
                        {
                            var unixTimeStamp = Convert.ToDouble(item["checkinTime"].ToString());
                            var checktime = UnixTimeStampToDateTime(unixTimeStamp);
                            LogData log = new LogData();
                            log.CheckTime = checktime;
                            log.UserID = item["personID"].ToString();
                            log.FullName = item["deviceID"].ToString();
                            log.FullName = item["personName"].ToString();
                            listLogs.Add(log);
                        }
                        catch (Exception x)
                        {
                            message += x.ToString();
                        }
                    }
                }
            }
            else
            {
                // co exception
                message += $"Error code: {returnCode}--- MessErr: {returnMessage}";
                return new List<LogData>();
            }
            return listLogs;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        private string DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            DateTime dtUtc = dateTime.ToUniversalTime();
            string result = (dtUtc.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc))).TotalMilliseconds.ToString();
            return result;
        }
        private void btnGetLogsByHysoon_Click(object sender, EventArgs e)
        {
            var logs = new List<LogData>();
            var message = "";

            var fromDate = dtFromDateByHysoon.Value.Date;
            var toDate = dtToDateByHysoon.Value.AddDays(1).Date.AddSeconds(-1);
            message += $"\nLay du lieu Hysoon:===Tu ngay: {fromDate}---Den ngay: {toDate}";
            if (fromDate.AddMonths(1) < toDate)
            {
                var logDatas = new List<LogData>();
                var fromDateNew = fromDate.Date;
                var toDateNew = fromDate.AddMonths(1);
                while (toDateNew < toDate)
                {
                    message += $"\nLay du lieu Hysoon chia nho:===Tu ngay: {fromDateNew}---Den ngay: {toDateNew}";
                    logDatas = GetLogsByHysoon(fromDateNew, toDateNew, ref message);
                    message += $"\nLay du lieu Hysoon chia nho:===Tu ngay: {fromDateNew}---Den ngay: {toDateNew}----SL: {logDatas.Count}";
                    logs.AddRange(logDatas);
                    // Tăng lên 1s để lấy dữ liệu liền kề tiếp
                    fromDateNew = toDateNew.AddDays(1).Date;
                    toDateNew = toDateNew.AddMonths(1);
                }
                logDatas = GetLogsByHysoon(fromDateNew, toDate, ref message);
                message += $"\nLay du lieu Hysoon chia nho:===Tu ngay: {fromDateNew}---Den ngay: {toDate}----SL: {logDatas.Count}";
                logs.AddRange(logDatas);
            }
            else
            {
                logs = GetLogsByHysoon(fromDate, toDate, ref message);
            }
            message += $"\nTong du lieu tren may Hysoon:===Tu ngay: {fromDate}---Den ngay: {toDate}----SL: {logs.Count}";
            if (logs.Count > 0)
            {
                BindToGridView(logs);
            }
        }
        private List<LogData> GetLogsByHysoon(DateTime fromDate, DateTime toDate, ref string message)
        {
            var queryParams = new Dictionary<string, object>()
            {
                { "FromDate", fromDate.Date },
                { "ToDate", toDate.Date.AddDays(1).AddMilliseconds(-1) },
            };
            //var res = CallTimesheetAPI<Dictionary<string, object>>($"{txtLinkByHysoon.Text}:{txtPortByHysoon.Text}/api/Hrm/getTimeKeeperDataLog", Method.GET, null, queryParams, false);

            var client = new RestClient($"{txtLinkByHysoon.Text}:{txtPortByHysoon.Text}/api/Hrm/getTimeKeeperDataLog");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            foreach (var p in queryParams)
            {
                request.AddParameter(p.Key, p.Value, ParameterType.QueryString);
            }
            var response = client.Execute(request);
            var content = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
            var listData = content.GetObject<List<Dictionary<string, object>>>("ReturnData");
            var logs = new List<LogData>();
            listData.ForEach(x =>
            {
                var log = new LogData();
                var userID = x.GetObject<string>("EnrollNumber");
                var time = x.GetObject<string>("ActionDate");
                var checkTime = DateTime.Now;
                if (DateTime.TryParse(time, out checkTime) && !string.IsNullOrWhiteSpace(userID))
                {
                    log.CheckTime = checkTime;
                    log.UserID = userID;
                    logs.Add(log);
                }
            });
            return logs;
        }
    }
}