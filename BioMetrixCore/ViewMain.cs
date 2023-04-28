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
using Mapster;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Data.Odbc;
using System.Data.SqlClient;
using AxFP_CLOCKLib;
using zkemkeeper;
using System.Threading;

namespace BioMetrixCore
{
    public partial class ViewMain : Form
    {
        private AxFP_CLOCK axCLOCK;
        private CZKEM axCZKEM;
        private bool _isConnected;
        private int[] ModeInArr = new int[3] { 0, 3, 4 };
        private int[] ModeOutArr = new int[3] { 1, 2, 5 };
        protected string[] formatDates = { "MM/dd/yyyy HH:mm", "MM/dd/yyyy H:mm", "MM/dd/yyyy HH:m", "MM/dd/yyyy HH:m", "M/dd/yyyy HH:mm", "M/dd/yyyy H:mm", "M/dd/yyyy HH:m", "M/dd/yyyy H:m", "M/d/yyyy HH:mm", "M/d/yyyy H:mm", "M/d/yyyy HH:m", "M/d/yyyy H:m", "MM/d/yyyy HH:mm", "MM/d/yyyy H:mm", "MM/d/yyyy HH:m", "MM/d/yyyy H:m" };

        public ViewMain()
        {
            InitializeComponent();
            ToggleControls(false);
            ShowStatusBar(string.Empty, true);
            DisplayEmpty();

            //var thr = new Thread(() =>
            //{
            //    this.axCLOCK = new AxFP_CLOCK();
            //});
            //thr.SetApartmentState(ApartmentState.STA);
            //thr.Start();
            //this.axCZKEM = new CZKEM();
        }

        DeviceManipulator manipulator = new DeviceManipulator();
        public ZkemClient objZkeeper;
        private bool isDeviceConnected = false;
        private readonly string[] formatDateTimes = { "dd/MM/yyyy HH:mm:ss", "dd/M/yyyy HH:mm:ss", "d/MM/yyyy HH:mm:ss", "d/M/yyyy HH:mm:ss", "M/d/yyyy HH:mm:ss", "MM/d/yyyy HH:mm:ss", "M/d/yyyy HH:mm:ss", "MM/dd/yyyy HH:mm:ss", "M/dd/yyyy HH:mm:ss" };

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
                Logger.LogError($"\btnPullData_Click: {message} ======== Count: {lstMachineInfo.Count}");

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

            if (dgvLogsByBioTime.Controls.Count > 2)
            {
                dgvLogsByBioTime.Controls.RemoveAt(2);
            }
            dgvLogsByBioTime.DataSource = null;
            dgvLogsByBioTime.Controls.Clear();
            dgvLogsByBioTime.Rows.Clear();
            dgvLogsByBioTime.Columns.Clear();

            if (dgvLogsByUbioXFace.Controls.Count > 2)
            {
                dgvLogsByUbioXFace.Controls.RemoveAt(2);
            }
            dgvLogsByUbioXFace.DataSource = null;
            dgvLogsByUbioXFace.Controls.Clear();
            dgvLogsByUbioXFace.Rows.Clear();
            dgvLogsByUbioXFace.Columns.Clear();


            if (dgvLogsByAIKYO.Controls.Count > 2)
            {
                dgvLogsByAIKYO.Controls.RemoveAt(2);
            }
            dgvLogsByAIKYO.DataSource = null;
            dgvLogsByAIKYO.Controls.Clear();
            dgvLogsByAIKYO.Rows.Clear();
            dgvLogsByAIKYO.Columns.Clear();

            if (dgvLogsByRonaldJack.Controls.Count > 2)
            {
                dgvLogsByRonaldJack.Controls.RemoveAt(2);
            }
            dgvLogsByRonaldJack.DataSource = null;
            dgvLogsByRonaldJack.Controls.Clear();
            dgvLogsByRonaldJack.Rows.Clear();
            dgvLogsByRonaldJack.Columns.Clear();
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

            dgvLogsByBioTime.DataSource = list;
            dgvLogsByBioTime.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvLogsByBioTime);

