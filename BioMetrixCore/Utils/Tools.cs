using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Utils
{
    /// <summary>
    /// ¹¤¾ßÀà
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// ²éÕÒÖµ
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string FindValue(string content, string key)
        {
            string sRet = string.Empty;
            int startKeyIndex = content.IndexOf(key);
            if (startKeyIndex < 0)
                return string.Empty;
            int endTabIndex = content.IndexOf("\t", startKeyIndex);
            if (startKeyIndex >= 0 && endTabIndex > 0)
            {
                sRet = content.Substring(startKeyIndex, content.IndexOf('\t', startKeyIndex) - startKeyIndex).Split('=')[1];
            }
            else if (startKeyIndex > 0 && endTabIndex < 0)
            {
                sRet = content.Substring(startKeyIndex);
                sRet = sRet.Substring(key.Length + 1);
            }
            return sRet;
        }
        /// <summary>
        /// ×éºÏ×Ö·û´®
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string UnionString(List<string> keys)
        {
            StringBuilder strKey = new StringBuilder();
            foreach (string key in keys)
            {
                strKey.Append(string.Format("'{0}',", key));
            }
            string unionKey = strKey.ToString();
            if (!string.IsNullOrEmpty(unionKey))
            {
                unionKey = unionKey.Substring(0, unionKey.Length - 1);
            }
            return unionKey;
        }
        /// <summary>Get the current time
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }
        public static string GetServerTZ()
        {
            string ServerTZ = DateTime.Now.ToString("zzz").Replace(":", "");
            return ServerTZ;
        }
        /// <summary>
        /// »ñÈ¡µ±Ç°Ê±¼ä×Ö·û´®
        /// </summary>
        /// <returns></returns>
        public static string GetDateTimeNowString()
        {
            return GetDateTimeNow().ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
        }
        public static string ToHexString(string number)
        {
            return string.IsNullOrWhiteSpace(number) ? "" : Convert.ToInt64(number).ToString("X2");
        }
        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
        /// <summary>String Convert to Int32
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static int TryConvertToInt32(string str, int defaultVal = 0)
        {
            try
            {
                return Convert.ToInt32(str);
            }
            catch
            {
                return defaultVal;
            }
        }

        /// <summary>String Convert to Int32
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static int TryConvertToInt32(object str, int defaultVal = 0)
        {
            try
            {
                return Convert.ToInt32(str);
            }
            catch
            {
                return defaultVal;
            }
        }

        /// <summary>
        /// ³õÊ¼»¯modelµÄÊý¾Ý
        /// </summary>
        /// <param name="model"></param>
        /// <param name="data"></param>
        public static void InitModel(object model, string data)
        {
            if (model != null)
            {
                Type objInfo = model.GetType();
                System.Reflection.PropertyInfo[] pinfos = objInfo.GetProperties();
                if (pinfos != null && pinfos.Length > 0)
                {
                    string[] infos = data.Split(",\t".ToCharArray());
                    if (infos != null && infos.Length > 0)
                    {
                        for (int i = 0; i < infos.Length; i++)
                        {
                            string[] columndata = infos[i].Split('=');
                            if (columndata != null && columndata.Length == 2 && !string.IsNullOrEmpty(columndata[1]))
                            {
                                columndata[0] = columndata[0].Replace("~", "");
                                foreach (System.Reflection.PropertyInfo pi in pinfos)
                                {
                                    if (pi.Name.ToLower() == columndata[0].ToLower())
                                    {
                                        try
                                        {
                                            string pvalue = columndata[1];
                                            if (string.IsNullOrWhiteSpace(pvalue) || pvalue.Trim().ToLower() == "null".ToLower())
                                                pvalue = string.Empty;
                                            SetKValue(model, pvalue, pi);
                                        }
                                        catch
                                        { }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ÉèÖÃÊôÐÔÊý¾Ý
        /// </summary>
        /// <param name="info"></param>
        /// <param name="pvalue"></param>
        /// <param name="pi"></param>
        public static void SetKValue(object info, string pvalue, System.Reflection.PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(System.Int32))
            {
                try
                {
                    int kvalue = int.Parse(pvalue);
                    pi.SetValue(info, kvalue, null);
                }
                catch
                { }
            }
            else if (pi.PropertyType == typeof(System.String))
            {
                pi.SetValue(info, pvalue, null);
            }
            else if (pi.PropertyType == typeof(System.DateTime))
            {
                try
                {
                    DateTime dt = DateTime.Parse(pvalue);
                    pi.SetValue(info, dt, null);
                }
                catch
                { }
            }
        }

        /// <summary>
        /// ×Ö·û´®²»Çø·Ö´óÐ¡Ð´Ìæ»»,½öÌæ»»µÚÒ»¸ö
        /// </summary>
        /// <param name="str">×Ö·û´®</param>
        /// <param name="oldStr">ÐèÒªÌæ»»²¿·Ö</param>
        /// <param name="newStr">Ìæ»»²¿·Ö</param>
        /// <param name="stringComparison">Çø·Ö´óÐ¡Ð´¹æÔò</param>
        /// <returns></returns>
        public static string Replace(string str, string oldStr, string newStr, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            int idx = str.IndexOf(oldStr, stringComparison);
            if (idx <= -1)
                return str;

            StringBuilder sb = new StringBuilder();
            sb.Append(str.Substring(0, idx));
            sb.Append(newStr);
            sb.Append(str.Substring(idx + oldStr.Length));

            return sb.ToString();
        }
        /// <summary>
        /// Éú³ÉSession
        /// </summary>
        /// <returns></returns>
        public static string GetSessionID()
        {
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
        public static int OldEncodeTime(int year, int mon, int day, int hour, int min, int sec)
        {
            int tt;
            tt = ((year - 2000) * 12 * 31 + ((mon - 1) * 31) + day - 1) * (24 * 60 * 60)
            + (hour * 60 + min) * 60 + sec;
            return tt;
        }

        /// <summary>
        /// ¸ù¾ÝÃëÊýµÃµ½datetime
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeBySeconds(int seconds)
        {
            int sec = seconds % 60;
            seconds /= 60;
            int min = seconds % 60;
            seconds /= 60;
            int hour = seconds % 24;
            seconds /= 24;
            int day = seconds % 31 + 1;
            seconds /= 31;
            int mon = seconds % 12 + 1;
            seconds /= 12;
            int year = seconds + 2000;
            return new DateTime(year, mon, day, hour, min, sec);
        }

        public static int TimezoneCovertInterger(string timeStart, string timeEnd)
        {
            if (string.IsNullOrEmpty(timeStart) || string.IsNullOrEmpty(timeEnd))
                return 0;
            string[] arr = timeStart.Split(':');
            int intergerStart = TryConvertToInt32(arr[0]) * 100 + TryConvertToInt32(arr[1]);
            arr = timeEnd.Split(':');
            int intergerEnd = TryConvertToInt32(arr[0]) * 100 + TryConvertToInt32(arr[1]);
            int nRtn = intergerStart << 16;
            nRtn += intergerEnd;
            return nRtn;
        }
        /// <summary>
        /// ´Ó×Ö·û´®ÖÐÈ¡Öµ
        /// </summary>
        /// <param name="str">×Ö·û´®</param>
        /// <param name="cSplit">Öµ¼ä¸ô·û</param>
        /// <param name="cSplitKV">¼üÖµ¼ä¸ô·û</param>
        /// <param name="keyToLower">¼üÊÇ·ñ×ª»¯ÎªÐ¡Ð´</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetKeyValues(string str, char cSplit = '\t', char cSplitKV = '=', bool keyToLower = true)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(str))
                return dic;

            string[] arr = str.Split(cSplit);
            foreach (string kv in arr)
            {
                int idx = kv.IndexOf(cSplitKV);
                if (idx <= 0)
                    continue;

                string key = kv.Substring(0, idx).Trim();
                if (keyToLower)
                    key = key.ToLower();

                if (string.IsNullOrEmpty(key) || dic.ContainsKey(key))
                    continue;

                dic.Add(key, kv.Substring(idx + 1));
            }

            return dic;
        }
        /// <summary>
        /// ´Ó×ÖµäÖÐÈ¡Öµ
        /// </summary>
        /// <param name="dic">×Öµä</param>
        /// <param name="key">¼ü</param>
        /// <param name="defaultVal">Ä¬ÈÏÖµ</param>
        /// <param name="keyToLower">¼ü×ª»¯ÎªÐ¡Ð´</param>
        /// <returns></returns>
        public static string GetValueFromDic(Dictionary<string, string> dic, string key, string defaultVal = "", bool keyToLower = true)
        {
            if (string.IsNullOrEmpty(key))
                return defaultVal;

            if (keyToLower)
                key = key.Trim().ToLower();

            if (dic.ContainsKey(key))
                return dic[key];

            return defaultVal;
        }

    }
}
