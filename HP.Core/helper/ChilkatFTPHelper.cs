using System;
using System.Text;
using System.Collections.Generic;
using hp.log;
using hp.util;

namespace hp.helper
{
    public class ChilkatFTPHelper
    {
        Chilkat.Ftp2 ftp;
        string _name = "[FTPHelper]";

        public ChilkatFTPHelper(ILogger logger)
        {
            Logger = logger;
            IsAuthTls = true;
            IsSSL = false;
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Pswd { get; set; }
        public bool IsAuthTls { get; set; }
        public bool IsSSL { get; set; }
        public bool IsPassive { get; set; }
        public string LastErrorText { get; set; }
        public int DownloadFileNum { get; set; }
        public List<string> DownloadFiles { get; set; }
        public ILogger Logger { get; set; }

        public bool Initialize()
        {
            bool bL_Result=true;

            try
            {
                ftp = new Chilkat.Ftp2();

                //  Any string unlocks the component for the 1st 30-days.
                //success = ftp.UnlockComponent("Anything for 30-day trial");
                //bL_Result = ftp.UnlockComponent("HPCOMbFTP_mK8EAn9R2ynO");
                bL_Result = ftp.UnlockComponent("DXCCOM.CB1122021_dUDyRCwU11jW");
                if (bL_Result != true)
                {
                    LastErrorText = ftp.LastErrorText;
                    WriteLine(string.Format("UnlockComponent failed: {0}", ftp.LastErrorText));
                    return bL_Result;
                }

                ftp.Hostname = Host;
                ftp.Port = Port;
                ftp.Username = User;
                ftp.Password = Pswd;
                ftp.AuthTls = IsAuthTls;
                ftp.Ssl = IsSSL;
                ftp.Passive = IsPassive;
                ftp.AutoFix = false;
                ftp.PassiveUseHostAddr = true;

                //ftp.Port = 990;
                //ftp.AuthTls = false;
                //ftp.Ssl = true;

                ftp.EnableEvents = true;
                ftp.OnEndDownloadFile += new Chilkat.Ftp2.EndDownloadFileEventHandler(ftp_OnEndDownloadFile);
                ftp.OnEndUploadFile += new Chilkat.Ftp2.EndUploadFileEventHandler(ftp_OnEndUploadFile);

                DownloadFileNum = 0;
                DownloadFiles = new List<string>();
            }
            catch
            {
                WriteLine(string.Format("FTP Initialize failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }
            return bL_Result;
        }

        public bool Connect()
        {
            bool bL_Result=true;

            try
            {
                //  Connect and login to the FTP server.
                bL_Result = ftp.Connect();
                if (bL_Result != true)
                {
                    LastErrorText = ftp.LastErrorText;
                    WriteLine(string.Format("Connect failed: [{0}][{1}][{2}][{3}][{4}][{5}][{6}]",
                        ftp.Hostname, ftp.Port, ftp.Username, ftp.Password, ftp.AuthTls, ftp.Ssl, ftp.Passive));
                    WriteLine(ftp.LastErrorText);
                    return bL_Result;
                }
            }
            catch
            {
                WriteLine(_name + "[Error]" + string.Format("FTP Connect failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }

            return bL_Result;
        }

        public bool Disconnect()
        {
            bool bL_Result;

            try
            {
                bL_Result = ftp.Disconnect();
                if (bL_Result != true)
                {
                    LastErrorText = ftp.LastErrorText;
                    WriteLine(_name + "[Error]" + string.Format("Disconnect failed."));
                    WriteLine(ftp.LastErrorText);
                    return bL_Result;
                }
            }
            catch
            {
                WriteLine(_name + "[Error]" + string.Format("FTP Disconnect failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }

            return bL_Result;
        }

        public bool ChangeRemoteDir(string remoteDir)
        {

            bool bL_Result=true;
            try
            {
                bL_Result = ftp.ChangeRemoteDir(remoteDir);
                if (bL_Result != true)
                {
                    LastErrorText = ftp.LastErrorText;
                    WriteLine(string.Format("ChangeRemoteDir failed: {0}", remoteDir));
                    WriteLine(ftp.LastErrorText);
                    bL_Result = false;
                }
                else
                {
                    WriteLine(string.Format("ChangeRemoteDir to : {0} successfully.", remoteDir));
                }
            }
            catch
            {
                WriteLine(string.Format("FTP ChangeRemoteDir failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }
            return bL_Result;
        }

        public bool Delete(string remoteFilename)
        {
            bool bL_Result=true;

            try
            {
                bL_Result = ftp.DeleteRemoteFile(remoteFilename);
                if (bL_Result != true)
                {
                    LastErrorText = ftp.LastErrorText;
                    WriteLine(string.Format("Delete Remote File failed: {0}", remoteFilename));
                    //WriteLine(ftp.LastErrorText);
                    return bL_Result;
                }

                WriteLine(string.Format("Delete Remote File: [{0}]", remoteFilename));
            }
            catch
            {
                WriteLine(string.Format("FTP Delete failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }

            return bL_Result;
        }

        public bool Put(string localFilename, string remoteFilename, out string sP_ErrorMsg)
        {
            bool bL_Result=true;
            sP_ErrorMsg = "";

            try
            {
                //  Upload a file.
                bL_Result = ftp.PutFile(localFilename, remoteFilename);
                if (bL_Result != true)
                {
                    LastErrorText = ftp.LastErrorText;
                    WriteLine(string.Format("PutFile failed: {0}", localFilename));
                    WriteLine(ftp.LastErrorText);
                    sP_ErrorMsg = ftp.LastErrorText;
                    return bL_Result;
                }

                WriteLine(string.Format("Upload File: [{0}]", localFilename));
            }
            catch
            {
                WriteLine(string.Format("FTP Put failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }

            return bL_Result;
        }
        public bool FTPSPut(string localFilename, string remoteFilename, out string sP_ErrorMsg)
        {
            bool bL_Result = true;
            sP_ErrorMsg = "";

            try
            {
                //  Upload a file.
                bL_Result = ftp.AppendFile(localFilename, remoteFilename);
                if (bL_Result != true)
                {
                    LastErrorText = ftp.LastErrorText;
                    WriteLine(string.Format("PutFile failed: {0}", localFilename));
                    WriteLine(ftp.LastErrorText);
                    sP_ErrorMsg = ftp.LastErrorText;
                    return bL_Result;
                }

                WriteLine(string.Format("Upload File: [{0}]", localFilename));
            }
            catch
            {
                WriteLine(string.Format("FTP Put failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }

            return bL_Result;
        }
        

        public bool Get(string localFilename, string remoteFilename, out string sP_ErrorMsg)
        {
            bool bL_Result=true;
            sP_ErrorMsg = "";
            try
            {
                bL_Result = ftp.GetFile(remoteFilename, localFilename);
                if (bL_Result != true)
                {
                    LastErrorText = ftp.LastErrorText;
                    WriteLine(string.Format("GetFile failed: {0} {1}", remoteFilename, localFilename));
                    //WriteLine(ftp.LastErrorText);
                    sP_ErrorMsg = ftp.LastErrorText;
                    return bL_Result;
                }
            }
            catch
            {
                WriteLine(string.Format("FTP Get failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }

            return bL_Result;
        }


        public bool MGet(string remotePattern, string localDir, out string sP_ErrorMsg)
        {
            bool bL_Result = true;
            sP_ErrorMsg = "";

            try
            {
                DownloadFiles = new List<string>();
                
                // Download all files with filenames matching 'remotePattern'
                // The files are downloaded into 'localDir'
                int numFilesDownloaded;
                numFilesDownloaded = ftp.MGetFiles(remotePattern, localDir);
                if (numFilesDownloaded < 0)
                {
                    LastErrorText = ftp.LastErrorText;
                    WriteLine(string.Format("MGetFiles failed: {0} {1}", remotePattern, localDir));
                    WriteLine(ftp.LastErrorText);
                    sP_ErrorMsg = ftp.LastErrorText;
                    return false;
                }

                DownloadFileNum = numFilesDownloaded;
            }
            catch
            {
                WriteLine(string.Format("FTP MGet 1 failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }

            return bL_Result;
        }

        public bool MGet(string localDir, List<string> getFiles, out string sP_ErrorMsg)
        {
            bool bL_Result=true;
            sP_ErrorMsg = "";
            try
            {
                foreach (string f in getFiles)
                {
                    bL_Result = ftp.GetFile(f, localDir + f);
                    if (bL_Result != true)
                    {
                        LastErrorText = ftp.LastErrorText;
                        WriteLine(string.Format("GetFile failed: {0} {1}", localDir, f));
                        WriteLine(ftp.LastErrorText);
                        sP_ErrorMsg = ftp.LastErrorText;
                        return bL_Result;
                    }
                }
            }
            catch
            {
                WriteLine(string.Format("FTP MGet 2 failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }


            return bL_Result;
        }

        public bool MGet(List<string> getFilePatterns, string localDir, out string sP_ErrorMsg)
        {
            bool bL_Result = true;

            int numFilesDownloaded;
            sP_ErrorMsg = "";

            try
            {
                foreach (string pattern in getFilePatterns)
                {
                    numFilesDownloaded = ftp.MGetFiles(pattern, localDir);
                    if (numFilesDownloaded < 0)
                    {
                        LastErrorText = ftp.LastErrorText;
                        WriteLine(string.Format("MGetFiles failed: {0} {1}", localDir, pattern));
                        WriteLine(ftp.LastErrorText);
                        sP_ErrorMsg = ftp.LastErrorText;
                        return false;
                    }
                }
            }
            catch
            {
                WriteLine(string.Format("FTP MGet 3 failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }

            return bL_Result;
        }

        public bool GetTextDirListing()
        {
            bool bL_Result = true;

            try
            {
                string buf = ftp.GetTextDirListing("*.*");
                if (buf == null)
                {
                    LastErrorText = "GetTextDirListing failed.";
                    WriteLine(string.Format("GetTextDirListing failed"));
                    return false;
                }

                LastErrorText = buf;
            }
            catch
            {
                WriteLine(string.Format("FTP GetTextDirListing failed: {0}", ftp.LastErrorText));
                bL_Result = false;
            }

            return bL_Result;
        }

        private void ftp_OnEndDownloadFile(object sender, Chilkat.FtpTreeEventArgs args)
        {
            DownloadFiles.Add(args.Path);
            WriteLine(string.Format("Download File: [{0}][{1}]", args.Path, args.NumBytes));
        }

        private void ftp_OnEndUploadFile(object sender, Chilkat.FtpTreeEventArgs args)
        {
            WriteLine(string.Format("Upload File: [{0}][{1}]", args.Path, args.NumBytes));
        }

        private void WriteLine(string msg)
        {
            if (Logger != null)
                Logger.WriteLine(_name, msg);
        }

        public void DoScript(string scriptFile)
        {
            Logger.WriteLine("", string.Format("Script: {0}", scriptFile));

            if (!Initialize()) return;

            List<string> ftpCmds = FileUtil.GetLines(scriptFile);

            foreach (string buf in ftpCmds)
            {
                                         
                string[] cmd = buf.Split(' ');

                if ((cmd[0].Length == 0) || (";".Equals(cmd[0].Substring(0, 1))))
                    continue;
                else if ("host".Equals(cmd[0]))
                {
                    Host = cmd[1];
                    ftp.Hostname = Host;
                }
                else if ("port".Equals(cmd[0]))
                {
                    Port = Convert.ToInt16(cmd[1]);
                    ftp.Port = Port;
                }
                else if ("user".Equals(cmd[0]))
                {
                    User = cmd[1];
                    ftp.Username = User;
                }
                else if ("pswd".Equals(cmd[0]))
                {
                    Pswd = cmd[1];
                    ftp.Password = Pswd;
                }
                else if ("AuthTls".Equals(cmd[0]))
                {
                    IsAuthTls = Convert.ToBoolean(cmd[1]);
                    ftp.AuthTls = IsAuthTls;
                }
                else if ("SSL".Equals(cmd[0]))
                {
                    IsSSL = Convert.ToBoolean(cmd[1]);
                    ftp.Ssl = IsSSL;
                }
                else if ("get".Equals(cmd[0]) || "put".Equals(cmd[0]) || "del".Equals(cmd[0]) || "lst".Equals(cmd[0]) || "cd".Equals(cmd[0]))
                {
                    DoFtpCmd(cmd);
                }
                else if ("open".Equals(cmd[0]))
                {
                    Connect();
                }
                else if ("quit".Equals(cmd[0]))
                {
                    Disconnect();
                }
            }
        }

        private void DoFtpCmd(string[] ftpCmd)
        {
            if (ftpCmd == null) return;

            bool success = false;
            string cmdBuf = "";
            string sL_ErrorMsg = "";
            if ("get".Equals(ftpCmd[0]))
            {
                success = Get(ftpCmd[1], ftpCmd[2], out sL_ErrorMsg);
                cmdBuf = ftpCmd[0] + " " + ftpCmd[1] + " " + ftpCmd[2];
            }
            else if ("put".Equals(ftpCmd[0]))
            {
                success = Put(ftpCmd[1], ftpCmd[2], out sL_ErrorMsg);
                cmdBuf = ftpCmd[0] + " " + ftpCmd[1] + " " + ftpCmd[2];
            }
            else if ("del".Equals(ftpCmd[0]))
            {
                success = Delete(ftpCmd[1]);
                cmdBuf = ftpCmd[0] + " " + ftpCmd[1];
            }
            else if ("lst".Equals(ftpCmd[0]))
            {
                success = GetTextDirListing();
                cmdBuf = ftpCmd[0];
            }
            else if ("cd".Equals(ftpCmd[0]))
            {
                success = ChangeRemoteDir(ftpCmd[1]);
                cmdBuf = ftpCmd[0] + " " + ftpCmd[1];
                LastErrorText = "";
            }

            Logger.WriteLine("", "[" + cmdBuf + "] : " + success.ToString());

            if (success)
                Logger.WriteLine("", LastErrorText);
        }
    }
}
