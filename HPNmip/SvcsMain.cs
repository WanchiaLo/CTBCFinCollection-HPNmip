//SKB ftp
//Test => ip:172.16.11.16, ID:10300000, passwd:10300000, folder: /tscc_svct/member/10300000
//Prod => ip:172.16.10.22, ID:10300000, passwd:z9S3G18m, folder: /tscc_svcp/member/10300000
using System;
using System.Text;
using System.IO;
using System.Linq;

using System.Collections.Generic;
using hp.template;
using hp.log;
using hp.util;

using HPSVCS.sys;
using HPSVCS.ask;

using hp.helper;

using ctbcfin.core.util;

namespace HPSVCS
{
    public class SvcsMain : ConsoleMain
    {
        public SvcsMain(string name, ILogger logger)
            : base(name, logger)
        {
        }

        public SysParam Param { get; set; }

        bool Go = true;

        public override bool Start()
        {
            Logger.WriteLine(_name, "SvcsMain Running Start() ...");

            Logger.WriteLine(_name, string.Format("HPNmip:       {0} {1}", "Version", Param.Version));
            Logger.WriteLine(_name, string.Format("BankID:       {0} {1}", Param.BankID, Param.BankName));
            Logger.WriteLine(_name, string.Format("BankPath:     {0}", Param.BankPath));
            Logger.WriteLine(_name, string.Format("FtpPath:      {0}", Param.FtpPath));
            Logger.WriteLine(_name, string.Format("WorkPath:     {0}", Param.WorkPath));
            Logger.WriteLine(_name, string.Format("LogPath:      {0}", Param.LogPath));

            return true;
        }

        public override void Stop()
        {
        }

        public override void TimerWork()
        {
            FileInfo cmdFile = new FileInfo(Param.CmdPath + "CMDSTOP");

            if (cmdFile.Exists)
            {
                cmdFile.Delete();
                Logger.WriteLine(_name, "Stop by CMDSTOP");
                Loop = false;
            }
        }
        
        public override void Work()
        {
            Logger.WriteLine(_name, "Running Work() ...");

            //* 讀取指令檔
            List<FileInfo> fileList = DirUtil.GetFiles(Param.WorkPath);

            IEnumerable<FileInfo> askFiles =
                        from fi in fileList
                        where (fi.Name.Length.Equals(10) &&
                               (fi.Name.Substring(0, 1).Equals("O") || fi.Name.Substring(0, 1).Equals("I") || fi.Name.Substring(0, 1).Equals("T")) &&
                               (fi.Name.EndsWith("00") || fi.Name.EndsWith("01")))
                        orderby fi.Name
                        select fi;

            foreach (FileInfo fi in askFiles)
            {
                DoAskWork(fi);
            }

            
            Logger.WriteLine(_name, "Exit Work() ...");

            ////Loop = false;
            ////Environment.Exit(0);            
        }

        private void DoAskWork(FileInfo askFile)
        {
            Logger.WriteLine(_name,"[DoAskWork] Ask Beg. " + askFile.FullName);

            Param.LoadAppConfig(Param.BankID);

            AskWork askWork = null;
            string askCmd = askFile.Name.Substring(0, 3) + askFile.Name.Substring(8, 2);

            Logger.WriteLine(_name, "[DoAskWork] Ask Beg. " + askCmd);

            if ("I0101".Equals(askCmd))
                askWork = new AskI0101(Logger);
            else if ("I0100".Equals(askCmd))
                askWork = new AskI0100(Logger);
            ////else if("O0301".Equals(askCmd))
            ////    askWork = new AskO0301(Logger);
            ////else if ("O0300".Equals(askCmd))
            ////    askWork = new AskO0300(Logger);
            ////else if ("I0301".Equals(askCmd))
            ////    askWork = new AskI0301(Logger);
            ////else if ("I0300".Equals(askCmd))
            ////    askWork = new AskI0300(Logger);
            //else if ("O0201".Equals(askCmd))
            //    askWork = new AskO0201(Logger);
            else
                askWork = new AskCmd(Logger);

            askWork.Param = Param;
            
            askWork.Execute(askFile);                        

            Logger.WriteLine(_name, "[DoAskWork] Ask End. " + askFile.FullName + Environment.NewLine);
        }
    }
}
