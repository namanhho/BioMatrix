using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NLog.Internal;

namespace BioMetrixCore.Utilities
{
    public static class Utility
    {
        public static bool Ping(string ip, ref string message, int retry = 0)
        {
            bool success = false;
            try
            {
                do
                {
                    Ping p = new Ping();
                    var r = p.Send(ip);
                    success = r.Status == IPStatus.Success;
                    retry--;
                } while (retry > 0);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return success;
        }
        public static string GetAppSetting(string key)
        {
            //var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //var value = config.AppSettings.Settings[key].Value;
            var value = System.Configuration.ConfigurationManager.AppSettings[key];
            return value;
        }

        public static void SaveAppSetting(string key, string value)
        {
            var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }
        public static string GetConnectionString(string name)
        {
            var value = System.Configuration.ConfigurationManager.ConnectionStrings[name].ConnectionString;
            return value;
        }
    }
}