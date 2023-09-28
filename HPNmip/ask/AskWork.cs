using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using hp.log;
using hp.util;
using hp.helper;
using HPSVCS.sys;

namespace HPSVCS.ask
{
    public abstract class AskWork
    {
        protected string _name;

        //SftpHelper sftpHelper = null;

        public AskWork(string name, ILogger logger)
        {
            _name = name;
            Logger = logger;
            ErrorCode = 0;
            ErrorMsg = string.Empty;
            BakFileDir = string.Empty;
            SFtpSuccess = false;
        }

        public ILogger Logger { get; set; }
        public SysParam Param { get; set; }

        public int ErrorCode { get; set; }
        public String ErrorMsg { get; set; }
        public FileInfo WorkFile { get; set; }
        public FileInfo ZipFile { get; set; }
        public FileInfo ZipRptFile { get; set; }
        public String RenameFileName { get; set; }
        public String BakFileDir { get; set; }
        //public String SFtpClient { get; set; }
        //public String SFtpHost { get; set; }
        //public int SFtpPort { get; set; }
        //public String SFtpUser { get; set; }
        //public String SFtpPswd { get; set; }

        public bool SFtpSuccess { get; set; }

        public abstract bool Run(FileInfo askFile);

        public bool Execute(FileInfo askFile)
        {
            bool success;

            try
            {
                success = Run(askFile);
            }
            catch (Exception ex)
            {
                success = false;
                ErrorCode = 9;
                ErrorMsg = ex.Message;
                Logger.WriteException(_name, "[AskWork Execute Run ex]", ex);
            }

            try
            {
                if (!"00".Equals(askFile.Name.Substring(8, 2)))
                {
                    string msgFile = askFile.FullName.Replace(askFile.Name, askFile.Name.Substring(0, 8) + "99");
                    using (StreamWriter sw = new StreamWriter(msgFile, false, Encoding.Default))
                    {
                        string buf = string.Format("{0:000}|{1}", ErrorCode, ErrorMsg);
                        sw.Write(buf);
                        Logger.WriteLine(_name, string.Format("MSG: [{0}]", buf));                        
                    }

                    string sL_RemoteFileName = askFile.Name.Substring(0, 8) + "99";

                    if (BakFileDir.Equals(string.Empty))
                    {
                        BakFileDir = Param.BakPath + DateTime.Now.ToString("yyyyMMdd-HHmmss") + @".bak\";

                        Logger.WriteLine(_name, string.Format("{0}: [{1}]", "BakFileDir", BakFileDir));

                        Directory.CreateDirectory(BakFileDir);

                        if (Directory.Exists(BakFileDir))
                        {
                            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "Create directory", BakFileDir));
                        }else
                        {
                            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "Error Create directory", BakFileDir));
                        }
                    }

                    //* 備份起來
                    if (Directory.Exists(BakFileDir))
                    {
                        //* 檔案備份
                        FileInfo Sourcefile = new FileInfo(msgFile);
                        Sourcefile.CopyTo(BakFileDir + sL_RemoteFileName, true);  
                        Logger.WriteLine(_name, string.Format("{0}: [{1}]", "Move", BakFileDir + sL_RemoteFileName));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException(_name, "[AskWork Execute ex]", ex);
            }

            return success;
        }

        public void DeleteFile(string f)
        {
            if (File.Exists(f))
            {
                File.Delete(f);
                Logger.WriteLine(_name, string.Format("Delete file : [{0}]", f));
            }
        }

        public void DeleteFile(FileInfo fi)
        {
            if (fi.Exists)
            {
                fi.Delete();
                Logger.WriteLine(_name, string.Format("Delete file: [{0}]", fi.FullName));
            }
        }

        public void DeleteRelativeFiles(string lstFile)
        {
            DeleteRelativeFiles(lstFile, true);
        }

        public void DeleteRelativeFiles(string lstFile, bool itself)
        {
            FileInfo fi = new FileInfo(lstFile);

            if (fi.Exists)
            {
                List<string> files = FileUtil.GetLines(fi.FullName);

                foreach (string f in files)
                {
                    string deleteFile = fi.Directory + @"\" + f;
                    File.Delete(deleteFile);
                    Logger.WriteLine(_name, string.Format("Delete file: [{0}]", deleteFile));
                }

                if (itself)
                {
                    fi.Delete();
                    Logger.WriteLine(_name, string.Format("Delete file: [{0}]", fi.FullName));
                }
            }
        }

    }
}
