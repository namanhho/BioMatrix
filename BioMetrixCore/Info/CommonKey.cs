using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore
{
    public class CommonKey
    {
        public const string ShortDatePattern = "dd/MM/yyyy";
        public const string BackupDatePattern = "dd.MM.yyyy";
        public const string ExportDatePattern = "dd.MM.yyyy";
        public const string ShortTimePattern12 = "hh:mm tt";
        public const string ShortTimePattern24 = "HH:mm";
        public const string CheckTimePattern = "dd/MM/yyyy HH:mm:ss";
        public const string DateTimeSuprema = "yyyy-MM-ddTHH:mm:ssZ";
        public const string DateSeparator = "/";

        public const string StartupKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        public const string StartupArg_Hidden = "-hidden";

        public const string DBNameFormat = "AMISTimesheet_{0}.sqlite";
        public const string DBNameFormatBackup = "AMISTimesheet_{0}_{1}.sqlite";
    }
}
