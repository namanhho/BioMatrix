﻿using BioMetrixCore.Utilities;
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
using Newtonsoft.Json;
using static BioMetrixCore.Dahahi;
using NLog.Fluent;
using System.ServiceModel.Channels;
using static BioMetrixCore.Hikvision;
using Microsoft.Win32;
using ZaloDotNetSDK;
using BioMetrixCore.Access;
using BioMetrixCore.Model;
using BioMetrixCore.BLL;
using BioMetrixCore.Utils;
using BioMetrixCore.SDK;

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
            SetStartup();
            ToggleControls(false);
            ShowStatusBar(string.Empty, true);
            DisplayEmpty();

            CHCNetSDK.NET_DVR_Init();
            this.axCLOCK = new AxFP_CLOCK();

            this.cmbInterface.SelectedIndex = 1;
            this.cmbComPort.SelectedIndex = 0;

            this.ipAddressControl1.Text = "115.79.213.137";
            this.textPort.Text = "5005";
            textPassword.Text = "0";

            P2SPort.Text = "0";
            P2STimeOut.Text = "0";

            this.cmbMachineNumber.SelectedIndex = 0;

            //var thr = new Thread(() =>
            //{
            //    this.axCLOCK = new AxFP_CLOCK();
            //});
            //thr.SetApartmentState(ApartmentState.STA);
            //thr.Start();
            //this.axCZKEM = new CZKEM();
        }

        /// <summary>Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmViewMain_Load(object sender, EventArgs e)
        {
            GetServerIP();
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

                //bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
                //if (!isValidIpA)
                //    throw new Exception("The Device IP is invalid !!");

                //isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
                //if (!isValidIpA)
                //    throw new Exception("The device at " + ipAddress + ":" + port + " did not respond!!");

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
                else
                {
                    int code = 0;
                    objZkeeper.GetLastError(ref code);
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

                ICollection<MachineInfo> lstMachineInfo = manipulator.GetLogData(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()), ref message, DateTime.Now.AddMonths(-1), DateTime.Now, 2, 1);
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

            if (dgvLogsByDahahi.Controls.Count > 2)
            {
                dgvLogsByDahahi.Controls.RemoveAt(2);
            }
            dgvLogsByDahahi.DataSource = null;
            dgvLogsByDahahi.Controls.Clear();
            dgvLogsByDahahi.Rows.Clear();
            dgvLogsByDahahi.Columns.Clear();

            if (dgvLogsByHikvision.Controls.Count > 2)
            {
                dgvLogsByHikvision.Controls.RemoveAt(2);
            }
            dgvLogsByHikvision.DataSource = null;
            dgvLogsByHikvision.Controls.Clear();
            dgvLogsByHikvision.Rows.Clear();
            dgvLogsByHikvision.Columns.Clear();

            if (dgvLogDataByZkSDKNew.Controls.Count > 2)
            {
                dgvLogDataByZkSDKNew.Controls.RemoveAt(2);
            }
            dgvLogDataByZkSDKNew.DataSource = null;
            dgvLogDataByZkSDKNew.Controls.Clear();
            dgvLogDataByZkSDKNew.Rows.Clear();
            dgvLogDataByZkSDKNew.Columns.Clear();

            if (dgvListLogByHikvisionSDK.Controls.Count > 2)
            {
                dgvListLogByHikvisionSDK.Controls.RemoveAt(2);
            }
            dgvListLogByHikvisionSDK.DataSource = null;
            dgvListLogByHikvisionSDK.Controls.Clear();
            dgvListLogByHikvisionSDK.Rows.Clear();
            dgvListLogByHikvisionSDK.Columns.Clear();
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

            dgvLogsByDahahi.DataSource = list;
            dgvLogsByDahahi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvLogsByDahahi);

            dgvLogsByHikvision.DataSource = list;
            dgvLogsByHikvision.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvLogsByHikvision);

            dgvLogDataByZkSDKNew.DataSource = list;
            dgvLogDataByZkSDKNew.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvLogDataByZkSDKNew);

            dgvListLogByHikvisionSDK.DataSource = list;
            dgvListLogByHikvisionSDK.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvListLogByHikvisionSDK);
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
        //public static OSPlatform GetOperatingSystem()
        //{
        //    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        //    {
        //        return OSPlatform.OSX;
        //    }

        //    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //    {
        //        return OSPlatform.Linux;
        //    }

        //    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //    {
        //        return OSPlatform.Windows;
        //    }

        //    throw new Exception("Cannot determine operating system!");
        //}
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
            catch (Exception ex)
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
            catch (Exception ex)
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
                else if (typeLoginByUbioXFace == 2)
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
                else if (typeLoginByUbioXFace == 3)
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
                            if (c.Name == "ucsinfo")
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
            catch (Exception ex)
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
            Logger.LogError($"\n==========Bat dau btnOpenDev_Click========");
            bool bRet;
            this.axFP_CLOCK = new AxFP_CLOCK();
            this.axFP_CLOCK.CreateControl();
            if (m_bDeviceOpened)
            {
                btnOpenDev.Text = "Open";
                m_bDeviceOpened = false;

                axFP_CLOCK.CloseCommPort();
                return;
            }
            Logger.LogError($"\n==========Bat dau btnOpenDev_Click------OpenCommPort========");
            this.axFP_CLOCK.OpenCommPort(m_nCurSelID);
            Logger.LogError($"\n==========Bat dau btnOpenDev_Click------OpenCommPort xong========");
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
                        Logger.LogError($"\n==========Bat dau btnOpenDev_Click------SetIPAddress========");
                        bRet = axFP_CLOCK.SetIPAddress(ref strIP, nPort, nPassword);
                        Logger.LogError($"\n==========Bat dau btnOpenDev_Click------SetIPAddress xong: {bRet}========");
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
            Logger.LogError($"\n==========Bat dau btnOpenDev_Click------OpenCommPort========");
            bRet = axFP_CLOCK.OpenCommPort(m_nCurSelID);
            Logger.LogError($"\n==========Bat dau btnOpenDev_Click------OpenCommPort xong: {bRet}========");
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
            bool bRet = pOcxObject.EnableDevice(m_nCurSelID, 0);
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
            Logger.LogError($"\n==========Error: {nErrorValue}========");
            labelInfo.Text = common.FormErrorStr(nErrorValue);
        }
        private void btnReadSLogData_Click(object sender, EventArgs e)
        {
            Logger.LogError($"\n==========Bat dau btnReadSLogData_Click========");
            InitSLogListView();

            DisableDevice();

            pOcxObject.ReadMark = checkBox1.Checked;

            bool bRet;
            Logger.LogError($"\n==========Bat dau btnReadSLogData_Click -----ReadSuperLogData========");
            bRet = pOcxObject.ReadSuperLogData(m_nCurSelID);
            Logger.LogError($"\n==========ReadSuperLogData xong: {bRet}========");
            if (!bRet)
            {
                ShowErrorInfo();

                pOcxObject.EnableDevice(m_nCurSelID, 1);

                return;
            }

            SuperLogInfo sLogInfo = new SuperLogInfo();
            List<SuperLogInfo> myArray = new List<SuperLogInfo>();
            Logger.LogError($"\n==========Bat dau btnReadSLogData_Click -----GetSuperLogData========");
            do
            {
                bRet = pOcxObject.GetSuperLogData(
                    m_nCurSelID,
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
            Logger.LogError($"\n==========GetSuperLogData xong--- Count: {myArray.Count}========");
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
            pOcxObject.EnableDevice(m_nCurSelID, 1);

        }
        private void btnReadAllSLogData_Click(object sender, EventArgs e)
        {
            Logger.LogError($"\n==========Bat dau btnReadAllSLogData_Click========");
            InitSLogListView();

            DisableDevice();

            bool bRet;
            Logger.LogError($"\n==========Bat dau btnReadAllSLogData_Click------ReadSuperLogData========");
            bRet = pOcxObject.ReadSuperLogData(m_nCurSelID);
            Logger.LogError($"\n==========ReadSuperLogData xong: {bRet}========");
            if (!bRet)
            {
                ShowErrorInfo();

                pOcxObject.EnableDevice(m_nCurSelID, 1);

                return;
            }

            SuperLogInfo sLogInfo = new SuperLogInfo();
            List<SuperLogInfo> myArray = new List<SuperLogInfo>();
            Logger.LogError($"\n==========Bat dau btnReadAllSLogData_Click------GetAllSLogData========");
            do
            {
                bRet = pOcxObject.GetAllSLogData(
                    m_nCurSelID,
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
            Logger.LogError($"\n==========GetAllSLogData xong---Count: {myArray.Count}========");
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
            pOcxObject.EnableDevice(m_nCurSelID, 1);
        }
        private void btnEmptySLogData_Click(object sender, EventArgs e)
        {
            Logger.LogError($"\n==========Bat dau btnEmptySLogData_Click========");
            bool bRet;

            DisableDevice();
            Logger.LogError($"\n==========Bat dau btnEmptySLogData_Click-----EmptySuperLogData========");
            bRet = pOcxObject.EmptySuperLogData(m_nCurSelID);
            Logger.LogError($"\n==========EmptySuperLogData xong; {bRet}========");
            if (bRet)
            {
                labelInfo.Text = "EmptySuperLogData OK";
            }
            else
            {
                ShowErrorInfo();
            }

            pOcxObject.EnableDevice(m_nCurSelID, 1);
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
            Logger.LogError($"\n==========Bat dau UDGLogRead_Click========");
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
            Logger.LogError($"\n==========Bat dau UDGLogRead_Click-------USBReadGeneralLogData========");
            bRet = pOcxObject.USBReadGeneralLogData(strFilePath);
            Logger.LogError($"\n==========USBReadGeneralLogData xong: {bRet}========");
            if (!bRet)
            {
                ShowErrorInfo();
                return;
            }

            GeneralLogInfo gLogInfo = new GeneralLogInfo();
            List<GeneralLogInfo> myArray = new List<GeneralLogInfo>();
            Logger.LogError($"\n==========Bat dau UDGLogRead_Click-------GetAllGLogData========");
            do
            {
                bRet = pOcxObject.GetAllGLogData(m_nCurSelID,
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
            Logger.LogError($"\n==========GetAllGLogData xong---Count: {myArray.Count}========");
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

            pOcxObject.EnableDevice(m_nCurSelID, 1);
        }
        private void btnReadGLogData_Click(object sender, EventArgs e)
        {
            Logger.LogError($"\n==========Bat dau btnReadGLogData_Click========");
            InitGLogListView();

            bool bRet;
            GeneralLogInfo gLogInfo = new GeneralLogInfo();

            List<GeneralLogInfo> myArray = new List<GeneralLogInfo>();

            // if true, only read new log
            pOcxObject.ReadMark = checkBox1.Checked;

            DisableDevice();
            Logger.LogError($"\n==========Bat dau btnReadGLogData_Click------ReadGeneralLogData========");
            bRet = pOcxObject.ReadGeneralLogData(m_nCurSelID);
            Logger.LogError($"\n==========ReadGeneralLogData xong: {bRet}========");
            if (!bRet)
            {
                ShowErrorInfo();

                pOcxObject.EnableDevice(m_nCurSelID, 1);
                return;
            }
            Logger.LogError($"\n==========Bat dau btnReadGLogData_Click-----GetGeneralLogData========");
            do
            {
                bRet = pOcxObject.GetGeneralLogData(m_nCurSelID,
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
            Logger.LogError($"\n==========GetGeneralLogData xong-----Count: {myArray.Count}========");
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

            pOcxObject.EnableDevice(m_nCurSelID, 1);
        }
        private void btnEmptyGLogData_Click(object sender, EventArgs e)
        {
            Logger.LogError($"\n==========Bat dau btnEmptyGLogData_Click========");
            bool bRet;

            DisableDevice();
            Logger.LogError($"\n==========Bat dau btnEmptyGLogData_Click-----EmptyGeneralLogData========");
            bRet = pOcxObject.EmptyGeneralLogData(m_nCurSelID);
            Logger.LogError($"\n==========Bat dau btnEmptyGLogData_Click-----EmptyGeneralLogData xong: {bRet}========");
            if (bRet)
            {
                labelInfo.Text = "EmptyGeneralLogData OK";
            }
            else
            {
                ShowErrorInfo();
            }

            pOcxObject.EnableDevice(m_nCurSelID, 1);

        }
        private void btnReadAllGLogData_Click(object sender, EventArgs e)
        {
            Logger.LogError($"\n==========Bat dau btnReadAllGLogData_Click========");
            InitGLogListView();

            bool bRet;
            GeneralLogInfo gLogInfo = new GeneralLogInfo();

            List<GeneralLogInfo> myArray = new List<GeneralLogInfo>();


            DisableDevice();
            Logger.LogError($"\n==========Bat dau btnReadAllGLogData_Click------ReadAllGLogData========");
            bRet = pOcxObject.ReadAllGLogData(m_nCurSelID);
            Logger.LogError($"\n==========ReadAllGLogData xong: {bRet}========");
            if (!bRet)
            {
                ShowErrorInfo();

                pOcxObject.EnableDevice(m_nCurSelID, 1);
                return;
            }
            Logger.LogError($"\n==========Bat dau btnReadAllGLogData_Click------GetAllGLogData========");
            do
            {
                bRet = pOcxObject.GetAllGLogData(m_nCurSelID,
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
            Logger.LogError($"\n==========GetAllGLogData xong----Count: {myArray.Count}========");
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

            pOcxObject.EnableDevice(m_nCurSelID, 1);
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
            var connectionString = "Driver={Microsoft Access Driver (*.mdb)};" + $"Dbq={textPathByAIKYO.Text};Uid=;Pwd=;ExtendedAnsiSQL=1;";
            //connectionString = "Driver={Microsoft Access Driver (*.mdb)};" + $"Dbq={textPathByAIKYO.Text};Uid=Admin;Pwd=Wse@2021!#$;ExtendedAnsiSQL=1;"; // DB có thiết lập pass

            //if (!string.IsNullOrWhiteSpace(Utility.GetAppSetting("ConnectionStringODBCRonaldJackAccess")))
            //{
            //    connectionString = Utility.GetAppSetting("ConnectionStringODBCRonaldJackAccess");
            //}
            List<LogData> timeKeeperDatas = new List<LogData>();
            try
            {
                var con = new OdbcConnection();
                con.ConnectionString = connectionString;
                var cmd = new OdbcCommand { Connection = con };
                con.Open();
                // "Driver={Microsoft Access Driver (*.mdb)};DBQ=C:\Users\ADMIN\Desktop\Git\BioMatrix\TaiLieu\AIKYO\RJData.mdb;Uid=;Pwd=;ExtendedAnsiSQL=1;"
                //cmd.CommandText = "SELECT a.UserFullCode, a.TimeStr FROM InOutRun a;";

                cmd.CommandText = "SELECT a.UserFullCode, a.TimeStr FROM InOutRun a WHERE a.TimeStr BETWEEN ? AND ?;";
                cmd.Parameters.Add("?", OdbcType.DateTime).Value = fromDate.Date;
                cmd.Parameters.Add("?", OdbcType.DateTime).Value = toDate.Date.AddDays(1).AddMilliseconds(-1);




                // "Driver={Microsoft Access Driver (*.mdb)};DBQ=C:\Users\ADMIN\Desktop\Git\BioMatrix\TaiLieu\Access.mdb;Uid=;Pwd=;ExtendedAnsiSQL=1;"
                //cmd.CommandText = "SELECT c.USERID, c.CHECKTIME FROM CHECKINOUT c;";

                //cmd.CommandText = "SELECT c.USERID, c.CHECKTIME FROM CHECKINOUT c WHERE c.CHECKTIME BETWEEN ? AND ?;";
                //cmd.Parameters.Add("?", OdbcType.DateTime).Value = fromDate.Date;
                //cmd.Parameters.Add("?", OdbcType.DateTime).Value = toDate.Date.AddDays(1).AddMilliseconds(-1);


                // DataBase có dùng mật khẩu admin => tham khảo https://www.connectionstrings.com/access/
                //"Driver={Microsoft Access Driver (*.mdb, *.accdb)};Dbq=C:\Users\ADMIN\Desktop\Git\BioMatrix\TaiLieu\WiseEyeMix3\WiseEyeMix3.mdb;Uid=Admin;Pwd=Wse@2021!#$;ExtendedAnsiSQL=1;";
                //cmd.CommandText = "SELECT c.UserEnrollNumber, c.TimeStr FROM CHECKINOUT c;";

                //cmd.CommandText = "SELECT c.UserEnrollNumber, c.TimeStr FROM CHECKINOUT c WHERE c.TimeStr BETWEEN ? AND ?;";
                //cmd.Parameters.Add("?", OdbcType.DateTime).Value = fromDate.Date;
                //cmd.Parameters.Add("?", OdbcType.DateTime).Value = toDate.Date.AddDays(1).AddMilliseconds(-1);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LogData timeKeeperData = new LogData();
                        var userID = 1;
                        if (reader[0] != null && int.TryParse(reader[0].ToString(), out userID))
                        {
                            timeKeeperData.UserID = userID.ToString();
                            var time = DateTime.Now;
                            if (reader[1] != null && DateTime.TryParse(reader[1].ToString(), out time))
                            {
                                timeKeeperData.CheckTime = time;
                                timeKeeperDatas.Add(timeKeeperData);
                            }
                        }
                    }
                }
                con.Close();
            }
            catch (Exception ex)
            {
                message += ex.ToString();
                Logger.LogInfo($"\n==============GetTimeKeeperDataFromAccess : Exception: {ex.Message}==============");
                return new List<LogData>();
            }
            return timeKeeperDatas;
        }
        #endregion
        #region Sunbeam
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
                message += ex.Message;
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
        #endregion
        #region Dahahi
        private static readonly HttpClient client = new HttpClient();
        private void btnLoginByDahahi_Click(object sender, EventArgs e)
        {
            var message = "";
            var success = ConnectByDahahi(ref message);
            tbTotalLogByDahahi.Text = success ? "Đăng nhập thành công" : "Đăng nhập thất bại";
        }

        private void btnGetLogsByDahahi_Click(object sender, EventArgs e)
        {
            var message = "";
            var logs = GetLogsByDahahi(dtFromDateByDahahi.Value, dtToDateByDahahi.Value, 1, ref message);
            tbTotalLogByDahahi.Text = logs.Count.ToString();
            BindToGridView(logs);
        }
        public bool ConnectByDahahi(ref string message)
        {
            bool success = false;

            //if (Config != null)
            //{
            var domain = string.Empty;
            if (!string.IsNullOrWhiteSpace(tbWebsiteByDahahi.Text))
            {
                if (tbWebsiteByDahahi.Text.StartsWith("http://"))
                {
                    domain = tbWebsiteByDahahi.Text.Replace("http://", "");
                }
                else if (tbWebsiteByDahahi.Text.StartsWith("https://"))
                {
                    domain = tbWebsiteByDahahi.Text.Replace("https://", "");
                }
                else
                {
                    domain = tbWebsiteByDahahi.Text;
                }
                if (domain.EndsWith("/"))
                {
                    domain = domain.Substring(0, domain.Length - 1);
                }
                if (!domain.EndsWith(".dahahi.vn"))
                {
                    domain = domain + ".dahahi.vn";
                }
            }
            success = Utility.Ping(domain, ref message, 3);
            if (success)
            {
                string url = $"{tbWebsiteByDahahi.Text}/api/faceid/getAllMachine";
                try
                {

                    var httpRequestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(url),
                        Headers = {
                                    {"AppKey", Utility.GetAppSetting("appKeyDahahi") },
                                    {"SecretKey", tbSecretKeyByDahahi.Text }
                                },
                    };

                    var response = client.SendAsync(httpRequestMessage).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert.DeserializeObject<ResponseDataMachine>(responseContent);
                        message += $"\nLogin Mess: {data.ErrorMessage}---ErrorCode: {data.ErrorCode}---Total: {data.Total}";
                        if (data.ErrorCode != "999999")
                        {
                            if (data.Data.Count > 0)
                            {
                                var machine = data.Data.Find(x => x.MachineBoxId == tbCodeByDahahi.Text);
                                if (machine != null && !string.IsNullOrWhiteSpace(machine.MachineBoxId))
                                {
                                    success = true;
                                }
                                else
                                {
                                    success = false;
                                    message += $"\nMachine don't exist";
                                }
                            }
                            else
                            {
                                success = false;
                                message += $"\nMachine don't exist";
                            }
                        }
                        else
                        {
                            success = false;
                            message += $"\nApi Secret Key wrong";
                        }
                    }
                    else
                    {
                        success = false;
                        message += $"\nCannot login to {domain} - " + response.StatusCode.ToString();
                    }
                }
                catch (Exception ex)
                {
                    message += $"\nError " + ex.Message;
                }

            }
            else
            {
                success = false;
                message += $"\nCannot login to {domain}";
            }
            //}

            return success;
        }

        public List<LogData> GetLogsByDahahi(DateTime? fromDate, DateTime? toDate, int? limit, ref string message)
        {
            var listLogs = GetLogDatasByDahahi(fromDate, toDate, 1, 50, ref message);
            return listLogs;
        }
        /// <summary>
        /// Đệ quy lấy dữ liệu
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private List<LogData> GetLogDatasByDahahi(DateTime? fromDate, DateTime? toDate, int page, int limit, ref string message)
        {
            var lstLog = new List<LogData>();
            var domain = string.Empty;
            if (tbWebsiteByDahahi.Text != null)
            {
                if (tbWebsiteByDahahi.Text.StartsWith("http://"))
                {
                    domain = tbWebsiteByDahahi.Text.Replace("http://", "");
                }
                else if (tbWebsiteByDahahi.Text.StartsWith("https://"))
                {
                    domain = tbWebsiteByDahahi.Text.Replace("https://", "");
                }
                else
                {
                    domain = tbWebsiteByDahahi.Text;
                }
                if (domain.EndsWith("/"))
                {
                    domain = domain.Substring(0, domain.Length - 1);
                }
                if (!domain.EndsWith(".dahahi.vn"))
                {
                    domain = domain + ".dahahi.vn";
                }
            }
            string url = $"{tbWebsiteByDahahi.Text}/api/facereg/checkinhis";
            var values = new Dictionary<string, string>
              {
                  { "MachineBoxId", tbCodeByDahahi.Text },
                  { "pageIndex", page.ToString() },
                  { "PageSize", limit.ToString() },
                  { "FromTimeStr", fromDate.Value.Date.ToString("dd/MM/yyyy HH:mm") },
                  { "ToTimeStr", toDate.Value.Date.AddDays(1).AddMilliseconds(-1).ToString("dd/MM/yyyy HH:mm") },
              };
            try
            {

                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url),
                    Headers = {
                        {"AppKey", Utility.GetAppSetting("appKeyDahahi") },
                        {"SecretKey", tbSecretKeyByDahahi.Text }
                    },
                    Content = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json")
                };

                var response = client.SendAsync(httpRequestMessage).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<ResponseData>(responseContent);
                    message += $"\nGetLog Mess: {data.ErrorMessage}---ErrorCode: {data.ErrorCode}---Total: {data.Total}";
                    if (data.Data.Count > 0)
                    {
                        foreach (var item in data.Data)
                        {
                            if (!string.IsNullOrEmpty(item.CheckinTimeStr))
                            {
                                if (!string.IsNullOrWhiteSpace(item.EmployeeCode))
                                {
                                    var log = new LogData()
                                    {
                                        FullName = item.EmployeeName,
                                        UserID = item.EmployeeCode,
                                        CheckTime = DateTime.ParseExact(item.CheckinTimeStr, "d/M/yyyy H:m:s", null)
                                    };
                                    lstLog.Add(log);
                                }
                            }
                        }
                        if (data.Total > limit * page)
                        {
                            var datas = GetLogDatasByDahahi(fromDate, toDate, page + 1, limit, ref message);
                            lstLog.AddRange(datas);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = $"\nGetData {JsonConvert.SerializeObject(values)} -- page: {page} -- limit: {limit}" + ex.Message;
            }

            return lstLog;
        }
        #endregion
        #region Hikvision
        private void btnLoginByHikvision_Click(object sender, EventArgs e)
        {
            var message = string.Empty;
            int typeHikvision = 0;
            int.TryParse(Utility.GetAppSetting("TypeHikvision"), out typeHikvision);
            var success = false;
            if (typeHikvision == 1)
            {
                success = ConnectByHikvision(ref message);
            }
            else if (typeHikvision == 2)
            {
                success = ConnectByHikvision_V2(ref message);
            }
            else
            {
                success = ConnectByHikvision(ref message);
            }
            Logger.LogError($"=============btnLoginByHikvision_Click: {message}================");
            tbTotalByHikvision.Text = success ? "Thành công" : "Thất bại";
        }
        private void btnGetLogsByHikvision_Click(object sender, EventArgs e)
        {
            var message = string.Empty;
            int typeHikvision = 0;
            int.TryParse(Utility.GetAppSetting("TypeHikvision"), out typeHikvision);
            var logs = new List<LogData>();
            var limit = 100;
            int.TryParse(Utility.GetAppSetting("LimitHikvision"), out limit);
            if (typeHikvision == 1)
            {
                logs = GetLogsByHikvision(dtFromDateByHikvision.Value.Date, dtToDateByHikvision.Value.Date.AddDays(1).AddSeconds(-1), limit, ref message);
            }
            else if (typeHikvision == 2)
            {
                logs = GetLogDatasByHikvision_V2(ref message);
            }
            else
            {
                logs = GetLogDatas_V2(dtFromDateByHikvision.Value.Date, dtToDateByHikvision.Value.Date.AddDays(1).AddSeconds(-1), 1, limit, ref message);
            }
            tbTotalByHikvision.Text = logs.Count.ToString();
            Logger.LogError($"=============btnGetLogsByHikvision_Click: {message}================");
            BindToGridView(logs);
        }
        public bool ConnectByHikvision(ref string message)
        {
            bool success = true;
            try
            {
                //if (Config != null)
                //{
                //success = Utility.Ping(tbIPByHikvision.Text, ref message, 3);
                if (success)
                {
                    string url = "http://" + tbIPByHikvision.Text + (int.Parse(tbPortByHikvision.Text) > 0 ? ":" + tbPortByHikvision.Text : "") + "/ISAPI/Security/userCheck?format=json";
                    string response = string.Empty;

                    HttpClient http = new HttpClient();
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Credentials = GetCredentialCacheByHikvision(url, tbUserNameByHikvision.Text, tbPassByHikvision.Text);
                    request.Method = "GET";
                    request.Timeout = 5000;
                    string strRsp = string.Empty;
                    try
                    {
                        Logger.LogError($"\nBat dau ConnectByHikvision_GetResponse----request: {JsonConvert.SerializeObject(request)}");
                        message += $"\nBat dau ConnectByHikvision_GetResponse";
                        WebResponse wr = request.GetResponse();
                        Logger.LogError($"\nBat dau ConnectByHikvision_GetResponse xong");
                        message += $"\nBat dau ConnectByHikvision_GetResponse xong";
                        strRsp = new StreamReader(wr.GetResponseStream()).ReadToEnd();
                        Logger.LogError($"\nBat dau ConnectByHikvision_GetResponseStream xong: {strRsp}");
                        message += $"\nBat dau ConnectByHikvision_GetResponseStream xong: {strRsp}";
                        wr.Close();
                        _isConnected = true;
                        success = true;
                    }
                    catch (WebException ex)
                    {
                        WebResponse wenReq = (HttpWebResponse)ex.Response;
                        if (wenReq != null)
                        {
                            strRsp = new StreamReader(wenReq.GetResponseStream()).ReadToEnd();
                            wenReq.Close();
                        }
                        success = false;
                        Logger.LogError($"\nCannot login to {tbIPByHikvision.Text}" + ex.Message);
                        message += $"\nCannot login to {tbIPByHikvision.Text}" + ex.Message;
                    }
                }
                else
                {
                    Logger.LogError("\nCannot connect to {tbIPByHikvision.Text}");
                    message += $"\nCannot connect to {tbIPByHikvision.Text}";
                }
                //}
                //else
                //{
                //    message = "Invalid config";
                //}
            }
            catch (Exception ex)
            {
                success = false;
                Logger.LogError($"\n{ex.Message}");
                message += $"\n{ex.Message}";
            }
            return success;
        }

        public List<LogData> GetLogsByHikvision(DateTime? fromDate, DateTime? toDate, int? limit, ref string message)
        {
            var listLogs = GetLogDatasByHikvision(fromDate, toDate, 1, 50, ref message);
            return listLogs;
        }
        /// <summary>
        /// Đệ quy lấy dữ liệu
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private List<LogData> GetLogDatasByHikvision(DateTime? fromDate, DateTime? toDate, int page, int limit, ref string message)
        {
            Logger.LogError($"\n===========GetLogDatasByHikvision: Page: {page}======Limit:{limit}===fromDate: {fromDate}===toDate: {toDate}");
            message += $"\n===========GetLogDatasByHikvision: Page: {page}======Limit:{limit}";
            var lstLog = new List<LogData>();
            string url = "http://" + tbIPByHikvision.Text + (int.Parse(tbPortByHikvision.Text) > 0 ? ":" + tbPortByHikvision.Text : "") + "/ISAPI/AccessControl/AcsEvent?format=json";
            string response = string.Empty;

            HttpClient http = new HttpClient();
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            Logger.LogError($"\n===========GetLogDatasByHikvision Credential: Page: {page}======Limit:{limit}===url: {url}");
            message += $"\n===========GetLogDatasByHikvision Credential: Page: {page}======Limit:{limit}";
            request.Credentials = GetCredentialCacheByHikvision(url, tbUserNameByHikvision.Text, tbPassByHikvision.Text);
            Logger.LogError($"\n===========GetLogDatasByHikvision Credential xong");
            message += $"\n===========GetLogDatasByHikvision Credential xong";
            request.Method = "POST";
            request.Timeout = 100000;
            // add request
            var req = request;
            bool customStrReq = false;
            Boolean.TryParse(Utility.GetAppSetting("CustomStrReq"), out customStrReq);
            string strReq = "{ \"AcsEventCond\": { \"searchID\": \"1\", \"searchResultPosition\": " + ((page - 1) * limit) + ", \"maxResults\": " + limit + ", \"startTime\": \"" + fromDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"endTime\": \"" + toDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"major\": 0, \"minor\": 0, \"timeReverseOrder\": true, \"eventAttribute\": \"attendance\" } }";
            if (customStrReq)
            {
                int customMajor = 0;
                int.TryParse(Utility.GetAppSetting("CustomMajor"), out customMajor);
                bool customPicEnable = false;
                Boolean.TryParse(Utility.GetAppSetting("CustomPicEnable"), out customPicEnable);
                if (customPicEnable)
                {
                    strReq = "{ \"AcsEventCond\": { \"searchID\": \"1\", \"searchResultPosition\": " + ((page - 1) * limit) + ", \"maxResults\": " + limit + ", \"startTime\": \"" + fromDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"endTime\": \"" + toDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"minor\": 0, \"major\": " + customMajor + ", \"picEnable\": true" + ", \"timeReverseOrder\": true, \"eventAttribute\": \"attendance\" } }";
                }
                else
                {
                    strReq = "{ \"AcsEventCond\": { \"searchID\": \"1\", \"searchResultPosition\": " + ((page - 1) * limit) + ", \"maxResults\": " + limit + ", \"startTime\": \"" + fromDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"endTime\": \"" + toDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"minor\": 0, \"major\": " + customMajor + ", \"picEnable\": false" + ", \"timeReverseOrder\": true, \"eventAttribute\": \"attendance\" } }";
                }
            }
            if (strReq.Length > 0)
            {
                Logger.LogError($"\n===========GetLogDatasByHikvision request: Page: {page}======Limit:{limit}");
                message += $"\n===========GetLogDatasByHikvisionn request: Page: {page}======Limit:{limit}";
                byte[] bs = Encoding.ASCII.GetBytes(strReq);

                request.ContentType = "application/json";
                request.ContentLength = bs.Length;
                Logger.LogError($"\n===========Bat dau Write: Page: {page}======Limit:{limit}===request: {JsonConvert.SerializeObject(request)}");
                message += $"\n===========Bat dau Write: Page: {page}======Limit:{limit}";
                bool customWriteHikvision = false;
                Boolean.TryParse(Utility.GetAppSetting("CustomWriteHikvision"), out customWriteHikvision);
                if (!customWriteHikvision)
                {
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                    }
                    message += $"\n===========WriteHikvision xong";
                }
            }
            string strRsp = string.Empty;
            bool getLogByHikvision = false;
            Boolean.TryParse(Utility.GetAppSetting("GetLogByHikvision"), out getLogByHikvision);
            try
            {
                Logger.LogError($"\n===========Bat dau lay du lieu: Page: {page}======Limit:{limit}===request: {JsonConvert.SerializeObject(request)}");
                message += $"\n===========Bat dau lay du lieu: Page: {page}======Limit:{limit}";
                using (WebResponse wr = request.GetResponse())
                {
                    strRsp = new StreamReader(wr.GetResponseStream()).ReadToEnd();
                }
                Logger.LogError($"\n==============Page: {page}=============Limit: {limit}=========================Data: {strRsp} ==============");
                if (getLogByHikvision)
                {
                    message += $"\n==============Page: {page}=============Limit: {limit}=========================Data: {strRsp} ==============";
                }
                else
                {
                    message += $"\n==============Page: {page}=============Limit: {limit}=========================";
                }
                GetDatasByHikvision(strRsp, fromDate, toDate, page, limit, ref lstLog, ref message);
            }
            catch (WebException ex)
            {
                Logger.LogError($"\nError1.1: " + ex.ToString());
                message += $"\nError1.1: " + ex.ToString();
                bool customGetResponseStream = false;
                Boolean.TryParse(Utility.GetAppSetting("CustomGetResponseStream"), out customGetResponseStream);
                if (!customGetResponseStream)
                {
                    WebResponse wenReq = (HttpWebResponse)ex.Response;
                    Logger.LogError($"\nError1.1: Khoi tao xong");
                    message += $"\nError1.1: Khoi tao xong";
                    if (wenReq != null)
                    {
                        message += $"\nError1.1: Bat dau doc du lieu";
                        strRsp = new StreamReader(wenReq.GetResponseStream()).ReadToEnd();
                        wenReq.Close();
                        Logger.LogError($"\n==============Error1.1=====Page: {page}=============Limit: {limit}=========================Data: {strRsp} ==============");
                        if (getLogByHikvision)
                        {
                            message += $"\n==============Error1.1=====Page: {page}=============Limit: {limit}=========================Data: {strRsp} ==============";
                        }
                        else
                        {
                            message += $"\n==============Error1.1=====Page: {page}=============Limit: {limit}=========================";
                        }
                        GetDatasByHikvision(strRsp, fromDate, toDate, page, limit, ref lstLog, ref message);
                    }
                }
                else
                {
                    WebResponse wenReq = (HttpWebResponse)ex.Response;
                    Logger.LogError($"\nError1.2: Khoi tao xong");
                    message += $"\nError1.2: Khoi tao xong";
                    if (wenReq != null)
                    {
                        message += $"\nError1.2: Bat dau doc du lieu";
                        using (WebResponse wr = request.GetResponse())
                        {
                            strRsp = new StreamReader(wr.GetResponseStream()).ReadToEnd();
                        }
                        Logger.LogError($"\n==============Error1.2======Page: {page}=============Limit: {limit}=========================Data: {strRsp} ==============");
                        if (getLogByHikvision)
                        {
                            message += $"\n==============Error1.2======Page: {page}=============Limit: {limit}=========================Data: {strRsp} ==============";
                        }
                        else
                        {
                            message += $"\n==============Error1.2======Page: {page}=============Limit: {limit}=========================";
                        }
                        GetDatasByHikvision(strRsp, fromDate, toDate, page, limit, ref lstLog, ref message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\nError2: " + ex.ToString());
                message += $"\nError2: " + ex.ToString();
            }

            return lstLog;
        }

        private void GetDatasByHikvision(string strRsp, DateTime? fromDate, DateTime? toDate, int page, int limit, ref List<LogData> lstLog, ref string message)
        {
            EventSearchRoot dr = JsonConvert.DeserializeObject<EventSearchRoot>(strRsp);
            if (string.IsNullOrWhiteSpace(strRsp))
            {
                message += $"\n==========strRsp IS NULL=========";
                var tmp = GetLogDatasByHikvision(fromDate, toDate, page, limit, ref message);
                message += $"\n---strRsp IS NULL=======Count {tmp.Count} --- ";
                if (tmp.Count > 0)
                {
                    lstLog.AddRange(tmp);
                }
            }
            int num = dr != null ? Int32.Parse(dr.AcsEvent.numOfMatches) : 0;
            int total = dr != null ? Int32.Parse(dr.AcsEvent.totalMatches) : 10000000;
            string status = dr != null ? dr.AcsEvent.responseStatusStrg : "MORE";
            Logger.LogError($"\nGetDatasByHikvision---num: {num}---total: {total}---status: {status}");
            if (num > 0 && dr != null)
            {
                for (int j = 0; j < dr.AcsEvent.InfoList.Count(); ++j)
                {
                    if (!string.IsNullOrWhiteSpace(dr.AcsEvent.InfoList[j].employeeNoString))
                    {
                        var log = new LogData()
                        {
                            CheckTime = DateTime.Parse(dr.AcsEvent.InfoList[j].time),
                            FullName = dr.AcsEvent.InfoList[j].name,
                            UserID = dr.AcsEvent.InfoList[j].employeeNoString,
                        };
                        lstLog.Add(log);
                    }
                }
                message += $"Page {page}, size {num}, count {lstLog.Count}, status {status} ---total: {total}----- ";
            }
            if ((page - 1) * limit + num < total)
            {
                int pa = page + 1;
                bool reloadPageSize = false;
                var pageSize = limit;
                Boolean.TryParse(Utility.GetAppSetting("ReloadPageSize"), out reloadPageSize);
                if (reloadPageSize)
                {
                    pageSize = num;
                }
                Logger.LogError($"\n Goi lai GetLogDatasByHikvision---pa: {pa}---num:{pageSize}");
                var tmp = GetLogDatasByHikvision(fromDate, toDate, pa, pageSize, ref message);
                message += $"--- Count {tmp.Count} --- ";

                if (tmp.Count > 0)
                {
                    lstLog.AddRange(tmp);
                }
            }
        }
        private CredentialCache GetCredentialCacheByHikvision(string sUrl, string strUserName, string strPassword)
        {
            Logger.LogError($"\n GetCredentialCacheByHikvision---sUrl: {sUrl}---strUserName:{strUserName}---strPassword: {strPassword}");
            var credentialCache = new CredentialCache();
            credentialCache.Add(new Uri(sUrl), "Digest", new NetworkCredential(strUserName, strPassword));
            return credentialCache;
        }


        public bool ConnectByHikvision_V2(ref string message)
        {
            var success = false;
            try
            {
                message += $"\nBat dau ConnectByHikvision_V2";
                var url = $"http://{tbIPByHikvision.Text}:{tbPortByHikvision.Text}/ISAPI/Security/userCheck?format=json";
                var client = new RestClient($"http://{tbIPByHikvision.Text}:{tbPortByHikvision.Text}/ISAPI/Security/userCheck?format=json");
                var request = new RestRequest("values", Method.GET);
                request.Timeout = 5000;
                request.AddHeader("Content-Type", "application/json");

                request.Credentials = GetCredentialCacheByHikvision(url, tbUserNameByHikvision.Text, tbPassByHikvision.Text);
                message += $"\nConnectByHikvision_V2----Du lieu truyen len: {Converter.JsonSerialize(request)}";
                var response = client.Execute(request);
                message += $"\nKet qua lay du lieu ConnectByHikvision_V2: success: {response.IsSuccessful}-----data: {Converter.JsonSerialize(response.Content)}----ErrorMessage: {response.ErrorMessage}";
            }
            catch (Exception ex)
            {
                message += $"\nConnectByHikvision_V2 Exception: {ex.Message}";
            }
            return success;
        }
        public List<LogData> GetLogDatasByHikvision_V2(ref string message)
        {
            var lstLog = new List<LogData>();
            try
            {
                message += $"\nBat dau GetLogDatasByHikvision_V2";
                var url = $"http://{tbIPByHikvision.Text}:{tbPortByHikvision.Text}/ISAPI/AccessControl/AcsEvent?format=json";
                var client = new RestClient($"http://{tbIPByHikvision.Text}:{tbPortByHikvision.Text}/ISAPI/AccessControl/AcsEvent?format=json");
                var request = new RestRequest("values", Method.POST);
                request.Timeout = 100000;
                request.AddHeader("Content-Type", "application/json");
                request.Credentials = GetCredentialCacheByHikvision(url, tbUserNameByHikvision.Text, tbPassByHikvision.Text);

                var total = 0;
                var num = 0;
                var page = 1;
                var limit = 100;
                int.TryParse(Utility.GetAppSetting("LimitHikvision"), out limit);
                do
                {
                    var param = new
                    {
                        AcsEventCond = new
                        {
                            searchID = "1",
                            searchResultPosition = ((page - 1) * limit),
                            maxResults = limit,
                            startTime = dtFromDateByHikvision.Value.Date.ToString("yyyy-MM-dd'T'HH:mm:sszzz"),
                            endTime = dtToDateByHikvision.Value.Date.AddDays(1).AddMinutes(-1).ToString("yyyy-MM-dd'T'HH:mm:sszzz"),
                            major = 0,
                            minor = 0,
                            timeReverseOrder = true,
                            eventAttribute = "attendance"
                        }
                    };
                    request.AddParameter("application/json", Converter.JsonSerialize(param), ParameterType.RequestBody);
                    message += $"\n GetLogDatasByHikvision_V2----Du lieu truyen len: {Converter.JsonSerialize(request)}";
                    var response = client.Execute(request);
                    message += $"\nKet qua lay du lieu nGetLogDatasByHikvision_V2: success: {response.IsSuccessful}-----data: {Converter.JsonSerialize(response.Content)}----ErrorMessage: {response.ErrorMessage}";
                    var logs = JsonConvert.DeserializeObject<EventSearchRoot>(response.Content);
                    var infoList = new List<Hikvision.EventInfo>();
                    if (logs != null && logs.AcsEvent != null)
                    {
                        infoList = logs.AcsEvent.InfoList;
                        num = int.TryParse(logs.AcsEvent.numOfMatches, out num) ? int.Parse(logs.AcsEvent.numOfMatches) : 0;
                        total = int.TryParse(logs.AcsEvent.totalMatches, out total) ? int.Parse(logs.AcsEvent.totalMatches) : 0;
                    }
                    message += $"\nGetLogDatasByHikvision_V2: infoList: {Converter.JsonSerialize(infoList)}";
                    foreach (var data in infoList)
                    {
                        var checkTime = DateTime.Now;
                        if (!string.IsNullOrWhiteSpace(data.employeeNoString) && DateTime.TryParse(data.time, out checkTime))
                        {
                            var log = new LogData()
                            {
                                CheckTime = DateTime.Parse(data.time),
                                FullName = data.name,
                                UserID = data.employeeNoString,
                            };
                            lstLog.Add(log);
                        }
                    }
                    message += $"\nGetLogDatasByHikvision_V2: lstLog: {Converter.JsonSerialize(lstLog)}";
                    page++;
                } while ((page - 1) * limit + num < total);
            }
            catch (Exception ex)
            {
                message += $"\nGetLogDatasByHikvision_V2 Exception: {ex.Message}";
            }
            return lstLog;
        }



        //private List<LogData> GetLogDatas_V2(DateTime? fromDate, DateTime? toDate, int page, int limit, ref string message)
        //{
        //    var lstLog = new List<LogData>();
        //    var total = 0;
        //    var pageIndex = page;
        //    var pageSize = limit;
        //    Logger.LogError($"\n===========GetLogDatas_V2: Page: {pageIndex}======Limit:{pageSize}");
        //    message += $"\n===========GetLogDatas_V2: Page: {pageIndex}======Limit:{pageSize}";
        //    do
        //    {
        //        string url = "http://" + tbIPByHikvision.Text + (int.Parse(tbPortByHikvision.Text) > 0 ? ":" + tbPortByHikvision.Text : "") + "/ISAPI/AccessControl/AcsEvent?format=json";
        //        string response = string.Empty;

        //        HttpClient http = new HttpClient();
        //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //        Logger.LogError($"\n===========GetLogDatas_V2 Credential: Page: {pageIndex}======Limit:{pageSize}===url: {url}");
        //        message += $"\n===========GetLogDatas_V2 Credential: Page: {pageIndex}======Limit:{pageSize}";
        //        request.Credentials = GetCredentialCacheByHikvision(url, tbUserNameByHikvision.Text, tbPassByHikvision.Text);
        //        request.Method = "POST";
        //        request.Timeout = 100000;
        //        // add request
        //        bool customStrReq = false;
        //        Boolean.TryParse(Utility.GetAppSetting("CustomStrReq"), out customStrReq);
        //        string strReq = "{ \"AcsEventCond\": { \"searchID\": \"1\", \"searchResultPosition\": " + ((page - 1) * limit) + ", \"maxResults\": " + limit + ", \"startTime\": \"" + fromDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"endTime\": \"" + toDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"major\": 0, \"minor\": 0, \"timeReverseOrder\": true, \"eventAttribute\": \"attendance\" } }";
        //        if (customStrReq)
        //        {
        //            int customMajor = 0;
        //            int.TryParse(Utility.GetAppSetting("CustomMajor"), out customMajor);
        //            bool customPicEnable = false;
        //            Boolean.TryParse(Utility.GetAppSetting("CustomPicEnable"), out customPicEnable);
        //            if (customPicEnable)
        //            {
        //                strReq = "{ \"AcsEventCond\": { \"searchID\": \"1\", \"searchResultPosition\": " + ((pageIndex - 1) * pageSize) 
        //                    + ", \"maxResults\": " + pageSize 
        //                    + ", \"startTime\": \"" + fromDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"endTime\": \"" + toDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") 
        //                    + "\", \"minor\": 0, \"major\": " + customMajor + ", \"picEnable\": true" + ", \"timeReverseOrder\": true, \"eventAttribute\": \"attendance\" } }";
        //            }
        //            else
        //            {
        //                strReq = "{ \"AcsEventCond\": { \"searchID\": \"1\", \"searchResultPosition\": " + ((pageIndex - 1) * pageSize) 
        //                    + ", \"maxResults\": " + pageSize 
        //                    + ", \"startTime\": \"" + fromDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"endTime\": \"" + toDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") 
        //                    + "\", \"minor\": 0, \"major\": " + customMajor + ", \"picEnable\": false" + ", \"timeReverseOrder\": true, \"eventAttribute\": \"attendance\" } }";
        //            }
        //        }
        //        if (strReq.Length > 0)
        //        {
        //            Logger.LogError($"\n===========GetLogDatas_V2 request: Page: {pageIndex}======Limit:{pageSize}");
        //            message += $"\n===========GetLogDatas_V2 request: Page: {pageIndex}======Limit:{pageSize}";
        //            byte[] bs = Encoding.ASCII.GetBytes(strReq);

        //            request.ContentType = "application/json";
        //            request.ContentLength = bs.Length;
        //            Logger.LogError($"\n===========GetLogDatas_V2 Write: Page: {pageIndex}======Limit:{pageSize}===request: {JsonConvert.SerializeObject(request)}");
        //            message += $"\n===========GetLogDatas_V2 Write: Page: {pageIndex}======Limit:{pageSize}";
        //            bool customWriteHikvision = false;
        //            Boolean.TryParse(Utility.GetAppSetting("CustomWriteHikvision"), out customWriteHikvision);
        //            if (!customWriteHikvision)
        //            {
        //                using (Stream reqStream = request.GetRequestStream())
        //                {
        //                    reqStream.Write(bs, 0, bs.Length);
        //                }
        //            }
        //        }
        //        string strRsp = string.Empty;
        //        bool getLogByHikvision = false;
        //        Boolean.TryParse(Utility.GetAppSetting("GetLogByHikvision"), out getLogByHikvision);
        //        try
        //        {
        //            Logger.LogError($"\n===========GetLogDatas_V2 Bat dau lay du lieu: Page: {pageIndex}======Limit:{pageSize}===request: {JsonConvert.SerializeObject(request)}");
        //            message += $"\n===========GetLogDatas_V2 Bat dau lay du lieu: Page: {pageIndex}======Limit:{pageSize}";
        //            using (WebResponse wr = request.GetResponse())
        //            {
        //                strRsp = new StreamReader(wr.GetResponseStream()).ReadToEnd();
        //            }
        //            Logger.LogError($"\n==============GetLogDatas_V2 Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============");
        //            if (getLogByHikvision)
        //            {
        //                message += $"\n==============Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============";
        //            }
        //            else
        //            {
        //                message += $"\n==============Page: {pageIndex}=============Limit: {pageSize}=========================";
        //            }
        //            GetDatas_V2(strRsp, ref lstLog, ref message, ref total, ref pageIndex, ref pageSize);
        //        }
        //        catch (WebException ex)
        //        {
        //            Logger.LogError($"\nError1.1: " + ex.ToString());
        //            message += $"\nError1.1: " + ex.ToString();
        //            bool customGetResponseStream = false;
        //            Boolean.TryParse(Utility.GetAppSetting("CustomGetResponseStream"), out customGetResponseStream);
        //            if (!customGetResponseStream)
        //            {
        //                WebResponse wenReq = (HttpWebResponse)ex.Response;
        //                Logger.LogError($"\nError1.1: Khoi tao xong");
        //                message += $"\nError1.1: Khoi tao xong";
        //                if (wenReq != null)
        //                {
        //                    Logger.LogError($"\nError1.1: Bat dau doc du lieu");
        //                    message += $"\nError1.1: Bat dau doc du lieu";
        //                    strRsp = new StreamReader(wenReq.GetResponseStream()).ReadToEnd();
        //                    wenReq.Close();
        //                    Logger.LogError($"\n==============Error1.1=====Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============");
        //                    if (getLogByHikvision)
        //                    {
        //                        message += $"\n==============Error1.1=====Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============";
        //                    }
        //                    else
        //                    {
        //                        message += $"\n==============Error1.1=====Page: {pageIndex}=============Limit: {pageSize}=========================";
        //                    }
        //                    GetDatas_V2(strRsp, ref lstLog, ref message, ref total, ref pageIndex, ref pageSize);
        //                }
        //            }
        //            else
        //            {
        //                WebResponse wenReq = (HttpWebResponse)ex.Response;
        //                Logger.LogError($"\nError1.2: Khoi tao xong");
        //                message += $"\nError1.2: Khoi tao xong";
        //                if (wenReq != null)
        //                {
        //                    Logger.LogError($"\nError1.2: Bat dau doc du lieu");
        //                    message += $"\nError1.2: Bat dau doc du lieu";
        //                    using (WebResponse wr = request.GetResponse())
        //                    {
        //                        strRsp = new StreamReader(wr.GetResponseStream()).ReadToEnd();
        //                    }
        //                    Logger.LogError($"\n==============Error1.2======Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============");
        //                    if (getLogByHikvision)
        //                    {
        //                        message += $"\n==============Error1.2======Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============";
        //                    }
        //                    else
        //                    {
        //                        message += $"\n==============Error1.2======Page: {pageIndex}=============Limit: {pageSize}=========================";
        //                    }
        //                    GetDatas_V2(strRsp, ref lstLog, ref message, ref total, ref pageIndex, ref pageSize);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.LogError($"\nError2: " + ex.ToString());
        //            message += $"\nError2: " + ex.ToString();
        //        }

        //    }
        //    while ((pageIndex - 1) * pageSize < total);

        //    return lstLog;
        //}
        //private void GetDatas_V2(string strRsp, ref List<LogData> lstLog, ref string message, ref int total, ref int pageIndex, ref int pageSize)
        //{
        //    Logger.LogError($"\nGetDatas_V2: Bat dau");
        //    if (string.IsNullOrWhiteSpace(strRsp))
        //    {
        //        return;
        //    }
        //    EventSearchRoot dr = JsonConvert.DeserializeObject<EventSearchRoot>(strRsp);
        //    int num = dr != null ? Int32.Parse(dr.AcsEvent.numOfMatches) : 0;
        //    total = dr != null ? Int32.Parse(dr.AcsEvent.totalMatches) : 10000000;
        //    string status = dr != null ? dr.AcsEvent.responseStatusStrg : "MORE";
        //    Logger.LogError($"\nGetDatas_V2---num: {num}---total: {total}---status: {status}");
        //    if (num > 0 && dr != null)
        //    {
        //        for (int j = 0; j < dr.AcsEvent.InfoList.Count(); ++j)
        //        {
        //            if (!string.IsNullOrWhiteSpace(dr.AcsEvent.InfoList[j].employeeNoString))
        //            {
        //                var log = new LogData()
        //                {
        //                    CheckTime = DateTime.Parse(dr.AcsEvent.InfoList[j].time),
        //                    FullName = dr.AcsEvent.InfoList[j].name,
        //                    UserID = dr.AcsEvent.InfoList[j].employeeNoString,
        //                    //EventType = EventType.IN
        //                };
        //                lstLog.Add(log);
        //            }
        //        }
        //        Logger.LogError($"\n Size numOfMatches: {num}, Count lstLog: {lstLog.Count}, status {status}, total totalMatches: {total} --- ");
        //        message += $"\n Size numOfMatches: {num}, Count lstLog: {lstLog.Count}, status {status}, total totalMatches: {total} --- ";
        //    }

        //    if ((pageIndex - 1) * pageSize + num < total)
        //    {
        //        pageIndex = pageIndex + 1;
        //        bool reloadPageSize = false;
        //        Boolean.TryParse(Utility.GetAppSetting("ReloadPageSize"), out reloadPageSize);
        //        if (reloadPageSize)
        //        {
        //            pageSize = num;
        //        }
        //        Logger.LogError($"\nGetDatas_V2---pageIndex: {pageIndex}---pageSize: {pageSize}");
        //    }
        //}


        private List<LogData> GetLogDatas_V2(DateTime? fromDate, DateTime? toDate, int page, int limit, ref string message)
        {
            var lstLog = new List<LogData>();
            var total = 0;
            var pageIndex = page;
            var pageSize = limit;
            message += $"\n===========GetLogDatas_V2: Page: {page}======Limit:{limit}===fromDate: {fromDate}===toDate: {toDate}";
            Logger.LogError($"\n===========GetLogDatas_V2: Page: {page}======Limit:{limit}===fromDate: {fromDate}===toDate: {toDate}");
            do
            {
                string url = "http://" + tbIPByHikvision.Text + (int.Parse(tbPortByHikvision.Text) > 0 ? ":" + tbPortByHikvision.Text : "") + "/ISAPI/AccessControl/AcsEvent?format=json";
                string response = string.Empty;

                HttpClient http = new HttpClient();
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                message += $"\n===========Bat dau kiem tra Credential: Page: {pageIndex}======Limit:{pageSize}";
                Logger.LogError($"\n===========Bat dau kiem tra Credential: Page: {pageIndex}======Limit:{pageSize}");
                request.Credentials = GetCredentialCacheByHikvision(url, tbUserNameByHikvision.Text, tbPassByHikvision.Text);
                request.Method = "POST";
                request.Timeout = 100000;
                // add request
                var req = request;
                bool customStrReq = false;
                Boolean.TryParse(Utility.GetAppSetting("CustomStrReq"), out customStrReq);
                string strReq = "{ \"AcsEventCond\": { \"searchID\": \"1\", \"searchResultPosition\": " + ((pageIndex - 1) * pageSize) + ", \"maxResults\": " + pageSize + ", \"startTime\": \"" + fromDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"endTime\": \"" + toDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"major\": 0, \"minor\": 0, \"timeReverseOrder\": true, \"eventAttribute\": \"attendance\" } }";
                if (customStrReq)
                {
                    int customMajor = 0;
                    int.TryParse(Utility.GetAppSetting("CustomMajor"), out customMajor);
                    bool customPicEnable = false;
                    Boolean.TryParse(Utility.GetAppSetting("CustomPicEnable"), out customPicEnable);
                    if (customPicEnable)
                    {
                        strReq = "{ \"AcsEventCond\": { \"searchID\": \"1\", \"searchResultPosition\": " + ((pageIndex - 1) * pageSize) + ", \"maxResults\": " + pageSize + ", \"startTime\": \"" + fromDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"endTime\": \"" + toDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"minor\": 0, \"major\": " + customMajor + ", \"picEnable\": true" + ", \"timeReverseOrder\": true, \"eventAttribute\": \"attendance\" } }";
                    }
                    else
                    {
                        strReq = "{ \"AcsEventCond\": { \"searchID\": \"1\", \"searchResultPosition\": " + ((pageIndex - 1) * pageSize) + ", \"maxResults\": " + pageSize + ", \"startTime\": \"" + fromDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"endTime\": \"" + toDate.Value.ToString("yyyy-M-d'T'HH:mm:sszzz") + "\", \"minor\": 0, \"major\": " + customMajor + ", \"picEnable\": false" + ", \"timeReverseOrder\": true, \"eventAttribute\": \"attendance\" } }";
                    }
                }
                if (strReq.Length > 0)
                {
                    message += $"\n===========Bat dau gan request: Page: {pageIndex}======Limit:{pageSize}";
                    Logger.LogError($"\n===========Bat dau gan request: Page: {pageIndex}======Limit:{pageSize}");
                    byte[] bs = Encoding.ASCII.GetBytes(strReq);

                    request.ContentType = "application/json";
                    request.ContentLength = bs.Length;
                    message += $"\n===========Bat dau Write: Page: {pageIndex}======Limit:{pageSize}";
                    Logger.LogError($"\n===========Bat dau Write: Page: {pageIndex}======Limit:{pageSize}");
                    bool customWriteHikvision = false;
                    Boolean.TryParse(Utility.GetAppSetting("CustomWriteHikvision"), out customWriteHikvision);
                    if (!customWriteHikvision)
                    {
                        using (Stream reqStream = request.GetRequestStream())
                        {
                            reqStream.Write(bs, 0, bs.Length);
                        }
                    }
                }
                string strRsp = string.Empty;
                bool getLogByHikvision = false;
                Boolean.TryParse(Utility.GetAppSetting("GetLogByHikvision"), out getLogByHikvision);
                try
                {
                    message += $"\n===========Bat dau lay du lieu: Page: {pageIndex}======Limit:{pageSize}";
                    Logger.LogError($"\n===========Bat dau lay du lieu: Page: {pageIndex}======Limit:{pageSize}");
                    using (WebResponse wr = request.GetResponse())
                    {
                        strRsp = new StreamReader(wr.GetResponseStream()).ReadToEnd();
                    }
                    Logger.LogError($"\n==============Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============");
                    if (getLogByHikvision)
                    {
                        message += $"\n==============Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============";
                    }
                    else
                    {
                        message += $"\n==============Page: {pageIndex}=============Limit: {pageSize}=========================";
                    }
                    GetDatas_V2(strRsp, ref lstLog, ref message, ref total, ref pageIndex, ref pageSize);
                }
                catch (WebException ex)
                {
                    message += $"\nError1.1: " + ex.ToString();
                    Logger.LogError($"\nError1.1: " + ex.ToString());
                    bool customGetResponseStream = false;
                    Boolean.TryParse(Utility.GetAppSetting("CustomGetResponseStream"), out customGetResponseStream);
                    if (!customGetResponseStream)
                    {
                        WebResponse wenReq = (HttpWebResponse)ex.Response;
                        message += $"\nError1.1: Khoi tao xong";
                        Logger.LogError($"\nError1.1: Khoi tao xong");
                        if (wenReq != null)
                        {
                            message += $"\nError1.1: Bat dau doc du lieu";
                            Logger.LogError($"\nError1.1: Bat dau doc du lieu");
                            strRsp = new StreamReader(wenReq.GetResponseStream()).ReadToEnd();
                            wenReq.Close();
                            Logger.LogError($"\n==============Error1.1=====Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============");
                            if (getLogByHikvision)
                            {
                                message += $"\n==============Error1.1=====Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============";
                            }
                            else
                            {
                                message += $"\n==============Error1.1=====Page: {pageIndex}=============Limit: {pageSize}=========================";
                            }
                            GetDatas_V2(strRsp, ref lstLog, ref message, ref total, ref pageIndex, ref pageSize);
                        }
                    }
                    else
                    {
                        WebResponse wenReq = (HttpWebResponse)ex.Response;
                        message += $"\nError1.2: Khoi tao xong";
                        Logger.LogError($"\nError1.2: Khoi tao xong");
                        if (wenReq != null)
                        {
                            message += $"\nError1.2: Bat dau doc du lieu";
                            Logger.LogError($"\nError1.2: Bat dau doc du lieu");
                            using (WebResponse wr = request.GetResponse())
                            {
                                strRsp = new StreamReader(wr.GetResponseStream()).ReadToEnd();
                            }
                            Logger.LogError($"\n==============Error1.2======Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============");
                            if (getLogByHikvision)
                            {
                                message += $"\n==============Error1.2======Page: {pageIndex}=============Limit: {pageSize}=========================Data: {strRsp} ==============";
                            }
                            else
                            {
                                message += $"\n==============Error1.2======Page: {pageIndex}=============Limit: {pageSize}=========================";
                            }
                            GetDatas_V2(strRsp, ref lstLog, ref message, ref total, ref pageIndex, ref pageSize);
                        }
                    }
                }
                catch (Exception ex)
                {
                    message += $"\nError2: " + ex.ToString();
                    Logger.LogError($"\nError2: " + ex.ToString());
                }

            }
            while ((pageIndex - 1) * pageSize < total);

            return lstLog;
        }
        private void GetDatas_V2(string strRsp, ref List<LogData> lstLog, ref string message, ref int total, ref int pageIndex, ref int pageSize)
        {
            Logger.LogError($"\nGetDatas_V2==strRsp: {strRsp}");
            if (string.IsNullOrWhiteSpace(strRsp))
            {
                return;
            }
            EventSearchRoot dr = JsonConvert.DeserializeObject<EventSearchRoot>(strRsp);
            int num = dr != null ? Int32.Parse(dr.AcsEvent.numOfMatches) : 0;
            total = dr != null ? Int32.Parse(dr.AcsEvent.totalMatches) : 10000000;
            string status = dr != null ? dr.AcsEvent.responseStatusStrg : "MORE";
            Logger.LogError($"\n num: {num}===total: {total}===status: {status}");
            if (num > 0 && dr != null)
            {
                for (int j = 0; j < dr.AcsEvent.InfoList.Count(); ++j)
                {
                    if (!string.IsNullOrWhiteSpace(dr.AcsEvent.InfoList[j].employeeNoString))
                    {
                        var log = new LogData()
                        {
                            CheckTime = DateTime.Parse(dr.AcsEvent.InfoList[j].time),
                            FullName = dr.AcsEvent.InfoList[j].name,
                            UserID = dr.AcsEvent.InfoList[j].employeeNoString,
                            //EventType = EventType.IN
                        };
                        lstLog.Add(log);
                    }
                }
                message += $"\n Size numOfMatches: {num}, Count lstLog: {lstLog.Count}, status {status}, total totalMatches: {total} --- ";
            }

            if ((pageIndex - 1) * pageSize + num < total)
            {
                pageIndex = pageIndex + 1;
                bool reloadPageSize = false;
                Boolean.TryParse(Utility.GetAppSetting("ReloadPageSize"), out reloadPageSize);
                if (reloadPageSize)
                {
                    pageSize = num;
                }
            }
        }
        #endregion


        #region SDK DEMO
        public SDKHelper _SDKHelper = new SDKHelper();
        private void btnTCPConnect_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            int ret = _SDKHelper.sta_ConnectTCP(lbSysOutputInfo, txtIP.Text.Trim(), txtPort.Text.Trim(), txtCommKey1.Text.Trim());

            if (_SDKHelper.GetConnectState())
            {
                _SDKHelper.sta_getBiometricType();
            }
            if (ret == 1)
            {
                this.txtIP.ReadOnly = true;
                this.txtPort.ReadOnly = true;
                this.txtCommKey1.ReadOnly = true;

                getCapacityInfo();
                getDeviceInfo();

                btnTCPConnect.Text = "DisConnect";
                btnTCPConnect.Refresh();

            }
            else if (ret == -2)
            {
                btnTCPConnect.Text = "Connect";
                btnTCPConnect.Refresh();
                this.txtIP.ReadOnly = false;
                this.txtPort.ReadOnly = false;
                this.txtCommKey1.ReadOnly = false;
            }
            Cursor = Cursors.Default;
        }
        private void getDeviceInfo()
        {
            string sFirmver = "";
            string sMac = "";
            string sPlatform = "";
            string sSN = "";
            string sProductTime = "";
            string sDeviceName = "";
            int iFPAlg = 0;
            int iFaceAlg = 0;
            string sProducter = "";

            _SDKHelper.sta_GetDeviceInfo(lbSysOutputInfo, out sFirmver, out sMac, out sPlatform, out sSN, out sProductTime, out sDeviceName, out iFPAlg, out iFaceAlg, out sProducter);

            txtSerialNumber.Text = sSN;
            txtPlatForm.Text = sPlatform;
            txtDeviceName.Text = sDeviceName;
            txtManufacturer.Text = sProducter;
        }
        private void getCapacityInfo()
        {
            int adminCnt = 0;
            int userCount = 0;
            int fpCnt = 0;
            int recordCnt = 0;
            int pwdCnt = 0;
            int oplogCnt = 0;
            int faceCnt = 0;
            _SDKHelper.sta_GetCapacityInfo(lbSysOutputInfo, out adminCnt, out userCount, out fpCnt, out recordCnt, out pwdCnt, out oplogCnt, out faceCnt);
        }
        private void btn_readAttLog_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (checkBox_timePeriod.Checked == true)
            {
                string fromTime = stime_log.Text.Trim().ToString();
                string toTime = etime_log.Text.Trim().ToString();

                DataTable dt_periodLog = new DataTable("dt_periodLog");
                gv_Attlog.AutoGenerateColumns = true;
                gv_Attlog.Columns.Clear();
                dt_periodLog.Columns.Add("User ID", System.Type.GetType("System.String"));
                dt_periodLog.Columns.Add("Verify Date", System.Type.GetType("System.String"));
                dt_periodLog.Columns.Add("Verify Type", System.Type.GetType("System.Int32"));
                dt_periodLog.Columns.Add("Verify State", System.Type.GetType("System.Int32"));
                dt_periodLog.Columns.Add("WorkCode", System.Type.GetType("System.Int32"));
                gv_Attlog.DataSource = dt_periodLog;


                //var fromDate = DateTime.Now.AddDays(10).Date.ToString("yyyy-MM-dd HH:mm:ss");
                //var toDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");

                //var fromDate = DateTime.Now.AddDays(-10).Date.ToString();
                //var toDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1).ToString();

                _SDKHelper.sta_readLogByPeriod(lbSysOutputInfo, dt_periodLog, fromTime, toTime);
            }
            else
            {
                DataTable dt_period = new DataTable("dt_period");
                gv_Attlog.AutoGenerateColumns = true;
                gv_Attlog.Columns.Clear();
                dt_period.Columns.Add("User ID", System.Type.GetType("System.String"));
                dt_period.Columns.Add("Verify Date", System.Type.GetType("System.String"));
                dt_period.Columns.Add("Verify Type", System.Type.GetType("System.Int32"));
                dt_period.Columns.Add("Verify State", System.Type.GetType("System.Int32"));
                dt_period.Columns.Add("WorkCode", System.Type.GetType("System.Int32"));
                gv_Attlog.DataSource = dt_period;

                _SDKHelper.sta_readAttLog(lbSysOutputInfo, dt_period);
            }
            Cursor = Cursors.Default;
        }

        private void checkBox_timePeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_timePeriod.Checked == true)
            {
                stime_log.Enabled = true;
                etime_log.Enabled = true;
            }
            else
            {
                stime_log.Enabled = false;
                etime_log.Enabled = false;

            }
        }
        private void lbSysOutputInfo_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();

                if (lbSysOutputInfo.Items[e.Index].ToString().Substring(0, 1) == "*")//if begin with *, the font color is red   
                {
                    e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(), e.Font, new SolidBrush(Color.Red), e.Bounds);
                }
                else
                {
                    e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);
                }
                e.DrawFocusRectangle();
            }
        }
        #endregion


        #region startup
        public static void SetStartup(string arg = "-hidden")
        {
            try
            {
                RegistryKey keyApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                var currentAssembly = Assembly.GetExecutingAssembly();
                var dataValue = currentAssembly.Location;
                if (!string.IsNullOrEmpty(arg))
                {
                    dataValue += " " + arg;
                }
                keyApp.SetValue(currentAssembly.GetName().Name, dataValue);
            }
            catch (Exception ex)
            {
                Logger.LogError($"SetStartup Exception: {ex.Message}");
            }
        }
        #endregion
        #region DB
        private void InitDB()
        {
            var dbFile = DBManager.GetDBPath("hnanh");
        }
        #endregion


        #region Zkteco Face
        private ListenClient listenClient = null;
        private Thread listenClientThread = null;
        DataTable _dt = null;
        /// <summary>
        /// get locale IP
        /// </summary>
        /// <returns></returns>
        private void GetServerIP()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            cmbIPByZktecoFace.Text = "";

            //获取服务器地址，且只保留IPV4地址
            foreach (IPAddress ip in ipHost.AddressList)
            {
                if (!Regex.IsMatch(ip.ToString(), @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                {
                    continue;
                }

                cmbIPByZktecoFace.Items.Add(ip.ToString());
            }
            cmbIPByZktecoFace.SelectedIndex = 0;
        }


        /// <summary>
        /// start to listening
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="Port"></param>
        private void StartListenling(string serverIP, string Port)
        {
            int port = string.IsNullOrEmpty(Port) ? 8080 : Int32.Parse(Port);
            listenClient = new ListenClient();
            listenClient.ServerIP = serverIP;
            listenClient.Port = port;
            listenClientThread = new Thread(new ThreadStart(listenClient.StartListening));
            listenClient.OnError += listenClient_OnError;
            listenClient.OnNewRealTimeLog += listenClient_OnNewRealTimeLog;
            listenClient.OnNewRealTimeState += listenClient_OnNewRealTimeState;
            listenClient.OnNewUser += listenClient_OnNewUser;
            listenClient.OnNewFP += listenClient_OnNewFP;
            listenClient.OnNewFace += listenClient_OnNewFace;
            listenClient.OnNewPalm += listenClient_OnNewPalm;
            listenClient.OnNewBioPhoto += listenClient_OnNewBioPhoto;
            listenClient.OnNewErrorLog += listenClient_OnNewErrorLog;
            listenClient.OnDeviceSync += listenClient_OnDeviceSync;
            listenClient.OnNewMachine += listenClient_OnNewMachine;
            listenClient.OnSendDataEvent += listenClient_OnSendDataEvent;
            listenClient.OnReceiveDataEvent += listenClient_OnReceiveDataEvent;
            listenClientThread.IsBackground = true;
            listenClientThread.Start();
        }

        /// <summary>
        /// stop listenling
        /// </summary>
        private void StopListenling()
        {
            if (listenClient != null && ListenClient.Listening)
            {
                listenClient.StopListening();
            }
        }

        private void listenClient_OnNewRealTimeLog(RealTimeLogModel realTimeLog)
        {
            Logger.LogError($"\n ===============================listenClient_OnNewRealTimeLog: NewRealTimeLog=================================================");
            AddNewRow(realTimeLog);
        }

        public void AddNewRow(RealTimeLogModel realTimeLogModel)
        {
            Logger.LogError($"\nAddNewRow: Bat dau");
            Logger.LogError($"\nAddNewRow: Du lieu--CardNo: {realTimeLogModel.CardNo}--Time: {realTimeLogModel.Time}--RealTimeLogModel: {realTimeLogModel?.ToString()}");
            Application.DoEvents();
            DataRow dr = _dt.NewRow();
            dr["PIN"] = realTimeLogModel.Pin;
            dr["CardNo"] = realTimeLogModel.CardNo;
            dr["Time"] = realTimeLogModel.Time;
            dr["DevSN"] = realTimeLogModel.DevSN;
            dr["Event"] = realTimeLogModel.Event;
            dr["EventAddr"] = realTimeLogModel.EventAddr;
            dr["InOutStatus"] = realTimeLogModel.InOutStatus;
            dr["VerifyType"] = realTimeLogModel.VerifyType;
            _dt.Rows.InsertAt(dr, 0);
            this.dgvLogByZktecoFace.DataSource = _dt;
            this.dgvLogByZktecoFace.Update();
            Logger.LogError($"\nAddNewRow: Xong");
        }

        /// <summary>Server start flag
        /// </summary>
        private bool _isStart = false;

        private void btnOpenHostByZktecoFace_Click(object sender, EventArgs e)
        {
            Logger.LogError($"\n btnOpenHostByZktecoFace_Click: Bat dau");
            if (_isStart)
            {//Stop Server
                StopListenling();
                btnOpenHostByZktecoFace.Text = "Start";
                btnOpenHostByZktecoFace.ForeColor = Color.FromArgb(37, 190, 167);
                AddCommInfo("", 4);
            }
            else
            {//Start Server
                StartListenling(cmbIPByZktecoFace.Text, txtPortByZktecoFace.Text);
                btnOpenHostByZktecoFace.Text = "Stop";
                btnOpenHostByZktecoFace.ForeColor = Color.Red;
                StartListenling(cmbIPByZktecoFace.Text, txtPortByZktecoFace.Text);
                AddCommInfo("", 3);
            }
            _isStart = !_isStart;

        }

        /// <summary>
        /// 增加交互信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="Mode"></param>
        public void AddCommInfo(string info, int Mode)
        {
            Logger.LogError($"\n AddCommInfo: Bat dau");
            string strNow = Tools.GetDateTimeNow().ToString("yyyy-MM-dd HH:mm:ss:fff");

            if (0 == Mode)
            {
                info = string.Format("Sever Receive Data:  {0}\r\n{1}\r\n", strNow, info.TrimEnd('\x00'));
            }
            else if (1 == Mode)
            {
                info = string.Format("Sever Send Data:  {0}\r\n{1}\r\n", strNow, info);
            }
            else if (3 == Mode)
            {
                info = string.Format("Sever Start:  {0}\r\n{1}\r\n", strNow, info);
            }
            else if (4 == Mode)
            {
                info = string.Format("Sever Stop:  {0}\r\n{1}\r\n", strNow, info);
            }
            Logger.LogError($"AddCommInfo: {info}");
            this.rtxtCommInfo.AppendText(info);
        }
        //Error Infor shows 
        private void listenClient_OnError(string errMessage)
        {
            Logger.LogError($"\n listenClient_OnError----errMessage: {errMessage}");
        }

        private void listenClient_OnNewRealTimeState(RealTimeStateModel realTimeState)
        {
            Logger.LogError($"\n listenClient_OnNewRealTimeState: Bat dau");
        }
        //UserInfo copy to Database
        private void listenClient_OnNewUser(UserInfoModel user)
        {
            Logger.LogError($"\n listenClient_OnNewUser: Bat dau");
        }

        private void listenClient_OnNewFP(List<TmpFPModel> fpList)
        {
            Logger.LogError($"\n listenClient_OnNewFP: Bat dau");
        }
        //Face tmplate copy to Database
        private void listenClient_OnNewFace(List<TmpFaceModel> faceList)
        {
            Logger.LogError($"\n listenClient_OnNewFace: Bat dau");

        }

        //palm tmplate copy to Database
        private void listenClient_OnNewPalm(TmpBioDataModel palm)
        {
            Logger.LogError($"\n listenClient_OnNewPalm: Bat dau");

        }
        private void listenClient_OnNewBioPhoto(List<TmpBioPhotoModel> bioPhotoList)
        {
            Logger.LogError($"\n listenClient_OnNewBioPhoto: Bat dau");


        }
        //Error copy to Database 
        private void listenClient_OnNewErrorLog(ErrorLogModel erlog)
        {
            Logger.LogError($"\n listenClient_OnNewErrorLog: Bat dau");
        }
        //sync time
        private void listenClient_OnDeviceSync(DeviceModel device)
        {
            //Update device
            Logger.LogError($"\n listenClient_OnDeviceSync: Bat dau");
        }
        //autoMatic to add Device and copy Device to Database 
        private void listenClient_OnNewMachine(DeviceModel device)
        {
            Logger.LogError($"\n listenClient_OnNewMachine: Bat dau");
        }

        //add Send data
        private void listenClient_OnSendDataEvent(string Data)
        {
            Logger.LogError($"\n listenClient_OnSendDataEvent----Data: {Data}");
            AddCommInfo(Data, 1);
        }

        //add receive data
        private void listenClient_OnReceiveDataEvent(string Data)
        {
            Logger.LogError($"\n listenClient_OnReceiveDataEvent----Data: {Data}");
            var logData = Regex.Replace(Regex.Replace(Data, "\r", @"\r"), "\n", @"\n");
            Logger.LogError($"\n listenClient_OnReceiveDataEvent----logData: {logData}");
            AddCommInfo(Data, 0);

            // Lưu lại thông tin log
            try
            {
                //string strReceive = $"POST /iclock/cdata?SN=0782232260009&table=ATTLOG&Stamp=9999 HTTP/1.1\nHost: 192.168.1.240:8080\nUser - Agent: iClock Proxy/ 1.09\nConnection: close\nAccept: */*\nContent-Type: text/plain \nContent-Length: 44\r\n\r\n3	2023-10-16 08:38:16	255	15	0	0	0	255	0	0	\n";
                if (Data.Substring(0, 4).ToUpper() == "POST" && Data.IndexOfEx("SN=") > 0 && Data.IndexOfEx("cdata?") > 0 && Data.IndexOfEx("table=ATTLOG", 1) > 0)
                {
                    var lstLog = new List<LogData>();
                    onRealTimeLog(Data, ref lstLog);
                    this.dgvLogByZktecoFace.DataSource = lstLog;
                    this.dgvLogByZktecoFace.Update();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n listenClient_OnReceiveDataEvent Exception: {ex.Message}");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //this.lblMsg.Visible = false;
            this.txtDevSN.Enabled = true;
            DeviceModel device = new DeviceModel();
            txtDevSN.Text = device.DeviceSN;
            txtDevName.Text = device.DeviceName;
            tb_RegistryCode.Text = GetRegistryCode();
            tb_TransTables.Text = device.TransTables;
        }

        //Device number and device polling time dictionary
        private Dictionary<String, int> _dicDevInterval = new Dictionary<String, int>();
        DeviceBll _bll = new DeviceBll();

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDevSN.Text))
            {
                //lblMsg.Visible = true;
                //lblMsg.Text = "Please input Device SN";
                return;
            }

            //lblMsg.Visible = false;
            txtDevSN.Enabled = false;
            //Device Exsit,Update
            DeviceModel device;
            if (null != (device = _bll.Get(txtDevSN.Text.Trim())))
            {
                device.DeviceName = txtDevName.Text;
                device.TransTables = tb_TransTables.Text.Trim();
                device.RegistryCode = tb_RegistryCode.Text.Trim();
                try
                {
                    if (_bll.Update(device) > 0)
                    {
                        //lblMsg.Visible = true;
                        //lblMsg.Text = "Update device success";
                    }
                    else
                    {
                        //lblMsg.Visible = true;
                        //lblMsg.Text = "Update device fail";
                    }
                    return;
                }
                catch { }
            }

            //Device No Exsit,Add
            device = new DeviceModel();
            device.DeviceSN = txtDevSN.Text.Trim();
            device.DeviceName = txtDevName.Text;
            device.TransTables = tb_TransTables.Text.Trim();
            device.RegistryCode = tb_RegistryCode.Text.Trim();
            device.SessionID = Tools.GetSessionID();
            try
            {
                if (_bll.Add(device) > 0)
                {
                    if (!_dicDevInterval.ContainsKey(device.DeviceSN))
                    {
                        _dicDevInterval.Add(device.DeviceSN, 0);
                    }

                    //lblMsg.Visible = true;
                    //lblMsg.Text = "Add Device SN " + txtDevSN.Text.Trim() + " Success";
                }
                else
                {
                    //lblMsg.Visible = true;
                    //lblMsg.Text = "Add Device SN " + txtDevSN.Text.Trim() + " Fail";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n btnSave_Click Exception: {ex.Message}");
            }
        }
        private string GetRegistryCode()
        {
            string registryCode = null;

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

            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                int index = random.Next(strList.Count);
                registryCode = registryCode + strList[index];
            }

            return registryCode;
        }

        /// <summary>
        /// Parse RealTimeLog for acc Device
        /// </summary>
        /// <param name="sBuffer"></param>
        private void onRealTimeLog(string sBuffer, ref List<LogData> lstLog)
        {
            Logger.LogError($"\n onRealTimeLog: Bat dau---sBuffer: {sBuffer}");
            string machineSN = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3);
            string SN = machineSN.Split('&')[0];

            int attindex = sBuffer.IndexOfEx("\r\n\r\n", 1);
            string attstr = sBuffer.Substring(attindex + 4);

            onRealTimeLogProcess(attstr, SN, ref lstLog);
        }

        private void onRealTimeLogProcess(string attstr, string machineSN, ref List<LogData> lstLog)
        {
            Logger.LogError($"\n onRealTimeLogProcess: Bat dau---attstr: {attstr}");
            try
            {
                string[] strlist = attstr.Split('\n', '\r');
                Logger.LogError($"\n onRealTimeLogProcess: Split xong---strlist: {Converter.JsonSerialize(strlist)}");
                foreach (string i in strlist)
                {
                    if (string.IsNullOrEmpty(i))
                        continue;
                    var log = onCreateRealTimelog(i.ToString(), machineSN);
                    lstLog.Add(log);

                    var realTimeLog = new RealTimeLogModel();
                    realTimeLog.DevSN = txtNameByZkFace.Text;
                    realTimeLog.CardNo = log.UserID;
                    realTimeLog.Time = log.CheckTime;
                    AddNewRow(realTimeLog);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n onRealTimeLogProcess Exception: {ex.Message}");
            }
        }
        private LogData onCreateRealTimelog(string realtimelog, string machineSN)
        {
            Logger.LogError($"\n onCreateRealTimelog: Bat dau---realtimelog: {realtimelog}");
            realtimelog = realtimelog.TrimEnd('\t');
            string[] realtimelogstr = realtimelog.Split('\t');
            Logger.LogError($"\n onCreateRealTimelog: Split xong---realtimelogstr: {Converter.JsonSerialize(realtimelogstr)}");
            LogData realTimeLog = new LogData();

            int cardNo = 0;
            var time = DateTime.Now;
            if (int.TryParse(realtimelogstr[0], out cardNo) && DateTime.TryParse(realtimelogstr[1], out time))
            {
                realTimeLog.UserID = realtimelogstr[0];
                realTimeLog.CheckTime = Convert.ToDateTime(realtimelogstr[1]);
                realTimeLog.DeviceID = machineSN;
            }
            return realTimeLog;
        }

        private void btnConnectByZkFace_Click(object sender, EventArgs e)
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

                string ipAddress = txtIPByZkFace.Text.Trim();
                string port = txtPortByZkFace.Text.Trim();
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

                string pw = txtPassByZkFace.Text.Trim();
                int password = 0;
                if (!int.TryParse(pw, out password))
                    throw new Exception("Not a valid password number");

                objZkeeper = new ZkemClient(RaiseDeviceEvent);
                objZkeeper.Beep(5000);
                objZkeeper.SetCommPassword(password);
                IsDeviceConnected = objZkeeper.Connect_Net(ipAddress, portNumber);

                if (IsDeviceConnected)
                {
                    var serialNo = "";
                    if (objZkeeper.GetSerialNumber(int.Parse(tbxMachineNumber.Text.Trim()), out serialNo))
                    {
                        if (string.IsNullOrWhiteSpace(serialNo))
                        {
                            return;
                        }
                        txtDevSN.Text = serialNo;
                        DeviceModel device;
                        if (null != (device = _bll.Get(serialNo.Trim())))
                        {
                            device.DeviceSN = serialNo.Trim();
                            device.DeviceName = txtNameByZkFace.Text;
                            device.IPAddress = txtIPByZkFace.Text;
                            device.TransTables = device.TransTables;
                            device.RegistryCode = GetRegistryCode();
                            try
                            {
                                if (_bll.Update(device) > 0)
                                {
                                    //lblMsg.Visible = true;
                                    //lblMsg.Text = "Update device success";
                                }
                                else
                                {
                                    //lblMsg.Visible = true;
                                    //lblMsg.Text = "Update device fail";
                                }
                                return;
                            }
                            catch { }
                        }

                        //Device No Exsit,Add
                        device = new DeviceModel();
                        device.DeviceSN = serialNo.Trim();
                        device.DeviceName = txtNameByZkFace.Text;
                        device.IPAddress = txtIPByZkFace.Text;
                        device.TransTables = device.TransTables;
                        device.RegistryCode = GetRegistryCode();
                        device.SessionID = Tools.GetSessionID();
                        try
                        {
                            if (_bll.Add(device) > 0)
                            {
                                if (!_dicDevInterval.ContainsKey(device.DeviceSN))
                                {
                                    _dicDevInterval.Add(device.DeviceSN, 0);
                                }

                                //lblMsg.Visible = true;
                                //lblMsg.Text = "Add Device SN " + txtDevSN.Text.Trim() + " Success";
                            }
                            else
                            {
                                //lblMsg.Visible = true;
                                //lblMsg.Text = "Add Device SN " + txtDevSN.Text.Trim() + " Fail";
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"\n btnSave_Click Exception: {ex.Message}");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ShowStatusBar(ex.Message, false);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion
        #region Zkteco SDK New
        private void btnPingByZkSDKNew_Click(object sender, EventArgs e)
        {
            tbTotalByZkSDKNew.Text = string.Empty;
            bool isValidIpA = UniversalStatic.ValidateIP(tbIPByZkSDKNew.Text);
            if (!isValidIpA)
            {
                richtbByZkSDKNew.AppendText("\n The Device IP is invalid !!");
                throw new Exception("The Device IP is invalid !!");
            }

            isValidIpA = UniversalStatic.PingTheDevice(tbIPByZkSDKNew.Text);
            if (isValidIpA)
            {
                richtbByZkSDKNew.AppendText("\n Ping success !!");
            }
            else
            {
                richtbByZkSDKNew.AppendText("\n Ping false !!");
            }
        }
        private void btnConnectByZkSDKNew_Click(object sender, EventArgs e)
        {
            try
            {
                tbTotalByZkSDKNew.Text = string.Empty;

                int portNumber = 4370;
                if (!int.TryParse(tbPortByZkSDKNew.Text.Trim(), out portNumber)) richtbByZkSDKNew.AppendText("\n Not a valid port number");

                int password = 0;
                if (!int.TryParse(tbPassByZkSDKNew.Text.Trim(), out password)) richtbByZkSDKNew.AppendText("\n Not a valid password number");

                objZkeeper = new ZkemClient(RaiseDeviceEvent);
                objZkeeper.Beep(5000);
                objZkeeper.SetCommPassword(password);
                IsDeviceConnected = objZkeeper.Connect_Net(tbIPByZkSDKNew.Text, portNumber);
                richtbByZkSDKNew.AppendText($"\n Connect_Net: {IsDeviceConnected}");
                Logger.LogInfo($"\n btnConnectByZkSDKNew_Click Connect_Net: {IsDeviceConnected}");

                if (IsDeviceConnected)
                {
                    string deviceInfo = manipulator.FetchDeviceInfo(objZkeeper, int.Parse(tbMachineNumberByZkSDKNew.Text.Trim()));
                    richtbByZkSDKNew.AppendText($"\n {deviceInfo}");
                }
                else
                {
                    int code = 0;
                    objZkeeper.GetLastError(ref code);
                }

            }
            catch (Exception ex)
            {
                Logger.LogError($"\n btnConnectByZkSDKNew_Click Exception: {ex.Message}");
                richtbByZkSDKNew.AppendText($"\n {ex.Message}");
            }

        }

        private void btnGetDataByZkSDKNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowStatusBar(string.Empty, true);
                string message = string.Empty;
                tbTotalByZkSDKNew.Text = string.Empty;

                int readType = 1, readLog = 1;
                int.TryParse(tbReadTypeByZkSDKNew.Text, out readType);
                int.TryParse(tbReadLogByZkSDKNew.Text, out readLog);

                ICollection<MachineInfo> lstMachineInfo = manipulator.GetLogData(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()), ref message, dtFromDateByZkSDKNew.Value, dtToDateByZkSDKNew.Value, readType, readLog);
                Logger.LogInfo($"\n btnGetDataByZkSDKNew_Click: {message} ======== Count: {lstMachineInfo.Count}");

                if (lstMachineInfo != null && lstMachineInfo.Count > 0)
                {
                    tbTotalByZkSDKNew.Text = lstMachineInfo.Count.ToString();
                    BindToGridView(lstMachineInfo);
                    richtbByZkSDKNew.AppendText($"\n {lstMachineInfo.Count} records found !!");
                }
                else richtbByZkSDKNew.AppendText("\n No records found");
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n btnGetDataByZkSDKNew_Click Exception: {ex.Message}");
                richtbByZkSDKNew.AppendText($"\n {ex.Message}");
            }
        }
        private void btnConnect_V2ByZkSDKNew_Click(object sender, EventArgs e)
        {
            int ret = _SDKHelper.sta_ConnectTCP(lbSysOutputInfo, tbIPByZkSDKNew.Text.Trim(), tbPortByZkSDKNew.Text.Trim(), tbPassByZkSDKNew.Text.Trim());
            if (_SDKHelper.GetConnectState())
            {
                _SDKHelper.sta_getBiometricType();
            }
            if (ret == 1)
            {
                richtbByZkSDKNew.AppendText("\n Connect_V2 Success");
                getCapacityInfo();
                getDeviceInfo();

            }
            else if (ret == -2)
            {
                richtbByZkSDKNew.AppendText("\n Connect_V2 Fail");
            }
            if (_SDKHelper.GetConnectState())
            {
                _SDKHelper.sta_getBiometricType();
            }
        }
        private void btnGetData_V2ByZkSDKNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowStatusBar(string.Empty, true);
                string message = string.Empty;
                tbTotalByZkSDKNew.Text = string.Empty;

                int readLog = 1;
                int.TryParse(tbReadLogByZkSDKNew.Text, out readLog);


                ICollection<MachineInfo> lstMachineInfo = _SDKHelper.GetLogData(int.Parse(tbMachineNumberByZkSDKNew.Text.Trim()), ref message, dtFromDateByZkSDKNew.Value, dtToDateByZkSDKNew.Value, readLog);
                Logger.LogInfo($"\n btnGetData_V2ByZkSDKNew_Click: {message} ======== Count: {lstMachineInfo.Count}");

                if (lstMachineInfo != null && lstMachineInfo.Count > 0)
                {
                    tbTotalByZkSDKNew.Text = lstMachineInfo.Count.ToString();
                    BindToGridView(lstMachineInfo);
                    richtbByZkSDKNew.AppendText($"\n {lstMachineInfo.Count} records found !!");
                }
                else richtbByZkSDKNew.AppendText("\n No records found");
            }
            catch (Exception ex)
            {
                Logger.LogError($"\n btnGetData_V2ByZkSDKNew_Click Exception: {ex.Message}");
                richtbByZkSDKNew.AppendText($"\n {ex.Message}");
            }
        }
        #endregion
        #region Hikvision SDK
        public int m_UserID = -1;
        private string CsTemp = null;
        private int m_lLogNum = 0;
        private string MinorType = null;
        private string MajorType = null;
        public int m_lGetAcsEventHandle = -1;
        Thread m_pDisplayListThread = null;
        public List<LogData> listLogByHikvisionSDK = new List<LogData> ();
        private void btnConnectByHikvisionSDK_Click(object sender, EventArgs e)
        {
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            struLoginInfo.sDeviceAddress = tbIPByHikvisionSDK.Text;
            struLoginInfo.sUserName = tbUserNameByHikvisionSDK.Text;
            struLoginInfo.sPassword = tbPassByHikvisionSDK.Text;
            ushort.TryParse(tbPortByHikvisionSDK.Text, out struLoginInfo.wPort);

            int lUserID = -1;
            lUserID = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
            if (lUserID >= 0)
            {
                tbTotalByHikvisionSDK.Text = "Login Successful";
                Logger.LogError($"\n =============Login Successful================");
            }
            else
            {
                uint nErr = CHCNetSDK.NET_DVR_GetLastError();
                if (nErr == CHCNetSDK.NET_DVR_PASSWORD_ERROR)
                {
                    Logger.LogError($"\n =============user name or password error!================");
                    if (1 == struDeviceInfoV40.bySupportLock)
                    {
                        string strTemp1 = string.Format("Left {0} try opportunity", struDeviceInfoV40.byRetryLoginTime);
                        Logger.LogError($"\n =============btnConnectByHikvisionSDK_Click: {strTemp1}================");
                    }
                }
                else if (nErr == CHCNetSDK.NET_DVR_USER_LOCKED)
                {
                    if (1 == struDeviceInfoV40.bySupportLock)
                    {
                        string strTemp1 = string.Format("user is locked, the remaining lock time is {0}", struDeviceInfoV40.dwSurplusLockTime);
                        Logger.LogError($"{strTemp1}");
                    }
                }
                else
                {
                    Logger.LogError($"net error or dvr is busy!");
                }
            }
        }

        private void btnGetLogByHikvisionSDK_Click(object sender, EventArgs e)
        {
            CHCNetSDK.NET_DVR_ACS_EVENT_COND struCond = new CHCNetSDK.NET_DVR_ACS_EVENT_COND();
            struCond.Init();
            struCond.dwSize = (uint)Marshal.SizeOf(struCond);

            var majorTypeByHikvisonSDK = Utility.GetAppSetting("MajorTypeByHikvisonSDK");
            var minorTypeByHikvisonSDK = Utility.GetAppSetting("MinorTypeByHikvisonSDK");

            MajorType = majorTypeByHikvisonSDK;
            struCond.dwMajor = GetAcsEventType.ReturnMajorTypeValue(ref MajorType);

            MinorType = minorTypeByHikvisonSDK;
            struCond.dwMinor = GetAcsEventType.ReturnMinorTypeValue(ref MinorType);


            struCond.struStartTime.dwYear = dtFromDateByHikvisionSDK.Value.Year;
            struCond.struStartTime.dwMonth = dtFromDateByHikvisionSDK.Value.Month;
            struCond.struStartTime.dwDay = dtFromDateByHikvisionSDK.Value.Day;
            struCond.struStartTime.dwHour = dtFromDateByHikvisionSDK.Value.Hour;
            struCond.struStartTime.dwMinute = dtFromDateByHikvisionSDK.Value.Minute;
            struCond.struStartTime.dwSecond = dtFromDateByHikvisionSDK.Value.Second;

            struCond.struEndTime.dwYear = dtToDateByHikvisionSDK.Value.Year;
            struCond.struEndTime.dwMonth = dtToDateByHikvisionSDK.Value.Month;
            struCond.struEndTime.dwDay = dtToDateByHikvisionSDK.Value.Day;
            struCond.struEndTime.dwHour = dtToDateByHikvisionSDK.Value.Hour;
            struCond.struEndTime.dwMinute = dtToDateByHikvisionSDK.Value.Minute;
            struCond.struEndTime.dwSecond = dtToDateByHikvisionSDK.Value.Second;

            struCond.byPicEnable = 0;
            struCond.szMonitorID = "";
            struCond.wInductiveEventType = 65535;

            //if (!StrToByteArray(ref struCond.byCardNo, textBoxCardNo.Text))
            //{
            //    return;
            //}

            //if (!StrToByteArray(ref struCond.byName, textBoxName.Text))
            //{
            //    return;
            //}
            //struCond.dwBeginSerialNo = 0;
            //struCond.dwEndSerialNo = 0;

            uint dwSize = struCond.dwSize;
            IntPtr ptrCond = Marshal.AllocHGlobal((int)dwSize);
            Marshal.StructureToPtr(struCond, ptrCond, false);
            m_lGetAcsEventHandle = CHCNetSDK.NET_DVR_StartRemoteConfig(m_UserID, CHCNetSDK.NET_DVR_GET_ACS_EVENT, ptrCond, (int)dwSize, null, IntPtr.Zero);
            if (-1 == m_lGetAcsEventHandle)
            {
                Marshal.FreeHGlobal(ptrCond);
                Logger.LogError($"\n =============btnGetLogByHikvisionSDK_Click: NET_DVR_StartRemoteConfig FAIL, ERROR CODE {CHCNetSDK.NET_DVR_GetLastError().ToString()}================");
                return;
            }

            m_pDisplayListThread = new Thread(ProcessEvent);
            m_pDisplayListThread.Start();
            Marshal.FreeHGlobal(ptrCond);
        }
        public void ProcessEvent()
        {
            int dwStatus = 0;
            Boolean Flag = true;
            CHCNetSDK.NET_DVR_ACS_EVENT_CFG struCFG = new CHCNetSDK.NET_DVR_ACS_EVENT_CFG();
            struCFG.dwSize = (uint)Marshal.SizeOf(struCFG);
            int dwOutBuffSize = (int)struCFG.dwSize;
            struCFG.init();
            while (Flag)
            {
                dwStatus = CHCNetSDK.NET_DVR_GetNextRemoteConfig(m_lGetAcsEventHandle, ref struCFG, dwOutBuffSize);
                switch (dwStatus)
                {
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_SUCCESS://成功读取到数据，处理完本次数据后需调用next
                        ProcessAcsEvent(ref struCFG, ref Flag);
                        break;
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_NEED_WAIT:
                        Thread.Sleep(200);
                        break;
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_FAILED:
                        CHCNetSDK.NET_DVR_StopRemoteConfig(m_lGetAcsEventHandle);
                        Logger.LogError($"\n =============ProcessEvent: NET_SDK_GET_NEXT_STATUS_FAILED {CHCNetSDK.NET_DVR_GetLastError().ToString()}================");
                        Flag = false;
                        break;
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_FINISH:
                        CHCNetSDK.NET_DVR_StopRemoteConfig(m_lGetAcsEventHandle);
                        Flag = false;
                        break;
                    default:
                        Logger.LogError($"\n =============ProcessEvent: NET_SDK_GET_NEXT_STATUS_UNKOWN {CHCNetSDK.NET_DVR_GetLastError().ToString()}================");
                        Flag = false;
                        CHCNetSDK.NET_DVR_StopRemoteConfig(m_lGetAcsEventHandle);
                        break;
                }
            }
        }
        public delegate void ShowCardListThread(CHCNetSDK.NET_DVR_ACS_EVENT_CFG struCFG);

        public void ShowCardList(CHCNetSDK.NET_DVR_ACS_EVENT_CFG struCFG)
        {
            if (this.InvokeRequired)
            {
                Delegate delegateProc = new ShowCardListThread(AddAcsEventToList);
                this.BeginInvoke(delegateProc, struCFG);
            }
            else
            {
                AddAcsEventToList(struCFG);
            }

        }

        private void ProcessAcsEvent(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struCFG, ref bool flag)
        {
            try
            {
                ShowCardList(struCFG);
            }
            catch
            {
                Logger.LogError($"\n =============ProcessAcsEvent: AddAcsEventToList Failed================");
                flag = false;
            }
        }
        private void AddAcsEventToList(CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            //this.listViewEvent.BeginUpdate();
            //ListViewItem Item = new ListViewItem();
            //Item.Text = (++m_lLogNum).ToString();

            //string LogTime = GetStrLogTime(ref struEventCfg.struTime);
            //Item.SubItems.Add(LogTime);

            //string Major = ProcessMajorType(ref struEventCfg.dwMajor);
            //Item.SubItems.Add(Major);

            //ProcessMinorType(ref struEventCfg);
            //Item.SubItems.Add(CsTemp);

            //CsTemp = System.Text.Encoding.UTF8.GetString(struEventCfg.struAcsEventInfo.byCardNo);
            //Item.SubItems.Add(CsTemp);

            //CardTypeMap(ref struEventCfg);
            //Item.SubItems.Add(CsTemp);

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.byWhiteListNo.ToString());//WhiteList

            //ProcessReportChannel(ref struEventCfg);
            //Item.SubItems.Add(CsTemp);

            //ProcessCardReader(ref struEventCfg);
            //Item.SubItems.Add(CsTemp);

            //CsTemp = struEventCfg.struAcsEventInfo.dwCardReaderNo.ToString();
            //Item.SubItems.Add(CsTemp);

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.dwDoorNo.ToString());

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.dwVerifyNo.ToString());

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.dwAlarmInNo.ToString());

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.dwAlarmOutNo.ToString());

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.dwCaseSensorNo.ToString());

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.dwRs485No.ToString());

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.dwMultiCardGroupNo.ToString());

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.wAccessChannel.ToString());

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.byDeviceNo.ToString());

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.dwEmployeeNo.ToString());

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.byDistractControlNo.ToString());

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.wLocalControllerID.ToString());

            //ProcessInternatAccess(ref struEventCfg);
            //Item.SubItems.Add(CsTemp);

            //ProcessByType(ref struEventCfg);
            //Item.SubItems.Add(CsTemp);

            //ProcessMacAdd(ref struEventCfg);
            //Item.SubItems.Add(CsTemp);

            //ProcessSwipeCard(ref struEventCfg);
            //Item.SubItems.Add(CsTemp);

            //Item.SubItems.Add(struEventCfg.struAcsEventInfo.dwSerialNo.ToString());

            //Item.SubItems.Add("0"/*struEventCfg.struAcsEventInfo.byChannelControllerID.ToString()*/);

            //Item.SubItems.Add("0"/*struEventCfg.struAcsEventInfo.byChannelControllerLampID.ToString()*/);

            //Item.SubItems.Add("0"/*struEventCfg.struAcsEventInfo.byChannelControllerIRAdaptorID.ToString()*/);

            //Item.SubItems.Add("0"/*struEventCfg.struAcsEventInfo.byChannelControllerIREmitterID.ToString()*/);

            ////if (struEventCfg.wInductiveEventType < (ushort)GetAcsEventType.NumOfInductiveEvent())
            ////{
            //Item.SubItems.Add("0"/*GetAcsEventType.FindKeyOfInductive(struEventCfg.wInductiveEventType)*/);
            ////}
            ////else
            ////{
            ////    Item.SubItems.Add("Invalid");
            ////}

            //Item.SubItems.Add("0");//RecordChannelNum

            ////ProcessbyUserType(ref struEventCfg);
            //Item.SubItems.Add("0");

            ////ProcessVerifyMode(ref struEventCfg);
            //Item.SubItems.Add("0");

            ////CsTemp = System.Text.Encoding.UTF8.GetString(struEventCfg.struAcsEventInfo.byEmployeeNo);
            //Item.SubItems.Add("0");

            //CsTemp = null;
            //this.listViewEvent.Items.Add(Item);


            string LogTime = GetStrLogTime(ref struEventCfg.struTime);
            CsTemp = System.Text.Encoding.UTF8.GetString(struEventCfg.struAcsEventInfo.byCardNo);
            Logger.LogError($"\n =============AddAcsEventToList: UserID --- {CsTemp} ===== LogTime --- {LogTime}================");
            var time = DateTime.Now;
            if (DateTime.TryParse(LogTime, out time))
            {
                listLogByHikvisionSDK.Add(new LogData()
                {
                    UserID = CsTemp,
                    CheckTime = time
                });
                tbTotalByHikvision.Text = listLogByHikvisionSDK.Count.ToString();
                BindToGridView(listLogByHikvisionSDK);

                this.dgvListLogByHikvisionSDK.DataSource = listLogByHikvisionSDK;
                this.dgvListLogByHikvisionSDK.Update();
            }

            //this.listViewEvent.EndUpdate();
        }
        private string ProcessMajorType(ref uint dwMajor)
        {
            string res = null;
            switch (dwMajor)
            {
                case 1:
                    res = "Alarm";
                    break;
                case 2:
                    res = "Exception";
                    break;
                case 3:
                    res = "Operation";
                    break;
                case 5:
                    res = "Event";
                    break;
                default:
                    res = "Unknown";
                    break;
            }
            return res;
        }

        private void AlarmMinorTypeMap(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            switch (struEventCfg.dwMinor)
            {
                case CHCNetSDK.MINOR_ALARMIN_SHORT_CIRCUIT:
                    CsTemp = "ALARMIN_SHORT_CIRCUIT";
                    break;
                case CHCNetSDK.MINOR_ALARMIN_BROKEN_CIRCUIT:
                    CsTemp = "ALARMIN_BROKEN_CIRCUIT";
                    break;
                case CHCNetSDK.MINOR_ALARMIN_EXCEPTION:
                    CsTemp = "ALARMIN_EXCEPTION";
                    break;
                case CHCNetSDK.MINOR_ALARMIN_RESUME:
                    CsTemp = "ALARMIN_RESUME";
                    break;
                case CHCNetSDK.MINOR_HOST_DESMANTLE_ALARM:
                    CsTemp = "HOST_DESMANTLE_ALARM";
                    break;
                case CHCNetSDK.MINOR_HOST_DESMANTLE_RESUME:
                    CsTemp = "HOST_DESMANTLE_RESUME";
                    break;
                case CHCNetSDK.MINOR_CARD_READER_DESMANTLE_ALARM:
                    CsTemp = "CARD_READER_DESMANTLE_ALARM";
                    break;
                case CHCNetSDK.MINOR_CARD_READER_DESMANTLE_RESUME:
                    CsTemp = "CARD_READER_DESMANTLE_RESUME";
                    break;
                case CHCNetSDK.MINOR_CASE_SENSOR_ALARM:
                    CsTemp = "CASE_SENSOR_ALARM";
                    break;
                case CHCNetSDK.MINOR_CASE_SENSOR_RESUME:
                    CsTemp = "CASE_SENSOR_RESUME";
                    break;
                case CHCNetSDK.MINOR_STRESS_ALARM:
                    CsTemp = "STRESS_ALARM";
                    break;
                case CHCNetSDK.MINOR_OFFLINE_ECENT_NEARLY_FULL:
                    CsTemp = "OFFLINE_ECENT_NEARLY_FULL";
                    break;
                case CHCNetSDK.MINOR_CARD_MAX_AUTHENTICATE_FAIL:
                    CsTemp = "CARD_MAX_AUTHENTICATE_FAIL";
                    break;
                case CHCNetSDK.MINOR_SD_CARD_FULL:
                    CsTemp = "MINOR_SD_CARD_FULL";
                    break;
                case CHCNetSDK.MINOR_LINKAGE_CAPTURE_PIC:
                    CsTemp = "MINOR_LINKAGE_CAPTURE_PIC";
                    break;
                case CHCNetSDK.MINOR_SECURITY_MODULE_DESMANTLE_ALARM:
                    CsTemp = "MINOR_SECURITY_MODULE_DESMANTLE_ALARM";
                    break;
                case CHCNetSDK.MINOR_SECURITY_MODULE_DESMANTLE_RESUME:
                    CsTemp = "MINOR_SECURITY_MODULE_DESMANTLE_RESUME";
                    break;
                case CHCNetSDK.MINOR_POS_START_ALARM:
                    CsTemp = "MINOR_POS_START_ALARM";
                    break;
                case CHCNetSDK.MINOR_POS_END_ALARM:
                    CsTemp = "MINOR_POS_END_ALARM";
                    break;
                case CHCNetSDK.MINOR_FACE_IMAGE_QUALITY_LOW:
                    CsTemp = "MINOR_FACE_IMAGE_QUALITY_LOW";
                    break;
                case CHCNetSDK.MINOR_FINGE_RPRINT_QUALITY_LOW:
                    CsTemp = "MINOR_FINGE_RPRINT_QUALITY_LOW";
                    break;
                case CHCNetSDK.MINOR_FIRE_IMPORT_SHORT_CIRCUIT:
                    CsTemp = "MINOR_FIRE_IMPORT_SHORT_CIRCUIT";
                    break;
                case CHCNetSDK.MINOR_FIRE_IMPORT_BROKEN_CIRCUIT:
                    CsTemp = "MINOR_FIRE_IMPORT_BROKEN_CIRCUIT";
                    break;
                case CHCNetSDK.MINOR_FIRE_IMPORT_RESUME:
                    CsTemp = "MINOR_FIRE_IMPORT_RESUME";
                    break;
                case CHCNetSDK.MINOR_FIRE_BUTTON_TRIGGER:
                    CsTemp = "FIRE_BUTTON_TRIGGER";
                    break;
                case CHCNetSDK.MINOR_FIRE_BUTTON_RESUME:
                    CsTemp = "FIRE_BUTTON_RESUME";
                    break;
                case CHCNetSDK.MINOR_MAINTENANCE_BUTTON_TRIGGER:
                    CsTemp = "MAINTENANCE_BUTTON_TRIGGER";
                    break;
                case CHCNetSDK.MINOR_MAINTENANCE_BUTTON_RESUME:
                    CsTemp = "MAINTENANCE_BUTTON_RESUME";
                    break;
                case CHCNetSDK.MINOR_EMERGENCY_BUTTON_TRIGGER:
                    CsTemp = "EMERGENCY_BUTTON_TRIGGER";
                    break;
                case CHCNetSDK.MINOR_EMERGENCY_BUTTON_RESUME:
                    CsTemp = "EMERGENCY_BUTTON_RESUME";
                    break;
                case CHCNetSDK.MINOR_DISTRACT_CONTROLLER_ALARM:
                    CsTemp = "DISTRACT_CONTROLLER_ALARM";
                    break;
                case CHCNetSDK.MINOR_DISTRACT_CONTROLLER_RESUME:
                    CsTemp = "DISTRACT_CONTROLLER_RESUME";
                    break;
                case CHCNetSDK.MINOR_CHANNEL_CONTROLLER_DESMANTLE_ALARM:
                    CsTemp = "MINOR_CHANNEL_CONTROLLER_DESMANTLE_ALARM";
                    break;
                case CHCNetSDK.MINOR_CHANNEL_CONTROLLER_DESMANTLE_RESUME:
                    CsTemp = "MINOR_CHANNEL_CONTROLLER_DESMANTLE_RESUME";
                    break;
                case CHCNetSDK.MINOR_CHANNEL_CONTROLLER_FIRE_IMPORT_ALARM:
                    CsTemp = "MINOR_CHANNEL_CONTROLLER_FIRE_IMPORT_ALARM";
                    break;
                case CHCNetSDK.MINOR_CHANNEL_CONTROLLER_FIRE_IMPORT_RESUME:
                    CsTemp = "MINOR_CHANNEL_CONTROLLER_FIRE_IMPORT_RESUME";
                    break;
                case CHCNetSDK.MINOR_LEGAL_EVENT_NEARLY_FULL:
                    CsTemp = "MINOR_LEGAL_EVENT_NEARLY_FULL";
                    break;
                default:
                    CsTemp = Convert.ToString(struEventCfg.dwMinor, 16);
                    break;
            }
        }

        private void ExceptionMinorTypeMap(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            switch (struEventCfg.dwMinor)
            {
                case CHCNetSDK.MINOR_NET_BROKEN:
                    CsTemp = "NET_BROKEN";
                    break;
                case CHCNetSDK.MINOR_RS485_DEVICE_ABNORMAL:
                    CsTemp = "RS485_DEVICE_ABNORMAL";
                    break;
                case CHCNetSDK.MINOR_RS485_DEVICE_REVERT:
                    CsTemp = "RS485_DEVICE_REVERT";
                    break;
                case CHCNetSDK.MINOR_DEV_POWER_ON:
                    CsTemp = "DEV_POWER_ON";
                    break;
                case CHCNetSDK.MINOR_DEV_POWER_OFF:
                    CsTemp = "DEV_POWER_OFF";
                    break;
                case CHCNetSDK.MINOR_WATCH_DOG_RESET:
                    CsTemp = "WATCH_DOG_RESET";
                    break;
                case CHCNetSDK.MINOR_LOW_BATTERY:
                    CsTemp = "LOW_BATTERY";
                    break;
                case CHCNetSDK.MINOR_BATTERY_RESUME:
                    CsTemp = "BATTERY_RESUME";
                    break;
                case CHCNetSDK.MINOR_AC_OFF:
                    CsTemp = "AC_OFF";
                    break;
                case CHCNetSDK.MINOR_AC_RESUME:
                    CsTemp = "AC_RESUME";
                    break;
                case CHCNetSDK.MINOR_NET_RESUME:
                    CsTemp = "NET_RESUME";
                    break;
                case CHCNetSDK.MINOR_FLASH_ABNORMAL:
                    CsTemp = "FLASH_ABNORMAL";
                    break;
                case CHCNetSDK.MINOR_CARD_READER_OFFLINE:
                    CsTemp = "CARD_READER_OFFLINE";
                    break;
                case CHCNetSDK.MINOR_CARD_READER_RESUME:
                    CsTemp = "CARD_READER_RESUME";
                    break;
                case CHCNetSDK.MINOR_INDICATOR_LIGHT_OFF:
                    CsTemp = "INDICATOR_LIGHT_OFF";
                    break;
                case CHCNetSDK.MINOR_INDICATOR_LIGHT_RESUME:
                    CsTemp = "INDICATOR_LIGHT_RESUME";
                    break;
                case CHCNetSDK.MINOR_CHANNEL_CONTROLLER_OFF:
                    CsTemp = "CHANNEL_CONTROLLER_OFF";
                    break;
                case CHCNetSDK.MINOR_CHANNEL_CONTROLLER_RESUME:
                    CsTemp = "CHANNEL_CONTROLLER_RESUME";
                    break;
                case CHCNetSDK.MINOR_SECURITY_MODULE_OFF:
                    CsTemp = "SECURITY_MODULE_OFF";
                    break;
                case CHCNetSDK.MINOR_SECURITY_MODULE_RESUME:
                    CsTemp = "SECURITY_MODULE_RESUME";
                    break;
                case CHCNetSDK.MINOR_BATTERY_ELECTRIC_LOW:
                    CsTemp = "BATTERY_ELECTRIC_LOW";
                    break;
                case CHCNetSDK.MINOR_BATTERY_ELECTRIC_RESUME:
                    CsTemp = "BATTERY_ELECTRIC_RESUME";
                    break;
                case CHCNetSDK.MINOR_LOCAL_CONTROL_NET_BROKEN:
                    CsTemp = "LOCAL_CONTROL_NET_BROKEN";
                    break;
                case CHCNetSDK.MINOR_LOCAL_CONTROL_NET_RSUME:
                    CsTemp = "LOCAL_CONTROL_NET_RSUME";
                    break;
                case CHCNetSDK.MINOR_MASTER_RS485_LOOPNODE_BROKEN:
                    CsTemp = "MASTER_RS485_LOOPNODE_BROKEN";
                    break;
                case CHCNetSDK.MINOR_MASTER_RS485_LOOPNODE_RESUME:
                    CsTemp = "MASTER_RS485_LOOPNODE_RESUME";
                    break;
                case CHCNetSDK.MINOR_LOCAL_CONTROL_OFFLINE:
                    CsTemp = "LOCAL_CONTROL_OFFLINE";
                    break;
                case CHCNetSDK.MINOR_LOCAL_CONTROL_RESUME:
                    CsTemp = "LOCAL_CONTROL_RESUME";
                    break;
                case CHCNetSDK.MINOR_LOCAL_DOWNSIDE_RS485_LOOPNODE_BROKEN:
                    CsTemp = "LOCAL_DOWNSIDE_RS485_LOOPNODE_BROKEN";
                    break;
                case CHCNetSDK.MINOR_LOCAL_DOWNSIDE_RS485_LOOPNODE_RESUME:
                    CsTemp = "LOCAL_DOWNSIDE_RS485_LOOPNODE_RESUME";
                    break;
                case CHCNetSDK.MINOR_DISTRACT_CONTROLLER_ONLINE:
                    CsTemp = "DISTRACT_CONTROLLER_ONLINE";
                    break;
                case CHCNetSDK.MINOR_DISTRACT_CONTROLLER_OFFLINE:
                    CsTemp = "DISTRACT_CONTROLLER_OFFLINE";
                    break;
                case CHCNetSDK.MINOR_ID_CARD_READER_NOT_CONNECT:
                    CsTemp = "ID_CARD_READER_NOT_CONNECT";
                    break;
                case CHCNetSDK.MINOR_ID_CARD_READER_RESUME:
                    CsTemp = "ID_CARD_READER_RESUME";
                    break;
                case CHCNetSDK.MINOR_FINGER_PRINT_MODULE_NOT_CONNECT:
                    CsTemp = "FINGER_PRINT_MODULE_NOT_CONNECT";
                    break;
                case CHCNetSDK.MINOR_FINGER_PRINT_MODULE_RESUME:
                    CsTemp = "FINGER_PRINT_MODULE_RESUME";
                    break;
                case CHCNetSDK.MINOR_CAMERA_NOT_CONNECT:
                    CsTemp = "CAMERA_NOT_CONNECT";
                    break;
                case CHCNetSDK.MINOR_CAMERA_RESUME:
                    CsTemp = "CAMERA_RESUME";
                    break;
                case CHCNetSDK.MINOR_COM_NOT_CONNECT:
                    CsTemp = "COM_NOT_CONNECT";
                    break;
                case CHCNetSDK.MINOR_COM_RESUME:
                    CsTemp = "COM_RESUME";
                    break;
                case CHCNetSDK.MINOR_DEVICE_NOT_AUTHORIZE:
                    CsTemp = "DEVICE_NOT_AUTHORIZE";
                    break;
                case CHCNetSDK.MINOR_PEOPLE_AND_ID_CARD_DEVICE_ONLINE:
                    CsTemp = "PEOPLE_AND_ID_CARD_DEVICE_ONLINE";
                    break;
                case CHCNetSDK.MINOR_PEOPLE_AND_ID_CARD_DEVICE_OFFLINE:
                    CsTemp = "PEOPLE_AND_ID_CARD_DEVICE_OFFLINE";
                    break;
                case CHCNetSDK.MINOR_LOCAL_LOGIN_LOCK:
                    CsTemp = "LOCAL_LOGIN_LOCK";
                    break;
                case CHCNetSDK.MINOR_LOCAL_LOGIN_UNLOCK:
                    CsTemp = "LOCAL_LOGIN_UNLOCK";
                    break;
                case CHCNetSDK.MINOR_SUBMARINEBACK_COMM_BREAK:
                    CsTemp = "SUBMARINEBACK_COMM_BREAK";
                    break;
                case CHCNetSDK.MINOR_SUBMARINEBACK_COMM_RESUME:
                    CsTemp = "SUBMARINEBACK_COMM_RESUME";
                    break;
                case CHCNetSDK.MINOR_MOTOR_SENSOR_EXCEPTION:
                    CsTemp = "MOTOR_SENSOR_EXCEPTION";
                    break;
                case CHCNetSDK.MINOR_CAN_BUS_EXCEPTION:
                    CsTemp = "CAN_BUS_EXCEPTION";
                    break;
                case CHCNetSDK.MINOR_CAN_BUS_RESUME:
                    CsTemp = "CAN_BUS_RESUME";
                    break;
                case CHCNetSDK.MINOR_GATE_TEMPERATURE_OVERRUN:
                    CsTemp = "GATE_TEMPERATURE_OVERRUN";
                    break;
                case CHCNetSDK.MINOR_IR_EMITTER_EXCEPTION:
                    CsTemp = "IR_EMITTER_EXCEPTION";
                    break;
                case CHCNetSDK.MINOR_IR_EMITTER_RESUME:
                    CsTemp = "IR_EMITTER_RESUME";
                    break;
                case CHCNetSDK.MINOR_LAMP_BOARD_COMM_EXCEPTION:
                    CsTemp = "LAMP_BOARD_COMM_EXCEPTION";
                    break;
                case CHCNetSDK.MINOR_LAMP_BOARD_COMM_RESUME:
                    CsTemp = "LAMP_BOARD_COMM_RESUME";
                    break;
                case CHCNetSDK.MINOR_IR_ADAPTOR_COMM_EXCEPTION:
                    CsTemp = "IR_ADAPTOR_COMM_EXCEPTION";
                    break;
                case CHCNetSDK.MINOR_IR_ADAPTOR_COMM_RESUME:
                    CsTemp = "IR_ADAPTOR_COMM_RESUME";
                    break;
                default:
                    CsTemp = Convert.ToString(struEventCfg.dwMinor, 16);
                    break;
            }
        }

        private void OperationMinorTypeMap(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            switch (struEventCfg.dwMinor)
            {
                case CHCNetSDK.MINOR_LOCAL_UPGRADE:
                    CsTemp = "LOCAL_UPGRADE";
                    break;
                case CHCNetSDK.MINOR_REMOTE_LOGIN:
                    CsTemp = "REMOTE_LOGIN";
                    break;
                case CHCNetSDK.MINOR_REMOTE_LOGOUT:
                    CsTemp = "REMOTE_LOGOUT";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ARM:
                    CsTemp = "REMOTE_ARM";
                    break;
                case CHCNetSDK.MINOR_REMOTE_DISARM:
                    CsTemp = "REMOTE_DISARM";
                    break;
                case CHCNetSDK.MINOR_REMOTE_REBOOT:
                    CsTemp = "REMOTE_REBOOT";
                    break;
                case CHCNetSDK.MINOR_REMOTE_UPGRADE:
                    CsTemp = "REMOTE_UPGRADE";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CFGFILE_OUTPUT:
                    CsTemp = "REMOTE_CFGFILE_OUTPUT";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CFGFILE_INTPUT:
                    CsTemp = "REMOTE_CFGFILE_INTPUT";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ALARMOUT_OPEN_MAN:
                    CsTemp = "REMOTE_ALARMOUT_OPEN_MAN";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ALARMOUT_CLOSE_MAN:
                    CsTemp = "REMOTE_ALARMOUT_CLOSE_MAN";
                    break;
                case CHCNetSDK.MINOR_REMOTE_OPEN_DOOR:
                    CsTemp = "REMOTE_OPEN_DOOR";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CLOSE_DOOR:
                    CsTemp = "REMOTE_CLOSE_DOOR";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ALWAYS_OPEN:
                    CsTemp = "REMOTE_ALWAYS_OPEN";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ALWAYS_CLOSE:
                    CsTemp = "REMOTE_ALWAYS_CLOSE";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CHECK_TIME:
                    CsTemp = "REMOTE_CHECK_TIME";
                    break;
                case CHCNetSDK.MINOR_NTP_CHECK_TIME:
                    CsTemp = "NTP_CHECK_TIME";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CLEAR_CARD:
                    CsTemp = "REMOTE_CLEAR_CARD";
                    break;
                case CHCNetSDK.MINOR_REMOTE_RESTORE_CFG:
                    CsTemp = "REMOTE_RESTORE_CFG";
                    break;
                case CHCNetSDK.MINOR_ALARMIN_ARM:
                    CsTemp = "ALARMIN_ARM";
                    break;
                case CHCNetSDK.MINOR_ALARMIN_DISARM:
                    CsTemp = "ALARMIN_DISARM";
                    break;
                case CHCNetSDK.MINOR_LOCAL_RESTORE_CFG:
                    CsTemp = "LOCAL_RESTORE_CFG";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CAPTURE_PIC:
                    CsTemp = "REMOTE_CAPTURE_PIC";
                    break;
                case CHCNetSDK.MINOR_MOD_NET_REPORT_CFG:
                    CsTemp = "MOD_NET_REPORT_CFG";
                    break;
                case CHCNetSDK.MINOR_MOD_GPRS_REPORT_PARAM:
                    CsTemp = "MOD_GPRS_REPORT_PARAM";
                    break;
                case CHCNetSDK.MINOR_MOD_REPORT_GROUP_PARAM:
                    CsTemp = "MOD_REPORT_GROUP_PARAM";
                    break;
                case CHCNetSDK.MINOR_UNLOCK_PASSWORD_OPEN_DOOR:
                    CsTemp = "UNLOCK_PASSWORD_OPEN_DOOR";
                    break;
                case CHCNetSDK.MINOR_AUTO_RENUMBER:
                    CsTemp = "AUTO_RENUMBER";
                    break;
                case CHCNetSDK.MINOR_AUTO_COMPLEMENT_NUMBER:
                    CsTemp = "AUTO_COMPLEMENT_NUMBER";
                    break;
                case CHCNetSDK.MINOR_NORMAL_CFGFILE_INPUT:
                    CsTemp = "NORMAL_CFGFILE_INPUT";
                    break;
                case CHCNetSDK.MINOR_NORMAL_CFGFILE_OUTTPUT:
                    CsTemp = "NORMAL_CFGFILE_OUTTPUT";
                    break;
                case CHCNetSDK.MINOR_CARD_RIGHT_INPUT:
                    CsTemp = "CARD_RIGHT_INPUT";
                    break;
                case CHCNetSDK.MINOR_CARD_RIGHT_OUTTPUT:
                    CsTemp = "CARD_RIGHT_OUTTPUT";
                    break;
                case CHCNetSDK.MINOR_LOCAL_USB_UPGRADE:
                    CsTemp = "LOCAL_USB_UPGRADE";
                    break;
                case CHCNetSDK.MINOR_REMOTE_VISITOR_CALL_LADDER:
                    CsTemp = "REMOTE_VISITOR_CALL_LADDER";
                    break;
                case CHCNetSDK.MINOR_REMOTE_HOUSEHOLD_CALL_LADDER:
                    CsTemp = "REMOTE_HOUSEHOLD_CALL_LADDER";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ACTUAL_GUARD:
                    CsTemp = "REMOTE_ACTUAL_GUARD";
                    break;
                case CHCNetSDK.MINOR_REMOTE_ACTUAL_UNGUARD:
                    CsTemp = "REMOTE_ACTUAL_UNGUARD";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CONTROL_NOT_CODE_OPER_FAILED:
                    CsTemp = "REMOTE_CONTROL_NOT_CODE_OPER_FAILED";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CONTROL_CLOSE_DOOR:
                    CsTemp = "REMOTE_CONTROL_CLOSE_DOOR";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CONTROL_OPEN_DOOR:
                    CsTemp = "REMOTE_CONTROL_OPEN_DOOR";
                    break;
                case CHCNetSDK.MINOR_REMOTE_CONTROL_ALWAYS_OPEN_DOOR:
                    CsTemp = "REMOTE_CONTROL_ALWAYS_OPEN_DOOR";
                    break;
                default:
                    CsTemp = Convert.ToString(struEventCfg.dwMinor, 16);
                    break;
            }
        }

        private void EventMinorTypeMap(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            switch (struEventCfg.dwMinor)
            {
                case CHCNetSDK.MINOR_LEGAL_CARD_PASS:
                    CsTemp = "LEGAL_CARD_PASS";
                    break;
                case CHCNetSDK.MINOR_CARD_AND_PSW_PASS:
                    CsTemp = "CARD_AND_PSW_PASS";
                    break;
                case CHCNetSDK.MINOR_CARD_AND_PSW_FAIL:
                    CsTemp = "CARD_AND_PSW_FAIL";
                    break;
                case CHCNetSDK.MINOR_CARD_AND_PSW_TIMEOUT:
                    CsTemp = "CARD_AND_PSW_TIMEOUT";
                    break;
                case CHCNetSDK.MINOR_CARD_AND_PSW_OVER_TIME:
                    CsTemp = "CARD_AND_PSW_OVER_TIME";
                    break;
                case CHCNetSDK.MINOR_CARD_NO_RIGHT:
                    CsTemp = "CARD_NO_RIGHT";
                    break;
                case CHCNetSDK.MINOR_CARD_INVALID_PERIOD:
                    CsTemp = "CARD_INVALID_PERIOD";
                    break;
                case CHCNetSDK.MINOR_CARD_OUT_OF_DATE:
                    CsTemp = "CARD_OUT_OF_DATE";
                    break;
                case CHCNetSDK.MINOR_INVALID_CARD:
                    CsTemp = "INVALID_CARD";
                    break;
                case CHCNetSDK.MINOR_ANTI_SNEAK_FAIL:
                    CsTemp = "ANTI_SNEAK_FAIL";
                    break;
                case CHCNetSDK.MINOR_INTERLOCK_DOOR_NOT_CLOSE:
                    CsTemp = "INTERLOCK_DOOR_NOT_CLOSE";
                    break;
                case CHCNetSDK.MINOR_NOT_BELONG_MULTI_GROUP:
                    CsTemp = "NOT_BELONG_MULTI_GROUP";
                    break;
                case CHCNetSDK.MINOR_INVALID_MULTI_VERIFY_PERIOD:
                    CsTemp = "INVALID_MULTI_VERIFY_PERIOD";
                    break;
                case CHCNetSDK.MINOR_MULTI_VERIFY_SUPER_RIGHT_FAIL:
                    CsTemp = "MULTI_VERIFY_SUPER_RIGHT_FAIL";
                    break;
                case CHCNetSDK.MINOR_MULTI_VERIFY_REMOTE_RIGHT_FAIL:
                    CsTemp = "MULTI_VERIFY_REMOTE_RIGHT_FAIL";
                    break;
                case CHCNetSDK.MINOR_MULTI_VERIFY_SUCCESS:
                    CsTemp = "MULTI_VERIFY_SUCCESS";
                    break;
                case CHCNetSDK.MINOR_LEADER_CARD_OPEN_BEGIN:
                    CsTemp = "LEADER_CARD_OPEN_BEGIN";
                    break;
                case CHCNetSDK.MINOR_LEADER_CARD_OPEN_END:
                    CsTemp = "LEADER_CARD_OPEN_END";
                    break;
                case CHCNetSDK.MINOR_ALWAYS_OPEN_BEGIN:
                    CsTemp = "ALWAYS_OPEN_BEGIN";
                    break;
                case CHCNetSDK.MINOR_ALWAYS_OPEN_END:
                    CsTemp = "ALWAYS_OPEN_END";
                    break;
                case CHCNetSDK.MINOR_LOCK_OPEN:
                    CsTemp = "LOCK_OPEN";
                    break;
                case CHCNetSDK.MINOR_LOCK_CLOSE:
                    CsTemp = "LOCK_CLOSE";
                    break;
                case CHCNetSDK.MINOR_DOOR_BUTTON_PRESS:
                    CsTemp = "DOOR_BUTTON_PRESS";
                    break;
                case CHCNetSDK.MINOR_DOOR_BUTTON_RELEASE:
                    CsTemp = "DOOR_BUTTON_RELEASE";
                    break;
                case CHCNetSDK.MINOR_DOOR_OPEN_NORMAL:
                    CsTemp = "DOOR_OPEN_NORMAL";
                    break;
                case CHCNetSDK.MINOR_DOOR_CLOSE_NORMAL:
                    CsTemp = "DOOR_CLOSE_NORMAL";
                    break;
                case CHCNetSDK.MINOR_DOOR_OPEN_ABNORMAL:
                    CsTemp = "DOOR_OPEN_ABNORMAL";
                    break;
                case CHCNetSDK.MINOR_DOOR_OPEN_TIMEOUT:
                    CsTemp = "DOOR_OPEN_TIMEOUT";
                    break;
                case CHCNetSDK.MINOR_ALARMOUT_ON:
                    CsTemp = "ALARMOUT_ON";
                    break;
                case CHCNetSDK.MINOR_ALARMOUT_OFF:
                    CsTemp = "ALARMOUT_OFF";
                    break;
                case CHCNetSDK.MINOR_ALWAYS_CLOSE_BEGIN:
                    CsTemp = "ALWAYS_CLOSE_BEGIN";
                    break;
                case CHCNetSDK.MINOR_ALWAYS_CLOSE_END:
                    CsTemp = "ALWAYS_CLOSE_END";
                    break;
                case CHCNetSDK.MINOR_MULTI_VERIFY_NEED_REMOTE_OPEN:
                    CsTemp = "MULTI_VERIFY_NEED_REMOTE_OPEN";
                    break;
                case CHCNetSDK.MINOR_MULTI_VERIFY_SUPERPASSWD_VERIFY_SUCCESS:
                    CsTemp = "MULTI_VERIFY_SUPERPASSWD_VERIFY_SUCCESS";
                    break;
                case CHCNetSDK.MINOR_MULTI_VERIFY_REPEAT_VERIFY:
                    CsTemp = "MULTI_VERIFY_REPEAT_VERIFY";
                    break;
                case CHCNetSDK.MINOR_MULTI_VERIFY_TIMEOUT:
                    CsTemp = "MULTI_VERIFY_TIMEOUT";
                    break;
                case CHCNetSDK.MINOR_DOORBELL_RINGING:
                    CsTemp = "DOORBELL_RINGING";
                    break;
                case CHCNetSDK.MINOR_FINGERPRINT_COMPARE_PASS:
                    CsTemp = "FINGERPRINT_COMPARE_PASS";
                    break;
                case CHCNetSDK.MINOR_FINGERPRINT_COMPARE_FAIL:
                    CsTemp = "FINGERPRINT_COMPARE_FAIL";
                    break;
                case CHCNetSDK.MINOR_CARD_FINGERPRINT_VERIFY_PASS:
                    CsTemp = "CARD_FINGERPRINT_VERIFY_PASS";
                    break;
                case CHCNetSDK.MINOR_CARD_FINGERPRINT_VERIFY_FAIL:
                    CsTemp = "CARD_FINGERPRINT_VERIFY_FAIL";
                    break;
                case CHCNetSDK.MINOR_CARD_FINGERPRINT_VERIFY_TIMEOUT:
                    CsTemp = "CARD_FINGERPRINT_VERIFY_TIMEOUT";
                    break;
                case CHCNetSDK.MINOR_CARD_FINGERPRINT_PASSWD_VERIFY_PASS:
                    CsTemp = "CARD_FINGERPRINT_PASSWD_VERIFY_PASS";
                    break;
                case CHCNetSDK.MINOR_CARD_FINGERPRINT_PASSWD_VERIFY_FAIL:
                    CsTemp = "CARD_FINGERPRINT_PASSWD_VERIFY_FAIL";
                    break;
                case CHCNetSDK.MINOR_CARD_FINGERPRINT_PASSWD_VERIFY_TIMEOUT:
                    CsTemp = "CARD_FINGERPRINT_PASSWD_VERIFY_TIMEOUT";
                    break;
                case CHCNetSDK.MINOR_FINGERPRINT_PASSWD_VERIFY_PASS:
                    CsTemp = "FINGERPRINT_PASSWD_VERIFY_PASS";
                    break;
                case CHCNetSDK.MINOR_FINGERPRINT_PASSWD_VERIFY_FAIL:
                    CsTemp = "FINGERPRINT_PASSWD_VERIFY_FAIL";
                    break;
                case CHCNetSDK.MINOR_FINGERPRINT_PASSWD_VERIFY_TIMEOUT:
                    CsTemp = "FINGERPRINT_PASSWD_VERIFY_TIMEOUT";
                    break;
                case CHCNetSDK.MINOR_FINGERPRINT_INEXISTENCE:
                    CsTemp = "FINGERPRINT_INEXISTENCE";
                    break;
                case CHCNetSDK.MINOR_CARD_PLATFORM_VERIFY:
                    CsTemp = "CARD_PLATFORM_VERIFY";
                    break;
                case CHCNetSDK.MINOR_MAC_DETECT:
                    CsTemp = "MINOR_MAC_DETECT";
                    break;
                case CHCNetSDK.MINOR_LEGAL_MESSAGE:
                    CsTemp = "MINOR_LEGAL_MESSAGE";
                    break;
                case CHCNetSDK.MINOR_ILLEGAL_MESSAGE:
                    CsTemp = "MINOR_ILLEGAL_MESSAGE";
                    break;
                case CHCNetSDK.MINOR_DOOR_OPEN_OR_DORMANT_FAIL:
                    CsTemp = "DOOR_OPEN_OR_DORMANT_FAIL";
                    break;
                case CHCNetSDK.MINOR_AUTH_PLAN_DORMANT_FAIL:
                    CsTemp = "AUTH_PLAN_DORMANT_FAIL";
                    break;
                case CHCNetSDK.MINOR_CARD_ENCRYPT_VERIFY_FAIL:
                    CsTemp = "CARD_ENCRYPT_VERIFY_FAIL";
                    break;
                case CHCNetSDK.MINOR_SUBMARINEBACK_REPLY_FAIL:
                    CsTemp = "SUBMARINEBACK_REPLY_FAIL";
                    break;
                case CHCNetSDK.MINOR_DOOR_OPEN_OR_DORMANT_OPEN_FAIL:
                    CsTemp = "DOOR_OPEN_OR_DORMANT_OPEN_FAIL";
                    break;
                case CHCNetSDK.MINOR_DOOR_OPEN_OR_DORMANT_LINKAGE_OPEN_FAIL:
                    CsTemp = "DOOR_OPEN_OR_DORMANT_LINKAGE_OPEN_FAIL";
                    break;
                case CHCNetSDK.MINOR_TRAILING:
                    CsTemp = "TRAILING";
                    break;
                case CHCNetSDK.MINOR_REVERSE_ACCESS:
                    CsTemp = "REVERSE_ACCESS";
                    break;
                case CHCNetSDK.MINOR_FORCE_ACCESS:
                    CsTemp = "FORCE_ACCESS";
                    break;
                case CHCNetSDK.MINOR_CLIMBING_OVER_GATE:
                    CsTemp = "CLIMBING_OVER_GATE";
                    break;
                case CHCNetSDK.MINOR_PASSING_TIMEOUT:
                    CsTemp = "PASSING_TIMEOUT";
                    break;
                case CHCNetSDK.MINOR_INTRUSION_ALARM:
                    CsTemp = "INTRUSION_ALARM";
                    break;
                case CHCNetSDK.MINOR_FREE_GATE_PASS_NOT_AUTH:
                    CsTemp = "FREE_GATE_PASS_NOT_AUTH";
                    break;
                case CHCNetSDK.MINOR_DROP_ARM_BLOCK:
                    CsTemp = "DROP_ARM_BLOCK";
                    break;
                case CHCNetSDK.MINOR_DROP_ARM_BLOCK_RESUME:
                    CsTemp = "DROP_ARM_BLOCK_RESUME";
                    break;
                case CHCNetSDK.MINOR_LOCAL_FACE_MODELING_FAIL:
                    CsTemp = "LOCAL_FACE_MODELING_FAIL";
                    break;
                case CHCNetSDK.MINOR_STAY_EVENT:
                    CsTemp = "STAY_EVENT";
                    break;
                case CHCNetSDK.MINOR_PASSWORD_MISMATCH:
                    CsTemp = "PASSWORD_MISMATCH";
                    break;
                case CHCNetSDK.MINOR_EMPLOYEE_NO_NOT_EXIST:
                    CsTemp = "EMPLOYEE_NO_NOT_EXIST";
                    break;
                case CHCNetSDK.MINOR_COMBINED_VERIFY_PASS:
                    CsTemp = "COMBINED_VERIFY_PASS";
                    break;
                case CHCNetSDK.MINOR_COMBINED_VERIFY_TIMEOUT:
                    CsTemp = "COMBINED_VERIFY_TIMEOUT";
                    break;
                case CHCNetSDK.MINOR_VERIFY_MODE_MISMATCH:
                    CsTemp = "VERIFY_MODE_MISMATCH";
                    break;
                default:
                    CsTemp = Convert.ToString(struEventCfg.dwMinor, 16);
                    break;
            }
        }

        private void ProcessMinorType(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            switch (struEventCfg.dwMajor)
            {
                case CHCNetSDK.MAJOR_ALARM:
                    AlarmMinorTypeMap(ref struEventCfg);
                    break;
                case CHCNetSDK.MAJOR_EXCEPTION:
                    ExceptionMinorTypeMap(ref struEventCfg);
                    break;
                case CHCNetSDK.MAJOR_OPERATION:
                    OperationMinorTypeMap(ref struEventCfg);
                    break;
                case CHCNetSDK.MAJOR_EVENT:
                    EventMinorTypeMap(ref struEventCfg);
                    break;
                default:
                    CsTemp = "Unknown";
                    break;
            }
        }

        private void CardTypeMap(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            switch (struEventCfg.struAcsEventInfo.byCardType)
            {
                case 1:
                    CsTemp = "Ordinary Card";
                    break;
                case 2:
                    CsTemp = "Disabled Card";
                    break;
                case 3:
                    CsTemp = "Black List Card";
                    break;
                case 4:
                    CsTemp = "Patrol Card";
                    break;
                case 5:
                    CsTemp = "Stress Card";
                    break;
                case 6:
                    CsTemp = "Super Card";
                    break;
                case 7:
                    CsTemp = "Guest Card";
                    break;
                case 8:
                    CsTemp = "Release Card";
                    break;
                default:
                    CsTemp = "No effect";
                    break;
            }
        }

        private void ProcessReportChannel(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG EventCfg)
        {
            switch (EventCfg.struAcsEventInfo.byReportChannel)
            {
                case 1:
                    CsTemp = "Upload";
                    break;
                case 2:
                    CsTemp = "Center 1 Upload";
                    break;
                case 3:
                    CsTemp = "Center 2 Upload";
                    break;
                default:
                    CsTemp = "No effect";
                    break;
            }
        }

        private void ProcessCardReader(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            switch (struEventCfg.struAcsEventInfo.byCardReaderKind)
            {
                case 1:
                    CsTemp = "IC Reader";
                    break;
                case 2:
                    CsTemp = "Certificate Reader";
                    break;
                case 3:
                    CsTemp = "Two-dimension Reader";
                    break;
                case 4:
                    CsTemp = "Finger Print Head";
                    break;
                default:
                    CsTemp = "No effect";
                    break;
            }
        }

        private void ProcessByType(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            switch (struEventCfg.struAcsEventInfo.byType)
            {
                case 0:
                    CsTemp = "Instant Zone";
                    break;
                case 1:
                    CsTemp = "24 Hour Zone";
                    break;
                case 2:
                    CsTemp = "Delay Zone";
                    break;
                case 3:
                    CsTemp = "Internal Zone";
                    break;
                case 4:
                    CsTemp = "Key Zone";
                    break;
                case 5:
                    CsTemp = "Fire Zone";
                    break;
                case 6:
                    CsTemp = "Perimeter Zone";
                    break;
                case 7:
                    CsTemp = "24 Hour Silent Zone";
                    break;
                case 8:
                    CsTemp = "24 Hour Auxiliary Zone";
                    break;
                case 9:
                    CsTemp = "24 Hour Vibration Zone";
                    break;
                case 10:
                    CsTemp = "Acs Emergency Open Zone";
                    break;
                case 11:
                    CsTemp = "Acs Emergency Close Zone";
                    break;
                default:
                    CsTemp = "No Effect";
                    break;
            }
        }
        private void ProcessInternatAccess(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            switch (struEventCfg.struAcsEventInfo.byInternetAccess)
            {
                case 1:
                    CsTemp = "Up Network Port 1";
                    break;
                case 2:
                    CsTemp = "Up Network Port 2";
                    break;
                case 3:
                    CsTemp = "Down Network Port 1";
                    break;
                default:
                    CsTemp = "No effect";
                    break;
            }
        }

        private void ProcessSwipeCard(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            if (struEventCfg.struAcsEventInfo.bySwipeCardType == 1)
            {
                CsTemp = "QR Code";
            }
            else
            {
                CsTemp = "No effect";
            }
        }

        private void ProcessMacAdd(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            if (struEventCfg.struAcsEventInfo.byMACAddr[0] != 0)
            {
                CsTemp = struEventCfg.struAcsEventInfo.byMACAddr[0].ToString() + ":" +
                    struEventCfg.struAcsEventInfo.byMACAddr[1].ToString() + ":" +
                    struEventCfg.struAcsEventInfo.byMACAddr[2].ToString() + ":" +
                    struEventCfg.struAcsEventInfo.byMACAddr[3].ToString() + ":" +
                    struEventCfg.struAcsEventInfo.byMACAddr[4].ToString() + ":" +
                    struEventCfg.struAcsEventInfo.byMACAddr[5].ToString();
            }
            else
            {
                CsTemp = "No Effect";
            }
        }

        private string GetStrLogTime(ref CHCNetSDK.NET_DVR_TIME time)
        {
            string res = time.dwYear.ToString() + ":" + time.dwMonth.ToString() + ":"
                + time.dwDay.ToString() + ":" + time.dwHour.ToString() + ":" + time.dwMinute.ToString()
                + ":" + time.dwSecond.ToString();
            return res;
        }

        #endregion
    }
}