            dgvLogsByUbioXFace.DataSource = list;
            dgvLogsByUbioXFace.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvLogsByUbioXFace);

            dgvLogsByAIKYO.DataSource = list;
            dgvLogsByAIKYO.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvLogsByAIKYO);

            dgvLogsByRonaldJack.DataSource = list;
            dgvLogsByRonaldJack.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvLogsByRonaldJack);
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
        #region BiFace
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
                Logger.LogInfo($"\nKet qua call API: apiPath: {apiPath}-----response: {response.Content}");
                var content = response.Content;
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
        #endregion
        #region GET BSSID
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
        #endregion
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
        #region HanetAI
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
                            while (page * size < total)
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
        #endregion
        #region Hysoon
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
                //var checkTime = DateTime.Now;
                DateTime tempDate;
                var isConvertDateTime = DateTime.TryParseExact(time, formatDateTimes, System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), System.Globalization.DateTimeStyles.None, out tempDate);
                if (isConvertDateTime && !string.IsNullOrWhiteSpace(userID))
                {
                    log.CheckTime = tempDate;
                    log.UserID = userID;
                    logs.Add(log);
                }
            });
            return logs;
        }
        #endregion
        #region BioTime
        private void btnGetLogsByBioTime_Click(object sender, EventArgs e)
        {
            //var a = "\"count\":47,\"next\":null,\"previous\":null,\"msg\":\"\",\"code\":0,\"data\":[{\"emp_code\":\"1\",\"first_name\":\"trieu chung tinh\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-09\",\"punch_times\":1,\"punch_set\":\"15:45:53\"},{\"emp_code\":\"1\",\"first_name\":\"trieu chung tinh\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-11\",\"punch_times\":2,\"punch_set\":\"07:01:08,17:02:09\"},{\"emp_code\":\"1\",\"first_name\":\"trieu chung tinh\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-13\",\"punch_times\":1,\"punch_set\":\"06:52:49\"},{\"emp_code\":\"104\",\"first_name\":\"pham viet hung\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-09\",\"punch_times\":1,\"punch_set\":\"14:01:49\"},{\"emp_code\":\"104\",\"first_name\":\"pham viet hung\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":1,\"punch_set\":\"05:47:58\"},{\"emp_code\":\"108\",\"first_name\":\"ta ngoc tuan\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":2,\"punch_set\":\"06:10:14,17:00:19\"},{\"emp_code\":\"108\",\"first_name\":\"ta ngoc tuan\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-11\",\"punch_times\":2,\"punch_set\":\"06:50:53,17:01:03\"},{\"emp_code\":\"11\",\"first_name\":\"ha phuong lam\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":2,\"punch_set\":\"06:24:07,17:03:57\"},{\"emp_code\":\"11\",\"first_name\":\"ha phuong lam\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-11\",\"punch_times\":1,\"punch_set\":\"07:55:26\"},{\"emp_code\":\"11\",\"first_name\":\"ha phuong lam\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-13\",\"punch_times\":1,\"punch_set\":\"07:25:31\"},{\"emp_code\":\"128\",\"first_name\":\"dang van nhi\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-09\",\"punch_times\":1,\"punch_set\":\"15:45:38\"},{\"emp_code\":\"128\",\"first_name\":\"dang van nhi\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":2,\"punch_set\":\"05:35:28,17:03:49\"},{\"emp_code\":\"128\",\"first_name\":\"dang van nhi\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-11\",\"punch_times\":2,\"punch_set\":\"06:51:55,17:00:58\"},{\"emp_code\":\"128\",\"first_name\":\"dang van nhi\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-13\",\"punch_times\":1,\"punch_set\":\"06:52:55\"},{\"emp_code\":\"139\",\"first_name\":\"do dinh huan\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":1,\"punch_set\":\"08:31:49\"},{\"emp_code\":\"14\",\"first_name\":\"han duc tho\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-13\",\"punch_times\":1,\"punch_set\":\"14:06:12\"},{\"emp_code\":\"16\",\"first_name\":\"hoang phuc duc\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-11\",\"punch_times\":2,\"punch_set\":\"06:46:17,16:23:08\"},{\"emp_code\":\"16\",\"first_name\":\"hoang phuc duc\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-13\",\"punch_times\":1,\"punch_set\":\"06:52:42\"},{\"emp_code\":\"24\",\"first_name\":\"le minh  man\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":1,\"punch_set\":\"05:39:01\"},{\"emp_code\":\"24\",\"first_name\":\"le minh  man\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-11\",\"punch_times\":1,\"punch_set\":\"07:01:02\"},{\"emp_code\":\"24\",\"first_name\":\"le minh  man\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-13\",\"punch_times\":1,\"punch_set\":\"06:55:31\"},{\"emp_code\":\"36\",\"first_name\":\"luong truong  son\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-09\",\"punch_times\":1,\"punch_set\":\"15:45:05\"},{\"emp_code\":\"36\",\"first_name\":\"luong truong  son\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":1,\"punch_set\":\"17:04:03\"},{\"emp_code\":\"38\",\"first_name\":\"maid vancuong\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-09\",\"punch_times\":1,\"punch_set\":\"15:15:02\"},{\"emp_code\":\"38\",\"first_name\":\"maid vancuong\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":2,\"punch_set\":\"05:35:07,17:00:08\"},{\"emp_code\":\"38\",\"first_name\":\"maid vancuong\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-11\",\"punch_times\":2,\"punch_set\":\"06:52:13,17:00:45\"},{\"emp_code\":\"38\",\"first_name\":\"maid vancuong\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-13\",\"punch_times\":1,\"punch_set\":\"06:51:54\"},{\"emp_code\":\"39\",\"first_name\":\"ngo quang ha\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-09\",\"punch_times\":1,\"punch_set\":\"14:04:11\"},{\"emp_code\":\"39\",\"first_name\":\"ngo quang ha\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":1,\"punch_set\":\"06:06:50\"},{\"emp_code\":\"39\",\"first_name\":\"ngo quang ha\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-13\",\"punch_times\":6,\"punch_set\":\"07:14:02,07:32:18,07:50:21,08:03:36,08:10:23,14:06:26\"},{\"emp_code\":\"4\",\"first_name\":\"cao thanh at\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":1,\"punch_set\":\"05:38:09\"},{\"emp_code\":\"49\",\"first_name\":\"nguyen hong duc\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":1,\"punch_set\":\"05:41:29\"},{\"emp_code\":\"55\",\"first_name\":\"nguyen ngoc sy\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-09\",\"punch_times\":4,\"punch_set\":\"14:02:47,14:04:07,15:10:14,15:23:53\"},{\"emp_code\":\"56\",\"first_name\":\"nguyen quang son\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":1,\"punch_set\":\"06:10:07\"},{\"emp_code\":\"58\",\"first_name\":\"nguyen thanh mai\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":2,\"punch_set\":\"05:46:01,17:00:25\"},{\"emp_code\":\"75\",\"first_name\":\"nguyen van anh\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":2,\"punch_set\":\"05:40:21,17:00:11\"},{\"emp_code\":\"75\",\"first_name\":\"nguyen van anh\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-11\",\"punch_times\":1,\"punch_set\":\"07:01:25\"},{\"emp_code\":\"79\",\"first_name\":\"nguyen van khoi\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-13\",\"punch_times\":1,\"punch_set\":\"14:09:02\"},{\"emp_code\":\"80\",\"first_name\":\"nguyen van mau\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-09\",\"punch_times\":1,\"punch_set\":\"15:10:26\"},{\"emp_code\":\"80\",\"first_name\":\"nguyen van mau\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":1,\"punch_set\":\"17:03:43\"},{\"emp_code\":\"80\",\"first_name\":\"nguyen van mau\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-13\",\"punch_times\":1,\"punch_set\":\"13:01:35\"},{\"emp_code\":\"82\",\"first_name\":\"nguyen van tan\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-09\",\"punch_times\":1,\"punch_set\":\"15:45:31\"},{\"emp_code\":\"87\",\"first_name\":\"nguyen  xuan quynh\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-10\",\"punch_times\":2,\"punch_set\":\"05:49:08,17:00:00\"},{\"emp_code\":\"92\",\"first_name\":\"nguyen duc tho\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-09\",\"punch_times\":1,\"punch_set\":\"15:44:22\"},{\"emp_code\":\"92\",\"first_name\":\"nguyen duc tho\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-11\",\"punch_times\":1,\"punch_set\":\"17:02:32\"},{\"emp_code\":\"92\",\"first_name\":\"nguyen duc tho\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-12\",\"punch_times\":1,\"punch_set\":\"06:24:58\"},{\"emp_code\":\"92\",\"first_name\":\"nguyen duc tho\",\"last_name\":null,\"nick_name\":null,\"gender\":null,\"dept_code\":\"1\",\"dept_name\":\"Department\",\"position_code\":null,\"position_name\":null,\"att_date\":\"2023-02-13\",\"punch_times\":1,\"punch_set\":\"06:18:54\"}]";
            //var content = Converter.JsonDeserialize<Dictionary<string, object>>(a);
            var logs = GetLogsByBioTime(dtFromDateByBioTime.Value, dtToDateByBioTime.Value, 1);
            
            //var a = new Dictionary<string, object>()
            //{
            //    { "emp_code", "108" },
            //    { "att_date", "2023-02-10" },
            //    { "punch_set", "06:10:14,17:00:19" }
            //};
            //var userID = a.GetObject<string>("emp_code");
            //var date = a.GetObject<DateTime>("att_date");
            //var times = a.GetObject<string>("punch_set");
            //var listTime = !string.IsNullOrWhiteSpace(times) ? times.Replace(';', ',').Split(',').ToList() : new List<string>();
            //listTime.ForEach(z =>
            //{
            //    var log = new LogData();
            //    var time = TimeSpan.Parse(z);
            //    var checkTime = date.Add(time);
            //    log.CheckTime = checkTime;
            //    log.UserID = userID;
            //    logs.Add(log);
            //});
            if (logs.Count > 0)
            {
                txtTotalByBioTime.Text = logs.Count.ToString();
                BindToGridView(logs);
            }
        }
        private string LoginByBioTime(int type)
        {
            var token = string.Empty;
            try
            {
                var param = new
                {
                    username = txtUserNameByBioTime.Text,
                    password = txtPassByBioTime.Text
                };

                var apiPath = $"{txtURLByBioTime.Text}:{txtPortByBioTime.Text}/api-token-auth/";
                if (type == 2)
                {
                    apiPath = $"{txtURLByBioTime.Text}:{txtPortByBioTime.Text}/jwt-api-token-auth/";
                }
                var client = new RestClient(apiPath);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);

                // Header
                request.AddHeader("Content-Type", "application/json");

                //Body
                if (param != null)
                {
                    request.AddParameter("application/json", Converter.JsonSerialize(param), ParameterType.RequestBody);
                }

                Logger.LogInfo($"\n================LoginByBioTime : Bat dau login by Bio Time: apiPath: {apiPath}==============");
                var response = client.Execute(request);
                Logger.LogInfo($"\nKet qua login: success: {response.IsSuccessful}-----data: {Converter.JsonSerialize(response.Content)}----ErrorMessage: {response.ErrorMessage}");
                if (response.IsSuccessful)
                {
                    var content = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
                    token = content.GetObject<string>("token");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n==============LoginByBioTime Exception: Err: {ex.Message}==============");
            }
            return token;
        }
        private List<int> GetDepartmentByBioTime(string token, int type)
        {
            var ids = new List<int>();
            try
            {
                var client = new RestClient($"{txtURLByBioTime.Text}:{txtPortByBioTime.Text}/personnel/api/departments/");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);

                // Header
                request.AddHeader("Content-Type", "application/json");
                if (type == 1)
                {
                    request.AddHeader("Authorization", $"Token {token}");
                }
                else
                {
                    request.AddHeader("Authorization", $"JWT {token}");
                }

                Logger.LogInfo($"\n==============GetDepartmentByBioTime : Bat dau lay phong ban by Bio Time: type: {type}----token: {token}==============");
                var response = client.Execute(request);
                Logger.LogInfo($"\nKet qua lay phong ban: success: {response.IsSuccessful}-----data: {Converter.JsonSerialize(response.Content)}----ErrorMessage: {response.ErrorMessage}");
                if (response.IsSuccessful)
                {
                    var content = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
                    var datas = content.GetObject<List<Dictionary<string, object>>>("data");
                    datas.ForEach(x =>
                    {
                        var id = x.ContainsKey("id") ? x["id"].ToString() : null;
                        var depID = 0;
                        if (!string.IsNullOrWhiteSpace(id) && int.TryParse(id, out depID))
                        {
                            ids.Add(depID);
                        }
                    });
                }
            }
            catch(Exception ex)
            {
                Logger.LogError($"\n==============GetDepartmentByBioTime Exception: Err: {ex.Message}==============");
            }
            return ids;
        }
        private List<LogData> GetLogsByBioTime(DateTime? fromDate, DateTime? toDate, int type)
        {
            var logs = new List<LogData>();
            try
            {
                var token = LoginByBioTime(type);
                var departmnetIDs = GetDepartmentByBioTime(token, type);
                departmnetIDs.ForEach(x =>
                {
                    var count = 0;
                    var pageIndex = 0;
                    var pageSize = 100;
                    do
                    {
                        pageIndex++;
                        var queryParams = new Dictionary<string, object>()
                        {
                            { "page", pageIndex },
                            { "page_size", pageSize},
                            { "start_date", dtFromDateByBioTime.Value.Date.ToString("yyyy-MM-dd") },
                            { "end_date", dtToDateByBioTime.Value.Date.AddDays(1).AddMilliseconds(-1).ToString("yyyy-MM-dd") },
                            { "departments", x },
                            { "areas", -1 },
                            { "groups", -1 },
                            { "employees", -1 },
                        };
                        var client = new RestClient($"{txtURLByBioTime.Text}:{txtPortByBioTime.Text}/att/api/timeCardReport/");
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);

                        //Header
                        request.AddHeader("Content-Type", "application/json");
                        if (type == 1)
                        {
                            request.AddHeader("Authorization", $"Token {token}");
                        }
                        else
                        {
                            request.AddHeader("Authorization", $"JWT {token}");
                        }

                        foreach (var p in queryParams)
                        {
                            request.AddParameter(p.Key, p.Value, ParameterType.QueryString);
                        }

                        Logger.LogInfo($"\n==============GetLogsByBioTime : Bat dau lay du lieu by Bio Time: type: {type}----token: {token}==============");
                        var response = client.Execute(request);
                        Logger.LogInfo($"\nKet qua lay du lieu: success: {response.IsSuccessful}-----data: {Converter.JsonSerialize(response.Content)}----ErrorMessage: {response.ErrorMessage}");
                        if (response.IsSuccessful)
                        {
                            var content = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
                            var datas = content.GetObject<List<Dictionary<string, object>>>("data");
                            count = content.GetObject<int>("count");
                            datas.ForEach(y =>
                            {
                                var userID = y.GetObject<string>("emp_code");
                                var date = y.GetObject<DateTime>("att_date");
                                var times = y.GetObject<string>("punch_set");
                                var listTime = !string.IsNullOrWhiteSpace(times) ? times.Replace(';', ',').Split(',').ToList() : new List<string>();
                                listTime.ForEach(z =>
                                {
                                    var log = new LogData();
                                    var time = TimeSpan.Parse(z);
                                    var checkTime = date.Add(time);
                                    log.CheckTime = checkTime;
                                    log.UserID = userID;
                                    logs.Add(log);
                                });
                            });
                        }
                    }
                    while (pageIndex * pageSize < count);
                });
            }
            catch(Exception ex)
            {
                Logger.LogError($"\n==============GetLogsByBioTime Exception: Err: {ex.Message}==============");
            }
            return logs.OrderBy(x => x.CheckTime).ToList();
        }
        private void btnGetLogs_V2ByBioTime_Click(object sender, EventArgs e)
        {
            var mess = string.Empty;
            var logs = GetLogsByBioTime(dtFromDateByBioTime.Value, dtToDateByBioTime.Value, 2);
            if (logs.Count > 0)
            {
                txtTotalByBioTime.Text = logs.Count.ToString();
                BindToGridView(logs);
            }
        }
        #endregion
        #region Ubio XFace
        private void btnGetLogsByUbioXFace_Click(object sender, EventArgs e)
        {
            var ucsinfo = string.Empty;
            var extinfo = string.Empty;
            var cookies = LoginByUbioXFace(ref ucsinfo, ref extinfo);
            var logs = GetLogsByUbioXFace(cookies, ucsinfo, extinfo);
            logs = logs.OrderBy(x => x.CheckTime).ToList();
            if (logs.Count > 0)
            {
                txtTotalByUbioXFace.Text = logs.Count.ToString();
                BindToGridView(logs);
            }
        }
        private Dictionary<string, Cookie> LoginByUbioXFace(ref string ucsinfo, ref string extinfo)
        {
            var cookies = new Dictionary<string, Cookie>();
            try
            {
                var client = new RestClient($"{txtURLByUbioXFace.Text}:{txtPortByUbioXFace.Text}/v1/login");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);

                // Header
                int typeLoginByUbioXFace = 2;
                int.TryParse(Utility.GetAppSetting("TypeLoginByUbioXFace"), out typeLoginByUbioXFace);
                if (typeLoginByUbioXFace == 1)
                {
                    request.AddHeader("Content-Type", "application/json");
                    request.AddJsonBody("test1=2&test2=3&%40d1%23userId=Master&%40d1%23password=0000&%40d1%23userType=2&%40d%23=%40d1%23&%40d1%23=dmLoginReq&%40d1%23tp=dm&");
                }
                else if(typeLoginByUbioXFace == 2)
                {
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("test1", 2);
                    request.AddParameter("test2", 3);
                    request.AddParameter("%40d1%23userId", txtUserNameByUbioXFace.Text);
                    request.AddParameter("%40d1%23password", txtPassByUbioXFace.Text);
                    request.AddParameter("%40d1%23userType", 2);
                    request.AddParameter("%40d%23", "%40d1%23");
                    request.AddParameter("%40d1%23", "dmLoginReq");
                    request.AddParameter("%40d1%23tp", "dm");
                }
                else if(typeLoginByUbioXFace == 3)
                {
                    request.AddParameter("application/x-www-form-urlencoded", $"test1=2&test2=3&%40d1%23userId={txtUserNameByUbioXFace.Text}&%40d1%23password={txtPassByUbioXFace.Text}&%40d1%23userType=2&%40d%23=%40d1%23&%40d1%23=dmLoginReq&%40d1%23tp=dm&", ParameterType.RequestBody);
                }
                else
                {
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("test1=2&test2=3&%40d1%23userId=Master&%40d1%23password=0000&%40d1%23userType=2&%40d%23=%40d1%23&%40d1%23=dmLoginReq&%40d1%23tp=dm&", "");
                }

                Logger.LogInfo($"\n================LoginByUbioXFace : Bat dau login by UbioXFace==================");
                var response = client.Execute(request);
                Logger.LogInfo($"\nKet qua login: success: {response.IsSuccessful}-----data: {Converter.JsonSerialize(response.Content)}----ErrorMessage: {response.ErrorMessage}");
                if (response.IsSuccessful)
                {
                    var content = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
                    //Get cookies
                    if (response.Cookies != null)
                    {
                        Logger.LogInfo($"\n==========LoginByUbioXFace-------Ket qua login: success: {response.IsSuccessful}-----Cookies: {Converter.JsonSerialize(response.Cookies)}");
                        foreach (var c in response.Cookies)
                        {
                            cookies.Add(c.Name, c.Adapt<Cookie>());
                            if(c.Name == "ucsinfo")
                            {
                                ucsinfo = c.Adapt<Cookie>().Value;
                            }
                            if (c.Name == "extinfo")
                            {
                                extinfo = c.Adapt<Cookie>().Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo($"\n================LoginByUbioXFace : Exception: {ex.Message}==================");
            }
            return cookies;
        }
        private List<LogData> GetLogsByUbioXFace(Dictionary<string, Cookie> cookies, string ucsinfo, string extinfo)
        {
            var logs = new List<LogData>();
            try
            {
                var pageSize = 100;
                var pageIndex = -1;
                var count = 0;
                do
                {
                    pageIndex++;
                    var client = new RestClient($"{txtURLByUbioXFace.Text}:{txtPortByUbioXFace.Text}/v1/authLogs");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);

                    var ucsinfoConfig = ucsinfo;
                    var extinfoConfig = extinfo;
                    if (!string.IsNullOrWhiteSpace(Utility.GetAppSetting("ucsinfo")) && !string.IsNullOrWhiteSpace(Utility.GetAppSetting("extinfo")))
                    {
                        ucsinfoConfig = Utility.GetAppSetting("ucsinfo");
                        extinfoConfig = Utility.GetAppSetting("extinfo");
                    }

                    if (!string.IsNullOrWhiteSpace(ucsinfoConfig) && !string.IsNullOrWhiteSpace(extinfoConfig))
                    {
                        request.AddCookie("ucsinfo", ucsinfoConfig);
                        request.AddCookie("extinfo", extinfoConfig);
                    }

                    // Header
                    //request.AddHeader("Content-Type", "application/json");

                    var queryParams = new Dictionary<string, object>()
                    {
                        { "startTime", dtFromDateByUbioXFace.Value.Date.ToString("yyyy-MM-dd HH:mm:ss") },
                        { "endTime", dtToDateByUbioXFace.Value.Date.AddDays(1).AddMilliseconds(-1).ToString("yyyy-MM-dd HH:mm:ss") },
                        { "offset", pageSize * pageIndex },
                        { "limit", pageSize },
                        { "groupID", 0 },
                        { "searchCategory", "all" },
                    };
                    foreach (var p in queryParams)
                    {
                        request.AddParameter(p.Key, p.Value, ParameterType.QueryString);
                    }

                    //if (queryParams != null)
                    //{
                    //    request.AddParameter("application/json", Converter.JsonSerialize(queryParams), ParameterType.RequestBody);
                    //}
                    Logger.LogInfo($"\n==============GetLogsByUbioXFace: Bat dau lay du lieu by Ubio XFace: cookies: {Converter.JsonSerialize(cookies)}--ucsinfo: {ucsinfoConfig}---extinfo: {extinfoConfig}-==============");
                    var response = client.Execute(request);
                    Logger.LogInfo($"\nKet qua lay du lieu: success: {response.IsSuccessful}-----data: {Converter.JsonSerialize(response.Content)}----ErrorMessage: {response.ErrorMessage}");
                    if (response.IsSuccessful)
                    {
                        Logger.LogInfo($"\nKet qua lay du lieu:----total: {count}----pageIndex: {pageIndex}----queryParams: {Converter.JsonSerialize(queryParams)}");
                        var content = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
                        var datas = content.GetObject<List<Dictionary<string, object>>>("AuthLogList");
                        var total = content.GetObject<Dictionary<string, object>>("Total");
                        count = total.GetObject<int>("Count");
                        datas.ForEach(y =>
                        {
                            var userID = y.GetObject<string>("UserID");
                            var date = y.GetObject<string>("EventTime");
                            var checkTime = DateTime.Now;
                            if (DateTime.TryParse(date, out checkTime) && !string.IsNullOrWhiteSpace(userID))
                            {
                                var log = new LogData();
                                log.CheckTime = checkTime;
                                log.UserID = userID;
                                logs.Add(log);
                            }
                        });
                    }
                }
                while (pageSize * pageIndex < count);
            }
            catch(Exception ex)
            {
                Logger.LogInfo($"\n==============GetLogsByUbioXFace : Exception: {ex.Message}==============");
            }
            return logs;
        }
        #endregion

        #region Ronald jack
        private bool m_bDeviceOpened = false;
        private int m_nCurSelID = 1;
        private AxFP_CLOCKLib.AxFP_CLOCK pOcxObject;
        private int m_nMachineNum;
        private void btnOpenDevice_Click(object sender, EventArgs e)
        {
            bool bRet;

            if (m_bDeviceOpened)
            {
                btnOpenDev.Text = "Open";
                m_bDeviceOpened = false;

                axFP_CLOCK.CloseCommPort();
                return;
            }

            this.axFP_CLOCK.OpenCommPort(m_nCurSelID);
            int nConnecttype = this.cmbInterface.SelectedIndex;

            switch (nConnecttype)
            {
                case (int)CURDEVICETYPE.DEVICE_COM:
                    {
                        this.axFP_CLOCK.CommPort = this.cmbComPort.SelectedIndex + 1;
                        axFP_CLOCK.Baudrate = 38400;

                    }
                    break;
                case (int)CURDEVICETYPE.DEVICE_NET:
                    {
                        int nPort = Convert.ToInt32(textPort.Text);
                        int nPassword = Convert.ToInt32(textPassword.Text);
                        string strIP = ipAddressControl1.IPAddress.ToString();
                        bRet = axFP_CLOCK.SetIPAddress(ref strIP, nPort, nPassword);
                        if (!bRet)
                        {
                            return;
                        }

                    }
                    break;
                case (int)CURDEVICETYPE.DEVICE_USB:
                    {
                        axFP_CLOCK.IsUSB = true;

                    }
                    break;
                case (int)CURDEVICETYPE.DEVICE_P2S:
                    {
                        int nPort = Convert.ToInt32(P2SPort.Text);
                        int nTimeOut = Convert.ToInt32(P2STimeOut.Text);

                        axFP_CLOCK.SetServerPortandtick(nPort, nTimeOut);
                    }
                    break;
            }

            bRet = axFP_CLOCK.OpenCommPort(m_nCurSelID);
            if (bRet)
            {
                m_bDeviceOpened = true;
                btnOpenDev.Text = "Close";
            }
        }
        private void btnOpenDev_Click(object sender, EventArgs e)
        {
            bool bRet;

            if (m_bDeviceOpened)
            {
                btnOpenDev.Text = "Open";
                m_bDeviceOpened = false;

                axFP_CLOCK.CloseCommPort();
                return;
            }

            this.axFP_CLOCK.OpenCommPort(m_nCurSelID);
            int nConnecttype = this.cmbInterface.SelectedIndex;

            switch (nConnecttype)
            {
                case (int)CURDEVICETYPE.DEVICE_COM:
                    {
                        this.axFP_CLOCK.CommPort = this.cmbComPort.SelectedIndex + 1;
                        axFP_CLOCK.Baudrate = 38400;

                    }
                    break;
                case (int)CURDEVICETYPE.DEVICE_NET:
                    {
                        int nPort = Convert.ToInt32(textPort.Text);
                        int nPassword = Convert.ToInt32(textPassword.Text);
                        string strIP = ipAddressControl1.IPAddress.ToString();
                        bRet = axFP_CLOCK.SetIPAddress(ref strIP, nPort, nPassword);
                        if (!bRet)
                        {
                            return;
                        }

                    }
                    break;
                case (int)CURDEVICETYPE.DEVICE_USB:
                    {
                        axFP_CLOCK.IsUSB = true;

                    }
                    break;
                case (int)CURDEVICETYPE.DEVICE_P2S:
                    {
                        int nPort = Convert.ToInt32(P2SPort.Text);
                        int nTimeOut = Convert.ToInt32(P2STimeOut.Text);

                        axFP_CLOCK.SetServerPortandtick(nPort, nTimeOut);
                    }
                    break;
            }

            bRet = axFP_CLOCK.OpenCommPort(m_nCurSelID);
            if (bRet)
            {
                m_bDeviceOpened = true;
                btnOpenDev.Text = "Close";
                this.pOcxObject = axFP_CLOCK;
            }
        }
        private void InitSLogListView()
        {
            listView1.Clear();
            listView1.Columns.Add("", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("TMNo", 50, HorizontalAlignment.Left);
            listView1.Columns.Add("SEnlNo", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("SMNo", 50, HorizontalAlignment.Left);
            listView1.Columns.Add("GEnlNo", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("GMNo", 50, HorizontalAlignment.Left);
            listView1.Columns.Add("Manipulation", 190, HorizontalAlignment.Left);
            listView1.Columns.Add("FpNo", 50, HorizontalAlignment.Left);
            listView1.Columns.Add("DateTime", 110, HorizontalAlignment.Left);
        }
        private bool DisableDevice()
        {
            labelInfo.Text = "Working...";
            bool bRet = pOcxObject.EnableDevice(m_nMachineNum, 0);
            if (bRet)
            {
                labelInfo.Text = "Disable Device Success!";
                return true;
            }
            else
            {
                labelInfo.Text = "No Device...";
                return false;
            }
        }
        private void ShowErrorInfo()
        {
            int nErrorValue = 0;
            pOcxObject.GetLastError(ref nErrorValue);
            labelInfo.Text = common.FormErrorStr(nErrorValue);
        }
        private void btnReadSLogData_Click(object sender, EventArgs e)
        {
            InitSLogListView();

            DisableDevice();

            pOcxObject.ReadMark = checkBox1.Checked;

            bool bRet;
            bRet = pOcxObject.ReadSuperLogData(m_nMachineNum);
            if (!bRet)
            {
                ShowErrorInfo();

                pOcxObject.EnableDevice(m_nMachineNum, 1);

                return;
            }

            SuperLogInfo sLogInfo = new SuperLogInfo();
            List<SuperLogInfo> myArray = new List<SuperLogInfo>();

            do
            {
                bRet = pOcxObject.GetSuperLogData(
                    m_nMachineNum,
                    ref sLogInfo.dwTMachineNumber,
                    ref sLogInfo.dwSEnrollNumber,
                    ref sLogInfo.dwSEMachineNumber,
                    ref sLogInfo.dwGEMachineNumber,
                    ref sLogInfo.dwGEMachineNumber,
                    ref sLogInfo.dwManipulation,
                    ref sLogInfo.dwFingerNumber,
                    ref sLogInfo.dwYear,
                    ref sLogInfo.dwMonth,
                    ref sLogInfo.dwDay,
                    ref sLogInfo.dwHour,
                    ref sLogInfo.dwMinute
                    );

                if (bRet)
                {
                    myArray.Add(sLogInfo);
                }

            } while (bRet);

            int i = 1;
            String str;
            foreach (SuperLogInfo sInfo in myArray)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = i.ToString();
                i++;

                lvi.SubItems.Add(sInfo.dwTMachineNumber.ToString());
                lvi.SubItems.Add(sInfo.dwSEnrollNumber.ToString("D8"));
                lvi.SubItems.Add(sInfo.dwSEMachineNumber.ToString());

                lvi.SubItems.Add(sInfo.dwGEnrollNumber.ToString());  //GEnlNo
                lvi.SubItems.Add(sInfo.dwGEMachineNumber.ToString());

                str = sInfo.dwManipulation.ToString("0--") + common.FormSLogStr(sInfo.dwManipulation);
                lvi.SubItems.Add(str);                                          // Verify Mode

                if (sInfo.dwFingerNumber < 10)
                {
                    str = sInfo.dwFingerNumber.ToString();
                }
                else if (sInfo.dwFingerNumber == 10)
                {
                    str = "Password";

                }
                else
                {
                    str = "Card";

                }

                lvi.SubItems.Add(str);

                //如果要提高效率
                DateTime dt = new DateTime(sInfo.dwYear,
                    sInfo.dwMonth,
                    sInfo.dwDay,
                    sInfo.dwHour,
                    sInfo.dwMinute,
                    0);

                lvi.SubItems.Add(dt.ToString("yyyy/MM/dd HH:mm"));


                listView1.Items.Add(lvi);

            }
            i -= 1;
            labelTotal.Text = i.ToString("Total Read 0");

            labelInfo.Text = "GetSuperLogData Success...";
            pOcxObject.EnableDevice(m_nMachineNum, 1);

        }
        private void btnReadAllSLogData_Click(object sender, EventArgs e)
        {
            InitSLogListView();

            DisableDevice();

            bool bRet;
            bRet = pOcxObject.ReadSuperLogData(m_nMachineNum);
            if (!bRet)
            {
                ShowErrorInfo();

                pOcxObject.EnableDevice(m_nMachineNum, 1);

                return;
            }

            SuperLogInfo sLogInfo = new SuperLogInfo();
            List<SuperLogInfo> myArray = new List<SuperLogInfo>();

            do
            {
                bRet = pOcxObject.GetAllSLogData(
                    m_nMachineNum,
                    ref sLogInfo.dwTMachineNumber,
                    ref sLogInfo.dwSEnrollNumber,
                    ref sLogInfo.dwSEMachineNumber,
                    ref sLogInfo.dwGEMachineNumber,
                    ref sLogInfo.dwGEMachineNumber,
                    ref sLogInfo.dwManipulation,
                    ref sLogInfo.dwFingerNumber,
                    ref sLogInfo.dwYear,
                    ref sLogInfo.dwMonth,
                    ref sLogInfo.dwDay,
                    ref sLogInfo.dwHour,
                    ref sLogInfo.dwMinute
                    );

                if (bRet)
                {
                    myArray.Add(sLogInfo);
                }

            } while (bRet);

            int i = 1;
            String str;
            foreach (SuperLogInfo sInfo in myArray)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = i.ToString();
                i++;

                lvi.SubItems.Add(sInfo.dwTMachineNumber.ToString());
                lvi.SubItems.Add(sInfo.dwSEnrollNumber.ToString("D8"));
                lvi.SubItems.Add(sInfo.dwSEMachineNumber.ToString());

                lvi.SubItems.Add(sInfo.dwGEnrollNumber.ToString("D8"));  //GEnlNo
                lvi.SubItems.Add(sInfo.dwGEMachineNumber.ToString());

                str = sInfo.dwManipulation.ToString("0--") + common.FormSLogStr(sInfo.dwManipulation);
                lvi.SubItems.Add(str);                                          // Verify Mode

                if (sInfo.dwFingerNumber < 10)
                {
                    str = sInfo.dwFingerNumber.ToString();
                }
                else if (sInfo.dwFingerNumber == 10)
                {
                    str = "Password";

                }
                else
                {
                    str = "Card";

                }

                lvi.SubItems.Add(str);

                //如果要提高效率
                DateTime dt = new DateTime(sInfo.dwYear,
                    sInfo.dwMonth,
                    sInfo.dwDay,
                    sInfo.dwHour,
                    sInfo.dwMinute,
                    0);

                lvi.SubItems.Add(dt.ToString("yyyy/MM/dd HH:mm"));


                listView1.Items.Add(lvi);

            }

            i -= 1;
            labelTotal.Text = i.ToString("Total Read 0");

            labelInfo.Text = "GetAllSLogData Success...";
            pOcxObject.EnableDevice(m_nMachineNum, 1);
        }
        private void btnEmptySLogData_Click(object sender, EventArgs e)
        {
            bool bRet;

            DisableDevice();

            bRet = pOcxObject.EmptySuperLogData(m_nMachineNum);
            if (bRet)
            {
                labelInfo.Text = "EmptySuperLogData OK";
            }
            else
            {
                ShowErrorInfo();
            }

            pOcxObject.EnableDevice(m_nMachineNum, 1);
        }
        private void InitGLogListView()
        {
            listView1.Clear();
            listView1.Columns.Add("", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("TMchNo", 90, HorizontalAlignment.Left);
            listView1.Columns.Add("EnrollNo", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("EMchNo", 90, HorizontalAlignment.Left);     //
            listView1.Columns.Add("InOut", 60, HorizontalAlignment.Left);
            listView1.Columns.Add("VeriMode", 130, HorizontalAlignment.Left);
            listView1.Columns.Add("DateTime", 130, HorizontalAlignment.Left);
        }
        private void UDGLogRead_Click(object sender, EventArgs e)
        {
            string strFilePath;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                strFilePath = openFileDialog1.FileName;

            }
            else
            {
                return;
            }

            bool bRet;
            bRet = pOcxObject.USBReadGeneralLogData(strFilePath);
            if (!bRet)
            {
                ShowErrorInfo();
                return;
            }

            GeneralLogInfo gLogInfo = new GeneralLogInfo();
            List<GeneralLogInfo> myArray = new List<GeneralLogInfo>();

            do
            {
                bRet = pOcxObject.GetAllGLogData(m_nMachineNum,
                ref gLogInfo.dwTMachineNumber,
                ref gLogInfo.dwEnrollNumber,
                ref gLogInfo.dwEMachineNumber,
                ref gLogInfo.dwVerifyMode,
                ref gLogInfo.dwYear,
                ref gLogInfo.dwMonth,
                ref gLogInfo.dwDay,
                ref gLogInfo.dwHour,
                ref gLogInfo.dwMinute
                );

                if (bRet)
                {
                    myArray.Add(gLogInfo);
                }

            } while (bRet);

            InitGLogListView();

            int i = 1;
            string str;
            foreach (GeneralLogInfo gInfo in myArray)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = i.ToString();
                i++;

                lvi.SubItems.Add(gInfo.dwTMachineNumber.ToString());
                lvi.SubItems.Add(gInfo.dwEnrollNumber.ToString("D8"));
                lvi.SubItems.Add(gInfo.dwEMachineNumber.ToString());

                int nInOut = gInfo.dwVerifyMode / 8;
                lvi.SubItems.Add(nInOut.ToString());                             //INOUT

                str = common.FormString(gInfo.dwVerifyMode, gInfo.dwEnrollNumber);
                lvi.SubItems.Add(str);                                          // Verify Mode

                DateTime dt = new DateTime(gInfo.dwYear,
                    gInfo.dwMonth,
                    gInfo.dwDay,
                    gInfo.dwHour,
                    gInfo.dwMinute,
                    0);

                lvi.SubItems.Add(dt.ToString("yyyy/MM/dd HH:mm"));


                listView1.Items.Add(lvi);

            }

            i -= 1;
            labelTotal.Text = i.ToString("Total Read 0");

            pOcxObject.EnableDevice(m_nMachineNum, 1);
        }
        private void btnReadGLogData_Click(object sender, EventArgs e)
        {
            InitGLogListView();

            bool bRet;
            GeneralLogInfo gLogInfo = new GeneralLogInfo();

            List<GeneralLogInfo> myArray = new List<GeneralLogInfo>();

            // if true, only read new log
            pOcxObject.ReadMark = checkBox1.Checked;

            DisableDevice();

            bRet = pOcxObject.ReadGeneralLogData(m_nMachineNum);
            if (!bRet)
            {
                ShowErrorInfo();

                pOcxObject.EnableDevice(m_nMachineNum, 1);
                return;
            }

            do
            {
                bRet = pOcxObject.GetGeneralLogData(m_nMachineNum,
                ref gLogInfo.dwTMachineNumber,
                ref gLogInfo.dwEnrollNumber,
                ref gLogInfo.dwEMachineNumber,
                ref gLogInfo.dwVerifyMode,
                ref gLogInfo.dwYear,
                ref gLogInfo.dwMonth,
                ref gLogInfo.dwDay,
                ref gLogInfo.dwHour,
                ref gLogInfo.dwMinute
                );

                if (bRet)
                {
                    myArray.Add(gLogInfo);
                }

            } while (bRet);

            int i = 1;
            foreach (GeneralLogInfo gInfo in myArray)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = i.ToString();
                i++;

                lvi.SubItems.Add(gInfo.dwTMachineNumber.ToString());
                lvi.SubItems.Add(gInfo.dwEnrollNumber.ToString("D8"));
                lvi.SubItems.Add(gInfo.dwEMachineNumber.ToString());

                int nInOut = gInfo.dwVerifyMode / 8;
                lvi.SubItems.Add(nInOut.ToString());                             //INOUT

                string str = common.FormString(gInfo.dwVerifyMode, gInfo.dwEnrollNumber);
                lvi.SubItems.Add(str);                                          // Verify Mode

                DateTime dt = new DateTime(gInfo.dwYear,
                    gInfo.dwMonth,
                    gInfo.dwDay,
                    gInfo.dwHour,
                    gInfo.dwMinute,
                    0);

                lvi.SubItems.Add(dt.ToString("yyyy/MM/dd HH:mm"));


                listView1.Items.Add(lvi);

            }

            labelInfo.Text = "success...";

            i -= 1;
            labelTotal.Text = i.ToString("Total Read 0");

            pOcxObject.EnableDevice(m_nMachineNum, 1);
        }
        private void btnEmptyGLogData_Click(object sender, EventArgs e)
        {
            bool bRet;

            DisableDevice();

            bRet = pOcxObject.EmptyGeneralLogData(m_nMachineNum);
            if (bRet)
            {
                labelInfo.Text = "EmptyGeneralLogData OK";
            }
            else
            {
                ShowErrorInfo();
            }

            pOcxObject.EnableDevice(m_nMachineNum, 1);

        }
        private void btnReadAllGLogData_Click(object sender, EventArgs e)
        {
            InitGLogListView();

            bool bRet;
            GeneralLogInfo gLogInfo = new GeneralLogInfo();

            List<GeneralLogInfo> myArray = new List<GeneralLogInfo>();


            DisableDevice();
            bRet = pOcxObject.ReadAllGLogData(m_nMachineNum);
            if (!bRet)
            {
                ShowErrorInfo();

                pOcxObject.EnableDevice(m_nMachineNum, 1);
                return;
            }

            do
            {
                bRet = pOcxObject.GetAllGLogData(m_nMachineNum,
                ref gLogInfo.dwTMachineNumber,
                ref gLogInfo.dwEnrollNumber,
                ref gLogInfo.dwEMachineNumber,
                ref gLogInfo.dwVerifyMode,
                ref gLogInfo.dwYear,
                ref gLogInfo.dwMonth,
                ref gLogInfo.dwDay,
                ref gLogInfo.dwHour,
                ref gLogInfo.dwMinute
                );

                if (bRet)
                {
                    myArray.Add(gLogInfo);
                }

            } while (bRet);

            int i = 1;
            string str;
            foreach (GeneralLogInfo gInfo in myArray)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = i.ToString();
                i++;

                lvi.SubItems.Add(gInfo.dwTMachineNumber.ToString());
                lvi.SubItems.Add(gInfo.dwEnrollNumber.ToString("D8"));
                lvi.SubItems.Add(gInfo.dwEMachineNumber.ToString());

                int nInOut = gInfo.dwVerifyMode / 8;
                lvi.SubItems.Add(nInOut.ToString());                             //INOUT

                str = common.FormString(gInfo.dwVerifyMode, gInfo.dwEnrollNumber);
                lvi.SubItems.Add(str);                                          // Verify Mode

                //如果要提高效率
                DateTime dt = new DateTime(gInfo.dwYear,
                    gInfo.dwMonth,
                    gInfo.dwDay,
                    gInfo.dwHour,
                    gInfo.dwMinute,
                    0);

                lvi.SubItems.Add(dt.ToString("yyyy/MM/dd HH:mm"));


                listView1.Items.Add(lvi);

            }

            i -= 1;
            labelTotal.Text = i.ToString("Total Read 0");

            pOcxObject.EnableDevice(m_nMachineNum, 1);
        }
        #endregion

        #region AIKYO
        private void btnGetLogsByAIKYO_Click(object sender, EventArgs e)
        {
            var mess = string.Empty;
            var logs = GetLogsByAIKYO(dtFromDateByAIKYO.Value, dtToDateByAIKYO.Value, 0, ref mess);
            if (logs.Count > 0)
            {
                BindToGridView(logs);
                textTotalByAIKYO.Text = logs.Count.ToString();
            }
        }
        public bool ConnectByAIKYO(ref string message)
        {
            var connectionString = "Driver={Microsoft Access Driver (*.mdb)};" + $"Dbq={textPathByAIKYO.Text};";
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrWhiteSpace(textPathByAIKYO.Text))
            {
                return false;
            }
            return true;
        }

        public List<LogData> GetLogsByAIKYO(DateTime? fromDate, DateTime? toDate, int? limit, ref string message)
        {
            var listLogs = new List<LogData>();
            try
            {
                var startDate = fromDate.HasValue ? fromDate.Value.Date : DateTime.Now;
                var endDate = toDate.HasValue ? toDate.Value.Date.AddDays(1).AddMilliseconds(-1) : DateTime.Now;
                // lấy dữ liệu từ DB access
                listLogs = GetTimeKeeperDataFromAccess(startDate, endDate, ref message);
                message += $"\n=========GetLogsByAIKYO-----Số lượng bản ghi: {listLogs.Count}============";
                return listLogs.OrderBy(x => x.CheckTime).ToList();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                Logger.LogInfo($"\n==============GetLogsByAIKYO : Exception: {ex.Message}==============");
            }
            return listLogs;
        }
        private List<LogData> GetTimeKeeperDataFromAccess(DateTime fromDate, DateTime toDate, ref string message)
        {
            var connectionString = "Driver={Microsoft Access Driver (*.mdb)};" + $"Dbq={textPathByAIKYO.Text};";
            if (!string.IsNullOrWhiteSpace(Utility.GetAppSetting("ConnectionStringODBCRonaldJackAccess")))
            {
                connectionString = Utility.GetAppSetting("ConnectionStringODBCRonaldJackAccess");
            }
            List<LogData> timeKeeperDatas = new List<LogData>();
            try
            {
                //var con = new OdbcConnection();
                //con.ConnectionString = connectionString;
                //var cmd = new OdbcCommand { Connection = con };
                //con.Open();
                //cmd.CommandText = "SELECT a.UserFullCode, a.UserEnrollNumber, a.TimeStr, a.MachineNo FROM InOutRun a WHERE a.TimeStr BETWEEN ? AND ?;";
                //cmd.Parameters.Add("?", OdbcType.DateTime).Value = fromDate;
                //cmd.Parameters.Add("?", OdbcType.DateTime).Value = toDate;

                //cmd.CommandText = "SELECT * FROM InOutRun;";

                //System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=D:\\Fullshare\\Data\\RJData.mdb;");

                ////cmd.CommandText = "SELECT * FROM InOutRun;";
                //string query = "SELECT * FROM InOutRun;";
                //System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(query, con);

                //using (var reader = cmd.ExecuteReader())
                //{
                //    while (reader.Read())
                //    {
                //        LogData timeKeeperData = new LogData();
                //        var userID = 1;
                //        if (reader[0] != null && int.TryParse(reader[0].ToString(), out userID))
                //        {
                //            timeKeeperData.UserID = userID.ToString();
                //            var time = DateTime.Now;
                //            if(reader[2] != null && DateTime.TryParse(reader[2].ToString(), out time))
                //            {
                //                timeKeeperData.CheckTime = time;
                //                timeKeeperData.DeviceID = reader[3] != null ? reader[3].ToString() : String.Empty;
                //                timeKeeperDatas.Add(timeKeeperData);
                //            }
                //        }
                //    }
                //}
                //con.Close();



                string connectionString1 =
                @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                @"Data Source=C:\Users\hnanh\Desktop\Data\RJData.mdb;" +
                @"User Id=;Password=;";

                string queryString = "SELECT * FROM InOutRun;";

                using (System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(connectionString1))   //tạo lớp kết nối vào .mbd
                using (System.Data.OleDb.OleDbCommand command = new System.Data.OleDb.OleDbCommand(queryString, connection))    //tạo lớp lệnh sql sử dụng lớp kết nối trên
                {
                    try
                    {
                        connection.Open();  //bắt đầu kết nối
                        System.Data.OleDb.OleDbDataReader reader = command.ExecuteReader();  //thực thi sql và trả về kết quả

                        while (reader.Read())  //đọc kết quả
                        {
                            Console.Write(reader[0].ToString() + ",");
                            Console.Write(reader[1].ToString() + ",");
                            Console.WriteLine(reader[2].ToString());
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                return timeKeeperDatas;
            }
            catch (Exception ex)
            {
                message += ex.ToString();
                Logger.LogInfo($"\n==============GetTimeKeeperDataFromAccess : Exception: {ex.Message}==============");
                return new List<LogData>();
            }
        }
        #endregion
        public bool ConnectByRonaldJack(ref string message)
        {
            bool success = false;
            bool ronaldJackConnectOnly = false;
            try
            {
                Boolean.TryParse(System.Configuration.ConfigurationSettings.AppSettings["RonaldJackConnectOnly"], out ronaldJackConnectOnly);
                // connect using zkteco sdk first
                if (!ronaldJackConnectOnly)
                {
                    success = ConnectTCP(ref message);
                }
                // if cannot connect then using ronald jack sdk
                else
                {
                    //success = ConnectTCP(ref message);
                    //if (!success)
                    //{
                    if (this.axCLOCK == null)
                    {
                        var thr = new Thread(() =>
                        {
                            this.axCLOCK = new AxFP_CLOCK();
                        });
                        thr.SetApartmentState(ApartmentState.STA);
                        thr.Start();
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        //thr.Abort();
                    }
                    if (!string.IsNullOrEmpty(txtIPByRonaldJack.Text))
                    {
                        message += $"Start connecting using ronald jack sdk \n";
                        var ip = txtIPByRonaldJack.Text;
                        int nPassword = int.Parse(txtPassByRonaldJack.Text);
                        bool fpClockCreateControl = true;
                        Boolean.TryParse(System.Configuration.ConfigurationSettings.AppSettings["FpClockCreateControl"], out fpClockCreateControl);
                        if (fpClockCreateControl)
                        {
                            axCLOCK.CreateControl();
                        }
                        success = axCLOCK.OpenCommPort(1);
                        message += $"OpenComPort 1st time: {success} \n";
                        axCLOCK.SetIPAddress(ref ip, int.Parse(txtPortByRonaldJack.Text), nPassword);

                        success = axCLOCK.OpenCommPort(1);
                        if (success)
                        {
                            _isConnected = true;

                            string sn = string.Empty;
                            axCLOCK.GetSerialNumber(1, ref sn);
                            message += $"GetSerialNumber: {sn} \n";
                            //Config.SerialNumber = sn;
                            axCLOCK.EnableDevice(1, 1);
                        }

                        if (!success)
                        {
                            int code = 0;
                            axCLOCK.GetLastError(ref code);
                            message += $"Connect ronald jack failed with code: {code}";
                        }
                    }
                    else
                    {
                        message += "Invalid config";
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return success;
        }

        private List<LogData> GetLogsFromDevice(DeviceInfo config, DateTime? fromDate, DateTime? toDate)
        {
            List<LogData> logs = new List<LogData>();

            try
            {
                Logger.LogError($"\nGetLogsFromDevice_Bat đau InitClient");
                Logger.LogError($"\nGetLogsFromDevice_InitClient xong");
                string message = string.Empty;
                var connected = true;
                bool customConnectByGetLogsFromDevice = false;
                Boolean.TryParse(Utility.GetAppSetting("CustomConnectByGetLogsFromDevice"), out customConnectByGetLogsFromDevice);
                if (!customConnectByGetLogsFromDevice)
                {
                    Logger.LogError($"\nGetLogsFromDevice_Bat đau ket noi");
                    connected = ConnectByRonaldJack(ref message);
                }
                Logger.LogError($"\nGetLogsFromDevice_Ket noi xong: {connected}");
                if (connected)
                {
                    int limitDataLog = 6;
                    Logger.LogError($"\nGetLogsFromDevice_Bat đau lay du lieu");
                    logs = GetLogsByRonalJack(fromDate, toDate, limitDataLog, ref message);
                    Logger.LogError($"\nGetLogsFromDevice_Lay du lieu xong, Count: {logs.Count}");
                }
                Logger.LogError($"\nGetLogsFromDevice: {message} ======== Count: {logs.Count}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"\nGetLogsFromDevice_Exception");
                Logger.HandleException(ex);
            }

            return logs;
        }
        private bool ConnectDevice()
        {
            var success = false;
            try
            {
                string message = string.Empty;
                success = ConnectByRonaldJack(ref message);
                if (success)
                {
                    Logger.LogError($"ConnectDevice : {success} - Message: {message}");
                }
                else
                {
                    Logger.LogError($"ConnectDevice Failed: {message}");
                }
            }
            catch (Exception ex)
            {
                Logger.HandleException(ex);
            }

            return success;
        }
        private bool ConnectTCP(ref string message)
        {
            bool success = false;
            try
            {
                int pass = 0;
                if (!string.IsNullOrEmpty(txtIPByRonaldJack.Text) && int.TryParse(txtPassByRonaldJack.Text, out pass))
                {
                    axCZKEM.Beep(5000);
                    axCZKEM.SetCommPassword(pass);
                    success = axCZKEM.Connect_Net(txtIPByRonaldJack.Text, int.Parse(txtPortByRonaldJack.Text));
                    if (success)
                    {
                        _isConnected = true;

                        axCZKEM.DisableDeviceWithTimeOut(1, 60);
                        string sn = string.Empty;
                        axCZKEM.GetSerialNumber(1, out sn);
                        //Config.SerialNumber = sn;
                        axCZKEM.EnableDevice(1, true);
                    }

                    if (!success)
                    {
                        int code = 0;
                        axCZKEM.GetLastError(ref code);
                        message += $"Connect zkteco failed with code: {code}";
                        Utility.SaveAppSetting("RonaldJackConnectOnly", "true");
                    }
                }
                else
                {
                    message += "Invalid config";
                    Utility.SaveAppSetting("RonaldJackConnectOnly", "true");
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                Utility.SaveAppSetting("RonaldJackConnectOnly", "true");
            }
            return success;
        }
        public List<LogData> GetLogsByRonalJack(DateTime? fromDate, DateTime? toDate, int? limit, ref string message)
        {
            var logs = new List<LogData>();
            try
            {
                if (_isConnected || true)
                {
                    var getLogOption = 0;
                    int.TryParse(System.Configuration.ConfigurationSettings.AppSettings["GetLogOption"], out getLogOption);
                    if (getLogOption > 0)
                    {
                        switch (getLogOption)
                        {
                            case 1:
                                logs = GetLogsRonaldJackGLog(ref message);
                                break;
                            case 2:
                                logs = GetLogsRonaldJackAllSLog(ref message);
                                break;
                            case 3:
                                logs = GetLogsRonaldJackSLog(ref message);
                                break;
                            case 4:
                                logs = GetLogsRonaldJackAllGLog(ref message);
                                break;
                            case 5:
                                logs = GetLogsRonaldJackAllGLogWithSecond(ref message);
                                break;
                            case 6:
                                logs = GetLogsZkTeco(ref message);
                                break;
                            default:
                                logs = GetLogsZkTeco(ref message);
                                break;
                        }
                    }
                    else
                    {
                        if (logs.Count == 0)
                        {
                            Utility.SaveAppSetting("GetLogOption", "5");
                            logs = GetLogsRonaldJackAllGLogWithSecond(ref message);
                        }
                        if (logs.Count == 0)
                        {
                            Utility.SaveAppSetting("GetLogOption", "4");
                            logs = GetLogsRonaldJackAllGLog(ref message);
                        }
                        if (logs.Count == 0)
                        {
                            Utility.SaveAppSetting("GetLogOption", "3");
                            logs = GetLogsRonaldJackSLog(ref message);
                        }
                        if (logs.Count == 0)
                        {
                            Utility.SaveAppSetting("GetLogOption", "2");
                            logs = GetLogsRonaldJackAllSLog(ref message);
                        }
                        if (logs.Count == 0)
                        {
                            Utility.SaveAppSetting("GetLogOption", "1");
                            logs = GetLogsRonaldJackGLog(ref message);
                        }
                        if (logs.Count == 0)
                        {
                            Utility.SaveAppSetting("GetLogOption", "6");
                            logs = GetLogsZkTeco(ref message);
                        }
                        if (logs.Count == 0)
                        {
                            Utility.SaveAppSetting("GetLogOption", "0");
                        }
                    }

                }
                else
                {
                    message = "Connect failed";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (logs.Count() == 0)
            {
                Utility.SaveAppSetting("GetLogOption", "0");
            }
            // ntgiang2 6.5.2021 nếu logs có nhiều hơn 2 bản ghi và checktime bản ghi đầu tên là gần nhất thì đảo lại để luôn cho bản ghi mới nhất là cuối cùng
            if (logs.Count() >= 2 && logs[0].CheckTime >= logs[1].CheckTime)
            {
                var sortedListLogs = logs.OrderBy(x => x.CheckTime).ToList();
                return sortedListLogs;
            }
            return logs;
        }
        public List<LogData> GetLogsZkTeco(ref string message)
        {
            var logs = new List<LogData>();
            try
            {
                ConnectTCP(ref message);

                if (_isConnected)
                {
                    axCZKEM.DisableDeviceWithTimeOut(1, 60);

                    var flagRead = axCZKEM.ReadGeneralLogData(1);
                    if (!flagRead)
                    {
                        int code = 0;
                        axCZKEM.GetLastError(ref code);
                        message += $"Zkteco can't read, failed with code: {code}";
                    }

                    string sdwEnrollNumber;
                    int idwEnrollNumber = 0,
                        dwVerifyMode = 0,
                        dwInOutMode = 0,
                        dwYear = 0,
                        dwMonth = 0,
                        dwDay = 0,
                        dwHour = 0,
                        dwMinute = 0,
                        dwSecond = 0,
                        dwWorkCode = 0;
                    int dwTMachineNumber = 0,
                        dwEMachineNumber = 0;
                    // ntgiang2 23.4.2021 lấy product code của máy. Nếu trong list bỏ check thì bỏ check xem có phải kiểu màn hình đen trắng không
                    bool colorMachine = false;

                    if (colorMachine || axCZKEM.IsTFTMachine(1))
                    {
                        while (axCZKEM.SSR_GetGeneralLogData(1,
                                                             out sdwEnrollNumber,
                                                             out dwVerifyMode,
                                                             out dwInOutMode,
                                                             out dwYear,
                                                             out dwMonth,
                                                             out dwDay,
                                                             out dwHour,
                                                             out dwMinute,
                                                             out dwSecond,
                                                             ref dwWorkCode))
                        {
                            var stringDate = $"{dwMonth}/{dwDay}/{dwYear} {dwHour}:{dwMinute}";
                            DateTime d;
                            if (DateTime.TryParseExact(stringDate, formatDates, System.Globalization.CultureInfo.GetCultureInfo("vi-VN"),
                                   System.Globalization.DateTimeStyles.None, out d))
                            {
                                var log = new LogData()
                                {
                                    UserID = sdwEnrollNumber,
                                    FullName = string.Empty,
                                    CheckTime = d,
                                };
                                logs.Add(log);
                            }
                            else
                            {
                                message += $"Error Data: Year: {dwYear} - Month: {dwMonth} - Day: {dwDay} - Hour: {dwHour} - Minute: {dwMinute}";
                            }
                        }
                    }
                    else
                    {
                        while (axCZKEM.GetGeneralLogData(1,
                                                         ref dwTMachineNumber,
                                                         ref idwEnrollNumber,
                                                         ref dwEMachineNumber,
                                                         ref dwVerifyMode,
                                                         ref dwInOutMode,
                                                         ref dwYear,
                                                         ref dwMonth,
                                                         ref dwDay,
                                                         ref dwHour,
                                                         ref dwMinute))
                        {
                            var stringDate = $"{dwMonth}/{dwDay}/{dwYear} {dwHour}:{dwMinute}";
                            DateTime d;
                            if (DateTime.TryParseExact(stringDate, formatDates, System.Globalization.CultureInfo.GetCultureInfo("vi-VN"),
                                   System.Globalization.DateTimeStyles.None, out d))
                            {
                                var log = new LogData()
                                {
                                    UserID = idwEnrollNumber.ToString(),
                                    FullName = string.Empty,
                                    CheckTime = d
                                };
                                logs.Add(log);
                            }
                            else
                            {
                                message += $"Error Data: Year: {dwYear} - Month: {dwMonth} - Day: {dwDay} - Hour: {dwHour} - Minute: {dwMinute}";
                            }
                        }
                    }
                }
                else
                {
                    message += "Connect failed";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            message += $"Zkteco get {logs.Count} records";
            return logs;
        }
        // get logs ronald jack 1
        public List<LogData> GetLogsRonaldJackGLog(ref string message)
        {
            var logs = new List<LogData>();
            if (this.axCLOCK == null)
            {
                var thr = new Thread(() =>
                {
                    this.axCLOCK = new AxFP_CLOCK();
                });
                thr.SetApartmentState(ApartmentState.STA);
                thr.Start();
                Thread.Sleep(3000);
            }
            // Tạo control
            bool fpClockCreateControl = true;
            Boolean.TryParse(System.Configuration.ConfigurationSettings.AppSettings["FpClockCreateControl1"], out fpClockCreateControl);
            if (fpClockCreateControl)
            {
                this.axCLOCK.CreateControl();
            }
            try
            {
                var isEnable = this.axCLOCK.EnableDevice(1, 0);
                var bRet = this.axCLOCK.ReadGeneralLogData(1);
                if (!bRet)
                {
                    int code = 0;
                    this.axCLOCK.GetLastError(ref code);
                    message += $"Ronald Jack 1 can't read, failed with code: {code}";
                    this.axCLOCK.EnableDevice(1, 1);
                    return logs;
                }

                do
                {
                    int dwEnrollNumber = 0,
                        dwVerifyMode = 0,
                        dwYear = 0,
                        dwMonth = 0,
                        dwDay = 0,
                        dwHour = 0,
                        dwMinute = 0;
                    int dwTMachineNumber = 0,
                        dwEMachineNumber = 0;
                    bRet = this.axCLOCK.GetGeneralLogData(1,
                    ref dwTMachineNumber,
                    ref dwEnrollNumber,
                    ref dwEMachineNumber,
                    ref dwVerifyMode,
                    ref dwYear,
                    ref dwMonth,
                    ref dwDay,
                    ref dwHour,
                    ref dwMinute
                    );

                    if (bRet)
                    {
                        var stringDate = $"{dwMonth}/{dwDay}/{dwYear} {dwHour}:{dwMinute}";
                        DateTime d;
                        if (DateTime.TryParseExact(stringDate, formatDates, System.Globalization.CultureInfo.GetCultureInfo("vi-VN"),
                               System.Globalization.DateTimeStyles.None, out d))
                        {
                            var log = new LogData()
                            {
                                UserID = dwEnrollNumber.ToString(),
                                FullName = string.Empty,
                                CheckTime = d,
                            };
                            logs.Add(log);
                        }
                        else
                        {
                            message += $"Error Data: Year: {dwYear} - Month: {dwMonth} - Day: {dwDay} - Hour: {dwHour} - Minute: {dwMinute}";
                        }
                    }

                } while (bRet);

                message += $"Ronald jack 1 get {logs.Count} records";
                this.axCLOCK.EnableDevice(1, 1);

            }
            catch (Exception ex)
            {
                message += ex.Message;
            }
            return logs;
        }
        // get logs ronald jack 2
        public List<LogData> GetLogsRonaldJackAllSLog(ref string message)
        {
            var logs = new List<LogData>();
            if (this.axCLOCK == null)
            {
                var thr = new Thread(() =>
                {
                    this.axCLOCK = new AxFP_CLOCK();
                });
                thr.SetApartmentState(ApartmentState.STA);
                thr.Start();
                Thread.Sleep(3000);
            }
            // Tạo control
            bool fpClockCreateControl = true;
            Boolean.TryParse(System.Configuration.ConfigurationSettings.AppSettings["FpClockCreateControl1"], out fpClockCreateControl);
            if (fpClockCreateControl)
            {
                this.axCLOCK.CreateControl();
            }
            try
            {
                var isEnable = this.axCLOCK.EnableDevice(1, 0);
                var bRet = this.axCLOCK.ReadSuperLogData(1);
                if (!bRet)
                {
                    int code = 0;
                    this.axCLOCK.GetLastError(ref code);
                    message += $"Ronald Jack 2 can't read, failed with code: {code}";
                    this.axCLOCK.EnableDevice(1, 1);
                    return logs;
                }

                do
                {
                    int dwSEnrollNumber = 0,
                        dwYear = 0,
                        dwMonth = 0,
                        dwDay = 0,
                        dwHour = 0,
                        dwMinute = 0;
                    int dwTMachineNumber = 0,
                        dwSEMachineNumber = 0,
                        dwGEMachineNumber = 0,
                        dwManipulation = 0,
                        dwFingerNumber = 0;
                    bRet = this.axCLOCK.GetAllSLogData(1,
                    ref dwTMachineNumber,
                    ref dwSEnrollNumber,
                    ref dwSEMachineNumber,
                    ref dwGEMachineNumber,
                    ref dwGEMachineNumber,
                    ref dwManipulation,
                    ref dwFingerNumber,
                    ref dwYear,
                    ref dwMonth,
                    ref dwDay,
                    ref dwHour,
                    ref dwMinute
                    );

                    if (bRet)
                    {
                        var stringDate = $"{dwMonth}/{dwDay}/{dwYear} {dwHour}:{dwMinute}";
                        DateTime d;
                        if (DateTime.TryParseExact(stringDate, formatDates, System.Globalization.CultureInfo.GetCultureInfo("vi-VN"),
                               System.Globalization.DateTimeStyles.None, out d))
                        {
                            var log = new LogData()
                            {
                                UserID = dwSEnrollNumber.ToString(),
                                FullName = string.Empty,
                                CheckTime = d,
                            };
                            logs.Add(log);
                        }
                        else
                        {
                            message += $"Error Data: Year: {dwYear} - Month: {dwMonth} - Day: {dwDay} - Hour: {dwHour} - Minute: {dwMinute}";
                        }
                    }

                } while (bRet);

                message += $"Ronald jack 2 get {logs.Count} records";
                this.axCLOCK.EnableDevice(1, 1);

            }
            catch (Exception ex)
            {
                message += ex.Message;
            }
            return logs;
        }
        // get logs ronald jack 3
        public List<LogData> GetLogsRonaldJackSLog(ref string message)
        {
            var logs = new List<LogData>();
            if (this.axCLOCK == null)
            {
                var thr = new Thread(() =>
                {
                    this.axCLOCK = new AxFP_CLOCK();
                });
                thr.SetApartmentState(ApartmentState.STA);
                thr.Start();
                Thread.Sleep(3000);
            }
            // Tạo control
            bool fpClockCreateControl = true;
            Boolean.TryParse(System.Configuration.ConfigurationSettings.AppSettings["FpClockCreateControl1"], out fpClockCreateControl);
            if (fpClockCreateControl)
            {
                this.axCLOCK.CreateControl();
            }
            try
            {
                var isEnable = this.axCLOCK.EnableDevice(1, 0);
                var bRet = this.axCLOCK.ReadSuperLogData(1);
                if (!bRet)
                {
                    int code = 0;
                    this.axCLOCK.GetLastError(ref code);
                    message += $"Ronald Jack 3 can't read, failed with code: {code}";
                    this.axCLOCK.EnableDevice(1, 1);
                    return logs;
                }

                do
                {
                    int dwSEnrollNumber = 0,
                        dwYear = 0,
                        dwMonth = 0,
                        dwDay = 0,
                        dwHour = 0,
                        dwMinute = 0;
                    int dwTMachineNumber = 0,
                        dwSEMachineNumber = 0,
                        dwGEMachineNumber = 0,
                        dwManipulation = 0,
                        dwFingerNumber = 0;
                    bRet = this.axCLOCK.GetSuperLogData(1,
                    ref dwTMachineNumber,
                    ref dwSEnrollNumber,
                    ref dwSEMachineNumber,
                    ref dwGEMachineNumber,
                    ref dwGEMachineNumber,
                    ref dwManipulation,
                    ref dwFingerNumber,
                    ref dwYear,
                    ref dwMonth,
                    ref dwDay,
                    ref dwHour,
                    ref dwMinute
                    );

                    if (bRet)
                    {
                        var stringDate = $"{dwMonth}/{dwDay}/{dwYear} {dwHour}:{dwMinute}";
                        DateTime d;
                        if (DateTime.TryParseExact(stringDate, formatDates, System.Globalization.CultureInfo.GetCultureInfo("vi-VN"),
                               System.Globalization.DateTimeStyles.None, out d))
                        {
                            var log = new LogData()
                            {
                                UserID = dwSEnrollNumber.ToString(),
                                FullName = string.Empty,
                                CheckTime = d,
                            };
                            logs.Add(log);
                        }
                        else
                        {
                            message += $"Error Data: Year: {dwYear} - Month: {dwMonth} - Day: {dwDay} - Hour: {dwHour} - Minute: {dwMinute}";
                        }
                    }

                } while (bRet);

                message += $"Ronald jack 3 get {logs.Count} records";
                this.axCLOCK.EnableDevice(1, 1);

            }
            catch (Exception ex)
            {
                message += ex.Message;
            }
            return logs;
        }
        // get logs ronald jack 4
        public List<LogData> GetLogsRonaldJackAllGLog(ref string message)
        {
            var logs = new List<LogData>();
            if (this.axCLOCK == null)
            {
                var thr = new Thread(() =>
                {
                    this.axCLOCK = new AxFP_CLOCK();
                });
                thr.SetApartmentState(ApartmentState.STA);
                thr.Start();
                Thread.Sleep(3000);
            }
            // Tạo control
            bool fpClockCreateControl = true;
            Boolean.TryParse(System.Configuration.ConfigurationSettings.AppSettings["FpClockCreateControl1"], out fpClockCreateControl);
            if (fpClockCreateControl)
            {
                this.axCLOCK.CreateControl();
            }
            try
            {
                var isEnable = this.axCLOCK.EnableDevice(1, 0);
                var bRet = this.axCLOCK.ReadAllGLogData(1);
                if (!bRet)
                {
                    int code = 0;
                    this.axCLOCK.GetLastError(ref code);
                    message += $"Ronald Jack 4 can't read, failed with code: {code}";
                    this.axCLOCK.EnableDevice(1, 1);
                    return logs;
                }

                do
                {
                    int dwTMachineNumber = 0,
                        dwEnrollNumber = 0,
                        dwEMachineNumber = 0,
                        dwVerifyMode = 0,
                        dwYear = 0,
                        dwMonth = 0,
                        dwDay = 0,
                        dwHour = 0,
                        dwMinute = 0;
                    bRet = this.axCLOCK.GetAllGLogData(1,
                                                                    ref dwTMachineNumber,
                                                                    ref dwEnrollNumber,
                                                                    ref dwEMachineNumber,
                                                                    ref dwVerifyMode,
                                                                    ref dwYear,
                                                                    ref dwMonth,
                                                                    ref dwDay,
                                                                    ref dwHour,
                                                                    ref dwMinute);

                    if (bRet)
                    {
                        var stringDate = $"{dwMonth}/{dwDay}/{dwYear} {dwHour}:{dwMinute}";
                        DateTime d;
                        if (DateTime.TryParseExact(stringDate, formatDates, System.Globalization.CultureInfo.GetCultureInfo("vi-VN"),
                               System.Globalization.DateTimeStyles.None, out d))
                        {
                            var log = new LogData()
                            {
                                UserID = dwEnrollNumber.ToString(),
                                FullName = string.Empty,
                                CheckTime = d,
                            };
                            logs.Add(log);
                        }
                        else
                        {
                            message += $"Error Data: Year: {dwYear} - Month: {dwMonth} - Day: {dwDay} - Hour: {dwHour} - Minute: {dwMinute}";
                        }
                    }

                } while (bRet);

                message += $"Ronald jack 4 get {logs.Count} records";
                this.axCLOCK.EnableDevice(1, 1);

            }
            catch (Exception ex)
            {
                message += ex.Message;
            }
            return logs;
        }
        // get logs ronald jack 5
        public List<LogData> GetLogsRonaldJackAllGLogWithSecond(ref string message)
        {
            var logs = new List<LogData>();
            if (this.axCLOCK == null)
            {
                var thr = new Thread(() =>
                {
                    this.axCLOCK = new AxFP_CLOCK();
                });
                thr.SetApartmentState(ApartmentState.STA);
                thr.Start();
                Thread.Sleep(3000);
            }
            // Tạo control
            bool fpClockCreateControl = true;
            Boolean.TryParse(System.Configuration.ConfigurationSettings.AppSettings["FpClockCreateControl1"], out fpClockCreateControl);
            if (fpClockCreateControl)
            {
                this.axCLOCK.CreateControl();
            }
            try
            {
                var isEnable = this.axCLOCK.EnableDevice(1, 0);
                var bRet = this.axCLOCK.ReadAllGLogData(1);
                if (!bRet)
                {
                    int code = 0;
                    this.axCLOCK.GetLastError(ref code);
                    message += $"Ronald Jack 5 can't read, failed with code: {code}";
                    this.axCLOCK.EnableDevice(1, 1);
                    return logs;
                }

                do
                {
                    int dwTMachineNumber = 0,
                        dwEnrollNumber = 0,
                        dwEMachineNumber = 0,
                        dwVerifyMode = 0,
                        dwInout = 0,
                        dwEvent = 0,
                        dwYear = 0,
                        dwMonth = 0,
                        dwDay = 0,
                        dwHour = 0,
                        dwMinute = 0,
                        dwSecond = 0;
                    bRet = this.axCLOCK.GetAllGLogDataWithSecond(1,
                                                                    ref dwTMachineNumber,
                                                                    ref dwEnrollNumber,
                                                                    ref dwEMachineNumber,
                                                                    ref dwVerifyMode,
                                                                    ref dwInout,
                                                                    ref dwEvent,
                                                                    ref dwYear,
                                                                    ref dwMonth,
                                                                    ref dwDay,
                                                                    ref dwHour,
                                                                    ref dwMinute,
                                                                    ref dwSecond);

                    if (bRet)
                    {
                        var stringDate = $"{dwMonth}/{dwDay}/{dwYear} {dwHour}:{dwMinute}";
                        DateTime d;
                        if (DateTime.TryParseExact(stringDate, formatDates, System.Globalization.CultureInfo.GetCultureInfo("vi-VN"),
                               System.Globalization.DateTimeStyles.None, out d))
                        {
                            var log = new LogData()
                            {
                                UserID = dwEnrollNumber.ToString(),
                                FullName = string.Empty,
                                CheckTime = d,
                            };
                            logs.Add(log);
                        }
                        else
                        {
                            message += $"Error Data: Year: {dwYear} - Month: {dwMonth} - Day: {dwDay} - Hour: {dwHour} - Minute: {dwMinute}";
                        }
                    }

                } while (bRet);

                message += $"Ronald jack 5 get {logs.Count} records";
                this.axCLOCK.EnableDevice(1, 1);

            }
            catch (Exception ex)
            {
                message += ex.Message;
            }
            return logs;
        }

        private void btnConnectByRonaldJack_Click(object sender, EventArgs e)
        {
            var thr = new Thread(() =>
            {
                this.axCLOCK = new AxFP_CLOCK();
            });
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();
            Thread.Sleep(3000);
            this.axCZKEM = new CZKEM();

            var connect = ConnectDevice();
            if (connect)
            {
                txtTotalLogsByRonaldJack.Text = "Kết nối thành công";
            }
            else
            {
                txtTotalLogsByRonaldJack.Text = "Kết nối thất bại";
            }
        }

        private void btnGetLogByRonaldJack_Click(object sender, EventArgs e)
        {
            var thr = new Thread(() =>
            {
                this.axCLOCK = new AxFP_CLOCK();
            });
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();
            Thread.Sleep(3000);
            this.axCZKEM = new CZKEM();

            var logs = GetLogsFromDevice(new DeviceInfo(), dtFromDateByRonaldJack.Value, dtToDateByRonaldJack.Value);
            logs = logs.OrderByDescending(x => x.CheckTime).ToList();
            txtTotalLogsByRonaldJack.Text = logs.Count.ToString();
            if (logs.Count > 0)
            {
                BindToGridView(logs);
            }
        }
    }
}