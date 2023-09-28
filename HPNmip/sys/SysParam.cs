using System;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using hp.util;
using ctbcfin.core.util;

namespace HPSVCS.sys
{
    public class SysParam
    {
        public SysParam(string bankID)
        {
            LoadAppConfig(bankID);
        }


        public string Version { get; set; }

        public string BankSvc { get; set; }
        public string BankID { get; set; }
        public string BankName { get; set; }
             
        public string BankPath { get; set; }
        public int Interval { get; set; }

        public string WorkPath { get { return BankPath + @"work\"; } }
        public string CmdPath { get { return BankPath + @"work\cmd\"; } }
        public string LogPath { get { return BankPath + @"work\log\"; } }
        public string BakPath { get { return BankPath + @"work\bak\"; } }
        public string FtpPath { get { return BankPath + @"ftp\"; } }
        public string InboundPath { get { return BankPath + @"ftp\inbound\"; } }
        public string OutboundPath { get { return BankPath + @"ftp\outbound\"; } }
        public string InboundBakPath { get { return BankPath + @"ftp\inbound\bak\"; } }
        public string OutboundBakPath { get { return BankPath + @"ftp\outbound\bak\"; } }
        public bool SFtpClient { get; set; }
        public bool SFtpMonitor { get; set; }
        public string SFtpHost { get; set; }
        public int SFtpPort { get; set; }  
        public string SFtpUser { get; set; }
        public string SFtpPswd { get; set; }
        public string SFtpCollDRptDir { get; set; }
        public string SFtpCollDRptPattern { get; set; }
        public string SFtpCollDRptZipPswd { get; set; }
        public string BankEnv { get; set; }

        public void LoadAppConfig(string bankID)
        {

            if (ConfigurationManager.AppSettings["Version"] != null)
            {
                Version = ConfigurationManager.AppSettings["Version"];
            }

            if (ConfigurationManager.AppSettings["BankSvc"] != null)
            {
                BankSvc = ConfigurationManager.AppSettings["BankSvc"];
            }

            BankID = bankID;            

            var section = ConfigurationManager.GetSection("BANK" + bankID) as NameValueCollection;
            
            BankName = section["BankName_" + bankID];            

            BankPath = section["BankPath_" + bankID]; /* 檔案路徑 */       

            Interval = Convert.ToInt16(section["Interval_" + bankID]);

            BankEnv = section["BankEnv_" + bankID];
            SFtpClient = Convert.ToBoolean(section["SFtpClient_" + bankID]);

            SFtpHost = section["SFtpHost_" + bankID + BankEnv];
            SFtpPort = Convert.ToInt16(section["SFtpPort_" + bankID + BankEnv]);            
            SFtpUser = section["SFtpUser_" + bankID + BankEnv];
            SFtpPswd = section["SFtpPswd_" + bankID + BankEnv];

            
            SFtpMonitor = Convert.ToBoolean(section["SFtpMonitor_" + bankID + BankEnv]);

            SFtpCollDRptDir = section["SFtpCollDRptDir_" + bankID + BankEnv];
            SFtpCollDRptPattern = section["SFtpCollDRptPattern_" + bankID + BankEnv];
            SFtpCollDRptZipPswd = section["SFtpCollDRptZipPswd_" + bankID + BankEnv];

        }
    }
}
