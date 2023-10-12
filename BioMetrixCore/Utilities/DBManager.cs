using BioMetrixCore.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore
{
    public static class DBManager
    {
        #region Common
        public static string GetDBPath(string tenantCode)
        {
            var dbFile = Path.Combine(CommonPath.PathFolderDatabase, string.Format(CommonKey.DBNameFormat, tenantCode));
            return dbFile;
        }

        public static string GetDBNameBackup(string tenantCode)
        {
            var name = string.Format(CommonKey.DBNameFormatBackup, tenantCode, DateTime.Now.ToString(CommonKey.BackupDatePattern));
            return name;
        }

        public static bool CheckDBExists(string tenantCode)
        {
            var exists = File.Exists(GetDBPath(tenantCode));
            return exists;
        }

        public static string CreateDB(string tenantCode)
        {
            var destFileName = GetDBPath(tenantCode);
            Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
            try
            {
                if (File.Exists(CommonPath.PathFileDBBlank))
                {
                    File.Copy(CommonPath.PathFileDBBlank, GetDBPath(tenantCode), false);
                }
            }
            catch (Exception ex)
            {
                destFileName = string.Empty;
                Logger.HandleException(ex);
            }
            return destFileName;
        }

        public static bool BackupDB(string source, string destination)
        {
            var success = true;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                if (File.Exists(source))
                {
                    File.Copy(source, destination, true);
                }
            }
            catch (Exception ex)
            {
                success = false;
                Logger.HandleException(ex);
            }
            return success;
        }

        public static bool RestoreDB(string source, string destination)
        {
            var success = true;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                if (File.Exists(source))
                {
                    File.Copy(source, destination, true);
                }
            }
            catch (Exception ex)
            {
                success = false;
                Logger.HandleException(ex);
            }
            return success;
        }
        #endregion

        #region ConvertDB
        /// <summary>
        /// Chuỗi split convert mặc định của MISA
        /// </summary>
        private const string mscSplitQueryScript = "/*MISA_DB_QueryScript*/";
        /// <summary>
        /// Tên bảng chứa convert script
        /// </summary>
        private const string mscConvertScriptTableName = "ConvertScript";
        /// <summary>
        /// (Bảng convert script) Tên cột thứ tự
        /// </summary>
        private const string mscOrderID = "OrderID";
        /// <summary>
        /// (Bảng convert script) Tên cột phiên bản
        /// </summary>
        private const string mscRelease = "Release";
        /// <summary>
        /// (Bảng convert script) Tên cột ID
        /// </summary>
        private const string mscID = "ID";
        /// <summary>
        /// (Bảng convert script) Tên cột nội dung script
        /// </summary>
        private const string mscSQLString = "SQLString";

        /// <summary>
        /// So sánh phiên bản của DB với phiên bản App
        /// </summary>
        /// <param name="sDBVersion">Phiên bản DB</param>
        /// <param name="sAppVersion">Phiên bản App</param>
        /// <returns>0: DB cũ hơn App (cần convert DB). 1: App cũ hơn DB (yêu cầu nâng cấp App). 2: DB cùng version với App (không cần làm gì)</returns>
        /// <remarks></remarks>
        public static EnumDBState CheckValidDBVersion(string sDBVersion, string sAppVersion)
        {
            EnumDBState result;

            long lDBConvertNumber = AnalyseVersion.GetConvertNumber(sDBVersion);
            long lAppConvertNumber = AnalyseVersion.GetConvertNumber(sAppVersion);

            if (lDBConvertNumber == lAppConvertNumber)
            {
                result = EnumDBState.AlreadyUptodate;
            }
            else if (lDBConvertNumber < lAppConvertNumber)
            {
                result = EnumDBState.NeedToConvert;
            }
            else
            {
                result = EnumDBState.AppObsolete;
            }

            return result;
        }

        /// <summary>
        /// Thực hiện convert dữ liệu lên phiên bản mới nhất
        /// </summary>
        /// <param name="fromDBVersion">Version hiện tại của DB</param>
        /// <param name="toDBVersion">Version hiện tại của chương trình</param>
        /// <param name="toDBVersionName">Tên version hiện tại của chương trình, dùng để Update vào DBInfo sau khi convert DB</param>
        /// <param name="fileScript">Tên file chứa convertScript (VD: Bin\MShopKeeper.ConvertScript.zip)</param>
        /// <param name="updateDBInfo">Cập nhật DBInfo sau khi convert thành công, mặc định True</param>
        /// <returns>True: Convert thành công. False: Convert thất bại</returns>
        public static bool ConvertDB(long fromDBVersion, long toDBVersion, string toDBVersionName, string fileScript, ref string mesageError, bool updateDBInfo = true)
        {
            bool success = true;

            if (File.Exists(fileScript))
            {
                //Kết nối vào db
                using (var cnn = new SQLiteConnection(Utility.GetConnectionString(ConnectionStringName.TimesheetAgent)))
                {
                    cnn.Open();
                    var ts = cnn.BeginTransaction();
                    try
                    {
                        success = RunSysScript(fromDBVersion, toDBVersion, fileScript, ts, ref mesageError);
                        success = success && UpdateDBInfo(toDBVersionName, ts);

                        //Update DBInfo
                        if (success)
                        {
                            ts.Commit();
                            Logger.LogInfo($"Convert successful from {fromDBVersion} to {toDBVersion}");
                        }
                        else
                        {
                            ts.Rollback();
                        }
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        ts.Rollback();
                        mesageError = ex.ToString();
                        Logger.HandleException(ex);
                    }
                    finally
                    {
                        cnn.Close();
                        cnn.Dispose();
                    }
                }
            }
            else
            {
                success = false;
                mesageError = $"Không tìm thấy file convert script [{fileScript}]";
            }

            return success;
        }

        /// <summary>
        /// Execute convert script vào DB
        /// </summary>
        /// <param name="fromVersion">Tìm các dòng có version lớn hơn giá trị này</param>
        /// <param name="toVersion">Tìm các dòng có version không vượt quá giá trị này</param>
        /// <param name="fileScript">Tên file chứa convertScript</param>
        /// <param name="ts">Transaction</param>
        /// <returns>True: Execute thành công. False: Execute thất bại</returns>
        /// <remarks></remarks>
        private static bool RunSysScript(long fromVersion, long toVersion, string fileScript, DbTransaction ts, ref string mesageError)
        {
            bool bResult = true;
            try
            {
                if (File.Exists(fileScript))
                {
                    DataSet dsScript = new DataSet();
                    string sSQLGetConvertScript = $"SELECT * FROM [ConvertScript] WHERE [Release] > '{fromVersion}' AND [Release] <= '{toVersion}' ORDER BY [Release], [OrderID]";
                    BuildDSScipt(ref dsScript);
                    GetDataFormFile(dsScript, sSQLGetConvertScript, fileScript);
                    DataView dvScript = new DataView(dsScript.Tables[mscConvertScriptTableName]);
                    SQLiteCommand cmdExec;
                    int iLength = dvScript.Count;
                    for (int i = 0; i <= dvScript.Count - 1; i++)
                    {
                        try
                        {
                            string sCommand = (string)(dvScript[i][mscSQLString]);
                            string[] arrSQL = sCommand.Split(new string[] { mscSplitQueryScript }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string strSQL in arrSQL)
                            {
                                if (!string.IsNullOrWhiteSpace(strSQL))
                                {
                                    try
                                    {
                                        cmdExec = new SQLiteCommand(strSQL, (SQLiteConnection)ts.Connection, (SQLiteTransaction)ts);
                                        //cmdExec.CommandText = strSQL;
                                        cmdExec.CommandType = CommandType.Text;
                                        cmdExec.CommandTimeout = 3600;
                                        //cmdExec.Transaction = ts;
                                        cmdExec.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        bResult = false;
                                        string version = dvScript[i][mscRelease].ToString();
                                        string order = dvScript[i][mscOrderID].ToString();
                                        string detail = "Chi tiết lỗi:" + Environment.NewLine + ex.ToString() + Environment.NewLine + "Script:" + Environment.NewLine + strSQL;
                                        string description = string.Format("Lỗi script phiên bản {0} tại dòng {1}\r\n{2}", version, order, detail);
                                        string error = string.Format("Convert từ {0} lên {1}\r\n{2}", fromVersion, toVersion, description);
                                        throw new Exception(error);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            bResult = false;
                            mesageError = ex.ToString();
                            throw;
                        }
                    }
                }
                else
                {
                    mesageError = $"Không tìm thấy file convert script [{fileScript}]";
                }
            }
            catch (Exception ex)
            {
                bResult = false;
                mesageError = ex.ToString();
                throw;
            }
            return bResult;
        }

        /// <summary>
        /// Nâng cấp phiên bản cho dữ liệu sau khi convert thành công
        /// </summary>
        /// <returns>True: Cập nhật thành công. False: Cập nhật thất bại</returns>
        /// <remarks></remarks>
        private static bool UpdateDBInfo(string versionName, DbTransaction ts)
        {
            SQLiteCommand cmd = new SQLiteCommand($"UPDATE [DBInfo] SET [Version] = '{versionName}', [ModifiedDate]='{DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss")}'", (SQLiteConnection)ts.Connection, (SQLiteTransaction)ts);
            cmd.CommandType = CommandType.Text;
            var bResult = cmd.ExecuteNonQuery() > 0;

            return bResult;
        }

        /// <summary>
        /// Tạo cấu trúc bảng ConverScript
        /// </summary>
        /// <param name="dsScript"></param>
        /// <remarks></remarks>
        private static void BuildDSScipt(ref DataSet dsScript)
        {
            DataTable tblScript = new DataTable(mscConvertScriptTableName);
            tblScript.Columns.Add(mscID, typeof(int));
            tblScript.Columns.Add(mscRelease, typeof(int));
            tblScript.Columns.Add(mscOrderID, typeof(int));
            tblScript.Columns.Add(mscSQLString, typeof(string));

            dsScript.Tables.Add(tblScript);
        }

        /// <summary>
        /// Lấy dữ liệu từ file ConvertScript
        /// </summary>
        /// <param name="dsConvertScript"></param>
        /// <param name="strSQL"></param>
        /// <remarks></remarks>
        private static void GetDataFormFile(DataSet dsConvertScript, string strSQL, string sConvertFile)
        {
            SQLiteConnection conn = null;
            try
            {
                var builder = new SQLiteConnectionStringBuilder()
                {
                    DataSource = sConvertFile
                };
                conn = new SQLiteConnection(builder.ToString());
                SQLiteCommand cmd = new SQLiteCommand(strSQL, conn);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(dsConvertScript, mscConvertScriptTableName);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Lấy ra version của DB từ bảng DBInfo
        /// </summary>
        /// <returns>Phiên bản dạng chuỗi (VD: 1.0.0.8)</returns>
        /// <remarks></remarks>
        public static string GetDBVersion()
        {
            string sVersion = "0.0.0.0";
            SQLiteConnection cnn = new SQLiteConnection(Utility.GetConnectionString(ConnectionStringName.TimesheetAgent));
            if (cnn != null)
            {
                try
                {
                    cnn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT [Version] FROM [DBInfo]", cnn);
                    cmd.CommandType = CommandType.Text;
                    sVersion = cmd.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    Logger.HandleException(ex);
                }
                finally
                {
                    cnn.Close();
                    cnn.Dispose();
                }
            }
            return sVersion;
        }
        #endregion
    }

    /// <summary>
    /// Kết quả kiểm tra trạng thái hiện tại của DB
    /// </summary>
    public enum EnumDBState : int
    {
        /// <summary>
        /// DBVer thấp cơn AppVer
        /// </summary>
        /// <remarks></remarks>
        NeedToConvert = 0,

        /// <summary>
        /// AppVer thấp hơn DBVer
        /// </summary>
        /// <remarks></remarks>
        AppObsolete = 1,

        /// <summary>
        /// AppVer = DBVer
        /// </summary>
        /// <remarks></remarks>
        AlreadyUptodate = 2
    }
}
