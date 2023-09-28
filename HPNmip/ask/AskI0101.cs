using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using hp.log;
using hp.helper;
using hp.util;
using System.Linq;
using System.Text.RegularExpressions;

namespace HPSVCS.ask
{
    public class AskI0101 : AskWork
    {
        IonicZipHelper zipHelper = null;

        SftpHelper sftpHelper = null;

        bool _ifZip = false;

        public AskI0101(ILogger logger)
            : base("[AskI01]", logger)
        {
        }

        public override bool Run(FileInfo askFile)
        {            

            //* Remane askfile             
            if (!Prepare(askFile)) return false;

            /* 代收 文件
             * Command file: I010210101 帳單代收檔 (目前沒用到)
             * Command file: I010210201 帳單代收回覆檔             
            */

            /* 解析 指令檔 */

            //* CMD file type
            string askFileType = askFile.Name.Substring(0, 3) + askFile.Name.Substring(5, 3);
            
            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "CMD file type", askFileType));

            //* 目前收檔支援 檔案類 I102
            if (!"I01102".Equals(askFileType))
            {
                ErrorCode = 900;
                ErrorMsg = "CMD file supports type 102. CMD file name :" + askFile.Name;
                Logger.Error(_name, string.Format("{0}: [{1}]", string.Empty + ErrorCode, ErrorMsg));
                return false;
            }

            //* 開啟指令檔讀行
            #region  //** 目前不啟用

            //List<string> rcvFiles = FileUtil.GetLines(WorkFile.FullName, true);

            //* 指令檔內容

            //* 收下當天檔案
            //if (rcvFiles.Count == 0)
            //{
            //    ErrorCode = 000;
            //    string Msg = "CMD file is empty. Go download from Inbound path:" + Param.InboundPath;
            //    Logger.Info(_name, string.Format("{0}: [{1}]", string.Empty, Msg));
            //}

            //* 收下備份檔案
            //if (rcvFiles.Count > 0)
            //{
            //    ErrorCode = 000;
            //    string Msg = "CMD file is not empty. Go download from Inbound path:" + Param.InboundPathBak;
            //    Logger.Info(_name, string.Format("{0}: [{1}]", string.Empty, Msg));
            //}

            ////int fileCount = 0;            
            ////List<string> sL_RealFileName = new List<string>();
            /// 

            #endregion

            //* 判斷啟用 sftp client 才執行
            if (Param.SFtpClient) //* 是否啟用SFTP Client
            {
                bool success = false;

                success = sftpHelper.Connect();

                if (!success)
                {                        
                        ErrorCode = 006;
                        ErrorMsg = "SFTP: Run SFTP connect fail.";
                        Logger.WriteLine(_name, string.Format(" Run SFTP connect : {0}", success.ToString()));
                        return false;
                }                                        
                
                Logger.WriteLine(_name, string.Format(" Run SFTP connect: {0}", "OK"));                                
                
                String sL_RemoteFolder = Param.SFtpCollDRptDir;    //* SFTP Server 下載路徑            

                //string sL_Pattern = Param.SFtpCollDRptPattern;

                List<string> sL_SFtpFiles = new List<string>();

                ArrayList L_RemoteFileNameAryList = new ArrayList();

                if (!sL_RemoteFolder.Substring(sL_RemoteFolder.Length - 1).Equals("/")) 
                { sL_RemoteFolder += "/"; }
                
                ////success = sftpHelper.ChangeRemoteDir(sL_RemoteFolder);
                ////if (!success)
                ////{
                ////    sftpHelper.Disconnect();
                ////    ErrorCode = 7;
                ////    ErrorMsg = "SFTP change remote dir error.";
                ////    return false;
                ////}
                ////Logger.WriteLine(_name, string.Format("SFTP ChangeRemoteDir: {0}", success.ToString()));

                //*  連線成功，下載檔案
                if (success)
                {                    
                    success = sftpHelper.MGet(Param.SFtpCollDRptPattern, Param.InboundPath, sL_RemoteFolder, ref L_RemoteFileNameAryList, ref sL_SFtpFiles);

                    if (!success)
                    {
                        sftpHelper.Disconnect();
                        ErrorCode = 007;
                        ErrorMsg = "SFTP: Run SFTP MGet error.";
                        Logger.WriteLine(_name, string.Format(" Run SFTP MGet: {0}{1}", ErrorCode, ErrorMsg));

                        return false;
                    }

                    foreach (string f in sL_SFtpFiles)
                    {
                        sftpHelper.Delete(sL_RemoteFolder + f);
                        Logger.WriteLine(_name, string.Format(" Run SFTP Delete: [{0}][{1}]", f, "SFTP: Run SFTP Delete OK."));
                    }
                    
                    sftpHelper.Disconnect();

                    Logger.WriteLine(_name, string.Format(" Run SFTP MGet: {0}{1}", "000", "SFTP: Run SFTP MGet ok."));
                }

            }

