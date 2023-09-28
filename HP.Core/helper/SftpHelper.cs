using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using hp.log;
using hp.util;
using Renci.SshNet;

namespace hp.helper
{
    public class SftpHelper
    {
        Renci.SshNet.SftpClient sftp;

        string _name = "[SFTPHelper]";

        public SftpHelper(ILogger logger)
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

        public string RemoteFolder { get; set; }
        public string RemotePattern { get; set; }
        public string LocalDir { get; set; }

        ArrayList ReceivedFileName { get; set; }

        public bool Initialize(string sP_Host, int nP_Port, string sP_UserName, string sP_Password)
        {
            bool bL_Result = true;

            try
            {
                sftp = new SftpClient(sP_Host, nP_Port, sP_UserName, sP_Password);

            }
            catch (Exception ex)
            {
                bL_Result = false;
            }
            return bL_Result;
        }

        public bool Connect()
        {
            bool bL_Result = true;

            try
            {
                //  Connect and login to the FTP server.
                sftp.Connect();                

            }
            catch (Exception ex)
            {
                WriteLine("[Error]" + string.Format("Connect failed."));
                bL_Result = false;
            }
            return bL_Result;
        }

        public bool Disconnect()
        {
            bool bL_Result = true;

            try
            {
                //  Connect and login to the FTP server.

                if (sftp.IsConnected)
                {
                    sftp.Disconnect();
                }

            }
            catch (Exception ex)
            {
                WriteLine("[Error]" + string.Format("Disonnect failed."));
                bL_Result = false;
            }
            return bL_Result;
        }

        public bool ListRemoteDir(string sL_RemoteDir, ref ArrayList P_RemoteFileNameAryList)
        {
            //bool success;

            // Download all files with filenames matching 'remotePattern'
            // The files are downloaded into 'localDir'
            bool bL_Result = true;
            try
            {
                WriteLine("[Info]" + "remote working dir:" + sftp.WorkingDirectory.ToString());


                var files = sftp.ListDirectory(sftp.WorkingDirectory);
                foreach (var file in files)
                {

                    P_RemoteFileNameAryList.Add(file.Name);
                    WriteLine("[Info]" + string.Format(" ListDirectory.=> ") + file.Name);


                }


            }
            catch (Exception ex)
            {
                bL_Result = false;
            }

            return bL_Result;
        }

        public bool ChangeRemoteDir(string remoteDir)
        {
            bool bL_Result = true;

            try
            {

                sftp.ChangeDirectory(remoteDir);

            }
            catch (Exception ex)
            {
                WriteLine("[Error]" + string.Format(" Change directory failed. Remote dir=> ") + remoteDir);
                bL_Result = false;
            }

            return bL_Result;
        }

        public bool Delete(string remoteFilename)
        {
            bool bL_Result = true;

            try
            {
                sftp.DeleteFile(remoteFilename);
            }
            catch (Exception ex)
            {
                WriteLine("[Error]" + string.Format(" delete file failed. Remote file=> ") + remoteFilename);
                bL_Result = false;
            }

            return bL_Result;
        }

        public bool Put(string localFilename, string sP_RemoteDir, string remoteFilename)
        {
            bool bL_Result = true;
            string sL_RemoteFileFullPath = sP_RemoteDir + remoteFilename;
            try
            {
                using (var file = File.OpenRead(localFilename))
                {
                    sftp.UploadFile(file, sL_RemoteFileFullPath);
                }
                //WriteLine(string.Format(" Upload file successed. Local file=> ") + localFilename + ",--RemoteFileFullPath:" + sL_RemoteFileFullPath + "--sftp working dir:" + sftp.WorkingDirectory.ToString());
            }
            catch (Exception ex)
            {
                WriteLine("[Error]" + string.Format(" Upload file failed. Local file=> ") + localFilename + ",RemoteFileFullPath:" + sL_RemoteFileFullPath);
                WriteLine("[Error]" + "Full exception=>");
                WriteLine("[Error]" + GetExceptionStr(ex));

                bL_Result = false;
            }
            return bL_Result;



        }

        public static string GetExceptionStr(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }

        public bool Get(string sP_LocalDir, string sP_LocalFilename, string sP_RemoteDir, string sP_RemoteFilename, ref ArrayList P_ReceivedFileNameAryList)
        {
            bool bL_Result = true;

            string sL_RemoteFileFullPath = sP_RemoteDir + sP_RemoteFilename;

            string sL_LocalFileFullPath = sP_LocalDir + sP_LocalFilename;

            Stream file1 = null;

            try
            {

                file1 = File.OpenWrite(sL_LocalFileFullPath);

                sftp.DownloadFile(sL_RemoteFileFullPath, file1);

                file1.Close();

                file1 = null;

                P_ReceivedFileNameAryList.Add(sP_LocalFilename);

            }
            catch (Exception ex)
            {
                if (file1 != null)
                {
                    file1.Close();
                    file1 = null;
                }
                if (File.Exists(sL_LocalFileFullPath))
                    File.Delete(sL_LocalFileFullPath);

                WriteLine("[Error]" + string.Format(" Get file failed. Remote file ==> ") + sP_RemoteFilename + "--Exception:" + ex.Message.ToString() + "----RemoteFileFullPath:" + sL_RemoteFileFullPath);

                bL_Result = false;
            }
            return bL_Result;
        }

        public bool MGet(string sP_RemotePattern, string sP_LocalDir, string sP_RemoteDir, ref ArrayList P_ReceivedFileName, ref List<string> P_SFtpFiles)
        {

            //* Download all files with filenames matching 'remotePattern'
            //* The files are downloaded into 'localDir'
            bool bL_Result = true;

            RemotePattern = sP_RemotePattern;
            LocalDir = sP_LocalDir;
            RemoteFolder = sP_RemoteDir;
            if (!RemoteFolder.Substring(0, 2).Equals("./"))
            { RemoteFolder = "./" + RemoteFolder; }

            try
            {
                string sL_FileName = string.Empty;

                Logger.WriteLine(_name, "[Info]" + string.Format(" MGet RemoteFolder => ") + RemoteFolder);
                Logger.WriteLine(_name, "[Info]" + string.Format(" MGet WorkingDirectory => ") + sftp.WorkingDirectory);

                //sftp.ChangeDirectory(sP_RemoteDir);
                //var files = sftp.ListDirectory(sftp.WorkingDirectory);

                var files = sftp.ListDirectory(RemoteFolder);


                foreach (var file in files)
                {

                    sL_FileName = file.Name;


                    if ((sL_FileName.IndexOf(RemotePattern) >= 0))
                    {
                        P_SFtpFiles.Add(sL_FileName);

                        bL_Result = Get(LocalDir, sL_FileName, RemoteFolder, sL_FileName, ref P_ReceivedFileName);
                        
                    } 
                    
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(_name, "[Error]" + string.Format(" MGet Exception ex => ") + ex.Message);
                bL_Result = false;
            }

            return bL_Result;
        }

        public bool MGet(string localDir, List<string> getFiles)
        {
            bool bL_Result = true;

            foreach (string f in getFiles)
            {
                var byt = sftp.ReadAllBytes(f);
                File.WriteAllBytes(localDir + f, byt);
            }

            return bL_Result;
        }

        private void WriteLine(string msg)
        {
            if (Logger != null)
                Logger.WriteLine(_name, msg);
        }

    }
}
