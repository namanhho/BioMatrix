using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Utilities
{
    /// <summary>
    /// Lớp phân tích phiên bản
    /// </summary>
    /// CreatedBy LDNGOC 20.03.2017
    public class AnalyseVersion
    {
        #region Declaration
        /// <summary>
        /// Enum các loại phiên bản
        /// </summary>
        public enum EnumVersionType : int
        {
            Dev,
            Alpha1,
            Alpha2,
            Alpha3,
            Beta,
            RC,
            R
        }
        #endregion

        #region Void
        /// <summary>
        /// Hàm phân tích chuỗi Version Info thành các Giá trị Major, Minor, Build, Rivision
        /// </summary>
        /// <param name="sRawVersion">Version dạng "a.b.c.d"</param>
        private static Version AnalyseProductVersion(string sRawVersion)
        {
            return new Version(sRawVersion);
        }
        /// <summary>
        /// Hàm phân tích long Version Info thành các Giá trị Major, Minor, Build, Rivision
        /// </summary>
        /// <param name="longVersion">Version kiểu long</param>
        private static Version AnalyseProductVersion(long longVersion)
        {
            int iMajor = (int)Math.Floor((decimal)(longVersion / 1000000));
            int iMinor = (int)Math.Floor((decimal)((longVersion - iMajor * 1000000) / 100000));
            int iBuild = (int)Math.Floor((decimal)((longVersion - iMajor * 1000000 - iMinor * 100000) / 10000));
            int iRevision = (int)longVersion % 10000;
            if (iBuild > 0 || iMinor > 0 || iMajor > 0)
            {
                iRevision = iRevision % 1000;
            }

            return new Version(iMajor, iMinor, iBuild, iRevision);
        }

        /// <summary>
        /// Hàm lấy version type
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private static EnumVersionType GetVersionType(Version version)
        {
            EnumVersionType VersionType = EnumVersionType.Dev;
            if (version.Major == 0)
            {
                if (version.Minor == 0)
                {
                    if (version.Build == 0)
                    {
                        if (version.Revision > 3000)
                        {
                            VersionType = EnumVersionType.Alpha3;
                        }
                        else if (version.Revision > 2000)
                        {
                            VersionType = EnumVersionType.Alpha2;
                        }
                        else if (version.Revision > 1000)
                        {
                            VersionType = EnumVersionType.Alpha1;
                        }
                        else
                        {
                            VersionType = EnumVersionType.Dev;
                        }
                    }
                    else
                    {
                        VersionType = EnumVersionType.Beta;
                    }
                }
                else
                {
                    VersionType = EnumVersionType.RC;
                }
            }
            else
            {
                VersionType = EnumVersionType.R;
            }
            return VersionType;
        }

        /// <summary>
        /// Hàm lấy giá trị kiểu long của version
        /// </summary>
        /// <param name="sRawVersion">Version dạng "a.b.c.d"</param>
        /// <returns>Giá trị long</returns>
        public static long GetConvertNumber(string sRawVersion)
        {
            long lConvertNumber = 0;
            var version = AnalyseProductVersion(sRawVersion);
            EnumVersionType eVersionType = GetVersionType(version);
            switch (eVersionType)
            {
                case EnumVersionType.Dev:
                case EnumVersionType.Alpha1:
                case EnumVersionType.Alpha2:
                case EnumVersionType.Alpha3:
                    lConvertNumber = version.Revision;
                    break;
                case EnumVersionType.Beta:
                    lConvertNumber = long.Parse(version.Build.ToString() + (1000 + version.Revision).ToString());
                    break;
                case EnumVersionType.RC:
                    lConvertNumber = long.Parse(version.Minor.ToString() + version.Build.ToString() + (1000 + version.Revision).ToString());
                    break;
                case EnumVersionType.R:
                    lConvertNumber = long.Parse(version.Major.ToString() + version.Minor.ToString() + version.Build.ToString() + (1000 + version.Revision).ToString());
                    break;
            }
            return lConvertNumber;
        }

        /// <summary>
        /// Hàm lấy tên phiên bản
        /// </summary>
        /// <param name="convertNumber">Giá trị long của phiên bản</param>
        /// <param name="bIncludeMinor">Đọc tên bao gồm giá trị Minor</param>
        /// <param name="bWithSpace">Tên có chứa khoảng trắng</param>
        /// <returns>Tên phiên bản</returns>
        public static string GetVersionName(long convertNumber, bool bIncludeMinor = true, bool bIncludeRevision = false, bool bWithSpace = false)
        {
            Version version = AnalyseProductVersion(convertNumber);
            EnumVersionType eVersionType = GetVersionType(version);
            string Separator = bWithSpace ? " " : string.Empty;
            string VersionName = BuildVersionName(eVersionType, version, Separator, bIncludeMinor, bIncludeRevision);
            return VersionName;
        }

        /// <summary>
        /// Hàm lấy tên phiên bản
        /// </summary>
        /// <param name="sRawVersion">Version dạng "a.b.c.d"</param>
        /// <param name="bIncludeMinor">Đọc tên bao gồm giá trị Minor</param>
        /// <param name="bWithSpace">Tên có chứa khoảng trắng</param>
        /// <returns></returns>
        public static string GetVersionName(string sRawVersion, bool bIncludeMinor = true, bool bIncludeRevision = false, bool bWithSpace = false)
        {
            Version version = AnalyseProductVersion(sRawVersion);
            EnumVersionType eVersionType = GetVersionType(version);
            string Separator = bWithSpace ? " " : string.Empty;
            string VersionName = BuildVersionName(eVersionType, version, Separator, bIncludeMinor, bIncludeRevision);
            return VersionName;
        }

        /// <summary>
        /// Hàm lấy tên phiên bản
        /// </summary>
        /// <param name="version">Version</param>
        /// <param name="bIncludeMinor">Đọc tên bao gồm giá trị Minor</param>
        /// <param name="bWithSpace">Tên có chứa khoảng trắng</param>
        /// <returns></returns>
        public static string GetVersionName(Version version, bool bIncludeMinor = true, bool bIncludeRevision = false, bool bWithSpace = false)
        {
            EnumVersionType eVersionType = GetVersionType(version);
            string Separator = bWithSpace ? " " : string.Empty;
            string VersionName = BuildVersionName(eVersionType, version, Separator, bIncludeMinor, bIncludeRevision);
            return VersionName;
        }

        /// <summary>
        /// Build tên phiên bản
        /// </summary>
        /// <param name="versionType"></param>
        /// <returns></returns>
        private static string BuildVersionName(EnumVersionType versionType, Version version, string separator, bool bIncludeMinor, bool bIncludeRevision)
        {
            string sVersionName = string.Empty;

            switch (versionType)
            {
                case EnumVersionType.Dev:
                    sVersionName = "Dev" + separator + version.Revision;
                    break;
                case EnumVersionType.Alpha1:
                    sVersionName = "Alpha" + separator + "1." + (version.Revision - 1000).ToString();
                    break;
                case EnumVersionType.Alpha2:
                    sVersionName = "Alpha" + separator + "2." + (version.Revision - 2000).ToString();
                    break;
                case EnumVersionType.Alpha3:
                    sVersionName = "Alpha" + separator + "3." + (version.Revision - 3000).ToString();
                    break;
                case EnumVersionType.Beta:
                    sVersionName = "Beta" + separator + version.Build + "." + version.Revision.ToString();
                    break;
                case EnumVersionType.RC:
                    sVersionName = "RC" + separator + version.Minor + "." + version.Revision.ToString();
                    break;
                case EnumVersionType.R:
                    //Major
                    sVersionName = "R" + separator + version.Major;

                    //Minor
                    if (bIncludeMinor)
                    {
                        sVersionName = sVersionName + "." + version.Minor.ToString();
                    }

                    //Revision
                    if (bIncludeRevision)
                    {
                        sVersionName = sVersionName + " build " + version.Revision;
                    }
                    break;
            }

            return sVersionName;
        }
        #endregion
    }
}