            BakFileDir = Param.BakPath; //* 預設的備份檔路徑   

            //* 處理下載檔案
            // Create a reference to the inbound directory.
            DirectoryInfo di = new DirectoryInfo(Param.InboundPath);            

            //* 成功交易之Daily Report: 企業識別碼 + yymmdd-daily-v40.txt            
            // 80503378-CollDailyRpt.txt            
            string sL_FileName = "80503378-CollDailyRpt.txt";
            string sL_WorkPathJocsFile = Param.WorkPath + sL_FileName; //* 給 JOCS的檔名，檔名固定 

            // Create an array representing the files in the inbound directory.                        
            string sL_RcvLookFiles = "*" + Param.SFtpCollDRptPattern;

            FileInfo[] fiArray = di.GetFiles(sL_RcvLookFiles);            

            if (fiArray.Length == 0)
            {
                ErrorCode = 903;
                ErrorMsg = "RCV File is not found.";
                Logger.WriteLine(_name, string.Format("{0}: [{1}]", "" + ErrorCode, ErrorMsg));
                return false;
            }

            Array.Sort(fiArray, (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.CreationTime, y.CreationTime));

            foreach (FileInfo fiTemp in fiArray)
            {
                string sL_RcvFilePath = fiTemp.FullName;
                string sL_RcvFileName = fiTemp.Name;
                string sL_RcvFilePathBak = Param.InboundBakPath + sL_RcvFileName;
                string sL_RcvWorkPathFile = Param.WorkPath + sL_RcvFileName;

                Logger.WriteLine(_name, string.Format("{0}: [{1}]", "RCV Path", sL_RcvFilePath));
                Logger.WriteLine(_name, string.Format("{0}: [{1}]", "RCV Bak Path", sL_RcvFilePathBak));
                Logger.WriteLine(_name, string.Format("{0}: [{1}]", "RCV Work Path", sL_RcvWorkPathFile));
                Logger.WriteLine(_name, string.Format("{0}: [{1}]", "RCV File", sL_RcvFileName));

                FileInfo rcvOriFileInfo = new FileInfo(sL_RcvFilePath);

                //* 先備份在Inbound\Bak\
                rcvOriFileInfo.CopyTo(sL_RcvFilePathBak, true);

                //* 在JOCS工作夾產生備份夾
                string sL_FullPathBakDir = Param.BakPath + rcvOriFileInfo.Name + @"\";
                if (!Directory.Exists(sL_FullPathBakDir))
                {
                    Directory.CreateDirectory(sL_FullPathBakDir);
                }

                //* 送檔的備份路徑
                BakFileDir = sL_FullPathBakDir;

                //* 搬至工作夾中 (Inbound 有備份，所以用搬的)
                if (File.Exists(sL_RcvWorkPathFile))
                {
                    File.Delete(sL_RcvWorkPathFile);
                }
                File.Move(sL_RcvFilePath, sL_RcvWorkPathFile);

                FileInfo rcvWorkFileInfo = new FileInfo(sL_RcvWorkPathFile);

                List<string> unzipListFiles = new List<string>(); ;

                //down, 解壓縮..                
                if (sL_RcvWorkPathFile.Substring(sL_RcvWorkPathFile.Length - 3, 3) == "zip")
                {
                    #region  
                    
                    Logger.WriteLine(_name, string.Format("do file unzip with pwsd: [{0}{1}]", sL_RcvWorkPathFile, Param.SFtpCollDRptZipPswd));

                    unzipListFiles = zipHelper.UnzipListPwsd(sL_RcvWorkPathFile, Param.WorkPath, Param.SFtpCollDRptZipPswd);
                    
                    #endregion                    

                }
                //up, 解壓縮..

                DirectoryInfo diunzip = new DirectoryInfo(Param.WorkPath);

                FileInfo[] fiunzipArray = diunzip.GetFiles("*.txt");
                Array.Sort(fiunzipArray, (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.CreationTime, y.CreationTime));

                Logger.WriteLine(_name, string.Format("{0}: [{1}]", "GetFiles *.txt Count", ""+ fiunzipArray.Length));

                foreach (string f in unzipListFiles)
                {
                ////}
                
                ////foreach (FileInfo fiunzipTemp in fiunzipArray)
                ////{
                    string sL_RcvUnzipFileName = f;
                    string sL_RcvUnzipFileFullName = Param.WorkPath + f;
                    string sL_RcvUnzipFileBakFullName = Param.InboundBakPath + sL_RcvUnzipFileName;
                    string sL_RcvUnzipFileWorkFullName = Param.WorkPath + sL_RcvUnzipFileName;

                    Logger.WriteLine(_name, string.Format("{0}: [{1}]", "Unzip File", sL_RcvUnzipFileName));
                    Logger.WriteLine(_name, string.Format("{0}: [{1}]", "Unzip Fullname", sL_RcvUnzipFileFullName));
                    Logger.WriteLine(_name, string.Format("{0}: [{1}]", "Unzip Bak Path", sL_RcvUnzipFileBakFullName));
                    Logger.WriteLine(_name, string.Format("{0}: [{1}]", "Unzip Work Path", sL_RcvUnzipFileWorkFullName));

                    #region //* utf-8轉碼為Big5
                    {
                        bool isUtf8 = MyEncodeUtil.IsUft8Encoding(sL_RcvUnzipFileFullName);
                        bool isBig5 = MyEncodeUtil.IsBig5Encoding(sL_RcvUnzipFileFullName);

                        Logger.WriteLine(_name, string.Format("{0}: [{1}]", "isUtf8", isUtf8.ToString()));
                        Logger.WriteLine(_name, string.Format("{0}: [{1}]", "isBig5", isBig5.ToString()));

                        if (isUtf8)
                        {
                            using (System.IO.FileStream fs = new System.IO.FileStream(sL_RcvUnzipFileFullName, System.IO.FileMode.Open))
                            {
                                var sr = new StreamReader(fs, Encoding.UTF8);
                                string content = sr.ReadToEnd();

                                //Encoding big5 = Encoding.GetEncoding(950);
                                //Encoding utf8 = Encoding.GetEncoding("utf-8");

                                ////讀uft8編碼bytes
                                //byte[] utf8Bytes = new byte[fs.Length];
                                //fs.Read(utf8Bytes, 0, (int)fs.Length);

                                //// convert encoding from utf8  to big5
                                //byte[] big5Bytes = Encoding.Convert(utf8, big5, utf8Bytes);

                                //char[] big5Chars = new char[big5.GetCharCount(big5Bytes, 0, big5Bytes.Length)];
                                //big5.GetChars(big5Bytes, 0, big5Bytes.Length, big5Chars, 0);

                                //string big5Str = new string(big5Chars);

                                string big5Str = MyEncodeUtil.ConvertUTF8toBIG5(content);

                                if (File.Exists(sL_WorkPathJocsFile))
                                {
                                    File.AppendAllText(sL_WorkPathJocsFile, Environment.NewLine + big5Str, Encoding.GetEncoding(950)); //* 存在則併檔
                                }
                                else
                                {
                                    System.IO.File.WriteAllText(sL_WorkPathJocsFile, big5Str, Encoding.GetEncoding(950));
                                }

                                Logger.WriteLine(_name, string.Format("{0}: [{1}]", "big5 File", sL_WorkPathJocsFile));

                            }
                        }
                        else if (isBig5)
                        {
                            rcvWorkFileInfo.CopyTo(sL_WorkPathJocsFile, true); //* 產生一份 JOCS要求的檔名
                            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "big5 File", sL_WorkPathJocsFile));
                        }
                        else
                        {
                            rcvWorkFileInfo.CopyTo(sL_WorkPathJocsFile, true); //* 產生一份 JOCS要求的檔名
                            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "undefined File", sL_WorkPathJocsFile));
                        }
                        

                        //* 工作檔案備份起來
                        //* 把Inbound來源檔
                        //* 把JOCS要求檔
                        if (Directory.Exists(sL_FullPathBakDir))
                        {
                            //* Inbound來源檔備份起來
                            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "Copy to", sL_FullPathBakDir + sL_RcvFileName));
                            rcvWorkFileInfo.CopyTo(sL_FullPathBakDir + sL_RcvFileName, true);
                            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "Copy ok", sL_FullPathBakDir + sL_RcvFileName));

                            //* JOCS要求檔備份起來
                            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "Copy ok", sL_FullPathBakDir + sL_FileName));
                            FileInfo ToJocsfile = new FileInfo(sL_WorkPathJocsFile);
                            ToJocsfile.CopyTo(sL_FullPathBakDir + sL_FileName, true);
                            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "Copy ok", sL_FullPathBakDir + sL_FileName));
                        }
                        ErrorCode = 000;
                        ErrorMsg = "RCV:" + sL_WorkPathJocsFile + "|" + sL_RcvFileName;
                    }
                    #endregion
                }

                var lines = File.ReadAllLines(sL_WorkPathJocsFile, Encoding.GetEncoding(950)).Where(arg => !string.IsNullOrWhiteSpace(arg));
                File.WriteAllLines(sL_WorkPathJocsFile, lines, Encoding.GetEncoding(950));
            }
           

            return true;
        }

        public bool Prepare(FileInfo askFile)
        {
            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "BankID:", Param.BankID));
            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "BankEnv:", Param.BankEnv));
            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "SFtpClient:", Param.SFtpClient.ToString()));

            zipHelper = new IonicZipHelper(Logger);
            zipHelper.Logger = Logger;

            string workFile = askFile.FullName + ".wk";
            DeleteFile(workFile);
            File.Move(askFile.FullName, workFile);
            WorkFile = new FileInfo(workFile);

            //* CMD file type
            string askFileType = askFile.Name.Substring(0, 3) + askFile.Name.Substring(5, 3);
            Logger.WriteLine(_name, string.Format("{0}: [{1}]", "CMD file type", askFileType));

            //* 目前收檔支援 檔案類 I102
            if (!"I01102".Equals(askFileType))
            {
                ErrorCode = 900;
                ErrorMsg = "Error. CMD file supports type 102. CMD file name :" + askFile.Name;
                Logger.WriteLine(_name, string.Format("{0}: [{1}]", string.Empty + ErrorCode, ErrorMsg));
                return false;
            }

            //* sftpHelper.Initialize
            if (Param.SFtpClient)
            {
                Logger.WriteLine(_name, string.Format("SFtpHost: [{0}]", Param.SFtpHost));
                Logger.WriteLine(_name, string.Format("SFtpPort: [{0}]", Param.SFtpPort));
                Logger.WriteLine(_name, string.Format("SFtpUser: [{0}]", Param.SFtpUser));
                Logger.WriteLine(_name, string.Format("SFtpPswd: [{0}]", Param.SFtpPswd));

                bool success = false;
                sftpHelper = new SftpHelper(Logger);

                success = sftpHelper.Initialize(Param.SFtpHost, Param.SFtpPort, Param.SFtpUser, Param.SFtpPswd);
                if (!success)
                {
                    ErrorCode = 005;
                    ErrorMsg = "SFTP: Sftp initial error.";
                    return false;
                }
            }            
            
            return true;
        }

        public bool ZipAndDeleteFiles(string zipFile, List<string> files, string path)
        {
            zipHelper.Zip(zipFile, files, path);
            Logger.WriteLine(_name, string.Format("ZIP file: [{0}]", zipFile));            

            foreach (string f in files)
                DeleteFile(path + f);

            return true;
        }

        public bool MonitorSftpStatus()
        {
            sftpHelper = new SftpHelper(Logger);

            bool success = sftpHelper.Initialize(Param.SFtpHost, Param.SFtpPort, Param.SFtpUser, Param.SFtpPswd);
            if (!success)
            {
                ErrorCode = 5;
                ErrorMsg = "SFTP initial error.";
                return false;
            }
            else
            {
                Logger.WriteLine(_name, string.Format("SFTP initial: [success {0}]", success.ToString()));
            }

            return success;
        }
     
    }
}
