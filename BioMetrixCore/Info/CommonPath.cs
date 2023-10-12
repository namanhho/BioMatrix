using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore
{
    public class CommonPath
    {
        #region Folder Path
        public static readonly string FolderDatabase = "Database";
        public static readonly string FolderDBBlank = "DBBlank";
        public static readonly string FolderExport = "Export";
        public static readonly string FolderBackup = "Backup";
        public static readonly string PathFolderRoot = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string PathFolderDatabase = Path.Combine(PathFolderRoot, FolderDatabase);
        public static readonly string PathFolderDBBlank = Path.Combine(PathFolderRoot, FolderDBBlank);
        public static readonly string PathFolderExport = Path.Combine(PathFolderRoot, FolderExport);
        public static readonly string PathFolderBackup = Path.Combine(PathFolderRoot, FolderBackup);
        #endregion

        #region File Path
        public static readonly string FileDBBlank = "AMISTimesheet_Blank.sqlite";
        public static readonly string FileConvertScript = "ConvertScript.sqlite";
        //public static readonly string FileConvertScript = "ConvertScript.xml";
        public static readonly string PathFileApp = Assembly.GetExecutingAssembly().Location;
        public static readonly string PathFileDBBlank = Path.Combine(PathFolderDBBlank, FileDBBlank);
        public static readonly string PathFileConvertScript = Path.Combine(PathFolderDatabase, FileConvertScript);
        #endregion
    }
}
