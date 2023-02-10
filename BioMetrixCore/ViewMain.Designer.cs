namespace BioMetrixCore
{
    partial class ViewMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewMain));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblDeviceInfo = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvRecords = new System.Windows.Forms.DataGridView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDownloadFingerPrint = new System.Windows.Forms.Button();
            this.btnPullData = new System.Windows.Forms.Button();
            this.btnGetAllUserID = new System.Windows.Forms.Button();
            this.btnGetDeviceTime = new System.Windows.Forms.Button();
            this.btnBeep = new System.Windows.Forms.Button();
            this.btnEnableDevice = new System.Windows.Forms.Button();
            this.btnDisableDevice = new System.Windows.Forms.Button();
            this.btnRestartDevice = new System.Windows.Forms.Button();
            this.btnPowerOff = new System.Windows.Forms.Button();
            this.btnUploadUserInfo = new System.Windows.Forms.Button();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxPassWord = new System.Windows.Forms.TextBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxDeviceIP = new System.Windows.Forms.TextBox();
            this.tbxPort = new System.Windows.Forms.TextBox();
            this.btnPingDevice = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxMachineNumber = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgvLogData = new System.Windows.Forms.DataGridView();
            this.tbxURL = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnGetData = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tbxPass = new System.Windows.Forms.TextBox();
            this.tbxUserName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbxCompanyCode = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dgvListBSSID = new System.Windows.Forms.DataGridView();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCmdText = new System.Windows.Forms.TextBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btnGetBSSID = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dgvListLogByHanetAI = new System.Windows.Forms.DataGridView();
            this.btnGetLogByHanetAIV2 = new System.Windows.Forms.Button();
            this.btnGetLogByHanetAI = new System.Windows.Forms.Button();
            this.dtToDateHanetAI = new System.Windows.Forms.DateTimePicker();
            this.dtFromDateHanetAI = new System.Windows.Forms.DateTimePicker();
            this.tbRefreshToken = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tbAccessToken = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbClientSecret = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbClientID = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label15 = new System.Windows.Forms.Label();
            this.txtLinkByHysoon = new System.Windows.Forms.TextBox();
            this.txtPortByHysoon = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.btnGetLogsByHysoon = new System.Windows.Forms.Button();
            this.dgvListLogByHysoon = new System.Windows.Forms.DataGridView();
            this.dtFromDateByHysoon = new System.Windows.Forms.DateTimePicker();
            this.dtToDateByHysoon = new System.Windows.Forms.DateTimePicker();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogData)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListBSSID)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListLogByHanetAI)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListLogByHysoon)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Controls.Add(this.tabPage4);
            this.tabControl.Controls.Add(this.tabPage5);
            this.tabControl.Location = new System.Drawing.Point(0, -3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(804, 454);
            this.tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblDeviceInfo);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.pnlHeader);
            this.tabPage1.Controls.Add(this.lblStatus);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(796, 428);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Zkteco";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblDeviceInfo
            // 
            this.lblDeviceInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDeviceInfo.Location = new System.Drawing.Point(6, 35);
            this.lblDeviceInfo.Name = "lblDeviceInfo";
            this.lblDeviceInfo.Size = new System.Drawing.Size(792, 18);
            this.lblDeviceInfo.TabIndex = 896;
            this.lblDeviceInfo.Text = "Device Info : --";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.dgvRecords);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Location = new System.Drawing.Point(7, 73);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(791, 398);
            this.panel1.TabIndex = 895;
            // 
            // dgvRecords
            // 
            this.dgvRecords.AllowUserToAddRows = false;
            this.dgvRecords.AllowUserToDeleteRows = false;
            this.dgvRecords.AllowUserToOrderColumns = true;
            this.dgvRecords.AllowUserToResizeColumns = false;
            this.dgvRecords.AllowUserToResizeRows = false;
            this.dgvRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecords.Location = new System.Drawing.Point(0, 54);
            this.dgvRecords.Name = "dgvRecords";
            this.dgvRecords.Size = new System.Drawing.Size(791, 344);
            this.dgvRecords.TabIndex = 883;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnDownloadFingerPrint);
            this.flowLayoutPanel1.Controls.Add(this.btnPullData);
            this.flowLayoutPanel1.Controls.Add(this.btnGetAllUserID);
            this.flowLayoutPanel1.Controls.Add(this.btnGetDeviceTime);
            this.flowLayoutPanel1.Controls.Add(this.btnBeep);
            this.flowLayoutPanel1.Controls.Add(this.btnEnableDevice);
            this.flowLayoutPanel1.Controls.Add(this.btnDisableDevice);
            this.flowLayoutPanel1.Controls.Add(this.btnRestartDevice);
            this.flowLayoutPanel1.Controls.Add(this.btnPowerOff);
            this.flowLayoutPanel1.Controls.Add(this.btnUploadUserInfo);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(791, 54);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnDownloadFingerPrint
            // 
            this.btnDownloadFingerPrint.Location = new System.Drawing.Point(3, 3);
            this.btnDownloadFingerPrint.Name = "btnDownloadFingerPrint";
            this.btnDownloadFingerPrint.Size = new System.Drawing.Size(112, 48);
            this.btnDownloadFingerPrint.TabIndex = 9;
            this.btnDownloadFingerPrint.Text = "Get All User Info";
            this.btnDownloadFingerPrint.UseVisualStyleBackColor = true;
            // 
            // btnPullData
            // 
            this.btnPullData.Location = new System.Drawing.Point(121, 3);
            this.btnPullData.Name = "btnPullData";
            this.btnPullData.Size = new System.Drawing.Size(80, 48);
            this.btnPullData.TabIndex = 10;
            this.btnPullData.Text = "Get Log Data";
            this.btnPullData.UseVisualStyleBackColor = true;
            this.btnPullData.Click += new System.EventHandler(this.btnPullData_Click);
            // 
            // btnGetAllUserID
            // 
            this.btnGetAllUserID.Location = new System.Drawing.Point(207, 3);
            this.btnGetAllUserID.Name = "btnGetAllUserID";
            this.btnGetAllUserID.Size = new System.Drawing.Size(72, 48);
            this.btnGetAllUserID.TabIndex = 892;
            this.btnGetAllUserID.Text = "Get All User ID";
            this.btnGetAllUserID.UseVisualStyleBackColor = true;
            // 
            // btnGetDeviceTime
            // 
            this.btnGetDeviceTime.Location = new System.Drawing.Point(285, 3);
            this.btnGetDeviceTime.Name = "btnGetDeviceTime";
            this.btnGetDeviceTime.Size = new System.Drawing.Size(78, 48);
            this.btnGetDeviceTime.TabIndex = 887;
            this.btnGetDeviceTime.Text = "Get Device Time";
            this.btnGetDeviceTime.UseVisualStyleBackColor = true;
            // 
            // btnBeep
            // 
            this.btnBeep.Location = new System.Drawing.Point(369, 3);
            this.btnBeep.Name = "btnBeep";
            this.btnBeep.Size = new System.Drawing.Size(59, 48);
            this.btnBeep.TabIndex = 5;
            this.btnBeep.Text = "Beep";
            this.btnBeep.UseVisualStyleBackColor = true;
            // 
            // btnEnableDevice
            // 
            this.btnEnableDevice.Location = new System.Drawing.Point(434, 3);
            this.btnEnableDevice.Name = "btnEnableDevice";
            this.btnEnableDevice.Size = new System.Drawing.Size(65, 48);
            this.btnEnableDevice.TabIndex = 889;
            this.btnEnableDevice.Text = "Enable Device";
            this.btnEnableDevice.UseVisualStyleBackColor = true;
            // 
            // btnDisableDevice
            // 
            this.btnDisableDevice.Location = new System.Drawing.Point(505, 3);
            this.btnDisableDevice.Name = "btnDisableDevice";
            this.btnDisableDevice.Size = new System.Drawing.Size(65, 48);
            this.btnDisableDevice.TabIndex = 890;
            this.btnDisableDevice.Text = "Disable Device";
            this.btnDisableDevice.UseVisualStyleBackColor = true;
            // 
            // btnRestartDevice
            // 
            this.btnRestartDevice.Location = new System.Drawing.Point(576, 3);
            this.btnRestartDevice.Name = "btnRestartDevice";
            this.btnRestartDevice.Size = new System.Drawing.Size(65, 48);
            this.btnRestartDevice.TabIndex = 886;
            this.btnRestartDevice.Text = "Restart Device";
            this.btnRestartDevice.UseVisualStyleBackColor = true;
            // 
            // btnPowerOff
            // 
            this.btnPowerOff.Location = new System.Drawing.Point(647, 3);
            this.btnPowerOff.Name = "btnPowerOff";
            this.btnPowerOff.Size = new System.Drawing.Size(65, 48);
            this.btnPowerOff.TabIndex = 885;
            this.btnPowerOff.Text = "Power Off";
            this.btnPowerOff.UseVisualStyleBackColor = true;
            // 
            // btnUploadUserInfo
            // 
            this.btnUploadUserInfo.Location = new System.Drawing.Point(718, 3);
            this.btnUploadUserInfo.Name = "btnUploadUserInfo";
            this.btnUploadUserInfo.Size = new System.Drawing.Size(65, 48);
            this.btnUploadUserInfo.TabIndex = 893;
            this.btnUploadUserInfo.Text = "Upload User Info";
            this.btnUploadUserInfo.UseVisualStyleBackColor = true;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.pnlHeader.Controls.Add(this.label4);
            this.pnlHeader.Controls.Add(this.tbxPassWord);
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Controls.Add(this.label1);
            this.pnlHeader.Controls.Add(this.tbxDeviceIP);
            this.pnlHeader.Controls.Add(this.tbxPort);
            this.pnlHeader.Controls.Add(this.btnPingDevice);
            this.pnlHeader.Controls.Add(this.label2);
            this.pnlHeader.Controls.Add(this.btnConnect);
            this.pnlHeader.Controls.Add(this.label3);
            this.pnlHeader.Controls.Add(this.tbxMachineNumber);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(3, 3);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(790, 50);
            this.pnlHeader.TabIndex = 894;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(353, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Mã kết nối";
            // 
            // tbxPassWord
            // 
            this.tbxPassWord.Location = new System.Drawing.Point(421, 7);
            this.tbxPassWord.Name = "tbxPassWord";
            this.tbxPassWord.Size = new System.Drawing.Size(56, 20);
            this.tbxPassWord.TabIndex = 9;
            this.tbxPassWord.Text = "0";
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(116)))), ((int)(((byte)(116)))));
            this.lblHeader.Location = new System.Drawing.Point(12, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(75, 19);
            this.lblHeader.TabIndex = 1;
            this.lblHeader.Text = "ZKTECO";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Device IP";
            // 
            // tbxDeviceIP
            // 
            this.tbxDeviceIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxDeviceIP.Location = new System.Drawing.Point(152, 7);
            this.tbxDeviceIP.Name = "tbxDeviceIP";
            this.tbxDeviceIP.Size = new System.Drawing.Size(99, 20);
            this.tbxDeviceIP.TabIndex = 0;
            this.tbxDeviceIP.Text = "172.18.12.222";
            // 
            // tbxPort
            // 
            this.tbxPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxPort.Location = new System.Drawing.Point(289, 6);
            this.tbxPort.MaxLength = 6;
            this.tbxPort.Name = "tbxPort";
            this.tbxPort.Size = new System.Drawing.Size(56, 20);
            this.tbxPort.TabIndex = 2;
            this.tbxPort.Text = "4370";
            // 
            // btnPingDevice
            // 
            this.btnPingDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPingDevice.Location = new System.Drawing.Point(695, 9);
            this.btnPingDevice.Name = "btnPingDevice";
            this.btnPingDevice.Size = new System.Drawing.Size(75, 23);
            this.btnPingDevice.TabIndex = 5;
            this.btnPingDevice.Text = "Ping Device";
            this.btnPingDevice.UseVisualStyleBackColor = true;
            this.btnPingDevice.Click += new System.EventHandler(this.btnPingDevice_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(257, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port";
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(614, 9);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(470, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Machine Number";
            // 
            // tbxMachineNumber
            // 
            this.tbxMachineNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxMachineNumber.Location = new System.Drawing.Point(571, 9);
            this.tbxMachineNumber.MaxLength = 3;
            this.tbxMachineNumber.Name = "tbxMachineNumber";
            this.tbxMachineNumber.Size = new System.Drawing.Size(37, 20);
            this.tbxMachineNumber.TabIndex = 8;
            this.tbxMachineNumber.Text = "1";
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(3, 395);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.lblStatus.Size = new System.Drawing.Size(790, 30);
            this.lblStatus.TabIndex = 893;
            this.lblStatus.Text = "label3";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvLogData);
            this.tabPage2.Controls.Add(this.tbxURL);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.btnGetData);
            this.tabPage2.Controls.Add(this.btnLogin);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.tbxPass);
            this.tabPage2.Controls.Add(this.tbxUserName);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.tbxCompanyCode);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(796, 428);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "BiFace";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvLogData
            // 
            this.dgvLogData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLogData.Location = new System.Drawing.Point(-12, 152);
            this.dgvLogData.Name = "dgvLogData";
            this.dgvLogData.Size = new System.Drawing.Size(796, 267);
            this.dgvLogData.TabIndex = 10;
            // 
            // tbxURL
            // 
            this.tbxURL.Location = new System.Drawing.Point(104, 129);
            this.tbxURL.Name = "tbxURL";
            this.tbxURL.Size = new System.Drawing.Size(238, 20);
            this.tbxURL.TabIndex = 9;
            this.tbxURL.Text = "https://cloud.beetai.com/api/login_integrate";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 136);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "URL";
            // 
            // btnGetData
            // 
            this.btnGetData.Location = new System.Drawing.Point(483, 50);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(75, 23);
            this.btnGetData.TabIndex = 7;
            this.btnGetData.Text = "Lấy dữ liệu";
            this.btnGetData.UseVisualStyleBackColor = true;
            this.btnGetData.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(483, 16);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 6;
            this.btnLogin.Text = "Đăng nhập";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.button1_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Mật khẩu";
            // 
            // tbxPass
            // 
            this.tbxPass.Location = new System.Drawing.Point(104, 88);
            this.tbxPass.Name = "tbxPass";
            this.tbxPass.Size = new System.Drawing.Size(238, 20);
            this.tbxPass.TabIndex = 4;
            this.tbxPass.Text = "c7knaG7mJahbbXxU";
            // 
            // tbxUserName
            // 
            this.tbxUserName.Location = new System.Drawing.Point(104, 47);
            this.tbxUserName.Name = "tbxUserName";
            this.tbxUserName.Size = new System.Drawing.Size(238, 20);
            this.tbxUserName.TabIndex = 3;
            this.tbxUserName.Text = "misa@gmail.com";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Email";
            // 
            // tbxCompanyCode
            // 
            this.tbxCompanyCode.Location = new System.Drawing.Point(104, 13);
            this.tbxCompanyCode.Name = "tbxCompanyCode";
            this.tbxCompanyCode.Size = new System.Drawing.Size(238, 20);
            this.tbxCompanyCode.TabIndex = 1;
            this.tbxCompanyCode.Text = "xuancuong";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Mã công ty";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dgvListBSSID);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.txtCmdText);
            this.tabPage3.Controls.Add(this.txtResult);
            this.tabPage3.Controls.Add(this.btnGetBSSID);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(796, 428);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "BSSID";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgvListBSSID
            // 
            this.dgvListBSSID.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvListBSSID.Location = new System.Drawing.Point(19, 183);
            this.dgvListBSSID.Name = "dgvListBSSID";
            this.dgvListBSSID.Size = new System.Drawing.Size(721, 234);
            this.dgvListBSSID.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(173, 60);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Kết quả";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(173, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Câu lệnh";
            // 
            // txtCmdText
            // 
            this.txtCmdText.Location = new System.Drawing.Point(257, 21);
            this.txtCmdText.Name = "txtCmdText";
            this.txtCmdText.Size = new System.Drawing.Size(483, 20);
            this.txtCmdText.TabIndex = 2;
            this.txtCmdText.Text = "netsh wlan show networks mode=Bssid";
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(257, 57);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(483, 109);
            this.txtResult.TabIndex = 1;
            // 
            // btnGetBSSID
            // 
            this.btnGetBSSID.Location = new System.Drawing.Point(19, 21);
            this.btnGetBSSID.Name = "btnGetBSSID";
            this.btnGetBSSID.Size = new System.Drawing.Size(75, 23);
            this.btnGetBSSID.TabIndex = 0;
            this.btnGetBSSID.Text = "Run";
            this.btnGetBSSID.UseVisualStyleBackColor = true;
            this.btnGetBSSID.Click += new System.EventHandler(this.btnGetBSSID_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.dgvListLogByHanetAI);
            this.tabPage4.Controls.Add(this.btnGetLogByHanetAIV2);
            this.tabPage4.Controls.Add(this.btnGetLogByHanetAI);
            this.tabPage4.Controls.Add(this.dtToDateHanetAI);
            this.tabPage4.Controls.Add(this.dtFromDateHanetAI);
            this.tabPage4.Controls.Add(this.tbRefreshToken);
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Controls.Add(this.tbAccessToken);
            this.tabPage4.Controls.Add(this.label13);
            this.tabPage4.Controls.Add(this.tbClientSecret);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Controls.Add(this.tbClientID);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(796, 428);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "HanetAI";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // dgvListLogByHanetAI
            // 
            this.dgvListLogByHanetAI.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvListLogByHanetAI.Location = new System.Drawing.Point(21, 172);
            this.dgvListLogByHanetAI.Name = "dgvListLogByHanetAI";
            this.dgvListLogByHanetAI.Size = new System.Drawing.Size(597, 231);
            this.dgvListLogByHanetAI.TabIndex = 12;
            // 
            // btnGetLogByHanetAIV2
            // 
            this.btnGetLogByHanetAIV2.Location = new System.Drawing.Point(543, 75);
            this.btnGetLogByHanetAIV2.Name = "btnGetLogByHanetAIV2";
            this.btnGetLogByHanetAIV2.Size = new System.Drawing.Size(75, 23);
            this.btnGetLogByHanetAIV2.TabIndex = 11;
            this.btnGetLogByHanetAIV2.Text = "Lấy dữ liệu V2";
            this.btnGetLogByHanetAIV2.UseVisualStyleBackColor = true;
            this.btnGetLogByHanetAIV2.Click += new System.EventHandler(this.btnGetLogByHanetAIV2_Click);
            // 
            // btnGetLogByHanetAI
            // 
            this.btnGetLogByHanetAI.Location = new System.Drawing.Point(403, 76);
            this.btnGetLogByHanetAI.Name = "btnGetLogByHanetAI";
            this.btnGetLogByHanetAI.Size = new System.Drawing.Size(75, 23);
            this.btnGetLogByHanetAI.TabIndex = 10;
            this.btnGetLogByHanetAI.Text = "Lấy dữ liệu";
            this.btnGetLogByHanetAI.UseVisualStyleBackColor = true;
            this.btnGetLogByHanetAI.Click += new System.EventHandler(this.btnGetLogByHanetAI_Click);
            // 
            // dtToDateHanetAI
            // 
            this.dtToDateHanetAI.Location = new System.Drawing.Point(403, 42);
            this.dtToDateHanetAI.Name = "dtToDateHanetAI";
            this.dtToDateHanetAI.Size = new System.Drawing.Size(200, 20);
            this.dtToDateHanetAI.TabIndex = 9;
            // 
            // dtFromDateHanetAI
            // 
            this.dtFromDateHanetAI.Location = new System.Drawing.Point(403, 10);
            this.dtFromDateHanetAI.Name = "dtFromDateHanetAI";
            this.dtFromDateHanetAI.Size = new System.Drawing.Size(200, 20);
            this.dtFromDateHanetAI.TabIndex = 8;
            // 
            // tbRefreshToken
            // 
            this.tbRefreshToken.Location = new System.Drawing.Point(109, 112);
            this.tbRefreshToken.Name = "tbRefreshToken";
            this.tbRefreshToken.Size = new System.Drawing.Size(168, 20);
            this.tbRefreshToken.TabIndex = 7;
            this.tbRefreshToken.Text = resources.GetString("tbRefreshToken.Text");
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(18, 115);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 13);
            this.label14.TabIndex = 6;
            this.label14.Text = "RefreshToken";
            // 
            // tbAccessToken
            // 
            this.tbAccessToken.Location = new System.Drawing.Point(109, 76);
            this.tbAccessToken.Name = "tbAccessToken";
            this.tbAccessToken.Size = new System.Drawing.Size(168, 20);
            this.tbAccessToken.TabIndex = 5;
            this.tbAccessToken.Text = resources.GetString("tbAccessToken.Text");
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 79);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 13);
            this.label13.TabIndex = 4;
            this.label13.Text = "AccessToken";
            // 
            // tbClientSecret
            // 
            this.tbClientSecret.Location = new System.Drawing.Point(109, 42);
            this.tbClientSecret.Name = "tbClientSecret";
            this.tbClientSecret.Size = new System.Drawing.Size(168, 20);
            this.tbClientSecret.TabIndex = 3;
            this.tbClientSecret.Text = "6c1ccb99f8b2b5b3f1a284af29b6521e";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 42);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "ClientSecret";
            // 
            // tbClientID
            // 
            this.tbClientID.Location = new System.Drawing.Point(109, 11);
            this.tbClientID.Name = "tbClientID";
            this.tbClientID.Size = new System.Drawing.Size(168, 20);
            this.tbClientID.TabIndex = 1;
            this.tbClientID.Text = "9aa03aa84943a9ac5fe4211e5b31da89";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 11);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "ClientID";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.dtToDateByHysoon);
            this.tabPage5.Controls.Add(this.dtFromDateByHysoon);
            this.tabPage5.Controls.Add(this.dgvListLogByHysoon);
            this.tabPage5.Controls.Add(this.btnGetLogsByHysoon);
            this.tabPage5.Controls.Add(this.txtPortByHysoon);
            this.tabPage5.Controls.Add(this.label16);
            this.tabPage5.Controls.Add(this.txtLinkByHysoon);
            this.tabPage5.Controls.Add(this.label15);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(796, 428);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Hysoon";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(25, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(60, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Đường dẫn";
            // 
            // txtLinkByHysoon
            // 
            this.txtLinkByHysoon.Location = new System.Drawing.Point(113, 21);
            this.txtLinkByHysoon.Name = "txtLinkByHysoon";
            this.txtLinkByHysoon.Size = new System.Drawing.Size(153, 20);
            this.txtLinkByHysoon.TabIndex = 1;
            this.txtLinkByHysoon.Text = "http://pos.lki.com.vn";
            // 
            // txtPortByHysoon
            // 
            this.txtPortByHysoon.Location = new System.Drawing.Point(113, 60);
            this.txtPortByHysoon.Name = "txtPortByHysoon";
            this.txtPortByHysoon.Size = new System.Drawing.Size(153, 20);
            this.txtPortByHysoon.TabIndex = 3;
            this.txtPortByHysoon.Text = "6868";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(25, 63);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "Cổng";
            // 
            // btnGetLogsByHysoon
            // 
            this.btnGetLogsByHysoon.Location = new System.Drawing.Point(574, 22);
            this.btnGetLogsByHysoon.Name = "btnGetLogsByHysoon";
            this.btnGetLogsByHysoon.Size = new System.Drawing.Size(75, 23);
            this.btnGetLogsByHysoon.TabIndex = 4;
            this.btnGetLogsByHysoon.Text = "Lấy dữ liệu";
            this.btnGetLogsByHysoon.UseVisualStyleBackColor = true;
            this.btnGetLogsByHysoon.Click += new System.EventHandler(this.btnGetLogsByHysoon_Click);
            // 
            // dgvListLogByHysoon
            // 
            this.dgvListLogByHysoon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvListLogByHysoon.Location = new System.Drawing.Point(26, 145);
            this.dgvListLogByHysoon.Name = "dgvListLogByHysoon";
            this.dgvListLogByHysoon.Size = new System.Drawing.Size(736, 272);
            this.dgvListLogByHysoon.TabIndex = 5;
            // 
            // dtFromDateByHysoon
            // 
            this.dtFromDateByHysoon.Location = new System.Drawing.Point(311, 21);
            this.dtFromDateByHysoon.Name = "dtFromDateByHysoon";
            this.dtFromDateByHysoon.Size = new System.Drawing.Size(200, 20);
            this.dtFromDateByHysoon.TabIndex = 6;
            // 
            // dtToDateByHysoon
            // 
            this.dtToDateByHysoon.Location = new System.Drawing.Point(311, 60);
            this.dtToDateByHysoon.Name = "dtToDateByHysoon";
            this.dtToDateByHysoon.Size = new System.Drawing.Size(200, 20);
            this.dtToDateByHysoon.TabIndex = 7;
            // 
            // ViewMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 448);
            this.Controls.Add(this.tabControl);
            this.Name = "ViewMain";
            this.Text = "Main";
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogData)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListBSSID)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListLogByHanetAI)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListLogByHysoon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lblDeviceInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvRecords;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnDownloadFingerPrint;
        private System.Windows.Forms.Button btnPullData;
        private System.Windows.Forms.Button btnGetAllUserID;
        private System.Windows.Forms.Button btnGetDeviceTime;
        private System.Windows.Forms.Button btnBeep;
        private System.Windows.Forms.Button btnEnableDevice;
        private System.Windows.Forms.Button btnDisableDevice;
        private System.Windows.Forms.Button btnRestartDevice;
        private System.Windows.Forms.Button btnPowerOff;
        private System.Windows.Forms.Button btnUploadUserInfo;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxPassWord;
        public System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxDeviceIP;
        private System.Windows.Forms.TextBox tbxPort;
        private System.Windows.Forms.Button btnPingDevice;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxMachineNumber;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbxPass;
        private System.Windows.Forms.TextBox tbxUserName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbxCompanyCode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxURL;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView dgvLogData;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnGetBSSID;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtCmdText;
        private System.Windows.Forms.DataGridView dgvListBSSID;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox tbRefreshToken;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tbAccessToken;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbClientSecret;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbClientID;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnGetLogByHanetAI;
        private System.Windows.Forms.DateTimePicker dtToDateHanetAI;
        private System.Windows.Forms.DateTimePicker dtFromDateHanetAI;
        private System.Windows.Forms.Button btnGetLogByHanetAIV2;
        private System.Windows.Forms.DataGridView dgvListLogByHanetAI;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button btnGetLogsByHysoon;
        private System.Windows.Forms.TextBox txtPortByHysoon;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtLinkByHysoon;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridView dgvListLogByHysoon;
        private System.Windows.Forms.DateTimePicker dtToDateByHysoon;
        private System.Windows.Forms.DateTimePicker dtFromDateByHysoon;
    }
}