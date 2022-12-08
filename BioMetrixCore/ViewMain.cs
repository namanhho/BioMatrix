using BioMetrixCore.Utilities;
using Microsoft.Exchange.WebServices.Data;
using NativeWifi;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
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
            // Add you new UserInfo Here and  uncomment the below code
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
            var fromDate = DateTime.Now.Date.AddYears(-1);
            var toDate = DateTime.Now.Date.AddYears(1);
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
            //    email = email,
            //    password = pass
            //};
            //request.AddParameter("application/json", Converter.JsonSerialize(param), ParameterType.RequestBody);

            //IRestResponse response = client.Execute(request);
            //if (!response.IsSuccessful)
            //{
            //    return;
            //}
            //var content = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
            //if (content.ContainsKey("status") && int.Parse(content["status"].ToString()) != 10000)
            //{
            //    mess = content.ContainsKey("error message") ? content["error message"].ToString() : "";
            //    return;
            //}
            //var data = content.ContainsKey("data") ? Converter.JsonDeserialize<Dictionary<string, object>>(content["data"].ToString()) : new Dictionary<string, object>();
            //url = data.ContainsKey("api_url") ? data["api_url"].ToString() : "";
            //token = data.ContainsKey("access_token") ?  data["access_token"].ToString() : "";

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
            //    company_code = tbxCompanyCode.Text.Trim(),
            //    start_time = DateTime.Now.Date,
            //    end_time = DateTime.Now.AddMonths(2)
            //};
            //request.AddParameter("application/json", Converter.JsonSerialize(param), ParameterType.RequestBody);

            //client.Authenticator = new JwtAuthenticator(token);
            //IRestResponse response = client.Execute(request);
            //if (!response.IsSuccessful)
            //{
            //    return logs;
            //}
            //var content = Converter.JsonDeserialize<Dictionary<string, object>>(response.Content);
            //if (content.ContainsKey("status") && int.Parse(content["status"].ToString()) != 10000)
            //{
            //    mess += content.ContainsKey("error message") ? content["error message"]?.ToString() : "";
            //    return logs;
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
            //    request.AddCookie("x-sessionid", this.SessionId);
            //    request.AddCookie("x-tenantid", this.TenantId);
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

        private void btnGetBSSID_Click(object sender, EventArgs e)
        {
            var wlanClient = new WlanClient();
            //foreach (WlanClient.WlanInterface wlanInterface in wlanClient.Interfaces)
            //{
            //    var test = wlanInterface.CurrentConnection.wlanAssociationAttributes.Dot11Bssid;
            //    Console.WriteLine(test);
            //}
        }
    }
}
