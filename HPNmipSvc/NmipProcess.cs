using System;
using System.Text;
using hp.sys;
using hp.log;
using System.IO;

namespace HPNmipSvc
{
    public class NmipProcess : ProcessBase
    {
        public NmipProcess(ILogger logger, ProcessInfo info)
            : base(logger, info)
        {
            BankID = info.ID;
        }

        public string BankID { get; set; }
        public string AppPath { get; set; }
        public string LogPath { get; set; }
        public string BinPath { get; set; }
        public string ProPath { get; set; }
        public string JocsPath { get; set; }
        public string CmdPath { get; set; }
        public string CmdDir { get; set; }
        public string CmdSTOP { get; set; }

        public override void Stop()
        {
            //Proc.StandardInput.Write("quit\n");
            try
            {
                FileInfo cmdFile = new FileInfo(string.Format(@"{0}", CmdPath + CmdSTOP));

                if (!cmdFile.Exists)
                {
                    FileStream fs = cmdFile.Create();
                    fs.Close();
                }

                Logger.WriteLine(Name, string.Format("CmdFile: {0}", cmdFile.FullName));
                Logger.WriteLine(Name, string.Format("Stop. PID: {0}", Proc.Id));
            }
            catch(Exception ex)
            {
                Logger.WriteException(Name, "[Stop]", ex);
            }
        }

        public override bool IsWork()
        {
            return true;
        }
    }
}
