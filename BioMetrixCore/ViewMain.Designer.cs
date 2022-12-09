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
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btnGetBSSID = new System.Windows.Forms.Button();
            this.txtCmdText = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogData)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
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
            this.lblStatus.Location = new System.Drawing.Point(3, 414);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.lblStatus.Size = new System.Drawing.Size(790, 11);
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
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(257, 60);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(483, 325);
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
            // txtCmdText
            // 
            this.txtCmdText.Location = new System.Drawing.Point(257, 21);
            this.txtCmdText.Name = "txtCmdText";
            this.txtCmdText.Size = new System.Drawing.Size(483, 20);
            this.txtCmdText.TabIndex = 2;
            this.txtCmdText.Text = "netsh wlan show networks mode=Bssid";
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
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(173, 60);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Kết quả";
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
    }
}